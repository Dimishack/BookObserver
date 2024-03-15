using BookObserver.Infrastructure.Converters.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace BookObserver.Infrastructure.Converters
{
    internal class BoolToStringCollectionStockConverter : Converter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ListCollectionView listBool) return null;
            var test = new BoolToStringStockConverter();
            IList listString = new List<string>();
            foreach (var item in listBool.SourceCollection)
                listString.Add(test.Convert(item, typeof(bool), null , culture));
            return listString;
        }
    }
}
