using System;


using System.Data.SqlClient;
using Readearth.Data;
/// <summary>
/// Part 类是用户管理的角色类。
/// 作者：张伟锋    日期：2009年5月24日         最后修改日期：2009年5月24日 
/// 公司：上海地听计算机技术有限公司
/// </summary>
public class Part
{
    private Database m_Database;
    private string m_PartID;
    private string m_Authority;
    private string m_Name;
    private string m_Description;
    private string m_Organization;
    private string m_TableName;
    public Part(Database db)
    {
		m_Database = db;
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    /// <summary>
    /// 角色ID
    /// </summary>
    public string PartID
    {
        get
        {
            return m_PartID;
        }
        set
        {
            m_PartID = value;
        }
    }

    /// <summary>
    /// 数据库
    /// </summary>
    public Database Database
    {
        get
        {
            return m_Database;
        }
        set
        {
            m_Database = value;
        }
    }

    /// <summary>
    /// 权限
    /// </summary>
    public string Authority
    {
        get
        {
            return m_Authority;
        }
        set
        {
            m_Authority = value;
        }
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name
    {
        get
        {
            return m_Name;
        }
        set
        {
            m_Name = value;
        }
    }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description
    {
        get
        {
            return m_Description;
        }
        set
        {
            m_Description = value;
        }
    }

    /// <summary>
    /// 组织结构
    /// </summary>
    public string Organization
    {
        get
        {
            return m_Organization;
        }
        set
        {
            m_Organization = value;
        }
    }

    /// <summary>
    /// 角色表
    /// </summary>
    public string TableName
    {
        get
        {
            return m_TableName;
        }
        set
        {
            m_TableName = value;
        }
    }
    /// <summary>
    /// 删除角色
    /// </summary>
    public bool Delete()
    {
        int nCount = 0;
        if (m_PartID != "")
        {
            string strSQL = "DELETE " + m_TableName + " WHERE ID = " + m_PartID;
            nCount = m_Database.Execute(strSQL);
        }
        return (nCount > 0);
    }

    /// <summary>
    /// 保存角色
    /// </summary>
    public bool Store()
    {
        int nCount = 0;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if (m_PartID != "")
        {
            sb.Append("UPDATE " + m_TableName);
            sb.Append(" SET JB = '" + m_Name);
            sb.Append("', DESCRIPTION = '" + m_Description);
            sb.Append("', CLASS = '" + m_Organization);
            sb.Append("', WEBAUTHORITY = '" + m_Authority);
            sb.Append("' WHERE ID = " + m_PartID);
        }
        else
        {
            sb.Append("INSERT INTO " + m_TableName);
            sb.Append("(JB,DESCRIPTION,CLASS,WEBAUTHORITY)");
            sb.Append(" VALUES('" + m_Name);
            sb.Append(" ','" + m_Description);
            sb.Append(" ','" + m_Organization);
            sb.Append(" ','" + m_Authority);
            sb.Append("')");
        }
        nCount = m_Database.Execute(sb.ToString());
        return (nCount > 0);
    }
}
