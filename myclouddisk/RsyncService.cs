using System;
using System.IO;
using System.Linq;
using System.Threading;
namespace myclouddisk
{
    class RsyncService
    {
        /// <summary>
        /// 程序第一次运行从云中拉取数据初始化本地文件夹
        /// </summary>
        /// 
        public static void InitLocalDirectory()
        {

            Console.WriteLine("初始化本地目录.....");
            Program.ui.setIcon("sync");

            string[] containers = HTTPClient.getRootContainer(Program.USER,Program.PASSWD,"scloud-container");
            string[] objects = HTTPClient.getRootContainer(Program.USER,Program.PASSWD, "scloud-object");

            initLocalObjects(objects);

            //Thread.Sleep(2000);

            getContainers(containers);

            //Thread.Sleep(2000);

            getObjects(containers);

            Program.ui.setIcon("default");
           
        }
        /// <summary>
        /// 获取目录列表，并初始化本地目录，该方法仅仅用于初次初始化本地目录
        /// </summary>
        /// <param name="containers">含目录名称的数组</param>
        private static void getContainers(string[] containers)
        {

            if (containers == null || containers.Length == 0)
            {
                return;
            }

            for (int i = 0; i < containers.Count(); ++i)
            {
                string dirPath = Program.MONITOR_PATH + containers[i].Trim();
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                    Console.WriteLine("新建目录："+dirPath);

                }
                Console.WriteLine("getContainers URI:" + dirPath);
                string[] containers2 = HTTPClient.GET(Program.USER, Program.PASSWD, "scloud_container", "scloud-container", dirPath,1);//1表示返回的是container数组

                if (containers2 == null)
                {
                    continue;
                }
                        
                for (int j = 0; j < containers2.Length; ++j)
                {
                    Console.WriteLine("Before:"+containers2[j]);
                    containers2[j] = containers[i].Trim() + "/" + containers2[j].Trim();
                    Console.WriteLine("After:"+containers2[j]);
                }
                      
                getContainers(containers2);
            }

        }
        /// <summary>
        /// 初始化本地文件，本方法仅仅用于程序初始化本地文件用
        /// </summary>
        /// <param name="containers">含目录名称的数组</param>

        private static void getObjects(string[] containers)
        {
            
            if(containers == null || containers.Length == 0)
            {
                return;
            }

            for(int i=0;i<containers.Length;++i)
            {
                string dirPath = Program.MONITOR_PATH + containers[i].Trim();

                string[] objects = HTTPClient.GET(Program.USER,Program.PASSWD,"scloud_container","scloud-container",dirPath,0);//0表示返回的是null，已经将文件写入本地

                if(objects != null)
                {
                    for(int j=0;j<objects.Length;++j)
                    {

                        //Console.WriteLine("" + objects[j]); 
                        objects[j] = containers[i].Trim() + "/" + objects[j].Trim();
                    }

                }

                initLocalObjects(objects);

                string[] containers2 = HTTPClient.GET(Program.USER, Program.PASSWD, "scloud_container", "scloud-container", dirPath,1);//1表示返回的是container数组

                if (containers2 == null)
                {
                    continue;
                }

                for (int j = 0; j < containers2.Length; ++j)
                {
                    containers2[j] = containers[i].Trim() + "/" + containers2[j].Trim();
                    Console.WriteLine("containers2:"+containers2[j]);
                }

                getContainers(containers2);
            }

        }
        /// <summary>
        /// 从服务器获取object即文件，并写入本地相对应的目录
        /// </summary>
        /// <param name="objects">object数组</param>
        private static void initLocalObjects(string[] objects)
        {
            if(objects == null || objects.Length==0)
            {
                return;
            }
            for(int i=0;i<objects.Length;++i)
            {
                Console.WriteLine("请求object：" + Program.SERVER_URL+"scloud_object/" + objects[i].Trim());
                //System.Threading.Thread.Sleep(2000);
                HTTPClient.GETFile(Program.USER, Program.PASSWD, Program.SERVER_URL+ "scloud_object/" + objects[i].Trim());
            }
            return;

        }
        /// <summary>
        /// 从配置文件中读取一些参数
        /// </summary>
        public static void initParameter()
        {
            //XmlDocument doc = new XmlDocument();
            //try
            //{
            //    doc.Load("config.xml");
            //    Program.MONITOR_PATH = doc.SelectSingleNode("default/global/monitorpath").ToString();
            //    Program.SERVER_URL = doc.SelectSingleNode("default/global/serverurl").ToString();
            //}
            //catch (System.Xml.XmlException ee)
            //{
            //    MessageBox.Show(ee.ToString(), "初始化参数异常", MessageBoxButtons.OK);
            //    Console.WriteLine(ee);
            //    Application.Exit();

            //}
            Program.IN_IP = HTTPClient.getLocalIP();
        }
           

    }
}
