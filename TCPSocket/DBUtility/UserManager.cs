using System;
using System.Data.SqlClient;
using Readearth.Data;
/// <summary>
/// UserManager 的摘要说明
/// </summary>
public class UserManager
{
    private Database m_Database;

    public UserManager(Database db)
    {
        m_Database = db;
    }

    /// <summary>
    /// 用户登陆，判断用户是否合法，合法的话返回True，否则返回False
    /// </summary>
    /// <param name="user">用户对象</param>
    /// <returns></returns>
    public bool Login(User user)
    {
        string countString = "SELECT ALIAS,BZ FROM T_User WHERE USERNAME = '" + user.ID + "' AND SN = '" + user.Password + "'";
        SqlDataReader dataReader = m_Database.GetDataReader(countString);
        if (dataReader.HasRows)
        {
            if (dataReader.Read())
            {
                user.Name = dataReader.GetString(0);
                user.Class = dataReader[1].ToString();
            }
            dataReader.Close();
            return true;
        }
        return false;
    }

    public Part CreatePart()
    {
        Part newPart = new Part(m_Database);
        newPart.TableName = "T_Classes";
        return newPart;
    }

    public Part QueryPart(string partId)
    {
        Part queryPart = null;
        string strSQL = "SELECT * FROM T_Classes WHERE ID = " + partId;
        SqlDataReader drPart = m_Database.GetDataReader(strSQL);
        if (drPart.HasRows)
        {
            if (drPart.Read())
            {
                queryPart = new Part(m_Database);
                queryPart.TableName = "T_Classes";
                queryPart.PartID = drPart[0].ToString();
                queryPart.Name = drPart[1].ToString();
                queryPart.Description = drPart[2].ToString();
                queryPart.Organization = drPart[4].ToString();
                queryPart.Authority = drPart[5].ToString();
            }
        }
        drPart.Close();

        return queryPart;
    }
}
