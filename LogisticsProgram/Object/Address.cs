using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Prism.Mvvm;
using SimpleJSON;

namespace LogisticsProgram
{
    public class Address : BindableBase, IDataErrorInfo
    {
        private bool isAddressValid = true;

        public bool IsAddressValid
        {
            get
            {
                return isAddressValid;

            }
        }

        private String stringAddressValue = "";
        public String StringAddressValue
        {
            get
            {
                return stringAddressValue;
            }
            set
            {
                stringAddressValue = value;
                Task.Run(() => GetAddressFromApi(value));
            }
        }

        private String addressValue = "";
        public String AddressValue
        {
            get
            {
                return addressValue;
            }
            set
            {
                Regex latlon = new Regex(@"(^(([-]?)\d+(\.\d+)),(([-]?)\d+(\.\d+))$)");
                if (latlon.IsMatch(value))
                    addressValue = value;
                else
                    throw new FormatException("Given string isn't coordinates");
            }
        }

        private ObservableCollection<AddressVariant> addressVariants = new /*Async*/ObservableCollection<AddressVariant>();

        public ObservableCollection<AddressVariant> AddressVariants
        {
            get
            {
                return addressVariants;
            }
        }

        private async Task GetAddressFromApi(String value)
        {
            //Really bad hack
            await App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                addressVariants.Clear();
            });
            try
            {
                AsyncObservableCollection<AddressVariant> addresses = await ApiUtility.GetInstance().GetSearchedAddresses(value);
                if (addresses.Count == 0)
                {
                    
                    //addressVariants.Clear();
                    isAddressValid = false;
                }
                else
                {
                    await App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        foreach (AddressVariant address in addresses)
                        {
                            addressVariants.Add(address);
                        }
                    });
                    /*addressVariants.Clear();
                    foreach (AddressVariant address in addresses)
                    {
                        addressVariants.Add(address);
                    }*/
                }
            }
            catch (Exception)
            {
                //addressVariants.Clear();
                isAddressValid = false;
            }
            RaisePropertyChanged("StringAddressValue");
            RaisePropertyChanged("AddressVariants");
        }

        public void SetAddressVariant(AddressVariant addressVariant)
        {
            isAddressValid = true;
            stringAddressValue = addressVariant.StringAddressValue;
            addressValue = addressVariant.AddressValue;
            RaisePropertyChanged("StringAddressValue");
            RaisePropertyChanged("AddressVariants");
            addressVariants.Clear();
        }

        public class AddressVariant
        {
            public String StringAddressValue { get; set; }
            public String AddressValue { get; set; }

            public AddressVariant(String stringAddressValue, String addressValue)
            {
                StringAddressValue = stringAddressValue;
                AddressValue = addressValue;
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "StringAddressValue":
                        if (!isAddressValid)
                            return "Address is invalid";
                        break;
                }

                return null;
            }
        }

        public string Error { get; }
    }
}
