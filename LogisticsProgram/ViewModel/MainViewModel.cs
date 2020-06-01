namespace LogisticsProgram
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            RoutesViewModel = new RoutesViewModel();
            PlacesViewModel = new PlacesViewModel();
        }

        public RoutesViewModel RoutesViewModel { get; }

        public PlacesViewModel PlacesViewModel { get; }
    }
}