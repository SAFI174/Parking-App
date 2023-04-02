using System;
using System.Windows.Data;

namespace ParkingApp.Classes
{
    public class EmptyTextBoxValidation : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            foreach (var item in values)
            {
                if (string.IsNullOrEmpty(item.ToString())) return false;
            }

            return true;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
