using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpLongHttpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.Write("1.Socket方式连接\n2.Http2方式连接");
            //string connectType = Console.ReadLine();
            //if (connectType == "1")
            //{
                Socket newsock = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                newsock.Connect("192.168.0.112", 9050);
                Task.Run(() =>
                {
                    while (true)
                    {
                        byte[] data = new byte[1024];
                        newsock.Receive(data);
                        string message = System.Text.Encoding.UTF8.GetString(data).Replace("\0", "").Trim();
                        if (string.IsNullOrEmpty(message))
                        {
                            newsock.Close();
                            Console.WriteLine($"服务器Socket关闭");
                            return;
                        }
                        Console.WriteLine($"服务器发来消息：{message}");
                    }
                });
                while (true)
                {
                    Console.Write("是否退出程序？(y/n):");
                    string isExit = Console.ReadLine();
                    if ("y".Equals(isExit, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
                newsock.Shutdown(SocketShutdown.Both);
                newsock.Dispose();
                newsock = null;
            //}
            //if (connectType == "2")
            //{
            //    HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://127.0.0.1:9050");
            //    //httpWebRequest.Headers.Add(HttpRequestHeader.Connection, "keep-alive");
            //    httpWebRequest.KeepAlive = true;
            //    while (true)
            //    {
            //        var response = httpWebRequest.GetResponseAsync().Result;
            //        Thread.Sleep(3000);
            //    }
            //}
        }
    }
}
