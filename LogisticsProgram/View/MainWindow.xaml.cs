using System.Windows;
using System.Windows.Controls;

namespace LogisticsProgram
{
    /// <summary>
    ///     Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Positions_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var listView = sender as ListView;
            var gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - 2 * SystemParameters.VerticalScrollBarWidth;
            var col2 = 80;
            var col3 = 80;
            var col4 = 20;
            var col1 = workingWidth - col2 - col3 - col4;

            if (col1 < 0) col1 = 0;

            gView.Columns[0].Width = col1;
            gView.Columns[1].Width = col2;
            gView.Columns[2].Width = col3;
            gView.Columns[3].Width = col4;
        }

        private void Route_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var listView = sender as ListView;
            var gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - 2 * SystemParameters.VerticalScrollBarWidth;
            var col1 = 20;
            var col3 = 80;
            var col4 = 80;
            var col2 = workingWidth - col1 - col3 - col4;

            if (col2 < 0) col2 = 0;

            gView.Columns[0].Width = col1;
            gView.Columns[1].Width = col2;
            gView.Columns[2].Width = col3;
            gView.Columns[3].Width = col4;
        }
    }
}