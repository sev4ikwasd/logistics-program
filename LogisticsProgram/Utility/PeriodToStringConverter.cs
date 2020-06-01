using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NodaTime;
using NodaTime.Text;

namespace LogisticsProgram
{
    public class PeriodToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var period = (Period) value;
                return period.Minutes.ToString();
            }
            catch (UnparsableValueException)
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var speriod = (string) value;
                var intperiod = int.Parse(speriod);
                return Period.FromMinutes(intperiod);
            }
            catch (FormatException)
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}