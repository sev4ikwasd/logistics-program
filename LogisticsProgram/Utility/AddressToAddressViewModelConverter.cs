using System;
using System.Globalization;
using System.Windows.Data;

namespace LogisticsProgram
{
    public class AddressToAddressViewModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var address = (Address) value;
            return new AddressViewModel(address);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}