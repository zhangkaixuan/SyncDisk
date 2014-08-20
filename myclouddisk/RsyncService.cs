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
            string[] containers = HTTPClient.getRootContainer("045130160", "123456");
            getContainers(containers);
        }
        private static void getContainers(string[] containers)
        {
            
            if (containers==null || containers.Length == 0)
            {
                return;
            }

            for (int i = 0; i < containers.Count(); ++i)
            {         
                string dirPath = @"C:\我的云盘\"+containers[i].Trim();
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                    
                }
                //Console.WriteLine("URI:"+dirPath);
                string[] containers2 = HTTPClient.GET("045130160", "123456", "scloud_container", "scloud-container",dirPath);
                if (containers2 == null)
                    continue;
                for (int j = 0; j < containers2.Length;++j )
                {
                    containers2[j] = containers[i].Trim() + "/" + containers2[j].Trim();
                }
                getContainers(containers2);
            }

        }
    }
}
