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
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UriCheckStatus o)
            {
                return o switch
                {
                    UriCheckStatus.Waiting or UriCheckStatus.Doing => new SolidColorBrush(Colors.Gray),
                    UriCheckStatus.Done => new SolidColorBrush(Colors.Green),
                    UriCheckStatus.Error => new SolidColorBrush(Colors.Red),
                    _ => new SolidColorBrush(Colors.Black)
                };
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
