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

        public int AmountOfVehicles
        {
            get
            {
                return model.AmountOfVehicles;
            }
            set
            {
                if (value <= 0)
                {
                    throw new FormatException("Amount of vehicles should be greater than zero");
                }
                model.AmountOfVehicles = value;
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
        public ObservableCollection<Route> Routes
        {
            get
            {
                return model.Routes;
            }
            set
            {

            }
        }
        private Boolean routesVisible = false;
        public Boolean RoutesVisible
        {
            get
            {
                return routesVisible;
            }
            set
            {
                routesVisible = value;
            }
        }

        public MainViewModel()
        {
            model.PropertyChanged += (s, e) => {
                RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName.Equals("Routes"))
                {
                    if (model.Routes.Count != 0)
                    {
                        routesVisible = true;
                    }
                    else
                    {
                        routesVisible = false;
                    }
                    RaisePropertyChanged("RoutesVisible");
                }

                //Code to make sure that all time windows are correct and to change them if required
                if (e.PropertyName.Equals("StartPosition_TimeFrom"))
                {
                    if (model.StartPosition.TimeFrom > model.StartPosition.TimeTo)
                    {
                        model.StartPosition.TimeTo = model.StartPosition.TimeFrom;
                    }
                    foreach (Position position in model.Positions)
                    {
                        if(position.TimeFrom < model.StartPosition.TimeFrom)
                        {
                            position.TimeFrom = model.StartPosition.TimeFrom;
                        }
                    }
                }
                if (e.PropertyName.Equals("StartPosition_TimeTo"))
                {
                    if (model.StartPosition.TimeTo < model.StartPosition.TimeFrom)
                    {
                        model.StartPosition.TimeFrom = model.StartPosition.TimeTo;
                    }
                    foreach (Position position in model.Positions)
                    {
                        if (position.TimeTo > model.StartPosition.TimeTo)
                        {
                            position.TimeTo = model.StartPosition.TimeTo;
                        }
                    }
                }
                if (e.PropertyName.Equals("Position_TimeFrom"))
                {
                    foreach (Position position in model.Positions)
                    {
                        if (position.TimeFrom > position.TimeTo)
                        {
                            position.TimeTo = position.TimeFrom;
                        }
                    }
                }
                if (e.PropertyName.Equals("Position_TimeTo"))
                {
                    foreach (Position position in model.Positions)
                    {
                        if (position.TimeTo < position.TimeFrom)
                        {
                            position.TimeFrom = position.TimeTo;
                        }
                    }
                }
            };

            AddPositionCommand = new DelegateCommand(() => {
                model.Positions.Add(new Position(new Address(), model.StartPosition.TimeFrom, model.StartPosition.TimeTo));
            });
            RemovePositionCommand = new DelegateCommand<Position>((item) => {
                model.Positions.Remove(item);
            });
            GenerateRouteCommand = new DelegateCommand(async () => {
                if(!String.IsNullOrEmpty(model.StartPosition.Address.AddressValue) && (model.Positions.Count > 0))
                    await model.GenerateRoute();
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
