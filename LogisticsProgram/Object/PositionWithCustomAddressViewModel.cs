using System;
using NodaTime;
using Prism.Mvvm;

namespace LogisticsProgram
{
    public class PositionWithCustomAddressViewModel<T> : BindableBase where T : BaseAddressModel
    {
        public PositionWithCustomAddressViewModel(Position position)
        {
            //Position.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };
            //May be a bottleneck
            AddressViewModel = new AddressViewModel((T) Activator.CreateInstance(typeof(T), position.Address));
            Position = position;
            //TimeFrom = Position.TimeFrom;
            //TimeTo = Position.TimeTo;
        }

        public Position Position { get; }

        public AddressViewModel AddressViewModel { get; set; }

        public LocalTime TimeFrom
        {
            get => Position.TimeFrom;
            set => Position.TimeFrom = value;
        }

        public LocalTime TimeTo
        {
            get => Position.TimeTo;
            set => Position.TimeTo = value;
        }
    }
}