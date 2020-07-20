using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows;

namespace LogisticsProgram
{
    public class PlacesAndSearchAddressModel : BaseAddressModel
    {
        private readonly DatabaseContext db;

        public PlacesAndSearchAddressModel(Address address) : base(address)
        {
            db = new DatabaseContext();
        }

        public override async Task GetAddress(string search)
        {
            //Bad hack but AsyncObservableCollections gives untrackable exceptions
            await Application.Current.Dispatcher.BeginInvoke((Action) delegate { AddressVariants.Clear(); });
            await db.Places.LoadAsync();
            await db.Addresses.LoadAsync();
            var places = new ObservableCollection<Place>(db.Places.Local.ToBindingList());
            foreach (var place in places)
                if (place.Name.ToLower().Contains(search.ToLower()) || place.Address.StringAddressValue.ToLower().Contains(search.ToLower()))
                    await Application.Current.Dispatcher.BeginInvoke((Action) delegate
                    {
                        AddressVariants.Add(new AddressVariant($"{place.Name} ({place.Address.StringAddressValue})",
                            place.Address.AddressValue));
                    });
            try
            {
                var addresses = await ApiUtility.GetInstance().GetSearchedAddresses(search);
                if (addresses.Count == 0)
                    IsAddressValid = false;
                else
                    await Application.Current.Dispatcher.BeginInvoke((Action) delegate
                    {
                        foreach (var address in addresses) AddressVariants.Add(address);
                    });
            }
            catch (Exception)
            {
                IsAddressValid = false;
            }
        }
    }
}