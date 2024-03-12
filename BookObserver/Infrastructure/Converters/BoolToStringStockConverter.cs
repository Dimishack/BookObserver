using BookObserver.Infrastructure.Converters.Base;
using System;
using System.Globalization;

namespace BookObserver.Infrastructure.Converters
{
    internal class BoolToStringStockConverter : Converter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool isStock) return null;
            return isStock? "да" : "нет";
        }
    }
}
