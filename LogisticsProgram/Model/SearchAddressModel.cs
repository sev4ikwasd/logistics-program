using System;
using System.Threading.Tasks;
using System.Windows;

namespace LogisticsProgram
{
    public class SearchAddressModel : BaseAddressModel
    {
        public SearchAddressModel(Address address) : base(address)
        {
        }

        public override async Task GetAddress(string search)
        {
            //Really bad hack
            await Application.Current.Dispatcher.BeginInvoke((Action) delegate { AddressVariants.Clear(); });
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