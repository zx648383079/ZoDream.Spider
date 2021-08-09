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
        private string title;

        public string Title
        {
            get => title;
            set => Set(ref title, value);
        }

        public string Source { get; set; }

        private UriStatus status = UriStatus.NONE;

        public UriStatus Status
        {
            get => status;
            set => Set(ref status, value);
        }

        private string message;

        public string Message
        {
            get => message;
            set => Set(ref message, value);
        }

        public UriType Type { get; set; } = UriType.File;

    }
}
