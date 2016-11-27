using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ZoDream.Spider.Model;

namespace ZoDream.Spider.Converter
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((UrlStatus)value)
            {
                case UrlStatus.Waiting:
                    return "等待中";
                case UrlStatus.Success:
                    return "下载成功";
                case UrlStatus.Failure:
                    return "下载失败";
                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
