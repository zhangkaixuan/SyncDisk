using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
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
            string[] containers = HTTPClient.getRootContainer("045130160", "123456","scloud-container");
            string[] objects = HTTPClient.getRootContainer("045130160", "123456", "scloud-object");
            getContainers(containers);
            getObjects(containers);
            Program.ui.setIcon("default");
           
        }
        private static void getContainers(string[] containers)
        {

            if (containers == null || containers.Length == 0)
            {
                return;
            }

            for (int i = 0; i < containers.Count(); ++i)
            {
                string dirPath = @"C:\我的云盘\" + containers[i].Trim();
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);

                }
                //Console.WriteLine("URI:"+dirPath);
                string[] containers2 = HTTPClient.GET("045130160", "123456", "scloud_container", "scloud-container", dirPath,1);

                if (containers2 == null)
                {
                    continue;
                }
                        
                for (int j = 0; j < containers2.Length; ++j)
                {
                    containers2[j] = containers[i].Trim() + "/" + containers2[j].Trim();
                }
                      
                getContainers(containers2);
            }

        }

        private static void getObjects(string[] containers)
        {
            
            if(containers == null || containers.Length == 0)
            {
                return;
            }

            for(int i=0;i<containers.Length;++i)
            {
                string dirPath = @"C:\我的云盘\" + containers[i].Trim();

                string[] objects = HTTPClient.GET("045130160","123456","scloud_container","scloud-container",dirPath,0);

                if(objects != null)
                {
                    for(int j=0;j<objects.Length;++j)
                    {

                        //Console.WriteLine("object*********" + objects[j]); 
                        objects[j] = containers[i].Trim() + "/" + objects[j].Trim();
                    }

                }

                initLocalObjects(objects);

                string[] containers2 = HTTPClient.GET("045130160", "123456", "scloud_container", "scloud-container", dirPath,1);

                if (containers2 == null)
                {
                    continue;
                }

                for (int j = 0; j < containers2.Length; ++j)
                {
                    containers2[j] = containers[i].Trim() + "/" + containers2[j].Trim();
                    //Console.WriteLine("containers2::"+containers2[j]);
                }

                getContainers(containers2);
            }

        }
        private static void initLocalObjects(string[] objects)
        {
            if(objects == null || objects.Length==0)
            {
                return;
            }
            for(int i=0;i<objects.Length;++i)
            {
                //Console.WriteLine("文件：" + objects[i]);
                //Console.WriteLine("请求object：" + "http://192.168.1.113:8080/scloud_object/" + objects[i].Trim());
                HTTPClient.GETFile("045130160", "123456", "http://192.168.1.113:8080/scloud_object/" + objects[i].Trim());
            }
            return;

        }
    }
}
