using System;
using System.Collections.ObjectModel;
using NodaTime;
using Prism.Commands;

namespace LogisticsProgram
{
    public class RoutesViewModel : BaseViewModel
    {
        private readonly RoutesModel model = new RoutesModel();

        public RoutesViewModel()
        {
            StartPositionAddressViewModel =
                new AddressViewModel(new PlacesAndSearchAddressModel(model.StartPosition.Address));
            Positions = new ObservableCollection<PositionsListItemViewModel>();
            foreach (var position in model.Positions)
                Positions.Add(new PositionsListItemViewModel(position, PositionsListItemTimeFromValidationFunction,
                    PositionsListItemTimeToValidationFunction));
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
            };
            Validate();
            StartPositionAddressViewModel.ErrorsChanged += (sender, args) => { Validate(); };
            Positions.CollectionChanged += (sender, args) => { Validate(); };

            AddPositionCommand = new DelegateCommand(() =>
            {
                var position = new Position(new Address(), model.StartPosition.TimeFrom, model.StartPosition.TimeTo);
                model.Positions.Add(position);
                Positions.Add(new PositionsListItemViewModel(position, PositionsListItemTimeFromValidationFunction,
                    PositionsListItemTimeToValidationFunction));
            });
            RemovePositionCommand = new DelegateCommand<PositionsListItemViewModel>(item =>
            {
                Positions.Remove(item);
                model.Positions.Remove(item.Position);
            });
            GenerateRouteCommand = new DelegateCommand(async () =>
            {
                if (!string.IsNullOrEmpty(model.StartPosition.Address.AddressValue) && model.Positions.Count > 0)
                    await model.GenerateRoute();
            });
        }

        public AddressViewModel StartPositionAddressViewModel { get; set; }

        public LocalTime StartPositionTimeFrom
        {
            get => model.StartPosition.TimeFrom;
            set => model.StartPosition.TimeFrom = value;
        }

        public LocalTime StartPositionTimeTo
        {
            get => model.StartPosition.TimeTo;
            set => model.StartPosition.TimeTo = value;
        }

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

        public ObservableCollection<PositionsListItemViewModel> Positions { get; set; }

        public ObservableCollection<Route> Routes => model.Routes;

        public bool IsCalculatingRoute => model.IsCalculatingRoute;
        public bool RoutesVisible { get; set; }


        public DelegateCommand AddPositionCommand { get; }
        public DelegateCommand<PositionsListItemViewModel> RemovePositionCommand { get; }

        public DelegateCommand GenerateRouteCommand { get; }

        protected override void Validate()
        {
            ValidateProperty("StartPositionTimeFrom", StartPositionTimeFrom, propertyWithErrorsList =>
            {
                if (StartPositionTimeTo < StartPositionTimeFrom)
                    propertyWithErrorsList.ListErrors.Add(
                        "Start position time \"from\" should be less than time \"to\"!");
                return propertyWithErrorsList;
            });
            ValidateProperty("StartPositionTimeTo", StartPositionTimeTo, propertyWithErrorsList =>
            {
                if (StartPositionTimeFrom > StartPositionTimeTo)
                    propertyWithErrorsList.ListErrors.Add(
                        "Start position time \"to\" should be greater than time \"from\"!");
                return propertyWithErrorsList;
            });
            ValidateProperty("Positions", Positions, propertyWithErrorsList =>
            {
                if (Positions.Count == 0) propertyWithErrorsList.ListErrors.Add("Positions list should not be empty!");
                return propertyWithErrorsList;
            });
            foreach (var positionsListItemViewModel in Positions)
            {
                positionsListItemViewModel.ForceValidate();
                if (positionsListItemViewModel.HasErrors) HasErrors = true;
            }

            if (StartPositionAddressViewModel.HasErrors) HasErrors = true;
        }

        private PropertyWithErrorsList PositionsListItemTimeFromValidationFunction(
            PropertyWithErrorsList propertyWithErrorsList)
        {
            if ((LocalTime) propertyWithErrorsList.Property < StartPositionTimeFrom)
                propertyWithErrorsList.ListErrors.Add(
                    "Position time \"from\" should be greater or equal to time \"from\" of the start position!");

            if ((LocalTime) propertyWithErrorsList.Property > StartPositionTimeTo)
                propertyWithErrorsList.ListErrors.Add(
                    "Position time \"from\" should be less than time \"to\" of the start position!");

            return propertyWithErrorsList;
        }

        private PropertyWithErrorsList PositionsListItemTimeToValidationFunction(
            PropertyWithErrorsList propertyWithErrorsList)
        {
            if ((LocalTime) propertyWithErrorsList.Property > StartPositionTimeTo)
                propertyWithErrorsList.ListErrors.Add(
                    "Position time \"to\" should be less than time \"to\" of the start position!");

            if ((LocalTime) propertyWithErrorsList.Property < StartPositionTimeFrom)
                propertyWithErrorsList.ListErrors.Add(
                    "Position time \"to\" should be greater or equal to time \"from\" of the start position!");

            return propertyWithErrorsList;
        }

        public class PositionsListItemViewModel : BaseViewModel
        {
            private readonly Func<PropertyWithErrorsList, PropertyWithErrorsList> timeFromValidationFunction;
            private readonly Func<PropertyWithErrorsList, PropertyWithErrorsList> timeToValidationFunction;

            public PositionsListItemViewModel(Position position,
                Func<PropertyWithErrorsList, PropertyWithErrorsList> timeFromValidationFunction,
                Func<PropertyWithErrorsList, PropertyWithErrorsList> timeToValidationFunction)
            {
                Position = position;
                AddressViewModel = new AddressViewModel(new PlacesAndSearchAddressModel(position.Address));
                this.timeFromValidationFunction = timeFromValidationFunction;
                this.timeToValidationFunction = timeToValidationFunction;
                position.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };
                Validate();
                AddressViewModel.ErrorsChanged += (sender, args) => { Validate(); };
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

            protected override void Validate()
            {
                ValidateProperty("TimeFrom", TimeFrom, timeFromValidationFunction);
                ValidateProperty("TimeTo", TimeTo, timeToValidationFunction);
                if (AddressViewModel.HasErrors) HasErrors = true;
            }

            public void ForceValidate()
            {
                Validate();
            }
        }
    }
}