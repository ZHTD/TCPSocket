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
        public int port = 8000;                //�����˿ں�
        public TcpClient tcpc;       //�Է������˽���TCP����
        public Socket tcpsend;          //���ʹ����׽���
        public bool connect_flag = false;
        public byte[] receive_buff = new byte[1024];
        public ManualResetEvent connectDone = new ManualResetEvent(false); //���ӵ��ź�
        public ManualResetEvent readDone = new ManualResetEvent(false);    //���ź�
        public ManualResetEvent sendDone = new ManualResetEvent(false);    //���ͽ���

        public SocketClient(string IP,int Port)
        {
            string address = IP;
            port = Port;
            try
            {
                tcpsend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//��ʼ���׽���
                IPEndPoint remotepoint = new IPEndPoint(IPAddress.Parse(address), port);//����ip��ַ�Ͷ˿ںŴ���Զ���ս��
                EndPoint end = (EndPoint)remotepoint;
                //  tcpsend.Connect(end);
                //  connect_flag = true;
                tcpsend.BeginConnect(end, new AsyncCallback(ConnectedCallback), tcpsend); //���ûص�����
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
            Bysend = System.Text.Encoding.Default.GetBytes(data); //���ַ���ָ����ָ��Byte����
            tcpsend.BeginSend(Bysend, 0, Bysend.Length, 0, new AsyncCallback(SendCallback), tcpsend); //�첽��������
            //int i = tcpsend.Send(Bysend);                         //��������
            sendDone.WaitOne();
        }

        private void SendCallback(IAsyncResult ar) //���͵Ļص�����
        {
            Socket client = (Socket)ar.AsyncState;
            int bytesSend = client.EndSend(ar);  //��ɷ���
            sendDone.Set();
        }

        public void Receive()   //��������
        {
            //byte[] receive=new byte[1024];
            tcpsend.BeginReceive(receive_buff, 0, receive_buff.Length, 0, new AsyncCallback(ReceiveCallback), tcpsend);
            sendDone.WaitOne();
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState; //��ȡ���
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
        //    //    textBox1.Text = "�ɹ�����Զ�̼����";
        //    //}
        //    //else
        //    //{
        //    //    textBox1.Text = "����ʧ��";
        //    //}
        //}

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    //try
        //    //{
        //    //    //    sendDone.Reset();
        //    //    send(richTextBox1.Text);
        //    //    //   sendDone.WaitOne();
        //    //    textBox1.Text = "���ͳɹ�";
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
        //    //    textBox1.Text = "�����쳣";
        //    //}
        //}

        //private void Form1_Leave(object sender, EventArgs e)
        //{
        //    tcpsend.Close();
        //}
        #endregion 

    }
}