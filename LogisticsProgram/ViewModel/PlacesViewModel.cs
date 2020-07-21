using System.Collections.ObjectModel;
using Prism.Commands;

namespace LogisticsProgram
{
    public class PlacesViewModel : BaseViewModel
    {
        private readonly PlacesModel model = new PlacesModel();

        private Place selectedPlace;
        private AddressViewModel selectedPlaceAddress = new AddressViewModel(new SearchAddressModel(new Address()));
        private string selectedPlaceName = "";
        private bool selectedPlaceVisible;

        public PlacesViewModel()
        {
            model.Initialize();
            AddPlaceCommand = new DelegateCommand(() =>
            {
                var place = new Place();
                SelectedPlace = place;
            });
            PlaceSelectedCommand = new DelegateCommand<Place>(place => { SelectedPlace = place; });
            SaveSelectedPlaceCommand = new DelegateCommand(() =>
            {
                SelectedPlace.Name = SelectedPlaceName;
                SelectedPlace.Address = selectedPlaceAddress.Address;
                model.AddOrUpdatePlace(SelectedPlace);
            });
            DeleteSelectedPlaceCommand = new DelegateCommand(() =>
            {
                if (model.Places.Contains(SelectedPlace))
                    model.DeletePlace(SelectedPlace);
                SelectedPlace = null;
            });
        }

        public ObservableCollection<Place> Places => model.Places;

        public Place SelectedPlace
        {
            get => selectedPlace;
            set
            {
                selectedPlace = value;
                if (value != null)
                {
                    SelectedPlaceVisible = true;
                    SelectedPlaceName = value.Name;
                    SelectedPlaceAddress = new AddressViewModel(new SearchAddressModel(value.Address));
                }
                else
                {
                    SelectedPlaceVisible = false;
                    //RaisePropertyChanged();

                    SelectedPlaceName = "";
                    SelectedPlaceAddress = new AddressViewModel(new SearchAddressModel(new Address()));
                }
            }
        }

        public bool SelectedPlaceVisible
        {
            get => selectedPlaceVisible;
            set
            {
                selectedPlaceVisible = value;
                RaisePropertyChanged();
            }
        }

        public string SelectedPlaceName
        {
            get => selectedPlaceName;
            set
            {
                selectedPlaceName = value;
                RaisePropertyChanged();
            }
        }

        public AddressViewModel SelectedPlaceAddress
        {
            get => selectedPlaceAddress;
            set
            {
                selectedPlaceAddress = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand AddPlaceCommand { get; }
        public DelegateCommand<Place> PlaceSelectedCommand { get; }
        public DelegateCommand SaveSelectedPlaceCommand { get; }
        public DelegateCommand DeleteSelectedPlaceCommand { get; }

        protected override void Validate()
        {
            ValidateProperty("SelectedPlaceName", SelectedPlaceName, propertyWithErrorsList =>
            {
                if (string.IsNullOrEmpty(SelectedPlaceName))
                    propertyWithErrorsList.ListErrors.Add("Name should not be empty!");
                return propertyWithErrorsList;
            });
        }
    }
}