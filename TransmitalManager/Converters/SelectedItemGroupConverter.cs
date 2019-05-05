using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TransmittalManager.ViewModel;

namespace TransmittalManager.Converters
{
    public class SelectedItemGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RecipientViewModel rvm)
            {
                if (rvm.IsSelected) return " True";
                return "False";
            }
            return "Unkown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
