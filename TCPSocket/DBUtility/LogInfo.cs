using System;
using System.Data;
using System.Configuration;


/// <summary>
///**********************************************************
///(C) Copyright 上海地听计算机技术有限公司. 2008
///文件名           :clsLogInfo.cls
///功能             :基础表中的系统日志实体
///作者             :张伟锋
///最后修改人       : 张伟锋
///最后更新日期     : 2008-3-7
///************************************************************
/// </summary>

public class LogInfo
{   
    private String mvarIP   ; //IP地址
    private String mvarOperatorContext; //操作内容
    private DateTime  mvarDatetime; //操作时间
    private String mvarUserName;//用户帐号
    private String mvarSQL;//操作SQL语句
    private String mvarRemark ; //操作备注
    private String mvarPartName; //操作者的角色
    private String mvarLogType; //操作的类型

	public LogInfo()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
    }

    #region
    public string LogType
    {
        get
        {
            return  mvarLogType;
        }
        set
        {
            this.mvarLogType = value;
        }
    }

    public string IP
    {
        get
        {
            return mvarIP;
        }
        set
        {
            this.mvarIP = value;
        }
    }

    public string OperatorContext
    {
        get
        {
            return mvarOperatorContext;
        }
        set
        {
            this.mvarOperatorContext = value;
        }
    }

    public  DateTime  Datetime
    {
        get
        {
            return mvarDatetime;
        }
        set
        {
            this.mvarDatetime = value;
        }
    }

    public string UserName
    {
        get
        {
            return mvarUserName;
        }
        set
        {
            this.mvarUserName = value;
        }
    }

    public string SQL
    {
        get
        {
            return mvarSQL;
        }
        set
        {
            this.mvarSQL = value;
        }
    }

    public string Remark
    {
        get
        {
            return mvarRemark;
        }
        set
        {
            this.mvarRemark = value;
        }
    }

    public string PartName
    {
        get
        {
            return mvarPartName;
        }
        set
        {
            this.mvarPartName = value;
        }
    }

    #endregion

}
