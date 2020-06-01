using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Prism.Mvvm;

namespace LogisticsProgram
{
    public class AddressModel : BindableBase
    {
        public AddressModel(Address address)
        {
            Address = address;
            Address.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };
            Address.PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName.Equals("StringAddressValue"))
                    await GetAddressFromApi(Address.StringAddressValue);
            };
            AddressVariants.CollectionChanged += (s, e) => { RaisePropertyChanged("AddressVariants"); };
        }

        public Address Address { get; }

        public bool IsAddressValid { get; private set; } = true;

        public ObservableCollection<AddressVariant> AddressVariants { get; } =
            new /*Async*/ObservableCollection<AddressVariant>();

        public async Task GetAddressFromApi(string value)
        {
            //Really bad hack
            await Application.Current.Dispatcher.BeginInvoke((Action) delegate { AddressVariants.Clear(); });
            try
            {
                /*Async*/
                var addresses = await ApiUtility.GetInstance().GetSearchedAddresses(value);
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

        public void SetAddressVariant(AddressVariant addressVariant)
        {
            IsAddressValid = true;
            Address.StringAddressValue = addressVariant.StringAddressValue;
            Address.AddressValue = addressVariant.AddressValue;
            AddressVariants.Clear();
        }

        public class AddressVariant
        {
            public AddressVariant(string stringAddressValue, string addressValue)
            {
                StringAddressValue = stringAddressValue;
                AddressValue = addressValue;
            }

            public string StringAddressValue { get; set; }
            public string AddressValue { get; set; }
        }
    }
}