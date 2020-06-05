using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;

namespace LogisticsProgram
{
    public class PlacesViewModel : BindableBase
    {
        private readonly PlacesModel model = new PlacesModel();

        private Place selectedPlace;
        private AddressViewModel selectedPlaceAddress = new AddressViewModel(new SearchAddressModel(new Address()));
        private string selectedPlaceName = "";
        private bool selectedPlaceVisible;

        public PlacesViewModel()
        {
            AddPlaceCommand = new DelegateCommand(() =>
            {
                var place = new Place();
                //model.AddPlace(place);
                SelectedPlace = place;
            });
            PlaceSelectedCommand = new DelegateCommand<Place>(place => { SelectedPlace = place; });
            SaveSelectedPlaceCommand = new DelegateCommand(() =>
            {
                SelectedPlace.Name = SelectedPlaceName;
                SelectedPlace.Address = selectedPlaceAddress.Address;
                model.AddPlace(SelectedPlace);
                //model.UpdatePlaces(SelectedPlace);
            });
            DeleteSelectedPlaceCommand = new DelegateCommand(() =>
            {
                model.DeletePlace(SelectedPlace);
                SelectedPlace = null;
            });
        }

        public ObservableCollection<Place> Places
        {
            get => model.Places;
            set { }
        }

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
                    RaisePropertyChanged();

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
    }
}