using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NodaTime;
using NodaTime.Text;

namespace LogisticsProgram
{
    internal class LocalTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var time = (LocalTime) value;
                return time.ToString("HH:mm", CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException)
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var stime = (string) value;
                return LocalTimePattern.Create("HH:mm", CultureInfo.InvariantCulture).Parse(stime).Value;
            }
            catch (UnparsableValueException)
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}