using System.IO;
using System.Net;
using System.Windows.Forms;
using System;
using System.Collections;
namespace myclouddisk
{
    class HTTPClient
    {
        public static string host;
        public static void Main()
        {
            //HTTPClient.createUser("045130160", "123456");
            //HTTPClient.PUT("045130160", "123456", "scloud_container", "scloud-container", @"C:\我的云盘\popmusic\jackson");
            //HTTPClient.GET("045130160", "123456", "scloud_container", "scloud-container", @"C:\我的云盘\popmusic");
            //HTTPClient.getRootContainer("045130160","123456");
            //HTTPClient.DELETE("045130160", "123456", "scloud_container", "scloud-container", @"C:\我的云盘\music");
            //HTTPClient.POST("045130160", "123456", "scloud_container", "scloud-container", @"C:\我的云盘\music","popmusic");
            //HTTPClient.PUT("045130160", "123456", "scloud_object", "scloud-object", @"C:\我的云盘\test.txt");
            HTTPClient.getRootContainer("045130160", "123456");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpURI"></param>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        public static string UploadFile(string httpURI, string localFilePath)
        {
            try
            {
                System.IO.FileInfo myfile = new System.IO.FileInfo(localFilePath);
                byte[] fileContentBytes = new byte[int.Parse(myfile.Length.ToString())];
                FileStream fsv = File.OpenRead(localFilePath);
                int nv = fsv.Read(fileContentBytes, 0, int.Parse(myfile.Length.ToString()));
                Uri destUri = new Uri(httpURI);
                MemoryStream inStream = new MemoryStream(fileContentBytes);
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(destUri);

                req.Method = "PUT";
                req.ContentType = "scloud-object";
                req.Headers.Add("Authorization", "045130160:123456");
                req.Headers.Add("X-CDMI-Specification-Version", "v1");
                req.Host = "192.168.1.113";
                req.Date = System.DateTime.Now;

                req.Timeout = 50000;

                Stream outStream = req.GetRequestStream();
                byte[] buffer = new byte[1024];
                while (true)
                {
                    int numBytesRead = inStream.Read(buffer, 0, buffer.Length);
                    if (numBytesRead <= 0)
                        break;
                    outStream.Write(buffer, 0, numBytesRead);
                }
                inStream.Close();
                outStream.Close();
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                Console.WriteLine(res.Headers);
                return "OK";


            }

            catch (System.Exception ee)
            {
                return ee.Message;
            }
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
            string httpuri = "http://192.168.1.113:8080/" + fileType;


            
            Uri uri = new Uri(localPath.Replace(@"C:\我的云盘", httpuri));
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
                FileStream rdr = null;
                try
                {
                    request.AllowWriteStreamBuffering = true;
                    // Retrieve request stream 
                    reqStream = request.GetRequestStream();
                    // Open the local file
                    rdr = new FileStream(localPath, FileMode.Open);

                    byte[] inData = new byte[1024];

                   
                    int bytesRead = rdr.Read(inData, 0, inData.Length);
                    Console.WriteLine("size:" + bytesRead);
                    
                    while (bytesRead > 0)
                    {
                        reqStream.Write(inData, 0, bytesRead);
                        bytesRead = rdr.Read(inData, 0, inData.Length);
                    }

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    rdr.Close();
                    reqStream.Close();
                    Console.WriteLine(response.Headers);
                    return;
                }

                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    if (reqStream != null)
                    {

                        reqStream.Close();

                    }

                    if (rdr != null)
                    {

                        rdr.Close();
                    }

                }
               
                return;
            }
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Console.WriteLine(response.Headers);
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.StatusDescription);
                Console.WriteLine(response.GetResponseStream());

                StreamReader streamReader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                string content = streamReader.ReadToEnd();
                Console.WriteLine(content);
                //StreamWriter sw = new StreamWriter("C:/我的云盘/test.txt");
                //sw.Write(content);
                //sw.Flush();
                //sw.Close();
                streamReader.Close();


                Console.WriteLine(content);
            }
            catch (System.Net.WebException ee)
            {
                Console.WriteLine(ee);
                return;
            }
            return;
        }
        public static string[] GET(string user, string password, string fileType, string contentType, string localPath)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create( generateHttpURI(fileType, localPath));

            request.ContentType = contentType;
            request.Method = "GET";
            request.Headers.Add("Authorization", user + ":" + password);
            request.Headers.Add("X-CDMI-Specification-Version", "v1");
            request.Host = "192.168.1.113";
            request.Date = System.DateTime.Now;

            request.Timeout = 50000;

            if(fileType == "scloud_object")
            {
                return null;
            }

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();


                string con = response.Headers.Get("Containers");
                if(con == null || con == "")
                {
                    return null;
                }

                string[] containers = con.Replace("[", "").Replace("]", "").Replace("'", "").Split(',');
                Console.WriteLine(response.Headers);
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.GetResponseStream());

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
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.GetResponseStream());

                StreamReader streamReader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                string content = streamReader.ReadToEnd();
                Console.WriteLine(content);
                //StreamWriter sw = new StreamWriter("C:/我的云盘/test.txt");
                //sw.Write(content);
                //sw.Flush();
                //sw.Close();
                streamReader.Close();

                Console.WriteLine(content);
            }
            catch (System.Net.WebException ee)
            {
                Console.WriteLine("无法连接到服务器：" + ee);
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
        public static string[] getRootContainer(string user, string password)
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

                string con = response.Headers.Get("Containers");
                if (con == null || con == "")
                {
                    return null;
                }
                string[] containers = con.Replace("[", "").Replace("]", "").Replace("'", "").Split(',');
                
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
            string httpuri = "http://192.168.1.113:8080/" + fileType + "/";
            Uri uri = new Uri(localPath.Replace(@"C:\我的云盘\", httpuri));
            Console.WriteLine("转化URI："+localPath+" -> "+uri.ToString());
            return uri;
        }




    }
}
