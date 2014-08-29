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
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
namespace myclouddisk
{
     /// <summary>
     /// 事件处理器
     /// </summary>
    class EventHandler
    {
        private ArrayList dirlist = new ArrayList();
        private ArrayList filelist = new ArrayList();
        private Log log = new Log("log\\eventhandler.log");
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
        /// <summary>
        /// 处理用户操作事件，即往云端同步
        /// </summary>
        /// <param name="e1">用户操作事件</param>

        private void handle(FileEvent e1)
        {
            if(e1==null)
            {
                Console.WriteLine("已知bug:位置：EventHandler::handle(FileEvent e1) 异常,可能是内存丢失导致事件不存在，无法被处理！");
                return;
            }
            //Console.WriteLine("文件操作分类和文件类型：" + e1.OpertionType + "  &&  " + e1.FileType);
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
                Thread.Sleep(2000);//希望给系统将发生的事件注入缓冲区
                if(Program.eventBuffer.Count == 0)//最后一个事件，缓冲区不再含有未处理事件
                {                    
                     if (Directory.GetDirectories(e1.NewFullPath).Length > 0 || Directory.GetFiles(e1.NewFullPath).Length > 0)//文件夹非空
                     {
                          //Console.WriteLine("***********创造了非空的文件目录，该非空目录中的文件可能不会同步！************" + e1.NewFullPath);
                          handleEventOneByOne(e1);
                          //
                          //以下是处理遗漏的操作
                          //
                          listDirectories(e1.NewFullPath);
                          for (int i = 0; i < dirlist.Count;++i )//处理create 目录事件
                          {
                              FileEvent e = new FileEvent();
                              e.FileType = FileType.DIRECTORY;
                              e.OpertionType = OperationType.CREATE;
                              e.NewFullPath = dirlist[i].ToString();
                              e.GenerateTime = e1.GenerateTime;

                              handleEventOneByOne(e);
                          }
                          dirlist.Clear();
                          listFiles(new DirectoryInfo(e1.NewFullPath));
                          for (int i = 0; i < filelist.Count; ++i)//处理create 文件事件
                          {
                              FileEvent e = new FileEvent();
                              e.FileType = FileType.FILE;
                              e.OpertionType = OperationType.CREATE;
                              e.NewFullPath = filelist[i].ToString();
                              e.GenerateTime = e1.GenerateTime;

                              handleEventOneByOne(e);
                          }
                          filelist.Clear();
                          //
                          //处理完毕，上述的事件并没有添加在缓冲队列里面，是因为事件顺序的原因
                          //
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
                        handleEventOneByOne(e1);
                        handle(e2);//递归调用
                    }

                }
  
            }
        }
        /// <summary>
        /// 处理单个事件，不考虑与后续事件的关系
        /// </summary>
        /// <param name="e">用户事件</param>
        private void handleEventOneByOne(FileEvent e)
        {
            Thread.Sleep(1000);
            switch (e.OpertionType)
            {

                case OperationType.CREATE:
                {
                    create(e);
                    break;
                }
                case OperationType.RENAME:
                {
                    rename(e);
                    break;

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
        /// <summary>
        /// 递归显示目录下面所有文件(包括子目录的文件)
        /// </summary>
        /// <param name="info">目录信息对象</param>
        private void listFiles(FileSystemInfo info)
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
                    filelist.Add(file.FullName);
                else
                    listFiles(files[i]);
            }
        }
        /// <summary>
        /// 递归显示目录下面所有目录(包括子子目录的子目录)
        /// </summary>
        /// <param name="strbasedir">根目录</param>
        private  void listDirectories(string strbasedir)
        {
            DirectoryInfo di = new DirectoryInfo(strbasedir);

            DirectoryInfo[] dia = di.GetDirectories();

            for (int i = 0; i < dia.Length; i++)
            {
                dirlist.Add(dia[i].FullName);
                listDirectories(dia[i].FullName);
            }

        }
        /// <summary>
        /// 处理用户新建文件或者文件夹事件
        /// </summary>
        /// <param name="e">用户事件</param>
        private void create(FileEvent e)
        {
            string contentType = "scloud-container";
            string fileType = "scloud_container";
            string localPath = e.NewFullPath;

            if (e.FileType == FileType.FILE)
            {
                contentType = "scloud-object";
                fileType = "scloud_object";
            }

            log.WriteLine(System.DateTime.Now.ToString() + " 事件处理器已受理：新建" + "[" + e.FileType + "]" + "[" + e.NewFullPath + "]");
            HTTPClient.PUT(Program.USER, Program.PASSWD, fileType, contentType, localPath);
            
        }
        /// <summary>
        /// 处理重命名事件
        /// </summary>
        /// <param name="e">用户事件</param>
        private void rename(FileEvent e)
        {
            string contentType = "scloud-container";
            string fileType = "scloud_container";

            string oldFullPath = e.OldFullPath;
            string[] path = e.NewName.Split('\\');
            string newName = path[path.Length - 1];
            Console.WriteLine("newName:"+newName);
            if (e.FileType == FileType.FILE)
            {
                contentType = "scloud-object";
                fileType = "scloud_object";
            }

            log.WriteLine(System.DateTime.Now.ToString()+" 事件处理器已受理：重命名 " + "[" + e.OldFullPath + "]" + "[" + e.NewFullPath + "]");
            HTTPClient.POST(Program.USER, Program.PASSWD, fileType, contentType, oldFullPath, newName);
                       
            
        }
        /// <summary>
        /// 用户更改文件事件，目前无法做到增量同步，实际是先删除后新建
        /// </summary>
        /// <param name="e">用户事件</param>
        private void modify(FileEvent e)
        {

            log.WriteLine(System.DateTime.Now.ToString() + " 事件处理器已受理：修改[FILE]" + "[" + e.NewFullPath + "]");
            delete(e);//删除
            create(e);//重新上传整个文件
            
        }
        /// <summary>
        /// 处理删除事件
        /// </summary>
        /// <param name="e">用户事件</param>
        private void delete(FileEvent e)
        {
            string contentType = "scloud-container";
            string fileType = "scloud_container";
            string localPath = e.NewFullPath;

            log.WriteLine(System.DateTime.Now.ToString() + " 事件处理器已受理：删除对象" + "[" + e.NewFullPath + "]");

            string[] path = e.NewFullPath.Split('\\');
            if (path[path.Length - 1].IndexOf('.') > -1)//是文件？仅仅依靠有没有后缀来判断是不是文件，这里并不保证一定可靠！
            {
                HTTPClient.DELETE(Program.USER, Program.PASSWD, "scloud_object", "scloud-object", localPath);
                return;
            }

            HTTPClient.DELETE(Program.USER, Program.PASSWD, fileType, contentType, localPath);


           
        }
    }
}
