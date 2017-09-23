#region　***************Copyright Description************
/**********************************************************
(C) Copyright 奇科计算机信息有限公司. 2007
FileName         :  SqlHelper.cs
Function         : 数据库操作的基础类















Author           : 陈涛
Last modified by : 陈涛
Last modified    : 2007-6-18
************************************************************/
#endregion
using System;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Xml;
using System.Data.SqlClient;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace DBUtility
{
    /// <summary>
    /// 数据库操作的基础类.
    /// </summary>
    public sealed class SqlHelper
    {

      #region****** 数据库连接字符串 ******

        // 数据库的连接字符串,从app.config文件中读取，使用此字符串生成一个数据库的连接

        //Provider=MSDAORA.1;Password=数据库的密码;User ID=您的用户名;Data Source=服务器的地址或主机名;Persist Security Info=True 

        /// <summary>
        /// OLeDb数据库连接字符串
        /// </summary>
        public static string ConnectionStringForOle = SysParams.SIGS_DBForAccess;
        /// <summary>
        /// Oracle数据库连接字符串
        /// </summary>
        public static string ConnectionStringForOracle = SysParams.SIGS_RemoteOracleDBConnStr;
       /// <summary>
        /// SQLServer数据库连接字符串
       /// </summary>
        public static string ConnectionStringForSQLServer = SysParams.SIGS_DBForSQLServer;//For SQLServer
      #endregion 

        #region 2007091399002  赵宝柱          增加批量处理数据方法
        /// <summary>
        /// 批量处理数据,多条语句全部成功，否则全部回滚
        /// </summary>
        /// <param name="QueryStrs">数据处理sql语句的字符串数组</param>
        public static int OperatebatchDataForOracle(List<string> QueryStrs)
        {
            int result = 0;
            

            OracleCommand cmd;
            OracleConnection conn;
            try
            {
                conn = new OracleConnection(ConnectionStringForOracle);
                conn.Open();
            }
            catch (Exception ex)
            {
                return -1;
            }
            cmd = conn.CreateCommand();
            cmd.Transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);


            try
            {
                foreach (string QueryStr in QueryStrs)
                {

                    cmd.CommandText = QueryStr;

                    result += cmd.ExecuteNonQuery();

                }
                cmd.Transaction.Commit();
                conn.Close();
            }
            catch (Exception e)
            {

                cmd.Transaction.Rollback();
                conn.Close();
                return -1;
            }



            return result;

        }

        #endregion

        /// <summary>
        /// For Oracle
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQueryForOracle(CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn;
            try
            {
                conn = new OracleConnection(ConnectionStringForOracle);
            }
            catch (Exception)
            {
                return -1;
            }

            PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
            
            int val = 0;
            try
            {
                val = cmd.ExecuteNonQuery();                
            }
            catch (Exception)
            {
                conn.Close();
                val = -1;
            }
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
                
            }
            catch (Exception e)
            {
                val = -1;
            }
            return val;
        }

        /// <summary>
        /// For Oracle
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static OracleDataReader ExecuteReaderForOracle(CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            ///以下是Oracle专用写法
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn;
            try
            {
                conn = new OracleConnection(ConnectionStringForOracle);              
               
            }
            catch (Exception)
            {               
                return null;
            }
            PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
            OracleDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            cmd.Parameters.Clear();
            return rdr;

        }

        /// <summary>
        /// For Oracle
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteDatasetForOracle(CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn;
            try
            {
                conn = new OracleConnection(ConnectionStringForOracle);
            }
            catch (Exception)
            {
                //MessageBox.Show(CPromptSentence.GetPromptSentence("PromptSentence/SysManage/NetWorkError"), "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            PrepareCommand(cmd, conn, null, commandType, commandText, commandParameters);
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch (OracleException)
            {
                return null;
            }
            cmd.Parameters.Clear();
            return ds;

        }


        /// <summary>
        /// For Oracle
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static object ExecuteScalarForOracle(CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            ///以下是Oracle专用写法
            OracleCommand cmd = new OracleCommand();

            using (OracleConnection connection = new OracleConnection(ConnectionStringForOracle))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }           

        }


        /// <summary>
        /// For Oracle
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, OracleParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                try
                {
                    conn.Open();
                }
                catch (Exception)
                {
                    //MessageBox.Show("请检查数据库连接！");//throw new Exception(ex.Message);
                }              
            }

            cmd.Connection = conn;
            cmd.CommandType = cmdType;
            cmd.CommandText = cmdText;

            if (trans != null)
            {
                cmd.Transaction = trans;
            }

            if (cmdParms != null)
            {
                foreach (OracleParameter parm in cmdParms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
        }
 
        /// <summary>
        /// 判断是否处于联机远程数据库服务器模式
        /// </summary>
        /// <returns></returns>
        public static bool isConnect()
        {
            string strIsConnect = GetCenterServerSettings("ServerConfing/CenterServer/IsConnect");
            if (strIsConnect.Equals("1"))
            {
                try
                {
                    using (OracleConnection Conn = new OracleConnection(ConnectionStringForOracle))
                    {
                        Conn.Open();
                        if (Conn.State == ConnectionState.Open) return true;
                        else return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
                return false;
        }
        /// <summary>
        /// 获取中央服务器配置，以判断数据库服务器连接状态

        /// </summary>
        /// <param name="settingNodeName"></param>
        /// <returns></returns>
        public static string  GetCenterServerSettings(string settingNodeName)
        {
            string m_strFullPath = "";
            Assembly Asm = Assembly.GetExecutingAssembly();
            //获取配置文件的路径

            m_strFullPath = Asm.Location.Substring(0, (Asm.Location.LastIndexOf("\\") + 1)) + "XMLServerSettings.xml";
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(m_strFullPath);
            XmlNode xmlNode = xmlDoc.SelectSingleNode(settingNodeName);
            //返回节点 的文字            
            string UserName = xmlNode.InnerText;
            return UserName;

        }

        //得到SDE工作空间
        public static IWorkspace OpenSDEWorkspace()
        {
            string sdeServer = GetCenterServerSettings("ServerConfing/LocalSDEServer/Server");
            string sdeInstance = GetCenterServerSettings("ServerConfing/LocalSDEServer/Instance");
            string sdeUser = GetCenterServerSettings("ServerConfing/LocalSDEServer/UserName");
            string sdePassword = GetCenterServerSettings("ServerConfing/LocalSDEServer/PassWord");
            string sdeVersion = GetCenterServerSettings("ServerConfing/LocalSDEServer/Version");

            IPropertySet pPropertSet = new PropertySetClass();
            pPropertSet.SetProperty("Server", sdeServer);
            pPropertSet.SetProperty("Instance", sdeInstance);
            pPropertSet.SetProperty("user", sdeUser);
            pPropertSet.SetProperty("password", sdePassword);
            pPropertSet.SetProperty("version", sdeVersion);

            IWorkspace pWorkspace = null;
            IWorkspaceFactory pWorkspaceF = new ESRI.ArcGIS.DataSourcesGDB.SdeWorkspaceFactoryClass();
            try
            {
                pWorkspace = pWorkspaceF.Open(pPropertSet, 0);
            }
            catch (Exception e)
            {
                pWorkspace = null;
            }
            return pWorkspace;
        }

        #region ***********SQLSERVER操作*************

        /// <summary>
        /// 根据输入的查询语句，获取符合条件的数据集，数据集与DataReader的区别
        /// 是DataReader是向前只读的，用于快速只读访问因此占用的资源少。Dataset
        /// 提供数据存储空间。
        /// </summary>
        /// <param name="queryString">查询条件</param>
        /// <returns>数据集</returns>
        public static DataSet ExecuteDatasetForSQL(string queryString)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionStringForSQLServer))
            {
                //创建Command对象
                SqlCommand dbCommand = new SqlCommand(queryString, connection);
                try
                {
                    connection.Open();

                    //创建数据适配器
                    SqlDataAdapter dataAdapter = new SqlDataAdapter();
                    dataAdapter.SelectCommand = dbCommand;

                    //创建数据集，提取数据
                    DataSet dataset = new DataSet();
                    dataAdapter.Fill(dataset);

                    return dataset;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 获取DataReader对象，用于获取只读数据
        /// </summary>
        /// <param name="queryString">查询条件</param>
        /// <returns></returns>
        public static SqlDataReader GetDataReaderForSQL(string queryString)
        {
            SqlConnection connection = new SqlConnection(ConnectionStringForSQLServer);
            try
            {
                //创建Command对象
                SqlCommand dbCommand = new SqlCommand(queryString, connection);
                connection.Open();

                //创建DataReader对象，用于获取只读数据
                SqlDataReader dataReader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
                return dataReader;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static int GetCountForSQL(string countString)
        {
            int recCount = 0;
            using (SqlConnection connection = new SqlConnection(ConnectionStringForSQLServer))
            {
                SqlCommand dbCommand = new SqlCommand(countString, connection);
                try
                {
                    connection.Open();
                    recCount = (int)dbCommand.ExecuteScalar();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return recCount;
        }

        /// <summary>
        /// 对连接对象执行 SQL 语句
        /// </summary>
        /// <param name="editSQL">被执行的SQL语句，一般是编辑类型的SQL语句，当然以可以执行其它类型的SQL语句</param>
        /// <returns>对于 UPDATE、INSERT 和 DELETE 语句，返回值为该命令所影响的行数。对于其他所有类型的语句，返回值为 -1</returns>
        public static int ExecuteEditForSQL(string editSQL)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionStringForSQLServer))
            {
                SqlCommand dbCommand = new SqlCommand(editSQL, connection);
                try
                {
                    connection.Open();
                    return dbCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        //执行SQL中的存储过程（无返回值）
        public static SqlCommand ExecuteStoredProcForSQL(string commandText, SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionStringForSQLServer))
            {
                SqlCommand dbCommand = new SqlCommand(commandText, connection);
                dbCommand.CommandType = CommandType.StoredProcedure;
                try
                {
                    connection.Open();
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        dbCommand.Parameters.Add(parameters[i]);
                    }

                    dbCommand.ExecuteNonQuery();
                    return dbCommand;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        //执行SQL中的存储过程（有返回值）
        public static SqlDataReader ExecuteDataReaderForSQL(string commandText, SqlParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(ConnectionStringForSQLServer);
            SqlCommand dbCommand = new SqlCommand(commandText, connection);
            dbCommand.CommandType = CommandType.StoredProcedure;
            try
            {
                connection.Open();
                for (int i = 0; i < parameters.Length; i++)
                {
                    dbCommand.Parameters.Add(parameters[i]);
                }

                return dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion ***********SQLSERVER操作*************

    }
}