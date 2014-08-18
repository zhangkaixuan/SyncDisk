using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
namespace myclouddisk
{
     //private static ArrayList dirlist = new arraylist();
     //private static arraylist basedirlist = new arraylist();
     //private static string basedir;
    class EventHandler
    {
        /// <summary>
        /// 处理用户产生的文件操作事件
        /// </summary>
        public void HandleEvent()
        {
            if(Program.eventBuffer.Count()>0)
            {
                handle(Program.eventBuffer.Dequeue());  
            }
        }

        private void handle(FileEvent e1)
        {
            if(e1==null)
            {
                Console.WriteLine("已知bug:位置：EventHandler::handle(FileEvent e1) 异常,可能是内存丢失导致事件不存在，无法被处理！");
                return;
            }
            //Console.WriteLine("#############文件操作分类和文件类型：" + e1.OpertionType + "  &&  " + e1.FileType);
            if(e1.OpertionType != OperationType.CREATE)
            {
                handleEventOneByOne(e1);
                return;
            }
            if(e1.FileType !=FileType.DIRECTORY)
            {
                handleEventOneByOne(e1);
                return;
            }
          
            if (e1.FileType == FileType.DIRECTORY)//当前事件为新建目录事件
            {
                Thread.Sleep(2000);//希望给系统将发生的事件注入缓冲区？是否有效不确定
                if(Program.eventBuffer.Count == 0)//最后一个事件，缓冲区不再含有未处理事件
                {                    
                     if (Directory.GetDirectories(e1.NewFullPath).Length > 0 || Directory.GetFiles(e1.NewFullPath).Length > 0)//文件夹非空
                     {
                          Console.WriteLine("***********创造了非空的文件目录，该非空目录中的文件可能不会同步！*********" + e1.NewFullPath);
                          handleEventOneByOne(e1);
                          return;
                      }                                        
                    handleEventOneByOne(e1);
                    return;
                }
                else//缓冲区还有未处理的事件 
                {
                    FileEvent e2 = Program.eventBuffer.Dequeue();//取下一个事件

                    if ((e2.NewFullPath.IndexOf(e1.NewFullPath) > -1) && e2.OpertionType == OperationType.CREATE)//有后续create事件，说明不是文件夹内的拖拽、移动或粘贴
                    {
                        handleEventOneByOne(e1);
                        handleEventOneByOne(e2);
                        return;
                    }
                    else
                    {
                        Console.WriteLine("可能发生了同一驱动器内的剪切、拖拽动作,目录为：" + e1.NewFullPath);
                        if (Directory.GetDirectories(e1.NewFullPath).Length == 0 && Directory.GetFiles(e1.NewFullPath).Length == 0)//文件夹非空
                        {
                            Console.WriteLine("虽然发生了拖拽事件，但是目录是空的，我们可以忽略之" + e1.NewFullPath);
                            return;
                        }
                        handleEventOneByOne(e1);
                        {
                            //
                            //@TODO
                            //处理遗漏的操作，即将该非空目录下
                            //针对同一驱动器有效，对不同驱动器可能会有冗余产生。
                            //
                        }
                        handleEventOneByOne(e1);
                        handleEventOneByOne(e2);
                    }

                }
  
            }
        }
        private void handleEventOneByOne(FileEvent e)
        {
            //Thread.Sleep(1000);
            switch (e.OpertionType)
            {

                case OperationType.CREATE:
                {
                        create(e);
                        break;
                }
                case OperationType.RENAME:
                {
                        rename(e); break;

                }
                case OperationType.MODIFY:
                {
                        modify(e); 
                        break;
                }

                case OperationType.DELETE:
                { 
                        delete(e); 
                        break;
                }

            }
        }
        /*
       *递归显示目录下面所有文件(包括子目录的文件)
       * 
       * */

        private static void ListFiles(FileSystemInfo info)
        {
            if (!info.Exists)
                return;
            DirectoryInfo dir = info as DirectoryInfo;
            if (dir == null)
                return;
            FileSystemInfo[] files = dir.GetFileSystemInfos();
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i] as FileInfo;
                if (file != null)
                    Console.WriteLine("[%%new a file] " + file.FullName);
                else
                    ListFiles(files[i]);
            }
        }
        /*
         *递归显示目录下面所有目录(包括子子目录的子目录)
         * 
         * */
        //private static void listdirectories(string strbasedir)
        //{
        //    DirectoryInfo di = new DirectoryInfo(strbasedir);

        //    DirectoryInfo[] dia = di.GetDirectories();

        //    for (int i = 0; i < dia.Length; i++)
        //    {
        //        dirlist.add(dia[i].FullName);
        //        listdirectories(dia[i].fullname);
        //    }

        //}
        private void create(FileEvent e)
        {
            Console.WriteLine("已同步：新建"+"["+e.FileType+"]"+"["+e.NewFullPath+"]");
        }
        private void rename(FileEvent e)
        {
            Console.WriteLine("已同步：重命名" + "[" + e.OldFullPath + "]" + "[" + e.NewFullPath + "]");
        }
        private void modify(FileEvent e)
        {
            Console.WriteLine("已同步：修改[FILE]"  + "[" + e.NewFullPath + "]");
        }
        public void delete(FileEvent e)
        {
            Console.WriteLine("已同步：删除对象" + "[" + e.NewFullPath + "]");
        }
    }
}
