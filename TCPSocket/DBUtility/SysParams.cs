#region ***************Copyright Description************
/**********************************************************
(C) Copyright 奇科计算机信息有限公司. 2007
FileName         :  SysParams.cs
Function         : 保存系统登录用户的信息以及数据库连接的信息
Author           : 陈涛
Last modified by : 陈涛
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
    /// 保存系统登录用户的信息以及数据库连接的信息
    /// </summary>
    public sealed class SysParams
    {
        #region ****** Field ******


        public static string DATABASE = GetDBServerSettings("SysConfig/CenterServer/RemoteDataBase");
        public static string USERID = GetDBServerSettings("SysConfig/CenterServer/RemoteUser");
        public static string PASSWORD = GetDBServerSettings("SysConfig/CenterServer/RemotePass");

        /// <summary>
        /// 远程Oracle DB Server数据库连接信息
        /// </summary>
        public static string SIGS_RemoteOracleDBConnStr = GetRemoteOracleDBserverConnStr();
        
        /// <summary>
        /// 本地SQL Server基础数据库连接信息
        /// </summary>
        public static string SIGS_DBForSQLServer = GetLocaleDBConnectionString(false);//@"workstation id=localhost;user id=sa;Integrated Security=SSPI;database=SIGS_DB;password=sa";
        /// <summary>
        /// 本地SQL Server自动化数据库连接信息
        /// </summary>
        public static string SIGS_DBForAudiWithSQLServer = GetLocaleDBConnectionString(true);

                                               //"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=F:\\SHSubwayLIU\\SHSubwaySystem\\bin\\Debug\\SIGS_DB.mdb;Persist Security Info=True;Jet OLEDB:Database password=YIdi_5ty_2UiIty"
        /// <summary>
        /// 本地Access基础数据库连接信息
        /// </summary>
        public static string SIGS_DBForAccess = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Assembly.GetExecutingAssembly().Location.Substring(0, (Assembly.GetExecutingAssembly().Location.LastIndexOf("\\"))) + @"\SIGS_DB.mdb;Persist Security Info=True;Jet OLEDB:Database password=YIdi_5ty_2UiIty";// ConfigurationManager.AppSettings["SIGS_DBForAccess"];
        /// <summary>
        /// 本地Access自动化数据库连接信息
        /// </summary>
        public static string SIGS_DBForAudiWithOleDb = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Assembly.GetExecutingAssembly().Location.Substring(0, (Assembly.GetExecutingAssembly().Location.LastIndexOf("\\"))) + @"\SIGS_AUDIDB.mdb;Persist Security Info=True;Jet OLEDB:Database password=YIdi_5ty_2UiIty";
       


        //以下是保存登录者的信息
        public static string USERNAME = "";
        public static string LOGINUSERID = "";
        public static string NAME = "";
        public static string IP = "";
        public static string ChatFormIsOpen = "";
        public static Array ChatWindowOpenList = null;

        #endregion




        #region ****** Method ******
        /// <summary>
        /// 获取全球唯一表示(做主键用)
        /// </summary>
        /// <returns></returns>
        public static string GetGUID()
        {
            return System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 获取当前中央数据库服务器是否可以使用
        /// </summary>
        /// <returns></returns>
        public static bool GetDBServerConnState()
        {
            return false;
        }

        /// <summary>
        /// 获取自动监测内容名
        /// </summary>
        /// <returns></returns>
        public static ArrayList GetAutoMonitorContentName()
        {
            ArrayList ContentName = new ArrayList();
            ContentName.Add("");
            return ContentName;
        }

        /// <summary>
        /// 获取本地网卡编号
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
        /// 获得中央服务器的连接信息
        /// </summary>
        /// <param name="settingNodeName">配置信息路径</param>
        /// <returns></returns>
        public static string GetDBServerSettings(string settingNodeName)
        {
            string m_strFullPath = "";
            Assembly Asm = Assembly.GetExecutingAssembly();
            //获取配置文件的路径
            m_strFullPath = Asm.Location.Substring(0, (Asm.Location.LastIndexOf("\\") + 1)) + "Settings.xml";
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(m_strFullPath);
            XmlNode xmlNode = xmlDoc.SelectSingleNode(settingNodeName);
            //返回节点 的文字            
            string UserName = xmlNode.InnerText;
            return UserName;

        }

        /// <summary>
        /// 获取本地SQLServer DB服务器连接信息
        /// </summary>
        /// <param name="isAuto">是否自动化库</param>
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
        /// 获取远程Oracle DB服务器连接信息
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
