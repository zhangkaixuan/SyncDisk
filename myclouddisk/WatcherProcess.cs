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
using System.IO;
namespace myclouddisk
{
    /// <summary>
    /// 监控器，为了多线程监控
    /// </summary>
    public class WatcherProcess
    {
        private object sender;
        private object eParam;
        public event RenamedEventHandler OnRenamed;
        public event FileSystemEventHandler OnChanged;
        public event FileSystemEventHandler OnCreated;
        public event FileSystemEventHandler OnDeleted;
        public event Completed OnCompleted;
        public WatcherProcess(object sender, object eParam)
        {
            this.sender = sender;
            this.eParam = eParam;
        }
        public void Process()
        {
            if (eParam.GetType() == typeof(RenamedEventArgs))
            {
                OnRenamed(sender, (RenamedEventArgs)eParam);
                OnCompleted(((RenamedEventArgs)eParam).FullPath);
            }
            else
            {
                FileSystemEventArgs e = (FileSystemEventArgs)eParam;
                if (e.ChangeType == WatcherChangeTypes.Created)
                {
                    OnCreated(sender, e);
                    OnCompleted(e.FullPath);
                }

                else if (e.ChangeType == WatcherChangeTypes.Changed)
                {
                    OnChanged(sender, e);
                    OnCompleted(e.FullPath);
                }
                else if (e.ChangeType == WatcherChangeTypes.Deleted)
                {
                    OnDeleted(sender, e);
                    OnCompleted(e.FullPath);
                }
                else
                {
                    OnCompleted(e.FullPath);
                }

            }

        }

    }
}
