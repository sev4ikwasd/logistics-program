using Prism.Mvvm;

namespace LogisticsProgram
{
    public class Place : BindableBase
    {
        private Address address = new Address();

        private string name = "";

        public Place()
        {
        }

        public Place(string name, Address address)
        {
            this.name = name;
            this.address = address;
        }

        public int Id { get; set; }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                RaisePropertyChanged();
            }
        }

        public virtual Address Address
        {
            get => address;
            set
            {
                address = value;
                RaisePropertyChanged();
            }
        }
    }
}