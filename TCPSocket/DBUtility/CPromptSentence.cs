#region ***************Copyright Description************
/**********************************************************
(C) Copyright 奇科计算机信息有限公司. 2007
FileName         : CPromptSentence.cs
Function         : 读取提示语语句操作的类
Author           : 占林
Last modified by : 占林
Last modified    : 2007-6-1
************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Runtime;
using System.Reflection;
namespace Readearth.Data
{
    /// <summary>
    /// 读取提示语语句操作的类
    /// </summary>
    public class CPromptSentence
    {
        #region ****** Method ******
        /// <summary>
        /// 获取提示语句
        /// </summary>
        /// <param name="sentenceNodeName">提示语句节点</param>
        /// <returns>提示语句</returns>
        public static string GetPromptSentence(string sentenceNodeName)
        {
            string m_strFullPath = "";
            Assembly Asm = Assembly.GetExecutingAssembly();
            //获取配置文件的路径
            m_strFullPath = Asm.Location.Substring(0, (Asm.Location.LastIndexOf("\\") + 1)) + "PromptSentence.xml";
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(m_strFullPath);
            XmlNode xmlNode = xmlDoc.SelectSingleNode(sentenceNodeName);
            //返回节点 的文字

            string promptSentence = xmlNode.InnerText;
            return promptSentence;
        }
        #endregion
    }
}
