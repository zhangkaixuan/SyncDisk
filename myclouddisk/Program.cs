using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.IO;
namespace myclouddisk
{
    public enum RsyncStatus
    {
        STARTING,
        RSYNCING,
        FINISHED,
        ERROR
    }
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        ///  
        public static Queue<FileEvent> eventBuffer = new Queue<FileEvent>();
        public static EventHandler eventHandler = new EventHandler();
        public static RsyncStatus status ;
        public static DateTime lastUpdateTime = DateTime.Now;
        public static UI ui ;
        public static bool flag = false;
        public static string monitorPath = @"C:\我的云盘";
        public static string currentUser="045130160";
        public static string hostURL="192.168.1.113:8080";
        public static string selfIP = "10.0.0.2";
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            status = RsyncStatus.STARTING;
            ui = new UI(monitorPath);
            if(!Directory.Exists(monitorPath))
            {
                Directory.CreateDirectory(monitorPath);
            }

            Thread t1 = new Thread(new ThreadStart(runMonitor));
            Thread t2 = new Thread(new ThreadStart(handleEvent));
            
            t1.Start();
            t2.Start();

            Application.Run();
        }
        private static void runMonitor()
        {
            Monitor.startUp(monitorPath);
        }
        private static void handleEvent()
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
