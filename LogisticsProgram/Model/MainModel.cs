using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using NodaTime;
using SimpleJSON;

namespace LogisticsProgram
{
    public class MainModel : BindableBase
    {
        private HttpClient client = new HttpClient();

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
                RaisePropertyChanged("StartPosition");
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

        private readonly ObservableCollection<Position> positions = new ObservableCollection<Position>();
        public ObservableCollection<Position> Positions
        {
            get
            {
                return positions;
            }
        }

        private readonly Route route = new Route();
        public Route Route
        {
            get
            {
                return route;
            }
        }

        public MainModel()
        {
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
            route.Positions.Clear();
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
            RaisePropertyChanged("Route");
        }

        private List<Position> FindShortestPossibleWay(List<Path> paths, Position position, List<Position> visitedPositions, LocalTime timePassed)
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
                    }

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
                        HttpResponseMessage response = await client.GetAsync($"https://api.tomtom.com/routing/1/calculateRoute/{fromPosition.Address.AddressValue}:{toPosition.Address.AddressValue}/json?maxAlternatives=1&key=rOwvGPEEBP70iDS2ohHSlqzBF1sp8d7z");
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
        }
    }
}
