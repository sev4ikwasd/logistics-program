using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;

namespace LogisticsProgram
{
    public class AddressViewModel : BaseViewModel
    {
        private readonly BaseAddressModel model;

        public AddressViewModel(BaseAddressModel model)
        {
            this.model = model;
            model.PropertyChanged += (s, e) =>
            {
                RaisePropertyChanged(e.PropertyName);
            };
            Validate();
            AddressChosenCommand = new DelegateCommand<BaseAddressModel.AddressVariant>(addressVariant =>
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
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<BaseAddressModel.AddressVariant> AddressVariants => model.AddressVariants;

        public DelegateCommand<BaseAddressModel.AddressVariant> AddressChosenCommand { get; }
        protected override void Validate()
        {
            ValidateProperty("StringAddressValue", StringAddressValue, propertyWithErrorsList =>
            {
                if(!model.IsAddressValid)
                    propertyWithErrorsList.ListErrors.Add("Address is not valid!");
                return propertyWithErrorsList;
            });
        }
    }
}