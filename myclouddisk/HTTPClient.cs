using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Threading;
//using Mono.HttpUtility;
namespace myclouddisk
{
    class HTTPClient
    {
        private static Log log = new Log("log\\httpclient.log");
        public static void Main()
        {
            //HTTPClient.createUser("20141003", "123456");
            //HTTPClient.PUT("admin", "ADMIN", "scloud_container", "scloud-container", @"C:\我的云盘\nihao");
            //HTTPClient.GET("test", "123456", "scloud_container", "scloud-container", @"C:\我的云盘\soft", 1);
            //HTTPClient.getRootContainer("045130160","123456");
            //HTTPClient.DELETE("045130160", "123456", "scloud_container", "scloud-container", @"C:\我的云盘\music");
            //HTTPClient.DELETE("045130160", "123456", "scloud_object", "scloud-object", @"C:\我的云盘\soft\testtools.zip");
            //HTTPClient.POST("045130160", "123456", "scloud_container", "scloud-container", @"C:\我的云盘\music","popmusic");
           // HTTPClient.PUT("045130160", "123456", "scloud_object", "scloud-object", @"C:\我的云盘\齐秦\月亮代表我的心.Mp3");
           // HTTPClient.getRootContainer("20141001", "123456","scloud-container");
            HTTPClient.GETFile("zhangmanyu", "123456", @"http://192.168.1.113:8081/scloud_object/popmusic/test.txt");
            //HTTPClient.DELETE("20141001", "123456", "scloud_object", "scloud-object", @"C:\我的云盘\document\film.docx");

       
             //string u = "[u'document', u'music', u'soft', u'\u9f50\u79e6']";
             //transfer(u);
            //string s = "[u'document', u'music', u'soft', u'\u9f50\u79e6']";
            // string d = s;
            // string soo = "\\ihao";
            //char c = '\u9f50';
            //Console.WriteLine(c);

            // Encoding.GetEncoding("gb2312").GetBytes(s);
                
            // Console.WriteLine(s+d+soo);
           // Console.WriteLine("http://192.168.1.113:8080/scloud_container/\u9f50\u79e6");
            
             //Console.WriteLine(MyUrlDeCode("http://192.168.1.113:8080/scloud_container/\u65b0\u5efa\u6587\u4ef6\u5939", null));

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
            log.WriteLine(System.DateTime.Now.ToString()+":PUT "+request.RequestUri);

            request.ContentType = contentType;
            request.Method = "PUT";
            request.Headers.Add("Authorization", user + ":" + password);
            request.Headers.Add("X-CDMI-Specification-Version", "v1");
            request.Host = Program.HOST;
            request.Date = System.DateTime.Now;
            request.Timeout = 10000;
            if (fileType == "scloud_object")
            {
                Stream reqStream = null;
                FileStream fs = null;
                try
                {
                    request.AllowWriteStreamBuffering = true;

                    reqStream = request.GetRequestStream();

                    //
                    //@TODO
                    //读一个文件之前应该判断是否被其他进程占用
                    //
                    fs = new FileStream(localPath,FileMode.Open,FileAccess.ReadWrite);
         
                    BinaryReader br = new BinaryReader(fs);

                    byte[] inData = br.ReadBytes((int)fs.Length);

                    reqStream.Write(inData,0,inData.Length);
                    br.Close();
                    reqStream.Close();
                    fs.Close();
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Console.WriteLine(response.Headers);
                  
                    return;
                }
                catch (System.Net.WebException ee)
                {
                    log.WriteLine(System.DateTime.Now.ToString() + " PUT OBJECT 异常 " + ee);
                    //MessageBox.Show( ee.ToString(),"请求异常", MessageBoxButtons.OK);
                    Console.WriteLine(ee);
                }
               
                return;
            }
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Console.WriteLine(response.Headers);
                return;
            }
            catch (System.Net.WebException ee)
            {
                log.WriteLine(System.DateTime.Now.ToString() + " PUT CONTAINER 异常 " + ee);
                //MessageBox.Show(ee.ToString(), "请求异常", MessageBoxButtons.OK);
                Console.WriteLine(ee);
                return;
            }
        }
        /// <summary>
        /// 向服务器请求一个文件资源
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="HttpURI">一个rest的URI，例如：http://192.168.1.102:8080/music/love.mp3</param>
        public static void GETFile(string user, string password,string HttpURI)
        {
           
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(HttpURI);

            Console.WriteLine("GET FILE 请求URI：" + request.RequestUri);

            log.WriteLine(System.DateTime.Now.ToString() + " GET FILE :" + request.RequestUri);

            request.ContentType = "scloud-object";
            request.Method = "GET";
            request.Headers.Add("Authorization", user + ":" + password);
            request.Headers.Add("X-CDMI-Specification-Version", "v1");
            request.Host = Program.HOST;
            request.Date = System.DateTime.Now;

            request.Timeout = 1000*60;

            try
            {
                Thread.Sleep(2000);
                string localPath = HttpURI.Replace(Program.SERVER_URL+"scloud_object", @"C:\我的云盘");
                localPath = localPath.Replace("/", @"\");

                Console.WriteLine("将要写入本地文件：" + localPath);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Console.WriteLine(response.Headers);

                Stream stream = response.GetResponseStream();

                BinaryReader br = new BinaryReader(response.GetResponseStream());

                byte[] inData = new byte[(int)response.ContentLength];

                FileStream fs = new FileStream(localPath, FileMode.OpenOrCreate);
                
                BinaryWriter bw = new BinaryWriter(fs);

                bw.Write(inData, 0, inData.Length);

                bw.Flush();
                bw.Close();
                fs.Close();
                stream.Close();

            }

            catch (System.Net.WebException ee)
            {
                //MessageBox.Show(ee.ToString(), "请求异常", MessageBoxButtons.OK);
                log.WriteLine(System.DateTime.Now.ToString() + " GET FILE 异常:" + ee);
                Console.WriteLine(ee);
            }


        }
        /// <summary>
        /// 获取某一目录下所有的对象（包括文件或者文件夹中的内容列表）
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="fileType">例如：scloud_object</param>
        /// <param name="contentType">例如：scloud-object</param>
        /// <param name="localPath">文件或者文件夹的绝对路径</param>
        /// <param name="flag">0或者1,0表示要获取文件，会将文件写入到本地相应的位置，1表示返回容器列表</param>
        /// <returns></returns>
        public static string[] GET(string user, string password, string fileType, string contentType, string localPath ,int flag)
        {
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(generateHttpURI(fileType, localPath));

            log.WriteLine(System.DateTime.Now.ToString() + " GET " + request.RequestUri);

            Console.WriteLine("GET 请求URI：" + request.RequestUri);

            request.ContentType = contentType;
            request.Method = "GET";
            request.Headers.Add("Authorization", user + ":" + password);
            request.Headers.Add("X-CDMI-Specification-Version", "v1");
            request.Host = Program.HOST;
            request.Date = System.DateTime.Now;

            request.Timeout = 1000*60;

            try
            {
                Thread.Sleep(2000);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Console.WriteLine(response.Headers);
                string[] c;
                if (flag == 1)
                {
                    c = transfer(response.Headers.Get("Containers"));
                }
                else
                {
                    c = transfer(response.Headers.Get("Objects"));
                    Console.WriteLine("**********"+c.ToString());
                }

                return c;
  
            }
            catch (System.Net.WebException ee)
            {
                //MessageBox.Show(ee.ToString(), "请求异常", MessageBoxButtons.OK);
                log.WriteLine(System.DateTime.Now.ToString() + " GET 异常" + ee);
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

            log.WriteLine(System.DateTime.Now.ToString() + " POST " + request.RequestUri);

            request.ContentType = contentType;
            request.Method = "POST";
            request.Headers.Add("Authorization", user + ":" + password);
            request.Headers.Add("X-CDMI-Specification-Version", "v1");
            request.Headers.Add("current-name", newName);

            request.Host = Program.HOST;
            request.Date = System.DateTime.Now;

            request.Timeout = 5000;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Console.WriteLine(response.Headers);
            }
            catch (System.Net.WebException ee)
            {
                //MessageBox.Show(ee.ToString(), "请求异常", MessageBoxButtons.OK);
                log.WriteLine(System.DateTime.Now.ToString() + " POST 异常 " + ee);
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

            log.WriteLine(System.DateTime.Now.ToString() + " DELETE " + request.RequestUri);

            request.Headers.Add("Authorization", user + ":" + password);
            request.Headers.Add("X-CDMI-Specification-Version", "v1");

            request.ContentType = contentType;
            request.Method = "DELETE";
            request.Host = Program.HOST;
            request.Date = System.DateTime.Now;

            request.Timeout = 5000;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Console.WriteLine(response.Headers);

                return true;
            }
            catch (System.Net.WebException ee)
            {
                //MessageBox.Show(ee.ToString(), "请求异常", MessageBoxButtons.OK);
                log.WriteLine(System.DateTime.Now.ToString() + " DELETE 异常 " + ee);
                Console.WriteLine(ee);
                return false;
            }

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

            log.WriteLine(System.DateTime.Now.ToString() + " CREATE USER " + user);

            request.ContentType = "scloud-user";
            request.Method = "PUT";
            request.Headers.Add("Authorization", user+":"+password);
            request.Headers.Add("X-CDMI-Specification-Version", "v1");
            request.Host = Program.HOST;
            request.Date = System.DateTime.Now;

            request.Timeout = 10000;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Console.WriteLine(response.Headers);
            }
            catch (System.Net.WebException ee)
            {
                log.WriteLine(System.DateTime.Now.ToString() + " CREATE USER 异常 " + ee);
                //MessageBox.Show(ee.ToString(), "请求异常", MessageBoxButtons.OK);
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

            log.WriteLine(System.DateTime.Now.ToString() + " GET ROOT CONTAINER " + request.RequestUri);

            request.ContentType = "scloud-domain";
            request.Method = "GET";
            request.Headers.Add("Authorization", user + ":" + password);
            request.Headers.Add("X-CDMI-Specification-Version", "v1");
            request.Host = Program.HOST;
            request.Date = System.DateTime.Now;

            request.Timeout = 1000*10;
            try
            {
                Thread.Sleep(2000);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Console.WriteLine(response.Headers);

                string cc = response.Headers.Get("Containers");
                string oo = response.Headers.Get("Objects");     
     
                string[] c = transfer(cc);
                string[] o = transfer(oo);

                if (contentType == "scloud-object")
                {
                    return o;
                }
                else return c;
            }
            catch (System.Net.WebException ee)
            {
                //MessageBox.Show(ee.ToString(), "请求异常", MessageBoxButtons.OK);
                log.WriteLine(System.DateTime.Now.ToString() + "GET ROOT CONTAINER 异常 " + ee);
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
            log.WriteLine(System.DateTime.Now.ToString() + " 无法获取本机IP地址 " + myHost.AddressList.ToString());
            MessageBox.Show("无法获取本机IP地址！" + myHost.AddressList.ToString(), "错误提示", MessageBoxButtons.OK);
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
            Uri uri = new Uri(newuri);
            //Console.WriteLine("转化URI：" + localPath + " -> " + newuri);
            return uri;
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
            string str1 = original.Replace("[", "").Replace("]", "").Trim();
            string str2 = str1.Replace("u'", "").Replace("'", "").Trim();

            String[] last = str2.Split(',');
            return last;
        }
        

    }
}
