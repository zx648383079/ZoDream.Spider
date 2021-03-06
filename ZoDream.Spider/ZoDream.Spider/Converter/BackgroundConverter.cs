﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using ZoDream.Spider.Model;

namespace ZoDream.Spider.Converter
{
    public class BackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((UrlStatus)value)
            {
                
                case UrlStatus.Waiting:
                    return new SolidColorBrush(Colors.Yellow);
                case UrlStatus.Success:
                    return new SolidColorBrush(Colors.Green);
                case UrlStatus.Failure:
                    return new SolidColorBrush(Colors.Red);
                default:
                    return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
