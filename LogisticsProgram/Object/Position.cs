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
        private Address address = new Address();

        public Address Address
        {
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

        public Position() { }
        public Position(Address address, LocalTime time)
        {
            this.address = address;
            this.time = time;
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
            hashCode = hashCode * -1521134295 + address.GetHashCode();
            hashCode = hashCode * -1521134295 + time.GetHashCode();
            return hashCode;
        }
    }
}
