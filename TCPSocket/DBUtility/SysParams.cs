#region ***************Copyright Description************
/**********************************************************
(C) Copyright ��Ƽ������Ϣ���޹�˾. 2007
FileName         :  SysParams.cs
Function         : ����ϵͳ��¼�û�����Ϣ�Լ����ݿ����ӵ���Ϣ
Author           : ����
Last modified by : ����
Last modified    : 2007-6-10
************************************************************/
#endregion
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Data;
using System.Data.OracleClient;

using System.Collections;
using System.Management;
namespace Readearth.Data
{
    /// <summary>
    /// ����ϵͳ��¼�û�����Ϣ�Լ����ݿ����ӵ���Ϣ
    /// </summary>
    public sealed class SysParams
    {
        #region ****** Field ******


        public static string DATABASE = GetDBServerSettings("SysConfig/CenterServer/RemoteDataBase");
        public static string USERID = GetDBServerSettings("SysConfig/CenterServer/RemoteUser");
        public static string PASSWORD = GetDBServerSettings("SysConfig/CenterServer/RemotePass");

        /// <summary>
        /// Զ��Oracle DB Server���ݿ�������Ϣ
        /// </summary>
        public static string SIGS_RemoteOracleDBConnStr = GetRemoteOracleDBserverConnStr();
        
        /// <summary>
        /// ����SQL Server�������ݿ�������Ϣ
        /// </summary>
        public static string SIGS_DBForSQLServer = GetLocaleDBConnectionString(false);//@"workstation id=localhost;user id=sa;Integrated Security=SSPI;database=SIGS_DB;password=sa";
        /// <summary>
        /// ����SQL Server�Զ������ݿ�������Ϣ
        /// </summary>
        public static string SIGS_DBForAudiWithSQLServer = GetLocaleDBConnectionString(true);

                                               //"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=F:\\SHSubwayLIU\\SHSubwaySystem\\bin\\Debug\\SIGS_DB.mdb;Persist Security Info=True;Jet OLEDB:Database password=YIdi_5ty_2UiIty"
        /// <summary>
        /// ����Access�������ݿ�������Ϣ
        /// </summary>
        public static string SIGS_DBForAccess = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Assembly.GetExecutingAssembly().Location.Substring(0, (Assembly.GetExecutingAssembly().Location.LastIndexOf("\\"))) + @"\SIGS_DB.mdb;Persist Security Info=True;Jet OLEDB:Database password=YIdi_5ty_2UiIty";// ConfigurationManager.AppSettings["SIGS_DBForAccess"];
        /// <summary>
        /// ����Access�Զ������ݿ�������Ϣ
        /// </summary>
        public static string SIGS_DBForAudiWithOleDb = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Assembly.GetExecutingAssembly().Location.Substring(0, (Assembly.GetExecutingAssembly().Location.LastIndexOf("\\"))) + @"\SIGS_AUDIDB.mdb;Persist Security Info=True;Jet OLEDB:Database password=YIdi_5ty_2UiIty";
       


        //�����Ǳ����¼�ߵ���Ϣ
        public static string USERNAME = "";
        public static string LOGINUSERID = "";
        public static string NAME = "";
        public static string IP = "";
        public static string ChatFormIsOpen = "";
        public static Array ChatWindowOpenList = null;

        #endregion




        #region ****** Method ******
        /// <summary>
        /// ��ȡȫ��Ψһ��ʾ(��������)
        /// </summary>
        /// <returns></returns>
        public static string GetGUID()
        {
            return System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// ��ȡ��ǰ�������ݿ�������Ƿ����ʹ��
        /// </summary>
        /// <returns></returns>
        public static bool GetDBServerConnState()
        {
            return false;
        }

        /// <summary>
        /// ��ȡ�Զ����������
        /// </summary>
        /// <returns></returns>
        public static ArrayList GetAutoMonitorContentName()
        {
            ArrayList ContentName = new ArrayList();
            ContentName.Add("");
            return ContentName;
        }

        /// <summary>
        /// ��ȡ�����������
        /// </summary>
        /// <returns></returns>
        public static string GetLocalNetWorkCardNumber()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc2 = mc.GetInstances();
            string strNetWorkCardNumber = "";
            foreach (ManagementObject mo in moc2)
            {
                if ((bool)mo["IPEnabled"] == true)
                    strNetWorkCardNumber= mo["MacAddress"].ToString();
                mo.Dispose();
            }
            return strNetWorkCardNumber;
        }

        /// <summary>
        /// ��������������������Ϣ
        /// </summary>
        /// <param name="settingNodeName">������Ϣ·��</param>
        /// <returns></returns>
        public static string GetDBServerSettings(string settingNodeName)
        {
            string m_strFullPath = "";
            Assembly Asm = Assembly.GetExecutingAssembly();
            //��ȡ�����ļ���·��
            m_strFullPath = Asm.Location.Substring(0, (Asm.Location.LastIndexOf("\\") + 1)) + "Settings.xml";
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(m_strFullPath);
            XmlNode xmlNode = xmlDoc.SelectSingleNode(settingNodeName);
            //���ؽڵ� ������            
            string UserName = xmlNode.InnerText;
            return UserName;

        }

        /// <summary>
        /// ��ȡ����SQLServer DB������������Ϣ
        /// </summary>
        /// <param name="isAuto">�Ƿ��Զ�����</param>
        /// <returns></returns>
        public static string GetLocaleDBConnectionString(bool isAuto)
        {
            string localDBServer = GetDBServerSettings("SysConfig/LocalDBServer/LocalHost");
            string locaDB = "";
            if (isAuto)
            {
                locaDB = GetDBServerSettings("SysConfig/LocalDBServer/AutoDB");
            }
            else
            {
                locaDB = GetDBServerSettings("SysConfig/LocalDBServer/SigsDB");
            };
            string userID = GetDBServerSettings("SysConfig/LocalDBServer/UserName");
            string userpsw = GetDBServerSettings("SysConfig/LocalDBServer/PassWord");
            string sqlConnstr = @"server="+localDBServer+";user id="+userID+";password="+userpsw+";database="+locaDB+";pooling=true;";
            return sqlConnstr;
        }
        /// <summary>
        /// ��ȡԶ��Oracle DB������������Ϣ
        /// </summary>
        /// <returns></returns>
        public static string GetRemoteOracleDBserverConnStr()
        {
            string _DATABASE = GetDBServerSettings("SysConfig/CenterServer/RemoteDataBase");
            string _USERID = GetDBServerSettings("SysConfig/CenterServer/RemoteUser");
            string _PASSWORD = GetDBServerSettings("SysConfig/CenterServer/RemotePass");
            string ConnectionStringForOracle = @"Data Source =" + _DATABASE + ";user id=" + _USERID + ";password=" + _PASSWORD + ";Unicode=True";
            return ConnectionStringForOracle;
        }
        #endregion
    }
}
