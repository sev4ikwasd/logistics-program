using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using Prism.Mvvm;

namespace LogisticsProgram
{
    public class PlacesModel : BindableBase
    {
        private readonly DatabaseContext db;
        private ObservableCollection<Place> places;

        public ObservableCollection<Place> Places
        {
            get => places;
        }

        public PlacesModel()
        {
            db = new DatabaseContext();
        }

        public async void Initialize() {
            await db.Places.LoadAsync();
            await db.Addresses.LoadAsync();
            places = new ObservableCollection<Place>(db.Places.Local.ToBindingList());
        }
        /*public async void AddPlace(Place place)
        {
            Places.Add(place);
            db.Places.Add(place);
            await db.SaveChangesAsync();
        }*/

        public async void AddOrUpdatePlace(Place place)
        {
            if (places.Contains(place))
            {
                db.Entry(place).State = EntityState.Modified;
            }
            else
            {
                Places.Add(place);
                db.Places.Add(place);
            }
            await db.SaveChangesAsync();
        }

        public async void DeletePlace(Place place)
        {
            Places.Remove(place);
            db.Places.Remove(place);
            await db.SaveChangesAsync();
        }

        /*public async void UpdatePlaces(Place place)
        {
            db.Entry(place).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }*/
    }
}