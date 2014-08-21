using System.IO;
using System.Net;
using System.Windows.Forms;
using System;
using System.Collections;
using System.Text;
namespace myclouddisk
{
    class HTTPClient
    {
        public static string host = "http://192.168.1.113:8080/";
        public static void Main()
        {
            //HTTPClient.createUser("045130160", "123456");
            //HTTPClient.PUT("045130160", "123456", "scloud_container", "scloud-container", @"C:\我的云盘\popmusic\jackson");
            //HTTPClient.GET("045130160", "123456", "scloud_container", "scloud-container", @"C:\我的云盘\music",1);
            //HTTPClient.getRootContainer("045130160","123456");
            //HTTPClient.DELETE("045130160", "123456", "scloud_container", "scloud-container", @"C:\我的云盘\music");
            //HTTPClient.DELETE("045130160", "123456", "scloud_container", "scloud-container", @"C:\我的云盘\music\test.txt");
            //HTTPClient.POST("045130160", "123456", "scloud_container", "scloud-container", @"C:\我的云盘\music","popmusic");
            //HTTPClient.PUT("045130160", "123456", "scloud_object", "scloud-object", @"C:\我的云盘\document\china history.docx");
            //HTTPClient.getRootContainer("045130160", "123456","scloud-container");
            //HTTPClient.GETFile("045130160","123456","http://192.168.1.113:8080/scloud_object/document/document.txt");
           // HTTPClient.GET("045130160","123456","scloud_container","scloud-container",@"C:\我的云盘\music");

        }
        public HTTPClient()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="fileType">scloud_domian.etc</param>
        /// <param name="contentType">scloud-doamin.etc</param>
        /// <param name="localPath">本地文件路径，例如"C:\我的云盘\XXX\YYYY"</param>
        public static void PUT(string user, string password, string fileType,string contentType, string localPath)
        {
            //string httpuri = "http://192.168.1.113:8080/" + fileType;     
            //Uri uri = new Uri(localPath.Replace(@"C:\我的云盘", httpuri));
            Uri uri = generateHttpURI(fileType, localPath);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            request.ContentType = contentType;
            request.Method = "PUT";
            request.Headers.Add("Authorization", user + ":" + password);
            request.Headers.Add("X-CDMI-Specification-Version", "v1");
            request.Host = "192.168.1.113";
            request.Date = System.DateTime.Now;
            request.Timeout = 50000;
            if (fileType == "scloud_object")
            {
                //HttpWebRequest req = request;
                Stream reqStream = null;
                FileStream fs = null;
                try
                {
                    request.AllowWriteStreamBuffering = true;
                    // Retrieve request stream 
                    reqStream = request.GetRequestStream();
                    // Open the local file
                    //
                    //@TODO
                    //读一个文件之前应该判断是否被其他进程占用
                    //
                    fs = new FileStream(localPath, FileMode.Open, FileAccess.ReadWrite);
         
                    BinaryReader br = new BinaryReader(fs);

                    byte[] inData = br.ReadBytes((int)fs.Length);

                    reqStream.Write(inData,0,inData.Length);
                    reqStream.Close();
                    fs.Close();            

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Console.WriteLine(response.Headers);
                    return;
                }
                catch (System.Net.WebException ee)
                {
                    Console.WriteLine(ee);
                }
               
                return;
            }
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Console.WriteLine(response.Headers);
            }
            catch (System.Net.WebException ee)
            {
                Console.WriteLine(ee);
                return;
            }
            return;
        }
        public static void GETFile(string user, string password,string HttpURI)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(HttpURI);

            request.ContentType = "scloud-object";
            request.Method = "GET";
            request.Headers.Add("Authorization", user + ":" + password);
            request.Headers.Add("X-CDMI-Specification-Version", "v1");
            request.Host = "192.168.1.113";
            request.Date = System.DateTime.Now;

            request.Timeout = 50000;

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                string content = streamReader.ReadToEnd();
                Console.WriteLine(response.Headers);
                string localPath = HttpURI.Replace("http://192.168.1.113:8080/scloud_object",@"C:\我的云盘");
                localPath = localPath.Replace("/",@"\");
                Console.WriteLine("写入本地文件：" + localPath);
                StreamWriter sw = new StreamWriter(localPath);
                sw.Write(content);
                sw.Flush();
                sw.Close();
                streamReader.Close();
            }

            catch (System.Net.WebException ee)
            {
                Console.WriteLine(ee);
            }


        }
        public static string[] GET(string user, string password, string fileType, string contentType, string localPath,int flag)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create( generateHttpURI(fileType, localPath));
            Console.WriteLine("请求的URI：" + generateHttpURI(fileType, localPath));
            request.ContentType = contentType;
            request.Method = "GET";
            request.Headers.Add("Authorization", user + ":" + password);
            request.Headers.Add("X-CDMI-Specification-Version", "v1");
            request.Host = "192.168.1.113";
            request.Date = System.DateTime.Now;

            request.Timeout = 50000;

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                string c = response.Headers.Get("Containers");
                string o = response.Headers.Get("Objects");
                if(c == null && o==null)
                {
                    return null;
                }

                string con = "";

                if (c != null)
                {
                    con = c.Replace("[", "").Replace("]", "").Replace("'", "").Trim();
                }
                  
                if (flag == 0 && o != null)//need object
                {
                    con = o.Replace("[", "").Replace("]", "").Replace("'", "").Trim();

                }
                if (con == "")
                    return null;
                    
                string[] containers = con.Split(',');

                Console.WriteLine(response.Headers);

                return containers;
            }
            catch (System.Net.WebException ee)
            {
                Console.WriteLine( ee);
                return null;
            }

        }
        /// <summary>
        /// 更改数据
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="fileType"></param>
        /// <param name="contentType"></param>
        /// <param name="oldFullPath"></param>
        /// <param name="newName"></param>

        public static void POST(string user, string password, string fileType, string contentType, string oldFullPath, string newName)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(generateHttpURI(fileType,oldFullPath));

            request.ContentType = contentType;
            request.Method = "POST";
            request.Headers.Add("Authorization", user + ":" + password);
            request.Headers.Add("X-CDMI-Specification-Version", "v1");
            request.Headers.Add("current-name", newName);

            request.Host = "192.168.1.113";
            request.Date = System.DateTime.Now;

            request.Timeout = 50000;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Console.WriteLine(response.Headers);
            }
            catch (System.Net.WebException ee)
            {
                Console.WriteLine(ee);
                return;
            }
        }
        /// <summary>
        /// 删除文件或者文件夹
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="fileType"></param>
        /// <param name="contentType"></param>
        /// <param name="localPath"></param>
        public static void DELETE(string user ,string password, string fileType, string contentType, string localPath)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(generateHttpURI(fileType, localPath));

            request.Headers.Add("Authorization", user + ":" + password);
            request.Headers.Add("X-CDMI-Specification-Version", "v1");

            request.ContentType = contentType;
            request.Method = "DELETE";
            request.Host = "192.168.1.113";
            request.Date = System.DateTime.Now;

            request.Timeout = 50000;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Console.WriteLine(response.Headers);
                Console.WriteLine(response.StatusCode);
            }
            catch (System.Net.WebException ee)
            {
                Console.WriteLine(ee);
                return;
            }

        }
        /// <summary>
        /// 新建用户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public static void createUser(string user,string password)
        {
            Uri uri = new Uri("http://192.168.1.113:8080/scloud_user/");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            request.ContentType = "scloud-user";
            request.Method = "PUT";
            request.Headers.Add("Authorization", user+":"+password);
            request.Headers.Add("X-CDMI-Specification-Version", "v1");
            request.Host = "192.168.1.113";
            request.Date = System.DateTime.Now;

            request.Timeout = 50000;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Console.WriteLine(response.Headers);
                Console.WriteLine(response.StatusCode);
            }
            catch (System.Net.WebException ee)
            {
                Console.WriteLine(ee);
                return;
            }

        }
        /// <summary>
        /// 获取用户根目录下的文件和目录
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string[] getRootContainer(string user, string password,string contentType)
        {
            Uri uri = new Uri("http://192.168.1.113:8080/scloud_domain/");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            request.ContentType = "scloud-domain";
            request.Method = "GET";
            request.Headers.Add("Authorization", user + ":" + password);
            request.Headers.Add("X-CDMI-Specification-Version", "v1");
            request.Host = "192.168.1.113";
            request.Date = System.DateTime.Now;

            request.Timeout = 50000;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();


                Console.WriteLine(response.Headers);
                Console.WriteLine(response.StatusCode);

                string c = response.Headers.Get("Containers");
                string o = response.Headers.Get("Objects");
                if(c==null && o== null)
                {
                    return null;
                }
                string con = c.Replace("[", "").Replace("]", "").Replace("'", "").Trim();
                
                if(contentType == "scloud-object")
                {
                    if (response.Headers.Get("Objects") == null)
                        return null;
                    con = response.Headers.Get("Objects").Replace("[", "").Replace("]", "").Replace("'", "").Trim();
                }
                if (con == null || con == "")
                {
                    return null;
                }
                string[] containers = con.Split(',');
                
                return containers;
            }
            catch (System.Net.WebException ee)
            {
                Console.WriteLine(ee);
                return null;
            }
        }
        /// <summary>
        /// 获取本机局域网ip地址
        /// </summary>
        /// <returns>IP地址</returns>
        private static string getLocalIP()
        {
            string ComputerName = Dns.GetHostName();
            IPHostEntry myHost = new IPHostEntry();
            myHost = Dns.GetHostEntry(ComputerName);
            for (int i = 0; i < myHost.AddressList.Length; i++)
            {
                if (myHost.AddressList[i].ToString().IndexOf("10") > -1 || myHost.AddressList[i].ToString().IndexOf("192") > -1)
                {
                    Console.WriteLine("本机ip："+myHost.AddressList[i].ToString());
                    return myHost.AddressList[i].ToString();
                }
                
            }
            return "ipERROR";
        }
        /// <summary>
        /// 根据本地的路径生成RESTFUL的http 的uri
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="localPath"></param>
        /// <returns></returns>
        private static Uri generateHttpURI(string fileType, string localPath)
        {
            string httpuri = "http://192.168.1.113:8080/" + fileType+"/" ;
            Uri uri = new Uri(localPath.Replace(@"C:\我的云盘\", httpuri));
            //Console.WriteLine("转化URI："+localPath+" -> "+uri.ToString());
            return uri;
        }




    }
}
