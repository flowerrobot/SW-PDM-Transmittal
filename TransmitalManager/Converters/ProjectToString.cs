using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TransmittalManager.Models;

namespace TransmittalManager.Converters
{
    public class ProjectToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Project prj)
            {
                return $"{prj.NumberStr} {prj.Name} ";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string val)
            {
                var num = int.Parse(val.Split(' ').First());
                return Project.AllProjects.FirstOrDefault(t => t.Number == num);
            }
            return null;
        }
    }
}
