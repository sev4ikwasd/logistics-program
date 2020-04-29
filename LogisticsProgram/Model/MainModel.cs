using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools.ConstraintSolver;
using Prism.Mvvm;
using NodaTime;
using SimpleJSON;

namespace LogisticsProgram
{
    public class MainModel : BindableBase
    {

        private Position startPosition = new Position();
        public Position StartPosition
        {
            get
            {
                return startPosition;
            }
            set
            {
                startPosition = value;
                //RaisePropertyChanged("StartPosition");
            }
        }

        private Period delayPeriod = Period.FromMinutes(0);
        public Period DelayPeriod
        {
            get
            {
                return delayPeriod;
            }
            set
            {
                delayPeriod = value;
            }
        }

        private int amountOfVehicles = 1;

        public int AmountOfVehicles
        {
            get
            {
                return amountOfVehicles;
            }
            set
            {
                amountOfVehicles = value;
            }
        }

        private readonly ObservableCollection<Position> positions = new ObservableCollection<Position>();
        public ObservableCollection<Position> Positions
        {
            get
            {
                return positions;
            }
        }

        private readonly ObservableCollection<Route> routes = new ObservableCollection<Route>();
        public ObservableCollection<Route> Routes
        {
            get
            {
                return routes;
            }
        }

        public MainModel()
        {
            startPosition.PropertyChanged += (s, e) => {
                RaisePropertyChanged("StartPosition_" + e.PropertyName);
            };

            positions.CollectionChanged += (s, a) =>
            {
                if (a.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (Position item in a.OldItems)
                    {
                        item.PropertyChanged -= PositionChanged;
                    }
                }
                else if (a.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (Position item in a.NewItems)
                    {
                        item.PropertyChanged += PositionChanged;
                    }
                }
                RaisePropertyChanged("Positions");
            };
        }
        private void PositionChanged(object sender, PropertyChangedEventArgs e)
        {
            String propertyName = "";
            if (e.PropertyName.Equals("TimeFrom"))
            {
                propertyName = "Position_TimeFrom";
            }
            else if (e.PropertyName.Equals("TimeTo"))
            {
                propertyName = "Position_TimeTo";
            }
            RaisePropertyChanged(propertyName);
        }

        public async Task GenerateRoute()
        {
            routes.Clear();

            List<Position> fullPositions = new List<Position>();
            fullPositions.Add(StartPosition);
            fullPositions.AddRange(positions);
            Period[,] periodMatrix = await ApiUtility.GetInstance().GetPeriodMatrix(fullPositions);
            LocalTime[,] timeWindowsList = generateTimeWindowsList();

            RoutingIndexManager manager = new RoutingIndexManager(
                fullPositions.Count,
                amountOfVehicles,
                0);

            RoutingModel routing = new RoutingModel(manager);

            int transitCallbackIndex = routing.RegisterTransitCallback(
                (long fromIndex, long toIndex) => {
                    var fromNode = manager.IndexToNode(fromIndex);
                    var toNode = manager.IndexToNode(toIndex);
                    var result = periodMatrix[fromNode, toNode].Seconds / 60;
                    if (fromNode != 0) result += delayPeriod.Minutes; // Adding delay time if not going from depot
                    return result;
                }
            );
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

            routing.AddDimension(
                transitCallbackIndex, // transit callback
                30, // allow waiting time
                99999999, // vehicle maximum capacities
                false,  // start cumul to zero
                "Time");
            RoutingDimension timeDimension = routing.GetMutableDimension("Time");
            for (int i = 1; i < fullPositions.Count; ++i)
            {
                long index = manager.NodeToIndex(i);
                var from = Period.Between((new LocalTime()), timeWindowsList[i, 0], PeriodUnits.Minutes).Minutes;
                var to = Period.Between((new LocalTime()), timeWindowsList[i, 1], PeriodUnits.Minutes).Minutes;
                timeDimension.CumulVar(index).SetRange(
                    from,
                    to);
            }
            for (int i = 0; i < amountOfVehicles; ++i)
            {
                long index = routing.Start(i);
                var from = Period.Between((new LocalTime()), timeWindowsList[0, 0], PeriodUnits.Minutes).Minutes;
                var to = Period.Between((new LocalTime()), timeWindowsList[0, 1], PeriodUnits.Minutes).Minutes;
                timeDimension.CumulVar(index).SetRange(
                    from,
                    to);
            }
            for (int i = 0; i < amountOfVehicles; ++i)
            {
                routing.AddVariableMinimizedByFinalizer(
                    timeDimension.CumulVar(routing.Start(i)));
                routing.AddVariableMinimizedByFinalizer(
                    timeDimension.CumulVar(routing.End(i)));
            }

            RoutingSearchParameters searchParameters =
                operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy =
                FirstSolutionStrategy.Types.Value.Automatic; // PathCheapestArc possible

            Assignment solution = routing.SolveWithParameters(searchParameters);

            if ((solution == null) || (solution.solver().State() == Solver.PROBLEM_INFEASIBLE))
            {
                //TODO
            }
            else
            {
                timeDimension = routing.GetMutableDimension("Time");
                for (int i = 0; i < amountOfVehicles; ++i)
                {
                    Route route = new Route(i + 1);
                    var index = routing.Start(i);
                    while (routing.IsEnd(index) == false)
                    {
                        var timeVar = timeDimension.CumulVar(index);
                        var min = new LocalTime(Convert.ToInt32(solution.Min(timeVar) / 60), Convert.ToInt32(solution.Min(timeVar) % 60));
                        var max = new LocalTime(Convert.ToInt32(solution.Max(timeVar) / 60), Convert.ToInt32(solution.Max(timeVar) % 60));
                        Position position = new Position(fullPositions[manager.IndexToNode(index)].Address, min, max);
                        route.Positions.Add(position);
                        index = solution.Value(routing.NextVar(index));
                    }
                    var timeVarF = timeDimension.CumulVar(index);
                    var minF = new LocalTime(Convert.ToInt32(solution.Min(timeVarF) / 60), Convert.ToInt32(solution.Min(timeVarF) % 60));
                    var maxF = new LocalTime(Convert.ToInt32(solution.Max(timeVarF) / 60), Convert.ToInt32(solution.Max(timeVarF) % 60));
                    Position positionF = new Position(fullPositions[manager.IndexToNode(index)].Address, minF, maxF);
                    route.Positions.Add(positionF);
                    routes.Add(route);
                    var endTimeVar = timeDimension.CumulVar(index);
                }
            }
            RaisePropertyChanged("Routes");

            /*route.Positions.Clear();
            List<Position> fullPositions = new List<Position>(positions);
            fullPositions.Add(StartPosition);
            List<Path> paths = await GetDistancesFromApi(fullPositions);
            //TODO fix algorithm
            //Bad practice but I am too lazy to fix algorithm for now
            List<Path> actualPaths = AddReversePaths(paths);
            List<Position> way =
                FindShortestPossibleWay(actualPaths, StartPosition, new List<Position>(), startPosition.TimeTo);
            way.RemoveAt(0);
            route.Positions.AddRange(way);
            RaisePropertyChanged("Route");*/
        }

        private LocalTime[,] generateTimeWindowsList()
        {
            List<Position> fullPositions = new List<Position>();
            fullPositions.Add(StartPosition);
            fullPositions.AddRange(positions);
            LocalTime[,] timeWindowsList = new LocalTime[fullPositions.Count, 2];
            for (int i = 0; i < fullPositions.Count; i++)
            {
                timeWindowsList[i, 0] = fullPositions[i].TimeFrom;
                timeWindowsList[i, 1] = fullPositions[i].TimeTo;
            }

            return timeWindowsList;
        }

        /*private List<Position> FindShortestPossibleWay(List<Path> paths, Position position, List<Position> visitedPositions, LocalTime timePassed)
        {
            if (position.TimeTo < timePassed)
            {
                throw new WayException();
            }

            if(!visitedPositions.Contains(position)) visitedPositions.Add(position);

            List<Path> nextPaths = paths.FindAll(node =>
                (node.fromPosition.Equals(position)) && (!visitedPositions.Contains(node.toPosition)));
            if (nextPaths.Count > 0)
            {
                List<List<Position>> possibleWays = new List<List<Position>>();
                foreach (Path nextPath in nextPaths)
                {
                    if (nextPath.toPosition.TimeTo < timePassed)
                    {
                        throw new WayException();
                    }
                    else
                    {
                        try
                        {
                            LocalTime currentTimePassed = new LocalTime(timePassed.Hour, timePassed.Minute, timePassed.Second, timePassed.Millisecond);
                            currentTimePassed = currentTimePassed.Plus(nextPath.timeBetween);
                            if (currentTimePassed < nextPath.toPosition.TimeFrom)
                            {
                                currentTimePassed = nextPath.toPosition.TimeFrom;
                            }
                            currentTimePassed = currentTimePassed.Plus(delayPeriod);
                            List<Position> newVisitedPositions = new List<Position>(visitedPositions);
                            List<Position> nextWay = FindShortestPossibleWay(paths, nextPath.toPosition,
                                newVisitedPositions, currentTimePassed);
                            possibleWays.Add(nextWay);
                        }
                        catch(WayException)
                        {

                        }
                    }
                }
                if(possibleWays.Count > 0) {
                    List<Position> shortestWay = null;

                    foreach (List<Position> way in possibleWays)
                    {
                        if (shortestWay == null)
                        {
                            shortestWay = way;
                        }
                        else
                        {
                            if (way.Count < shortestWay.Count)
                            {
                                shortestWay = way;
                            }
                        }
                    }n

                    return shortestWay;
                }
                else
                {
                    throw new WayException();
                }
            }
            else
            {
                return visitedPositions;
            }
        }

        public class WayException : Exception
        {
            public WayException() : base("Way is impossible") { }
        }

        private List<Path> AddReversePaths(List<Path> paths)
        {
            List<Path> result = new List<Path>();
            List<Path> reversePaths = new List<Path>();
            foreach (Path path in paths)
            {
                reversePaths.Add(new Path(path.toPosition, path.fromPosition, path.timeBetween));
            }
            result.AddRange(paths);
            result.AddRange(reversePaths);
            return result;
        }

        private async Task<List<Path>> GetDistancesFromApi(List<Position> positions)
        {
            List<Path> result = new List<Path>();
            foreach (Position fromPosition in positions)
            {
                foreach (Position toPosition in positions)
                {
                    if (fromPosition != toPosition)
                    {
                        Period timeBetween;
                        HttpResponseMessage response = await ApiUtility.GetInstance()
                            .GetRoute(fromPosition.Address.AddressValue, toPosition.Address.AddressValue);
                        if (response.IsSuccessStatusCode)
                        {
                            String strResponse = await response.Content.ReadAsStringAsync();
                            var N = JSON.Parse(strResponse);
                            if (N["routes"].Count > 0)
                            {
                                int timeInSeconds = N["routes"][0]["summary"]["travelTimeInSeconds"].AsInt;
                                var builder = new PeriodBuilder();
                                timeBetween = Period.FromSeconds(timeInSeconds);
                                result.Add(new Path(fromPosition, toPosition, timeBetween));
                            }
                        }
                    }
                }
            }
            return result;
        }

        public struct Path
        {
            public Position fromPosition, toPosition;
            public Period timeBetween;

            public Path(Position fromPosition, Position toPosition, Period timeBetween)
            {
                this.fromPosition = fromPosition;
                this.toPosition = toPosition;
                this.timeBetween = timeBetween;
            }
        }*/
    }
}
