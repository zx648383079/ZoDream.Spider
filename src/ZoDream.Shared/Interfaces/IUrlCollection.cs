﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    /// <summary>
    /// 这是转载所有网址的容器，用于验证网址是否重复
    /// </summary>
    public interface IUrlCollection: IEnumerable<UriItem>, ILoader
    {

        public void Add(string url);

        public void Remove(string url);

        public bool Contains(string url);
    }
}
