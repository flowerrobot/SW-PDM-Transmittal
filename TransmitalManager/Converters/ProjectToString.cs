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
                return $"{prj.NumberStr} {prj.Name}";
            }
            else if(value is List<Project> prjs)
            {
                var lPrj = new List<string>();
                foreach(var pj in prjs)
                {
                    lPrj.Add($"{pj.NumberStr} {pj.Name}");
                }
                return lPrj;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string val)
            {
                var num = int.Parse(val.Split(' ').First());
                return Project.AllProjects.FirstOrDefault(t => t.Number == num);
            }
            return value;
        }
    }
}
