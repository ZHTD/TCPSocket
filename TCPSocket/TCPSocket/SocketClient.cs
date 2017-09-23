using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TCPSocket
{
    public class SocketClient
    {
        public int port = 8000;                //监听端口号
        public TcpClient tcpc;       //对服务器端建立TCP连接
        public Socket tcpsend;          //发送创建套接字
        public bool connect_flag = false;
        public byte[] receive_buff = new byte[1024];
        public ManualResetEvent connectDone = new ManualResetEvent(false); //连接的信号
        public ManualResetEvent readDone = new ManualResetEvent(false);    //读信号
        public ManualResetEvent sendDone = new ManualResetEvent(false);    //发送结束

        public SocketClient(string IP,int Port)
        {
            string address = IP;
            port = Port;
            try
            {
                tcpsend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//初始化套接字
                IPEndPoint remotepoint = new IPEndPoint(IPAddress.Parse(address), port);//根据ip地址和端口号创建远程终结点
                EndPoint end = (EndPoint)remotepoint;
                //  tcpsend.Connect(end);
                //  connect_flag = true;
                tcpsend.BeginConnect(end, new AsyncCallback(ConnectedCallback), tcpsend); //调用回调函数
                connectDone.WaitOne();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void ConnectedCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            client.EndConnect(ar);//
            connect_flag = true;
            connectDone.Set();
        }

        public void Send(string data)
        {
            int length = data.Length;
            Byte[] Bysend = new byte[length];
            Bysend = System.Text.Encoding.Default.GetBytes(data); //将字符串指定到指定Byte数组
            tcpsend.BeginSend(Bysend, 0, Bysend.Length, 0, new AsyncCallback(SendCallback), tcpsend); //异步发送数据
            //int i = tcpsend.Send(Bysend);                         //发送数据
            sendDone.WaitOne();
        }

        private void SendCallback(IAsyncResult ar) //发送的回调函数
        {
            Socket client = (Socket)ar.AsyncState;
            int bytesSend = client.EndSend(ar);  //完成发送
            sendDone.Set();
        }

        public void Receive()   //接收数据
        {
            //byte[] receive=new byte[1024];
            tcpsend.BeginReceive(receive_buff, 0, receive_buff.Length, 0, new AsyncCallback(ReceiveCallback), tcpsend);
            sendDone.WaitOne();
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState; //获取句柄
                int bytesread = client.EndReceive(ar);

                if (bytesread > 0)
                {
                    client.BeginReceive(receive_buff, 0, receive_buff.Length, 0, new AsyncCallback(ReceiveCallback), client);
                    string content = Encoding.ASCII.GetString(receive_buff, 0, receive_buff.Length);
                    content = content.Trim('\0');
                    Console.WriteLine(content);// receive_buff.ToString();
                }
                else
                {
                    readDone.Set();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        #region 
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    //connectDone.Reset();
        //    //if (connect(textBox2.Text) == true)
        //    //{
        //    //    connectDone.WaitOne();
        //    //    textBox1.Text = "成功连接远程计算机";
        //    //}
        //    //else
        //    //{
        //    //    textBox1.Text = "连接失败";
        //    //}
        //}

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    //try
        //    //{
        //    //    //    sendDone.Reset();
        //    //    send(richTextBox1.Text);
        //    //    //   sendDone.WaitOne();
        //    //    textBox1.Text = "发送成功";
        //    //    //    readDone.Reset();
        //    //    receive();
        //    //    //    readDone.WaitOne();
        //    //    // byte[] back = new byte[1024];
        //    //    // int count = send.tcpsend.Receive((back));
        //    //    //  richTextBox2.Text = Encoding.ASCII.GetString(back, 0, count);
        //    //    //  send.tcpsend.Close();
        //    //}
        //    //catch
        //    //{
        //    //    textBox1.Text = "发送异常";
        //    //}
        //}

        //private void Form1_Leave(object sender, EventArgs e)
        //{
        //    tcpsend.Close();
        //}
        #endregion 

    }
}