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
    public class RecipientsSelectionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is RecipientsSelectionViewModel RSVm)
            {
                string Res = "";
               foreach(var R in RSVm.AllRecipients)
                {
                    Res += R.Name + "; ";
                }
                return Res;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
