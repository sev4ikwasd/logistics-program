using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace LogisticsProgram
{
    public class OrdinalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var lvi = value as ListViewItem;
            var ordinal = 0;

            if (lvi != null)
            {
                var lv = ItemsControl.ItemsControlFromItemContainer(lvi) as ListView;
                ordinal = lv.ItemContainerGenerator.IndexFromContainer(lvi) + 1;
            }

            return ordinal.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}