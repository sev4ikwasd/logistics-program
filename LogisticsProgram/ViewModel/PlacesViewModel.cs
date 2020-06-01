using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;

namespace LogisticsProgram
{
    public class PlacesViewModel : BindableBase
    {
        private readonly PlacesModel model = new PlacesModel();

        private Place selectedPlace;
        private Address selectedPlaceAddress = new Address();

        //These two variables are a buffer to prevent values from getting written in the model before user clicked save button
        private string selectedPlaceName = "";

        private bool selectedPlaceVisible;

        public PlacesViewModel()
        {
            AddPlaceCommand = new DelegateCommand(() =>
            {
                var place = new Place();
                model.AddPlace(place);
                SelectedPlace = place;
            });
            PlaceSelectedCommand = new DelegateCommand<Place>(place => { SelectedPlace = place; });
            SaveSelectedPlaceCommand = new DelegateCommand(() =>
            {
                SelectedPlace.Name = SelectedPlaceName;
                SelectedPlace.Address = selectedPlaceAddress;
                model.UpdatePlaces(SelectedPlace);
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
                    SelectedPlaceVisible = true;
                else
                    SelectedPlaceVisible = false;
                RaisePropertyChanged();

                SelectedPlaceName = value.Name;
                SelectedPlaceAddress = value.Address;
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

        public Address SelectedPlaceAddress
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