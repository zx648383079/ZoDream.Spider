using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.Converters
{
    public class StatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case UriStatus.DOING:
                    return "pack://application:,,,/Assets/wait.png";
                case UriStatus.DONE:
                    return "pack://application:,,,/Assets/success.png";
                case UriStatus.ERROR:
                    return "pack://application:,,,/Assets/failure.png";
                default:
                    return "pack://application:,,,/Assets/none.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
