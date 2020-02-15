using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsProgram
{
    public class Route
    {
        private readonly ObservableCollection<Position> positions = new ObservableCollection<Position>();
        public ObservableCollection<Position> Positions => positions;

        public override bool Equals(object obj)
        {
            if (obj is Route route && (route.positions.Count == positions.Count))
            {
                bool equals = true;
                for(int i = 0; i < positions.Count; i++)
                {
                    if (!positions[i].Equals(route.positions[i]))
                    {
                        equals = false;
                    }
                }
                return equals;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return -1378504013 + EqualityComparer<ObservableCollection<Position>>.Default.GetHashCode(positions);
        }
    }
}
