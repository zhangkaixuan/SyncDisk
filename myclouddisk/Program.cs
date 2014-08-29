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
using System.IO;
using System.Threading;
using System.Windows.Forms;
namespace myclouddisk
{
    /// <summary>
    ///同步状态
    /// </summary>
    public enum RsyncStatus
    {
        STARTING,
        RSYNCING,
        FINISHED,
        ERROR
    }
    /// <summary>
    /// 主类
    /// </summary>
    static class Program
    {
        public static Queue<FileEvent> eventBuffer = new Queue<FileEvent>();
        public static EventHandler eventHandler = new EventHandler();
        public static RsyncStatus status = RsyncStatus.STARTING;
        public static DateTime lastUpdateTime = DateTime.Now;
        public static UI ui ;
        public static bool flag = false;

        public static string MONITOR_PATH = @"C:\我的云盘\";
        public static string USER = "one";
        public static string PASSWD = "123456";
        //public static string SERVER_URL = "http://172.20.46.160:8081/";
        public static string SERVER_URL = "http://192.168.1.113:8081/";
        public static string IN_IP;
        public static string PUB_IP = "";
        public static string HOST = "cloud.ecust.edu.cn";
        /// <summary>
        /// 程序入口，即主函数
        /// </summary>
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            init();

            Application.Run();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private static  void init()
        {
            RsyncService.initParameter();//初始化参数

            if (!Directory.Exists(MONITOR_PATH))//监察被监视目录是否存在
            {
                Directory.CreateDirectory(MONITOR_PATH);
            }

            ui = new UI(MONITOR_PATH);//载入UI

            RsyncService.InitLocalDirectory();//初始化本地目录

            Thread t1 = new Thread(new ThreadStart(runMonitor));//开启监控
            Thread t2 = new Thread(new ThreadStart(runEventHandler));//开启事件处理器

            t1.Start();
            t2.Start();


        }
        /// <summary>
        /// 启动本地监控服务
        /// </summary>
        private static void runMonitor()
        {
            Monitor.startUp(MONITOR_PATH);
        }
        /// <summary>
        /// 启动事件处理服务
        /// </summary>
        private static void runEventHandler()
        {
            while (true)
            {
                Thread.Sleep(2);
                if (status == RsyncStatus.RSYNCING)
                {
                    ui.setIcon("sync");
                }
                if (status == RsyncStatus.FINISHED)
                {
                    ui.setIcon("defult");
                }

                if (eventBuffer.Count > 0)
                {
                    status = RsyncStatus.RSYNCING;
                    ui.setIcon("sync");
                    eventHandler.HandleEvent();
                    flag = false;
                }
                else
                {
                    status = RsyncStatus.FINISHED;
                    if (flag == false)
                    {
                        lastUpdateTime = DateTime.Now;
                        ui.setIcon("defult");
                        flag = true;

                    }

                }

            }
                    
                
        }

    }
}
