﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using NodaTime;
using NodaTime.Text;

namespace LogisticsProgram
{
    class LocalTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var time = (LocalTime)value;
                return time.ToString("HH:mm", CultureInfo.InvariantCulture);
            }
            catch (System.InvalidCastException)
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var stime = (String)value;
                return LocalTimePattern.Create("HH:mm", CultureInfo.InvariantCulture).Parse(stime).Value;
            }
            catch (UnparsableValueException)
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
