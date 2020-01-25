using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Prism.Mvvm;
using Prism.Commands;
using NodaTime;
using NodaTime.Text;

namespace LogisticsProgram
{
    class MainViewModel : BindableBase
    {
        private readonly MainModel model = new MainModel();

        public String StartAddress
        {
            get
            {
                return model.StartAddress;
            }
            set
            {
                model.StartAddress = value;
            }
        }
        public LocalTime StartTime
        {
            get
            {
                return model.StartTime;
            }
            set
            {
                model.StartTime = value;
            }
        }
        public ObservableCollection<Position> Positions
        {
            get
            {
                return model.Positions;
            }
            set
            {
                
            }
        }
        public Route Route
        {
            get
            {
                return model.Route;
            }
            set
            {

            }
        }
        public Boolean RouteVisible;

        public MainViewModel()
        {//TODO validation
            model.PropertyChanged += (s, e) => {
                RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName.Equals("Route"))
                {
                    if ((s != null) && (((MainModel)s).Route.Positions.Count != 0))
                    {
                        RouteVisible = true;
                    }
                    else
                    {
                        RouteVisible = false;
                    }
                }
            };

            AddPositionCommand = new DelegateCommand(() => {
                model.Positions.Add(new Position());
            });
            RemovePositionCommand = new DelegateCommand<Position>((item) => {
                model.Positions.Remove(item);
            });
            GenerateRouteCommand = new DelegateCommand(() => {
                model.GenerateRoute();
            });
        }
        public DelegateCommand AddPositionCommand { get; }
        public DelegateCommand<Position> RemovePositionCommand { get; }
        public DelegateCommand GenerateRouteCommand { get; }
    }
}
