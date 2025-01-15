using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZoDream.Shared.IO
{
    /// <summary>
    /// 分段流合并
    /// </summary>
    public class MultipartStream : Stream
    {
        public MultipartStream(Stream output, long length)
        {
            BaseStream = output;
            SetLength(length);
            _maxLength = length;
        }

        public MultipartStream(Stream output)
        {
            BaseStream = output;
            _maxLength = output.Length;
        }

        private readonly Stream BaseStream;
        private readonly long _maxLength;

        private readonly List<Tuple<long, long>> _items = [];

        public bool IsCompleted 
        {
            get {
                var pos = 0L;
                foreach (var item in _items.OrderBy(i => i.Item1))
                {
                    if (pos < item.Item1)
                    {
                        return false;
                    }
                    pos = Math.Max(pos, item.Item1 + item.Item2);
                }
                return _maxLength == 0 || pos >= _maxLength;
            }
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => _maxLength;

        public override long Position 
        { 
            get => BaseStream.Position; 
            set => Seek(value, SeekOrigin.Begin); 
        }

        public void Write(long offset, long count, Stream input)
        {
            if (BaseStream.Length < offset)
            {
                SetLength(offset + count);
            }
            Seek(offset, SeekOrigin.Begin);
            input.Seek(0, SeekOrigin.Begin);
            input.CopyTo(BaseStream, count);
            _items.Add(new Tuple<long, long>(offset, count));
        }

        public void Write(Stream input)
        {
            Write(_maxLength == 0 ? BaseStream.Position : 0, input.Length, input);
        }

        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return BaseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            BaseStream.Dispose();
        }
    }
}
