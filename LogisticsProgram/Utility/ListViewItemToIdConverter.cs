using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace LogisticsProgram
{
    class ListViewItemToIdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ListViewItem lvi = (ListViewItem)value;
            ListView lv = ItemsControl.ItemsControlFromItemContainer(lvi) as ListView;
            //int index = lv.Items.IndexOf(lvi);
            int index = 0;
            foreach (Position pos in lv.ItemsSource)
            {
                if (pos == lvi.Content)
                {
                    break;
                }
                index++;
            }
            return index;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
