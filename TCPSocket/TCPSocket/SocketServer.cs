using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Text.RegularExpressions;
using LogManagerClass;
using Readearth.Data;

namespace TCPSocket
{
    public class SocketServer
    {

        public int port = 8000;
        public Thread threadread;
        public IPEndPoint tcplisener;
        public bool listen_flag = false;
        public Socket read;
        public Thread accept;
        public Queue DataStack = new Queue();
        public Database db;
        public DataTable Table_code;

        public ManualResetEvent AcceptDone = new ManualResetEvent(false); //连接的信号

        public SocketServer()
        {
            db = new Database();
            string sql="select * from D_ItemCode";
            Table_code = db.GetDataTable(sql);
        }

        public void StatTCP(int Port)
        {
            port = Port;
            tcplisener = new IPEndPoint(IPAddress.Any, port);
            read = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            read.Bind(tcplisener); //绑定
            read.Listen(500); //开始监听

            accept = new Thread(new ThreadStart(Listen));
            accept.Start();
        }

        private void Listen()
        {
            //Thread.CurrentThread.IsBackground = true; //后台线程
            try
            {
                while (true)
                {
                    AcceptDone.Reset();
                    read.BeginAccept(new AsyncCallback(AcceptCallback), read);  //异步调用
                    AcceptDone.WaitOne();
                }
            }
            catch (Exception er)
            {
                LogManager.WriteLog("error", er.ToString());
            }
        }

        public virtual void AcceptCallback(IAsyncResult ar) //accpet的回调处理函数
        {
            AcceptDone.Set();
            Socket temp_socket = (Socket)ar.AsyncState;
            Socket client = temp_socket.EndAccept(ar); //获取远程的客户端

            Console.WriteLine("建立连接");
            IPEndPoint remotepoint = (IPEndPoint)client.RemoteEndPoint;//获取远程的端口
            string remoteaddr = remotepoint.Address.ToString();        //获取远程端口的ip地址
            Console.WriteLine("接收来自" + remoteaddr + "的连接");

            StateObject state = new StateObject();
            state.workSocket = client;
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        public virtual void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            try
            {
                int bytesRead = handler.EndReceive(ar);
                if (bytesRead > 0)
                {
                    string receiveData = ""; //接收到的数据
                    string sendData = "ST=91;CN=9013;";
                    string str_CRC = "";
                    receiveData += Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss : ") + receiveData);
                    bool Inte = Integrity(ref receiveData, ref str_CRC);
                    try
                    {
                        ReceiveMession ReceiveM = new ReceiveMession(receiveData);
                        if (Inte)
                        {
                            sendData = sendData + ReceiveM.PW + ";" + ReceiveM.MN + ";" + "Flag=0;CP=&&" + ReceiveM.QN + "&&";
                            int Count = sendData.Length;
                            sendData = "##" + Count.ToString("0000") + sendData + CRC.getCrc16Code(str_CRC) + "\r\n";

                            lock (this)
                            {
                                DataStack.Enqueue(ReceiveM);
                            }
                        }
                        else
                        {
                            sendData = sendData + ReceiveM.PW + ";" + ReceiveM.MN + ";" + "Flag=1;CP=&&" + ReceiveM.QN + "&&";
                            int Count = sendData.Length;
                            sendData = "##" + Count.ToString("0000") + sendData + CRC.getCrc16Code(str_CRC) + "\r\n";
                        }
                        byte[] byteData = Encoding.ASCII.GetBytes(sendData);//回发信息
                        handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public virtual void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        //数据完整性验证
        private static bool Integrity(ref string receiveData,ref string str_CRC)
        {
            bool Wanzheng = true;
            string Str = receiveData.Substring(0,2);
            if (Str != "##")
                Wanzheng = false;
            else
            {
                receiveData = receiveData.Substring(2);
                Str = receiveData.Substring(receiveData.Length - 2, 2);
                if (Str != "\r\n")
                    Wanzheng = false;
                else
                {
                    receiveData = receiveData.Substring(0, receiveData.Length - 2);
                    Str = receiveData.Substring(0, 4);
                    str_CRC = receiveData.Substring(receiveData.Length - 4, 4);
                    receiveData = receiveData.Substring(4, receiveData.Length - 8);
                    if (receiveData.Length != int.Parse(Str))
                        Wanzheng = false;
                }
            }
            return Wanzheng;
        }

        //处理数据
        public void ReadData()
        {
            while (true)
            {
                try
                {
                    ReceiveMession Rm;
                    lock (this)
                    {
                        Rm = (ReceiveMession)DataStack.Dequeue();
                    }
                    ToDatabase(Rm);
                }
                catch
                {
                    Thread.Sleep(1000*60);
                }
            }
        }

        private void ToDatabase(ReceiveMession Rm)
        {
            string SQL_str = string.Format("insert into T_SuperStationmin (StartTime,EndTime,City,Station,Item,Value,Quality,Type,DValue,OPCode) values('{0}','{1}','{2}','{3}'", Rm.dateTime, Rm.dateTime, "上海", "环科院站");
            for (int i = 0; i < Rm.Data.Length; i++)
            {
                string OPCode = "N";
                if (Rm.Data[i].Contains(","))
                    OPCode = Rm.Data[i].Split(',')[1].Split('=')[1];
                string data = Rm.Data[i].Split(',')[0];
                string[] code = GetItemCode(data);
                string Str_SQL = string.Format(",{0},{1},'{2}','{3}',{4},'{5}')", code[0], double.Parse(data.Split('=')[1]), "", code[1], double.Parse(data.Split('=')[1]), OPCode);
                Str_SQL = SQL_str + Str_SQL;
                try
                {
                    db.Execute(Str_SQL);
                }
                catch
                { }
            }
        }

        private struct ReceiveMession
        {
            public string QN;
            public string ST;
            public string CN;
            public string PW;
            public string MN;
            public DateTime dateTime;
            public string[] Data;

            public ReceiveMession(string receiveData)
            {
                QN = "";
                ST = "";
                CN = "";
                PW = "";
                MN = "";
                receiveData = receiveData.Replace("&&", "");
                string[] info = Regex.Split(receiveData, ";CP=", RegexOptions.None)[0].Split(';');
                Data = Regex.Split(receiveData, ";CP=", RegexOptions.None)[1].Split(';');
                for (int i = 0; i < info.Length; i++)
                {
                    if (info[i].Contains("QN"))
                        QN = info[i];
                    if (info[i].Contains("ST"))
                        ST = info[i];
                    if (info[i].Contains("CN"))
                        CN = info[i];
                    if (info[i].Contains("PW"))
                        PW = info[i];
                    if (info[i].Contains("MN"))
                        MN = info[i];
                }
                dateTime = DateTime.Parse(Data[0].Split('=')[1].Insert(4, "-").Insert(7, "-").Insert(10, " ").Insert(13, ":").Insert(16, ":")); 
                ArrayList al = new ArrayList(Data);
                al.RemoveAt(0);
                Data = (string[])al.ToArray(typeof(string));
            }
        }

        private string[] GetItemCode(string data)
        {
            string[] code = new string[2];
            DataRow[] dr = Table_code.Select("Code='" + data.Split('=')[0].Split('-')[0] + "'");
            if (dr.Length > 0)
            {
                code[0] = dr[0]["DM"].ToString();
                code[1] = dr[0]["Type"].ToString();
            }
            return code;
        }
    }

}
