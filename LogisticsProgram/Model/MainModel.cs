using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using NodaTime;

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

        public Route GenerateRoute()
        {
            //TODO
            Route.Positions.Clear();
            List<Position> positionsList = new List<Position>(positions.ToList());
            positionsList.Sort();
            foreach(Position position in positionsList)
            {
                route.Positions.Add(position);
            }
            RaisePropertyChanged("Route");
            return route;
        }
    }
}
