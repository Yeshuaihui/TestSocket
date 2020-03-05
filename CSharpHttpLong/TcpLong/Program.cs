using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TcpLong
{
    class Program
    {
        static Dictionary<IPEndPoint, Socket> sockets = new Dictionary<IPEndPoint, Socket>();
        static void Main(string[] args)
        {
            //该服务器可接收可配置在系统中任何网络接口上的接入连接请求
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
            Socket newsock = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            //套接字与ip,端口绑定
            newsock.Bind(ipep);

            //套接字收听接入连接
            newsock.Listen(100);

            Console.WriteLine("等待客户端连接....");

            //接收来自客户机的接入连接尝试，
            //返回一个新的套接字，在于客户机的通信中使用这个新的套接字
            Task.Run(() =>
            {
                while (true)
                {
                    Socket client = newsock.Accept();//同步方法
                    IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;//客户机的ip和端口信息
                    if (!sockets.ContainsKey(clientep))
                    {
                        sockets.Add(clientep, client);
                    }
                    //Receive(clientep);
                }
            });
            bool isBreak = false;
            while (!isBreak)
            {
                Console.Write("是否推送消息？y/n:");
                string isSend = Console.ReadLine();
                if ("y".Equals(isSend, StringComparison.OrdinalIgnoreCase))
                {
                    Console.Write("请输入需要推送的消息：");
                    string message = Console.ReadLine();
                    Send(message);
                }
                else
                {
                    isBreak = true;
                }
            }

            //string welcom = "welcom to my test server";
            //data = Encoding.ASCII.GetBytes(welcom);

            ////发出欢迎信息
            //client.Send(data, data.Length, SocketFlags.None);

            ////循环等待来自客户端的信息
            //while (true)
            //{
            //    //Recive在使用buffer的同时，重新设置了buffer的大小，因此重新设置为原大小
            //    data = new byte[1024];
            //    recv = client.Receive(data);//同步方法
            //    if (recv == 0)
            //    {
            //        break;//客户端退出则服务器端退出                 
            //    }
            //    Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
            //    client.Send(data, recv, SocketFlags.None);//发出应答信息               
            //}

            //Console.WriteLine("disconnected from {0}", clientep.Address);
            //client.Close();//关闭客户机套接字
            CloseSocket();
            newsock.Close();//不需要其它的连接关闭原始的套接字
        }
        static void Receive(IPEndPoint iPEndPoint)
        {
            Socket client = sockets[iPEndPoint];
            if (client != null)
            {
                client.SendTimeout = 5000;
                Task.Run(() =>
                {
                    while (true)
                    {
                        byte[] data = new byte[1024];
                        int recv = client.Receive(data);
                        if (recv == 0)
                        {
                            client.Close();
                            break;//客户端退出则服务器端退出                 
                        }
                    }
                });
            }
        }


        static void Send(string message)
        {
            var keys = sockets.Keys;
            foreach (var key in keys)
            {
                Task.Run(() =>
                {
                    if (sockets[key].Connected)
                    {
                        try
                        {
                            sockets[key].Send(Encoding.UTF8.GetBytes(message));
                        }
                        catch (SocketException ex)
                        {
                            if (ex.NativeErrorCode == 10054)
                            {
                                sockets[key].Close();
                                sockets.Remove(key);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            sockets[key].Connect(key);
                            sockets[key].Send(Encoding.UTF8.GetBytes(message));
                        }
                        catch (SocketException ex)
                        {
                            if (ex.NativeErrorCode == 10054)
                            {
                                sockets[key].Close();
                                sockets.Remove(key);
                            }
                        }
                    }
                });
            }
        }


        static void CloseSocket()
        {
            foreach (var item in sockets)
            {
                item.Value.Close();
                item.Value.Dispose();
            }
            sockets = null;
        }
    }
}