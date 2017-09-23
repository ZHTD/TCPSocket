using System;
using System.Data;

namespace Readearth.Data
{
    /// <summary>
    /// User 的摘要说明
    /// </summary>
    public class User
    {
        private string m_ID;
        private string m_Pwd;
        private string m_Name;
        private string m_Class;
        public User()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        public User(string id, string pwd, string name)
        {
            m_ID = id;
            m_Pwd = pwd;
            m_Name = name;
        }
        /// <summary>
        /// 帐号
        /// </summary>
        public string ID
        {
            get
            {
                return m_ID;
            }
            set
            {
                this.m_ID = value;
            }
        }

        /// <summary>
        /// 用户名
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

        public string Password
        {
            get
            {
                return m_Pwd;
            }
            set
            {
                m_Pwd = value;
            }
        }

        public string Class
        {
            get
            {
                return m_Class;
            }
            set
            {
                m_Class = value;
            }
        }


    }
}