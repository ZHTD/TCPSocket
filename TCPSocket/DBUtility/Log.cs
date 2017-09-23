using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using Readearth.Data.Entity;
using Readearth.Data;


/// <summary>
///**********************************************************
///(C) Copyright 上海地听计算机技术有限公司. 2008
///文件名           :clsLog.cls
///功能             :操作日志
///作者             :张伟锋
///最后修改人       : 张伟锋
///最后更新日期     : 2008-3-7
///************************************************************
/// </summary>


public class Log
{
    private Database m_Database;//实体所在的数据库;
    private Entity m_LogEntity;//实体对象

    public Log(Database db)
	{
		m_Database = db;
        m_LogEntity = new Entity(m_Database,"logs");//初始化实体变量
	}

    public void AddLog(LogInfo pLoginfo)
    {
        //目的：把日志信息插入到日志表中
        String strSQL ;
        LogInfo p = new LogInfo();
        p = pLoginfo;

        strSQL = "INSERT INTO " + m_LogEntity.TableName + " VALUES ('" + p.PartName + "','" + p.UserName + "','" + p.IP + "','" + p.OperatorContext + "','" + p.SQL + "','" + p.Datetime + "','" + p.Remark + "','" + p.LogType + "')";
        m_Database.Execute(strSQL);
      
    }

    public void DeleteLog(int  logID)
    {
        //目的：通过日志ID，删除日志信息
        String strSQL ;
    
        strSQL = "DELETE " + m_LogEntity.TableName + " WHERE ID = " + logID ;
        m_Database.Execute(strSQL);

    }

    public void DeleteLogByFilter(String whereCause)
    {
        //目的：根据条件删除日志
        String strSQL ;

        strSQL = "DELETE " + m_LogEntity.TableName + " WHERE " + whereCause ;
        m_Database.Execute(strSQL);
    }

    public DataSet GetLogs(String whereCause)
    {
        //目的：根据条件返回日志
        DataSet LogDetail = m_LogEntity.Query(whereCause, "");
        return LogDetail;
    }
    public DataSet GetLogs()
    {
        //目的：根据条件返回日志
        m_LogEntity.EntityState =EntityStateContants.esQuery;
        DataSet LogDetail = m_LogEntity.Submit(null);
        return LogDetail;
    }
}
