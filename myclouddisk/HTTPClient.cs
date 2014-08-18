using System.IO;
using System.Net;
using System.Windows.Forms;

namespace cloudstorageMonitor
{
    class HTTPClient
    {
        public HTTPClient()
        {

        }
        public HttpWebRequest createRequest(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                return request;
            }
            catch (WebException exp)
            {
                MessageBox.Show(exp.Message,"无法创建请求！");
            }
            return null;
            
        }
        public HttpWebResponse getResponse(HttpWebRequest request)
        {
            request.Timeout = 10000;
            return (HttpWebResponse)request.GetResponse();
        }
        public StreamReader getResponseStream(HttpWebRequest request)
        {
            StreamReader streamReader = new StreamReader(getResponse(request).GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
            return streamReader;
        }
        public string getResponseBody(HttpWebRequest request)
        {
            //string content = getResponseStream(request).ReadToEnd;
            return "";
        }
        public void getResponseJson()
        {

        }
        public void getResponseXML()
        {

        }
        public bool getResponseFile(HttpWebRequest request)
        {
            Stream str=getResponse(request).GetResponseStream();
            StreamReader streamReader=new StreamReader(str,System.Text.Encoding.GetEncoding("UTF-8"));
            byte[] mbyte = new byte[100000];
　　        int allmybyte = (int)mbyte.Length;
　　        int startmbyte = 0;
            //开始写入文件
　　        while(allmybyte>0)
　　       {
　　          int m = str.Read(mbyte,startmbyte,allmybyte);
　　          if(m==0)
　　              break;
　　
　　          startmbyte+=m;
　　          allmybyte-=m;
　　        }         
　　
　　      //FileStream fstr = new FileStream(Path,FileMode.OpenOrCreate,FileAccess.Write);
          //需要知道文件的格式，即扩展名
          //
          FileStream fstr = new FileStream(@"D:\test\downlodfile.txt", FileMode.OpenOrCreate, FileAccess.Write);
　　      fstr.Write(mbyte,0,startmbyte);
　　      str.Close();
　　      fstr.Close();
          return true;
          //成功写入文件　　

        }
        public bool upLoadFile(HttpWebRequest request)
        {
            return true;
        }

    }
}
