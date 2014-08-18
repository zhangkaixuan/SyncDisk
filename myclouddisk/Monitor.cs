using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;

namespace myclouddisk
{
    class Monitor
    {
        public Log log;
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

            string type = "object";
            if (GetFileType(e.FullPath) == 1)
            {
                type = "file";
                eve.FileType = FileType.FILE;
            }
            if (GetFileType(e.FullPath) == 0)
            {
                type = "directory";
                eve.FileType = FileType.DIRECTORY;
            }
            
            Log log = new Log("log.txt");
            log.WriteLine("[new a " + type + "] " + e.FullPath);
            Program.eventBuffer.Enqueue(eve);
            Console.WriteLine("[new a " + type + "] " + e.FullPath);
        }

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
                Log log = new Log("log.txt");
                log.WriteLine("[modify a file] " + e.FullPath);
                Program.eventBuffer.Enqueue(eve);

                Console.WriteLine("[modify a file]"+e.FullPath);

            }
        }
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
            Log log = new Log("log.txt");
            log.WriteLine("[delete an object] " + e.FullPath);
            Program.eventBuffer.Enqueue(eve);

            Console.WriteLine("[delete an object] " + e.FullPath);
        
        }
        private static void OnRenamed(object source, RenamedEventArgs e)
        {

            FileEvent eve = new FileEvent();
           
            if (DocumentIsChanged(e.OldName))
            {
                string type;
                if (GetFileType(e.FullPath) == 1)
                {
                    eve.FileType = FileType.FILE;
                    type = "file";
                }
                else if (GetFileType(e.FullPath) == 0)
                {
                    eve.FileType = FileType.DIRECTORY;
                    type = "directory";
                }
                else type = "object";

                eve.NewFullPath = e.FullPath;
                eve.OldFullPath = e.OldFullPath;
                eve.NewName = e.Name;
                eve.OldName = e.OldName;
                eve.GenerateTime = DateTime.Now;
                eve.OpertionType = OperationType.RENAME;
                eve.FileType = FileType.FILE;
                Log log = new Log("log.txt");
                log.WriteLine("[rename a " + type + "] " + e.OldFullPath + " [rename as] " + e.FullPath);
                Program.eventBuffer.Enqueue(eve);

                Console.WriteLine("[rename a " + type + "] " + e.OldFullPath + " [rename as] " + e.FullPath);
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
                eve.FileType = FileType.OBJECT;
                
                string type;
                if (GetFileType(e.FullPath) == 1)
                {
                    eve.FileType = FileType.FILE;
                    type = "file";
                }
                else if (GetFileType(e.FullPath) == 0)
                {
                    eve.FileType = FileType.DIRECTORY;
                    type = "directory";
                }
                else type = "object";
                Log log = new Log("log.txt");
                log.WriteLine("[rename a " + type + "] " + e.OldFullPath + " [rename as] " + e.FullPath);
                Program.eventBuffer.Enqueue(eve);
                Console.WriteLine("[rename a " + type + "] " + e.OldFullPath + " [rename as] " + e.FullPath);

            }


        }
        private static int GetFileType(string fullpath)
        {
            //Console.WriteLine(fullpath);
            if (File.Exists(fullpath))
            {
                return 1;
            }
            else if (Directory.Exists(fullpath))
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
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
