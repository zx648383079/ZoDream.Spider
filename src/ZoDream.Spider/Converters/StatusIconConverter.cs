using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.Converters
{
    public class StatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UriCheckStatus o)
            {
                return o switch
                {
                    UriCheckStatus.Waiting => "\uE916",
                    UriCheckStatus.Doing => "\uE712",
                    UriCheckStatus.Done => "\uE930",
                    UriCheckStatus.Error => "\uE7BA",
                    _ => string.Empty
                };
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
