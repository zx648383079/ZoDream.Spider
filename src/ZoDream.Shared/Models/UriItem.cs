using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Shared.Models
{
    public class UriItem: BindableBase
    {
        private string title = string.Empty;

        public string Title
        {
            get => title;
            set => Set(ref title, value);
        }

        public string Source { get; set; } = string.Empty;

        private UriStatus status = UriStatus.NONE;

        public UriStatus Status
        {
            get => status;
            set => Set(ref status, value);
        }

        private string message = string.Empty;

        public string Message
        {
            get => message;
            set => Set(ref message, value);
        }

        public UriType Type { get; set; } = UriType.File;
        public string FormatTip { 
            get
            {
                return $"{FormatStatus}: {Source}";
            }
        }

        public string FormatStatus
        {
            get
            {
                switch (Status)
                {
                    case UriStatus.DOING:
                        return "作业中";
                    case UriStatus.DONE:
                        return "完成作业";
                    case UriStatus.ERROR:
                        return "作业失败";
                    default:
                        return "等待中";
                }
            }
        }
    }
}
