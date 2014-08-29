/*********************************************
 *                                           *
 * Copyright （C） 2014-2014 zhangkaixuan    *
 * All rights reserved                       *
 * Project Name : myclouddisk                *
 * Create Time : 2014-08-13                  *
 * Author : zhangkaixuan                     *
 * Contact Author : zhangkxuan@gmail.com     *
 * Version : v1.0                            *
 *                                           *
 * ******************************************/
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;

namespace myclouddisk
{
    /// <summary>
    /// 监控类，监被监视的文件夹中发生的操作
    /// </summary>
    class Monitor
    {
        private static Log log = new Log("log\\monitor.log");
        /// <summary>
        /// 启动监控
        /// </summary>
        /// <param name="path">待监控的文件绝对路径</param>
        internal static void startUp(String path)
        {
            MyFileSystemWather myWather;

            if (path == "")
            {
                myWather = new MyFileSystemWather(@"D:\test", "");
            }
            else
                myWather = new MyFileSystemWather(path, "");

            myWather.OnChanged += new FileSystemEventHandler(OnChanged);
            myWather.OnCreated += new FileSystemEventHandler(OnCreated);
            myWather.OnRenamed += new RenamedEventHandler(OnRenamed);
            myWather.OnDeleted += new FileSystemEventHandler(OnDeleted);
            myWather.Start();
            //Console.ReadKey();
        }
        /// <summary>
        /// 启动监视器
        /// </summary>
        /// <param name="path">待监控的文件绝对路径</param>
        /// <param name="filter">指定那些操作类型被监控</param>
        private static void WatcherStrat(string path, string filter)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.Filter = filter;
            watcher.Changed += new FileSystemEventHandler(OnProcess);
            watcher.Created += new FileSystemEventHandler(OnProcess);
            watcher.Deleted += new FileSystemEventHandler(OnProcess);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.EnableRaisingEvents = true;

        }
        /// <summary>
        /// 捕获系统发生的动作
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e">产生的文件系统事件，由系统提供</param>
        private static void OnProcess(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                OnCreated(source, e);
            }

            else if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                OnChanged(source, e);
            }

            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                OnDeleted(source, e);
            }

        }
        /// <summary>
        /// 处理新建事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e">文件系统事件</param>
        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            if (HasSpecidirListString(e.Name))
                return;
            FileEvent eve = new FileEvent();
            eve.FileType = FileType.OBJECT;
            eve.GenerateTime = DateTime.Now;
            eve.NewFullPath = e.FullPath;
            eve.NewName = e.Name;
            eve.OpertionType = OperationType.CREATE;
            eve.FileType = GetFileType(e.FullPath);
            FileType type = eve.FileType;
           
            log.WriteLine(eve.GenerateTime.ToUniversalTime() +" [new a " + type + "] " + e.FullPath);

            Program.eventBuffer.Enqueue(eve);
            //Console.WriteLine(eve.GenerateTime.ToUniversalTime()+" [new a " + type + "] " + e.FullPath);
        }
        /// <summary>
        /// 处理更改事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            if (HasSpecidirListString(e.Name))
            {
                return;
            }
            if (!Directory.Exists(e.FullPath))
            {
                FileEvent eve = new FileEvent();

                eve.FileType = FileType.FILE;
                eve.GenerateTime = DateTime.Now;
                eve.NewFullPath = e.FullPath;
                eve.NewName = e.Name;
                eve.OpertionType = OperationType.MODIFY;
                log.WriteLine(eve.GenerateTime.ToUniversalTime()+" [modify a FILE] " + e.FullPath);
                Program.eventBuffer.Enqueue(eve);

                //Console.WriteLine(eve.GenerateTime.ToUniversalTime()+" [modify a FILE]"+e.FullPath);

            }
        }
        /// <summary>
        /// 处理删除事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            if (HasSpecidirListString(e.Name))
                return;

            FileEvent eve = new FileEvent();
            eve.FileType = FileType.OBJECT;
            eve.GenerateTime = DateTime.Now;
            eve.NewFullPath = e.FullPath;
            eve.NewName = e.Name;
            eve.OpertionType = OperationType.DELETE;
            log.WriteLine(eve.GenerateTime.ToUniversalTime()+" [delete an OBJECT] " + e.FullPath);
            Program.eventBuffer.Enqueue(eve);

            //Console.WriteLine(eve.GenerateTime.ToUniversalTime()+" [delete an OBJECT] " + e.FullPath);
        
        }
        /// <summary>
        /// 处理重命名事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnRenamed(object source, RenamedEventArgs e)
        {

            FileEvent eve = new FileEvent();
           
            if (DocumentIsChanged(e.OldName))
            {                
                eve.NewFullPath = e.FullPath;
                eve.OldFullPath = e.OldFullPath;
                eve.NewName = e.Name;
                eve.OldName = e.OldName;
                eve.GenerateTime = DateTime.Now;
                eve.OpertionType = OperationType.RENAME;
                eve.FileType = GetFileType(e.FullPath);
                FileType type = eve.FileType;

                log.WriteLine(eve.GenerateTime.ToUniversalTime() +" [rename a " + type + "] " + e.OldFullPath + " [rename as] " + e.FullPath);
                Program.eventBuffer.Enqueue(eve);

                //Console.WriteLine(eve.GenerateTime.ToUniversalTime() +" [rename a " + type + "] " + e.OldFullPath + " [rename as] " + e.FullPath);
                return;
            }
            if (DocumentIsChanged(e.Name))
            {
                return;
            }
            else
            {
                eve.NewFullPath = e.FullPath;
                eve.OldFullPath = e.OldFullPath;
                eve.NewName = e.Name;
                eve.OldName = e.OldName;
                eve.GenerateTime = DateTime.Now;
                eve.OpertionType = OperationType.RENAME;
                eve.FileType = GetFileType(e.FullPath);
                FileType type = eve.FileType;

                log.WriteLine(eve.GenerateTime.ToUniversalTime()+" [rename a " + type + "] " + e.OldFullPath + " [rename as] " + e.FullPath);
                Program.eventBuffer.Enqueue(eve);
                //Console.WriteLine(eve.GenerateTime.ToUniversalTime()+" [rename a " + type + "] " + e.OldFullPath + " [rename as] " + e.FullPath);

            }

        }
        /// <summary>
        /// 获取文件类型，这里指file和directory
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        private static FileType GetFileType(string fullpath)
        {
            //Console.WriteLine(fullpath);
            if (File.Exists(fullpath))
            {
                return FileType.FILE;
            }
            else if (Directory.Exists(fullpath))
            {
                return FileType.DIRECTORY;
            }
            else
            {
                return FileType.NONE;
            }
        }
        /// <summary>
        /// 判断给定的文件是不是特殊文件，即一些临时文件
        /// </summary>
        /// <param name="path">文件绝对路径</param>
        /// <returns></returns>
        private static bool HasSpecidirListString(string path)
        {

            Regex reg2 = new Regex("~$*");
            Regex reg1 = new Regex("~*.tmp");

            if (reg2.IsMatch(path) || reg1.IsMatch(path))
            {
                return true;
            }
            else return false;

        }
        /// <summary>
        /// 判断文件是否发生改变，主要针对办公文档
        /// </summary>
        /// <param name="name">文件绝对路径</param>
        /// <returns></returns>
        private static bool DocumentIsChanged(string name)
        {
            Regex reg1 = new Regex("~*.tmp");
            if (reg1.IsMatch(name))
            {
                //Console.WriteLine("文件已经发生改变 ");
                return true;
            }
            else
                return false;

        }
      
    }
}
