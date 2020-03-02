using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Commands;
using NodaTime;
using NodaTime.Text;

namespace LogisticsProgram
{
    class MainViewModel : BindableBase
    {
        private readonly MainModel model = new MainModel();

        public Position StartPosition
        {
            get
            {
                return model.StartPosition;
            }
            set
            {
                model.StartPosition = value;
            }
        }

        public Period DelayPeriod
        {
            get
            {
                return model.DelayPeriod;
            }
            set
            {
                model.DelayPeriod = value;
            }
        }
        public ObservableCollection<Position> Positions
        {
            get
            {
                return model.Positions;
            }
            set
            {

            }
        }
        public Route Route
        {
            get
            {
                return model.Route;
            }
            set
            {

            }
        }
        private Boolean routeVisible = false;
        public Boolean RouteVisible
        {
            get
            {
                return routeVisible;
            }
            set
            {
                routeVisible = value;
            }
        }

        public MainViewModel()
        {//TODO validation
            model.PropertyChanged += (s, e) => {
                RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName.Equals("Route"))
                {
                    if (model.Route.Positions.Count != 0)
                    {
                        RouteVisible = true;
                    }
                    else
                    {
                        RouteVisible = false;
                    }
                    RaisePropertyChanged("RouteVisible");
                }
            };

            AddPositionCommand = new DelegateCommand(() => {
                model.Positions.Add(new Position());
            });
            RemovePositionCommand = new DelegateCommand<Position>((item) => {
                model.Positions.Remove(item);
            });
            GenerateRouteCommand = new DelegateCommand(() => {
                if(!String.IsNullOrEmpty(model.StartPosition.Address.AddressValue) && (model.Positions.Count > 0))
                    model.GenerateRoute();
            });
            AddressChosenCommand = new DelegateCommand<object[]>((objects =>
            {
                var position = objects[0] as Position;
                var addressVariant = objects[1] as Address.AddressVariant;
                if ((position != null) && (addressVariant != null))
                    position.Address.SetAddressVariant(addressVariant);
            }));
        }
        public DelegateCommand AddPositionCommand { get; }
        public DelegateCommand<Position> RemovePositionCommand { get; }
        public DelegateCommand GenerateRouteCommand { get; }
        public DelegateCommand<object[]> AddressChosenCommand { get; }
    }
}
