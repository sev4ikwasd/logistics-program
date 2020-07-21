using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using Google.OrTools.ConstraintSolver;
using NodaTime;
using Prism.Mvvm;

namespace LogisticsProgram
{
    public class RoutesModel : BindableBase
    {
        private bool isCalculatingRoute;

        public RoutesModel()
        {
            StartPosition.PropertyChanged += (s, e) => { RaisePropertyChanged("StartPosition_" + e.PropertyName); };

            Positions.CollectionChanged += (s, a) =>
            {
                if (a.Action == NotifyCollectionChangedAction.Remove)
                    foreach (Position item in a.OldItems)
                        item.PropertyChanged -= PositionChanged;
                else if (a.Action == NotifyCollectionChangedAction.Add)
                    foreach (Position item in a.NewItems)
                        item.PropertyChanged += PositionChanged;

                RaisePropertyChanged("Positions");
            };
        }

        public Position StartPosition
        {
            get;
            set;
            //RaisePropertyChanged("StartPosition");
        } = new Position();

        public Period DelayPeriod { get; set; } = Period.FromMinutes(0);

        public int AmountOfVehicles { get; set; } = 1;

        public ObservableCollection<Position> Positions { get; } = new ObservableCollection<Position>();

        public ObservableCollection<Route> Routes { get; } = new ObservableCollection<Route>();

        public bool IsCalculatingRoute
        {
            get => isCalculatingRoute;
            private set
            {
                isCalculatingRoute = value;
                RaisePropertyChanged();
            }
        }

        private void PositionChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged("Positions_" + e.PropertyName);
        }

        public async Task GenerateRoute()
        {
            Routes.Clear();
            IsCalculatingRoute = true;

            var fullPositions = new List<Position>();
            fullPositions.Add(StartPosition);
            fullPositions.AddRange(Positions);
            var periodMatrix = await ApiUtility.GetInstance().GetPeriodMatrix(fullPositions);
            var timeWindowsList = generateTimeWindowsList();

            var manager = new RoutingIndexManager(
                fullPositions.Count,
                AmountOfVehicles,
                0);

            var routing = new RoutingModel(manager);

            var transitCallbackIndex = routing.RegisterTransitCallback(
                (fromIndex, toIndex) =>
                {
                    var fromNode = manager.IndexToNode(fromIndex);
                    var toNode = manager.IndexToNode(toIndex);
                    var result = periodMatrix[fromNode, toNode].Seconds / 60;
                    if (fromNode != 0) result += DelayPeriod.Minutes; // Adding delay time if not going from depot
                    return result;
                }
            );
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

            routing.AddDimension(
                transitCallbackIndex, // transit callback
                30, // allow waiting time
                99999999, // vehicle maximum capacities
                false, // start cumul to zero
                "Time");
            var timeDimension = routing.GetMutableDimension("Time");
            for (var i = 1; i < fullPositions.Count; ++i)
            {
                var index = manager.NodeToIndex(i);
                var from = Period.Between(new LocalTime(), timeWindowsList[i, 0], PeriodUnits.Minutes).Minutes;
                var to = Period.Between(new LocalTime(), timeWindowsList[i, 1], PeriodUnits.Minutes).Minutes;
                timeDimension.CumulVar(index).SetRange(
                    from,
                    to);
            }

            for (var i = 0; i < AmountOfVehicles; ++i)
            {
                var index = routing.Start(i);
                var from = Period.Between(new LocalTime(), timeWindowsList[0, 0], PeriodUnits.Minutes).Minutes;
                var to = Period.Between(new LocalTime(), timeWindowsList[0, 1], PeriodUnits.Minutes).Minutes;
                timeDimension.CumulVar(index).SetRange(
                    from,
                    to);
            }

            for (var i = 0; i < AmountOfVehicles; ++i)
            {
                routing.AddVariableMinimizedByFinalizer(
                    timeDimension.CumulVar(routing.Start(i)));
                routing.AddVariableMinimizedByFinalizer(
                    timeDimension.CumulVar(routing.End(i)));
            }

            var searchParameters =
                operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy =
                FirstSolutionStrategy.Types.Value.PathCheapestArc; // PathCheapestArc possible

            var solution = routing.SolveWithParameters(searchParameters);

            if (solution == null || solution.solver().State() == Solver.PROBLEM_INFEASIBLE)
            {
                //TODO
            }
            else
            {
                timeDimension = routing.GetMutableDimension("Time");
                for (var i = 0; i < AmountOfVehicles; ++i)
                {
                    var route = new Route(i + 1);
                    var index = routing.Start(i);
                    while (routing.IsEnd(index) == false)
                    {
                        var timeVar = timeDimension.CumulVar(index);
                        var min = new LocalTime(Convert.ToInt32(solution.Min(timeVar) / 60),
                            Convert.ToInt32(solution.Min(timeVar) % 60));
                        var max = new LocalTime(Convert.ToInt32(solution.Max(timeVar) / 60),
                            Convert.ToInt32(solution.Max(timeVar) % 60));
                        var position = new Position(fullPositions[manager.IndexToNode(index)].Address, min, max);
                        route.Positions.Add(position);
                        index = solution.Value(routing.NextVar(index));
                    }

                    var timeVarF = timeDimension.CumulVar(index);
                    var minF = new LocalTime(Convert.ToInt32(solution.Min(timeVarF) / 60),
                        Convert.ToInt32(solution.Min(timeVarF) % 60));
                    var maxF = new LocalTime(Convert.ToInt32(solution.Max(timeVarF) / 60),
                        Convert.ToInt32(solution.Max(timeVarF) % 60));
                    var positionF = new Position(fullPositions[manager.IndexToNode(index)].Address, minF, maxF);
                    route.Positions.Add(positionF);
                    Routes.Add(route);
                    var endTimeVar = timeDimension.CumulVar(index);
                }
            }

            IsCalculatingRoute = false;
            RaisePropertyChanged("Routes");
        }

        private LocalTime[,] generateTimeWindowsList()
        {
            var fullPositions = new List<Position>();
            fullPositions.Add(StartPosition);
            fullPositions.AddRange(Positions);
            var timeWindowsList = new LocalTime[fullPositions.Count, 2];
            for (var i = 0; i < fullPositions.Count; i++)
            {
                timeWindowsList[i, 0] = fullPositions[i].TimeFrom;
                timeWindowsList[i, 1] = fullPositions[i].TimeTo;
            }

            return timeWindowsList;
        }
    }
}