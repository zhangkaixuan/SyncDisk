using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Data;
namespace myclouddisk
{
    public class Log
    {

        private string _fileName;
        private static Dictionary<long, long> lockDic = new Dictionary<long, long>();
        /// <summary>
        /// 获取或设置文件名称
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="byteCount">每次开辟位数大小，这个直接影响到记录文件的效率</param>
        /// <param name="fileName">文件全路径名</param>
        public Log(string fileName)
        {   // string r_path = System.Environment.CurrentDirectory;
            // string log_file = Path.Combine(r_path,fileName);
            _fileName = fileName;
        }
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="fileName"></param>
        public void Create(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
            {
                using (System.IO.FileStream fs = System.IO.File.Create(fileName))
                {
                    fs.Close();
                }
            }
        }
        /// <summary>
        /// 写入文本
        /// </summary>
        /// <param name="content">文本内容</param>
        private void Write(string content, string newLine)
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                throw new Exception("FileName不能为空！");
            }
            using (System.IO.FileStream fs = new System.IO.FileStream(_fileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite, 8, System.IO.FileOptions.Asynchronous))
            {
                //Byte[] dataArray = System.Text.Encoding.ASCII.GetBytes(System.DateTime.Now.ToString() + content + "/r/n");
                Byte[] dataArray = System.Text.Encoding.Default.GetBytes(content + newLine);
                bool flag = true;
                long slen = dataArray.Length;
                long len = 0;
                while (flag)
                {
                    try
                    {
                        if (len >= fs.Length)
                        {
                            fs.Lock(len, slen);
                            lockDic[len] = slen;
                            flag = false;
                        }
                        else
                        {
                            len = fs.Length;
                        }
                    }
                    catch (Exception ex)
                    {
                        //
                        //有异常！
                        Console.WriteLine(ex.Message);
                        while (!lockDic.ContainsKey(len))
                        {
                            len += lockDic[len];
                        }
                    }
                }
                fs.Seek(len, System.IO.SeekOrigin.Begin);
                fs.Write(dataArray, 0, dataArray.Length);
                fs.Close();
            }
        }
        /// <summary>
        /// 写入文件内容
        /// </summary>
        /// <param name="content"></param>
        public void WriteLine(string content)
        {
            this.Write(content, System.Environment.NewLine);
        }
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="content"></param>
        public void Write(string content)
        {
            this.Write(content, "");
        }
    }
}

