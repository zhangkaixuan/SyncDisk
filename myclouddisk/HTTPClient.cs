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
        public static void Main()
        {
            //HTTPClient.createUser("045130160", "123456");
            // HTTPClient.PUT("045130160", "123456", "scloud_container", "scloud-container", @"C:\我的云盘\齐秦");
            //HTTPClient.GET("aaaaaaaaab", "aaaaaaaaab", "scloud_container", "scloud-container", @"C:\我的云盘\邓丽君", 1);
            //HTTPClient.getRootContainer("045130160","123456");
            //HTTPClient.DELETE("045130160", "123456", "scloud_container", "scloud-container", @"C:\我的云盘\music");
            //HTTPClient.DELETE("045130160", "123456", "scloud_container", "scloud-container", @"C:\我的云盘\music\test.txt");
            //HTTPClient.POST("045130160", "123456", "scloud_container", "scloud-container", @"C:\我的云盘\music","popmusic");
            //HTTPClient.PUT("045130160", "123456", "scloud_object", "scloud-object", @"C:\我的云盘\document\ceshi3.docx");
             HTTPClient.getRootContainer("045130160", "123456","scloud-container");
           // HTTPClient.GETFile("045130160","123456","http://192.168.1.113:8080/scloud_object/document/ceshi3.docx");
           // HTTPClient.GET("045130160","123456","scloud_container","scloud-container",@"C:\我的云盘\music");
           //  Directory.CreateDirectory("C:\\\u725b\u903c\u7684\u4eba\u554afadfadf");
             string s = "\u65b0\u5efa\u6587\u4ef6\u5939";
             Console.WriteLine(toGbk(s));
             Console.WriteLine(s);            
        }
        /// <summary>
        /// PUT资源操作
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="fileType">例如:scloud_domian</param>
        /// <param name="contentType">例如：scloud-doamin</param>
        /// <param name="localPath">本地文件路径，例如"C:\我的云盘\XXX\YYYY"</param>
        public static void PUT(string user, string password, string fileType,string contentType, string localPath)
        {
            Uri uri = generateHttpURI(fileType, localPath);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            request.ContentType = contentType;
            request.Method = "PUT";
            request.Headers.Add("Authorization", user + ":" + password);
            request.Headers.Add("X-CDMI-Specification-Version", "v1");
            request.Host = "192.168.1.113";
            request.Date = System.DateTime.Now;
            request.Timeout = 10000;
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
                    fs = new FileStream(localPath,FileMode.Open,FileAccess.ReadWrite);
         
                    BinaryReader br = new BinaryReader(fs);

                    byte[] inData = br.ReadBytes((int)fs.Length);

                    reqStream.Write(inData,0,inData.Length);
                    reqStream.Close();
                    fs.Close();            

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                  
                    return;
                }
                catch (System.Net.WebException ee)
                {
                    //MessageBox.Show( ee.ToString(),"请求异常", MessageBoxButtons.OK);
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
                MessageBox.Show(ee.ToString(), "请求异常", MessageBoxButtons.OK);
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
                string localPath = HttpURI.Replace(Program.SERVER_URL+"scloud_object", @"C:\我的云盘");
                localPath = localPath.Replace("/", @"\");
                Console.WriteLine("写入本地文件：" + localPath);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
              
                Stream stream = response.GetResponseStream();
              
               
                BinaryReader br = new BinaryReader(response.GetResponseStream());
                byte[] inData = new byte[(int)response.ContentLength];
               
                FileStream fs = new FileStream(localPath, FileMode.OpenOrCreate);
                inData = new byte[(int)response.ContentLength];
                BinaryWriter bw = new BinaryWriter(fs);
               
                bw.Write(inData,0,inData.Length);
                bw.Flush();
                bw.Close();
                fs.Close();
                br.Close();

            }

            catch (System.Net.WebException ee)
            {
                //MessageBox.Show(ee.ToString(), "请求异常", MessageBoxButtons.OK);
                Console.WriteLine(ee);
            }


        }
        /// <summary>
        /// 获取资源（文件或者文件夹中的内容列表）
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="fileType">例如：scloud_object</param>
        /// <param name="contentType">例如：scloud-object</param>
        /// <param name="localPath">文件或者文件夹的绝对路径</param>
        /// <param name="flag">0或者1,0表示要获取文件，会将文件写入到本地相应的位置，1表示返回容器列表</param>
        /// <returns></returns>
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

            request.Timeout = 10000;

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Console.WriteLine(transfer(response.Headers.ToString()));
                
                string[] c = transfer(response.Headers.Get("Containers"));
                string[] o = transfer(response.Headers.Get("Objects"));

                if(c == null && o==null)
                {
                    return null;
                }
                if (flag == 0)
                {
                    return o;
                }
                else return c;
                
            }
            catch (System.Net.WebException ee)
            {
                //MessageBox.Show(ee.ToString(), "请求异常", MessageBoxButtons.OK);
                Console.WriteLine( ee);
                return null;
            }

        }
        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="fileType">例如：scloud_object、scloud_container</param>
        /// <param name="contentType">例如：scloud-object</param>
        /// <param name="oldFullPath">旧的文件夹或者文件绝对路径</param>
        /// <param name="newName">新名字</param>

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
                //MessageBox.Show(ee.ToString(), "请求异常", MessageBoxButtons.OK);
                Console.WriteLine(ee);
                return;
            }
        }
        /// <summary>
        /// 删除文件或者文件夹
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="fileType">例如：scloud_container、scloud_object</param>
        /// <param name="contentType">例如：scloud-object</param>
        /// <param name="localPath">要删除的本地文件或者文件夹的绝对路径</param>
        public static bool DELETE(string user ,string password, string fileType, string contentType, string localPath)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(generateHttpURI(fileType, localPath));

            request.Headers.Add("Authorization", user + ":" + password);
            request.Headers.Add("X-CDMI-Specification-Version", "v1");

            request.ContentType = contentType;
            request.Method = "DELETE";
            request.Host = "192.168.1.113";
            request.Date = System.DateTime.Now;

            request.Timeout = 2000;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Console.WriteLine(response.Headers);
                Console.WriteLine(response.StatusCode);
                return true;
            }
            catch (System.Net.WebException ee)
            {
                //MessageBox.Show(ee.ToString(), "请求异常", MessageBoxButtons.OK);
                Console.WriteLine(ee);
                return false;
            }
            return false;

        }
        /// <summary>
        /// 新建用户
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        public static void createUser(string user,string password)
        {
            Uri uri = new Uri(Program.SERVER_URL+"scloud_user/");
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
                MessageBox.Show(ee.ToString(), "请求异常", MessageBoxButtons.OK);
                Console.WriteLine(ee);
                return;
            }

        }
        /// <summary>
        /// 获取用户根目录下的文件列表或者文件夹列表
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="contentType">指定获取的是文件还是文件夹：即其值为：scloud-container或者scloud-object</param>
        /// <returns>用户根目录下的container数组或者object数组，数组的元素仅仅是名字</returns>
        public static string[] getRootContainer(string user, string password,string contentType)
        {
            Uri uri = new Uri(Program.SERVER_URL+"scloud_domain/");
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

                string[] c = transfer(response.Headers.Get("Containers"));
                string[] o = transfer(response.Headers.Get("Objects"));
                Console.WriteLine(response.Headers);
                if (contentType == "scloud-object")
                {
                    return o;
                }
                else return c;
            }
            catch (System.Net.WebException ee)
            {
                //MessageBox.Show(ee.ToString(), "请求异常", MessageBoxButtons.OK);
                Console.WriteLine(ee);
                return null;
            }
        }
        /// <summary>
        /// 获取本机局域网ip地址
        /// </summary>
        /// <returns>局域网IP地址</returns>
        public static string getLocalIP()
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
        /// 根据本地的路径生成RESTFUL的http的uri
        /// </summary>
        /// <param name="fileType">uri的类型，如scloud_container</param>
        /// <param name="localPath">本地文件或者文件夹绝对路径</param>
        /// <returns>rest的URI，如："C:\我的云盘\music\love.mp3"转化为"http://192.168.1.113:8080/music/love.mp3"</returns>
        public static Uri generateHttpURI(string fileType, string localPath)
        {
            string httpuri = Program.SERVER_URL + fileType+"/" ;
            string newuri = localPath.Replace(@"C:\我的云盘\", httpuri);
           ;
            //Uri uri = new Uri(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(newuri)));
           Uri uri = new Uri(newuri);
            //Console.WriteLine("转化URI：" + localPath + " -> " + Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(newuri)));
            return uri;
        }
        public static byte[] utf8Encoding(string unicodeString)
        {
            Byte[] encodedBytes = new UTF8Encoding().GetBytes(unicodeString);
            return encodedBytes;
        }
        public static string utf8Decoding(byte[] encodedBytes)
        {
            String decodedString = new UTF8Encoding().GetString(encodedBytes);
            return decodedString;
        }
        public static string[] transfer(string original)
        {
            if (original == null)
                return null;
            string str0 = original.Replace("[", "").Replace("]", "");
            if(str0 == "" || str0 ==null)
            {
                return null;
            }
            string str1 = original.Replace("[","").Replace("]","").Trim();          
            string str2 = str1.Replace("u'", "").Replace("'","").Trim();
            //str2 = toGbk(str2);
            Console.WriteLine("这里是转化后的结果"+str2);
            String[] last = str2.Split(',');
            {
                foreach(string s in last)
                {
                    Console.WriteLine(s.Trim());
                }
            }
            
           
            return last;
        }
        public static string toGbk(string utf8string)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(utf8string); 
            string text = Encoding.GetEncoding("utf-8").GetString(buffer);
            return text;

        }

    }
}
