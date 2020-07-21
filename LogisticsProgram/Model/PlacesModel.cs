using System.Collections.ObjectModel;
using System.Data.Entity;
using Prism.Mvvm;

namespace LogisticsProgram
{
    public class PlacesModel : BindableBase
    {
        private readonly DatabaseContext db;

        public PlacesModel()
        {
            db = new DatabaseContext();
        }

        public ObservableCollection<Place> Places { get; private set; }

        public async void Initialize()
        {
            await db.Places.LoadAsync();
            await db.Addresses.LoadAsync();
            Places = new ObservableCollection<Place>(db.Places.Local.ToBindingList());
        }
        /*public async void AddPlace(Place place)
        {
            Places.Add(place);
            db.Places.Add(place);
            await db.SaveChangesAsync();
        }*/

        public async void AddOrUpdatePlace(Place place)
        {
            if (Places.Contains(place))
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