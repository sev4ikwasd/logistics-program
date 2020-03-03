using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogisticsProgram
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Positions_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - 2 * SystemParameters.VerticalScrollBarWidth;
            var col2 = 80;
            var col3 = 80;
            var col4 = 20;
            var col1 = workingWidth - col2 - col3 - col4;

            gView.Columns[0].Width = col1;
            gView.Columns[1].Width = col2;
            gView.Columns[2].Width = col3;
            gView.Columns[3].Width = col4;
        }

        private void Route_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - 2 * SystemParameters.VerticalScrollBarWidth;
            var col1 = 20;
            var col3 = 80;
            var col4 = 80;
            var col2 = workingWidth - col1 - col3 - col4;

            gView.Columns[0].Width = col1;
            gView.Columns[1].Width = col2;
            gView.Columns[2].Width = col3;
            gView.Columns[3].Width = col4;
        }
    }
}
