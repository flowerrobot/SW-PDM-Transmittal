using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace TransmittalManager.Converters
{
    public class IssueTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Unknown";
            return ((IssueType)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string)) return value;

            foreach (IssueType issueType in IssueTypes)
            {
                if (issueType.ToString() == (string)value )
                {
                    return issueType;
                }
            }
            return IssueType.Unknown;

        }

        public static List<IssueType> IssueTypes => new List<IssueType>(new[] { IssueType.ForManufacture, IssueType.ForInformation, IssueType.ForApproval });
    }
}
