﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using SimpleJSON;

namespace LogisticsProgram
{
    public class Address : BindableBase, IDataErrorInfo
    {
        private bool isAddressValid = true;

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
        }

        private AsyncObservableCollection<AddressVariant> addressVariants = new AsyncObservableCollection<AddressVariant>();

        public AsyncObservableCollection<AddressVariant> AddressVariants
        {
            get
            {
                return addressVariants;
            }
        }

        private async Task GetAddressFromApi(String value)
        {
            var encValue = WebUtility.UrlEncode(value);
            HttpResponseMessage response = await ApiUtility.GetInstance().GetSearchedAddresses(encValue);
            if (response.IsSuccessStatusCode)
            {
                String strResponse = await response.Content.ReadAsStringAsync();
                var N = JSON.Parse(strResponse);
                if (N["results"].Count > 0)
                {
                    addressVariants.Clear();
                    //isAddressValid = true;
                    for (int i = 0; i < N["results"].Count; i++)
                    {
                        String freeFormAddress = N["results"][i]["address"]["freeformAddress"];
                        String lat = N["results"][i]["position"]["lat"].Value;
                        String lon = N["results"][i]["position"]["lon"].Value;
                        var address = $"{lat},{lon}";
                        addressVariants.Add(new AddressVariant(freeFormAddress, address));
                    }
                }
                else
                {
                    addressVariants.Clear();
                    isAddressValid = false;
                }
                RaisePropertyChanged("StringAddressValue");
                RaisePropertyChanged("AddressVariants");
            }
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

        public class AddressException : Exception
        {
            public AddressException(string message) : base(message) { }
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
