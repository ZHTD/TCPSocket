#region ***************Copyright Description************
/**********************************************************
(C) Copyright ��Ƽ������Ϣ���޹�˾. 2007
FileName         : CPromptSentence.cs
Function         : ��ȡ��ʾ������������
Author           : ռ��
Last modified by : ռ��
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
    /// ��ȡ��ʾ������������
    /// </summary>
    public class CPromptSentence
    {
        #region ****** Method ******
        /// <summary>
        /// ��ȡ��ʾ���
        /// </summary>
        /// <param name="sentenceNodeName">��ʾ���ڵ�</param>
        /// <returns>��ʾ���</returns>
        public static string GetPromptSentence(string sentenceNodeName)
        {
            string m_strFullPath = "";
            Assembly Asm = Assembly.GetExecutingAssembly();
            //��ȡ�����ļ���·��
            m_strFullPath = Asm.Location.Substring(0, (Asm.Location.LastIndexOf("\\") + 1)) + "PromptSentence.xml";
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(m_strFullPath);
            XmlNode xmlNode = xmlDoc.SelectSingleNode(sentenceNodeName);
            //���ؽڵ� ������

            string promptSentence = xmlNode.InnerText;
            return promptSentence;
        }
        #endregion
    }
}
