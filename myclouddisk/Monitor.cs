using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;

namespace myclouddisk
{
    class Monitor
    {
        //public Log log;
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
            eve.FileType = GetFileType(e.FullPath);
            FileType type = eve.FileType;
           
            Log log = new Log("log.txt");
            log.WriteLine(eve.GenerateTime.ToUniversalTime() +" [new a " + type + "] " + e.FullPath);
            Program.eventBuffer.Enqueue(eve);
            Console.WriteLine(eve.GenerateTime.ToUniversalTime()+" [new a " + type + "] " + e.FullPath);
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
                log.WriteLine(eve.GenerateTime.ToUniversalTime()+" [modify a FILE] " + e.FullPath);
                Program.eventBuffer.Enqueue(eve);

                Console.WriteLine(eve.GenerateTime.ToUniversalTime()+" [modify a FILE]"+e.FullPath);

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
            log.WriteLine(eve.GenerateTime.ToUniversalTime()+" [delete an OBJECT] " + e.FullPath);
            Program.eventBuffer.Enqueue(eve);

            Console.WriteLine(eve.GenerateTime.ToUniversalTime()+" [delete an OBJECT] " + e.FullPath);
        
        }
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

                Log log = new Log("log.txt");
                log.WriteLine(eve.GenerateTime.ToUniversalTime() +" [rename a " + type + "] " + e.OldFullPath + " [rename as] " + e.FullPath);
                Program.eventBuffer.Enqueue(eve);

                Console.WriteLine(eve.GenerateTime.ToUniversalTime() +" [rename a " + type + "] " + e.OldFullPath + " [rename as] " + e.FullPath);
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
                Log log = new Log("log.txt");
                log.WriteLine(eve.GenerateTime.ToUniversalTime()+" [rename a " + type + "] " + e.OldFullPath + " [rename as] " + e.FullPath);
                Program.eventBuffer.Enqueue(eve);
                Console.WriteLine(eve.GenerateTime.ToUniversalTime()+" [rename a " + type + "] " + e.OldFullPath + " [rename as] " + e.FullPath);

            }


        }
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
