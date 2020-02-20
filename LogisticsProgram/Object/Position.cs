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

        private LocalTime timeFrom = new LocalTime();
        public LocalTime TimeFrom {
            get
            {
                return timeFrom;
            }
            set
            {
                timeFrom = value;
            }
        }

        private LocalTime timeTo = new LocalTime();
        public LocalTime TimeTo
        {
            get
            {
                return timeTo;
            }
            set
            {
                timeTo = value;
            }
        }

        public Position() { }
        public Position(Address address, LocalTime timeFrom, LocalTime timeTo)
        {
            this.address = address;
            this.timeFrom = timeFrom;
            this.timeTo = timeTo;
        }

        public int CompareTo(Position position)
        {
            return timeFrom.CompareTo(position.TimeFrom);
        }

        public override bool Equals(object obj)
        {
            return obj is Position position &&
                   address == position.Address &&
                   timeFrom == position.TimeFrom &&
                   timeTo == position.TimeTo;
        }
    }
}
