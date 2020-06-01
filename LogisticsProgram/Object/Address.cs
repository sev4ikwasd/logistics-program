using System;
using System.Text.RegularExpressions;
using Prism.Mvvm;

namespace LogisticsProgram
{
    public class Address : BindableBase
    {
        private string addressValue = "";
        private string stringAddressValue = "";

        public Address()
        {
        }

        public Address(string addressValue, string stringAddressValue)
        {
            this.addressValue = addressValue;
            this.stringAddressValue = stringAddressValue;
        }

        public int Id { get; set; }

        public string StringAddressValue
        {
            get => stringAddressValue;
            set
            {
                stringAddressValue = value;
                RaisePropertyChanged();
            }
        }

        public string AddressValue
        {
            get => addressValue;
            set
            {
                var latlon = new Regex(@"(^(([-]?)\d+(\.\d+)),(([-]?)\d+(\.\d+))$)");
                if (latlon.IsMatch(value))
                    addressValue = value;
                else
                    throw new FormatException("Given string isn't coordinates");
                RaisePropertyChanged();
            }
        }
    }
}