using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LogisticsProgram
{
    public class Route
    {
        public Route(int vehicleId)
        {
            VehicleId = vehicleId;
        }

        public int VehicleId { get; set; }

        public ObservableCollection<Position> Positions { get; } = new ObservableCollection<Position>();

        public override bool Equals(object obj)
        {
            if (obj is Route route && route.Positions.Count == Positions.Count)
            {
                var equals = true;
                for (var i = 0; i < Positions.Count; i++)
                    if (!Positions[i].Equals(route.Positions[i]))
                        equals = false;
                return equals;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return -1378504013 + EqualityComparer<ObservableCollection<Position>>.Default.GetHashCode(Positions);
        }
    }
}