using ZoDream.Shared.ViewModel;

namespace ZoDream.Shared.Models
{
    public class UriLoadItem : BindableBase
    {
        private string title = string.Empty;

        public string Title
        {
            get => title;
            set => Set(ref title, value);
        }

        public string Source { get; set; } = string.Empty;

        private UriCheckStatus status = UriCheckStatus.None;

        public UriCheckStatus Status
        {
            get => status;
            set => Set(ref status, value);
        }

        private double progress;

        public double Progress {
            get => progress;
            set => Set(ref progress, value);
        }


        private string message = string.Empty;

        public string Message
        {
            get => message;
            set => Set(ref message, value);
        }

        public UriType Type { get; set; } = UriType.File;
        public string FormatTip => $"{FormatStatus}: {Source}";

        public string FormatStatus
        {
            get
            {
                return Status switch
                {
                    UriCheckStatus.Doing => "作业中",
                    UriCheckStatus.Done => "完成作业",
                    UriCheckStatus.Error => "作业失败",
                    UriCheckStatus.Jump => "跳过",
                    _ => "等待中",
                };
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is UriItem item)
            {
                return item.Source == Source;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public UriLoadItem()
        {
            
        }

        public UriLoadItem(string url)
        {
            Source = url;
        }

        public UriLoadItem(UriItem uri)
        {
            Title = uri.Title;
            Source = uri.Source;
            Status = uri.Status;
            Type = uri.Type;
            Progress = uri.Progress.Value;
        }
    }
}
