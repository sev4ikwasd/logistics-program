using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace LogisticsProgram
{
    public abstract class BaseAddressModel : BindableBase
    {
        public BaseAddressModel(Address address)
        {
            Address = address;
            if (string.IsNullOrWhiteSpace(Address.AddressValue))
                IsAddressValid = false;
            Address.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };
            Address.PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName.Equals("StringAddressValue"))
                    await GetAddress(Address.StringAddressValue);
            };
            AddressVariants.CollectionChanged += (s, e) => { RaisePropertyChanged("AddressVariants"); };
        }

        public Address Address { get; }

        private bool isAddressValid = true;
        public bool IsAddressValid
        {
            get => isAddressValid;
            protected set
            {
                isAddressValid = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<AddressVariant> AddressVariants { get; } =
            new /*Async*/ObservableCollection<AddressVariant>();

        public abstract Task GetAddress(string search);

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