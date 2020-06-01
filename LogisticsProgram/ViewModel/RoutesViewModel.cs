using System;
using System.Collections.ObjectModel;
using NodaTime;
using Prism.Commands;
using Prism.Mvvm;

namespace LogisticsProgram
{
    public class RoutesViewModel : BindableBase
    {
        private readonly RoutesModel model = new RoutesModel();

        public RoutesViewModel()
        {
            model.PropertyChanged += (s, e) =>
            {
                RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName.Equals("Routes"))
                {
                    if (model.Routes.Count != 0)
                        RoutesVisible = true;
                    else
                        RoutesVisible = false;
                    RaisePropertyChanged("RoutesVisible");
                }

                //Code to make sure that all time windows are correct and to change them if required
                if (e.PropertyName.Equals("StartPosition_TimeFrom"))
                {
                    if (model.StartPosition.TimeFrom > model.StartPosition.TimeTo)
                        model.StartPosition.TimeTo = model.StartPosition.TimeFrom;
                    foreach (var position in model.Positions)
                        if (position.TimeFrom < model.StartPosition.TimeFrom)
                            position.TimeFrom = model.StartPosition.TimeFrom;
                }

                if (e.PropertyName.Equals("StartPosition_TimeTo"))
                {
                    if (model.StartPosition.TimeTo < model.StartPosition.TimeFrom)
                        model.StartPosition.TimeFrom = model.StartPosition.TimeTo;
                    foreach (var position in model.Positions)
                        if (position.TimeTo > model.StartPosition.TimeTo)
                            position.TimeTo = model.StartPosition.TimeTo;
                }

                if (e.PropertyName.Equals("Positions_TimeFrom"))
                    foreach (var position in model.Positions)
                        if (position.TimeFrom > position.TimeTo)
                            position.TimeTo = position.TimeFrom;
                if (e.PropertyName.Equals("Positions_TimeTo"))
                    foreach (var position in model.Positions)
                        if (position.TimeTo < position.TimeFrom)
                            position.TimeFrom = position.TimeTo;
            };

            AddPositionCommand = new DelegateCommand(() =>
            {
                model.Positions.Add(new Position(new Address(), model.StartPosition.TimeFrom,
                    model.StartPosition.TimeTo));
            });
            RemovePositionCommand = new DelegateCommand<Position>(item => { model.Positions.Remove(item); });
            GenerateRouteCommand = new DelegateCommand(async () =>
            {
                if (!string.IsNullOrEmpty(model.StartPosition.Address.AddressValue) && model.Positions.Count > 0)
                    await model.GenerateRoute();
            });
        }

        public Position StartPosition => model.StartPosition;

        public Period DelayPeriod
        {
            get => model.DelayPeriod;
            set => model.DelayPeriod = value;
        }

        public int AmountOfVehicles
        {
            get => model.AmountOfVehicles;
            set
            {
                if (value <= 0) throw new FormatException("Amount of vehicles should be greater than zero");
                model.AmountOfVehicles = value;
            }
        }

        public ObservableCollection<Position> Positions => model.Positions;

        public ObservableCollection<Route> Routes => model.Routes;

        public bool RoutesVisible { get; set; }

        public DelegateCommand AddPositionCommand { get; }
        public DelegateCommand<Position> RemovePositionCommand { get; }

        public DelegateCommand GenerateRouteCommand { get; }
    }
}