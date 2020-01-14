using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace LogisticsProgram
{
    public class Position : IComparable<Position>
    {
        private String address = "";
        public String Address {
            get
            {
                return address;
            }
            set
            {
                address = value;
            }
        }

        private LocalTime time = new LocalTime();
        public LocalTime Time {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }

        public int CompareTo(Position position)
        {
            return time.CompareTo(position.Time);
        }

        public override bool Equals(object obj)
        {
            return obj is Position position &&
                   address == position.address &&
                   time == position.time;
        }

        public override int GetHashCode()
        {
            var hashCode = -242955985;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(address);
            hashCode = hashCode * -1521134295 + time.GetHashCode();
            return hashCode;
        }
    }
}
