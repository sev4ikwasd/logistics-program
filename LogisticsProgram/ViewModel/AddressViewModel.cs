using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;

namespace LogisticsProgram
{
    public class AddressViewModel : BindableBase
    {
        private readonly AddressModel model;

        public AddressViewModel(Address address)
        {
            model = new AddressModel(address);
            model.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };
            AddressChosenCommand = new DelegateCommand<AddressModel.AddressVariant>(addressVariant =>
            {
                if (addressVariant != null) model.SetAddressVariant(addressVariant);
            });
        }

        public Address Address => model.Address;

        public string StringAddressValue
        {
            get => model.Address.StringAddressValue;
            set
            {
                model.Address.StringAddressValue = value;
                if (!model.IsAddressValid) throw new Exception("Address is not valid");
            }
        }

        public ObservableCollection<AddressModel.AddressVariant> AddressVariants => model.AddressVariants;

        public DelegateCommand<AddressModel.AddressVariant> AddressChosenCommand { get; }
    }
}