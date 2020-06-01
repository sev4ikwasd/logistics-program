using NodaTime;
using Prism.Mvvm;

namespace LogisticsProgram
{
    public class Position : BindableBase
    {
        private Address address = new Address();

        private LocalTime timeFrom;

        private LocalTime timeTo;

        public Position()
        {
        }

        public Position(Address address, LocalTime timeFrom, LocalTime timeTo)
        {
            this.address = address;
            this.timeFrom = timeFrom;
            this.timeTo = timeTo;
        }

        public Address Address
        {
            get => address;
            set
            {
                address = value;
                RaisePropertyChanged();
            }
        }

        public LocalTime TimeFrom
        {
            get => timeFrom;
            set
            {
                timeFrom = value;
                RaisePropertyChanged();
            }
        }

        public LocalTime TimeTo
        {
            get => timeTo;
            set
            {
                timeTo = value;
                RaisePropertyChanged();
            }
        }
    }
}