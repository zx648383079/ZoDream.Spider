﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    public interface IStorageProvider<FolderT, FileT, FileStreamT>
    {

        /// <summary>
        /// 文件备选保存目录
        /// </summary>
        public FolderT BaseFolder { get; set; }
        /// <summary>
        /// 根据项目文件设置保存目录
        /// </summary>
        public FileT EntranceFile { set; }
        /// <summary>
        /// 根据项目文件夹设置保存目录
        /// </summary>
        public FolderT? EntranceFolder { set; }

        public Task CreateAsync(string fileName, byte[] data);
        /// <summary>
        /// 以UTF8 no-bom 写入文本内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Task CreateAsync(string fileName, string content);

        public Task CreateAsync(UriItem uri, byte[] data);

        /// <summary>
        /// 创造写入流
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Task<FileStreamT> CreateStreamAsync(string fileName);
        /// <summary>
        /// 创造写入流，
        /// </summary>
        /// <param name="uri">已经确定的不需要走规则确定</param>
        /// <returns></returns>
        public Task<FileStreamT> CreateStreamAsync(UriItem uri);

        public string GetFileName(UriItem uri);

        public Task<FileStreamT?> OpenStreamAsync(string fileName);
        public Task<FileStreamT?> OpenStreamAsync(UriItem uri);

        public string GetAbsolutePath(string fileName);
        public string GetRelativePath(string fileName);
        public string GetRelativePath(string baseFileName, string fileName);


    }
}
