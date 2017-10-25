using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using log4net;
using Dashboard.Core.Log4netHelper;
using System.Reflection;

namespace Dashboard.Core.Utility
{
    public static class EmailHelper
    {
        #region Public Methods

        /// <summary> 
        /// 发送邮件 
        /// </summary> 
        /// <param name="sender">发件人邮件地址</param> 
        /// <param name="reciver">收件人邮件地址</param> 
        /// <param name="subject">邮件主题</param> 
        /// <param name="body">邮件内容</param> 
        /// <param name="username">登录smtp主机时用到的用户名,注意是邮件地址'@'以前的部分</param> 
        /// <param name="password">登录smtp主机时用到的用户密码</param> 
        public static bool SendMail(string sender, string reciver, string subject, string body, string userName, string userPwd, string smtpHost, int port)
        {
            try
            {
                MailAddress from = new MailAddress(sender);
                MailAddress to = new MailAddress(reciver);
                MailMessage mes = new MailMessage(from, to);
                mes.Subject = subject;      //主题
                mes.IsBodyHtml = true;     //设置为主题用HTML格式
                mes.Priority = MailPriority.High;
                mes.Body = body;            //内容
                //设置主题与内容编码
                mes.SubjectEncoding = System.Text.Encoding.UTF8;
                mes.BodyEncoding = System.Text.Encoding.UTF8;

                //设置发送人身份验证
                SmtpClient smtp = new SmtpClient(smtpHost);
                smtp.Credentials = new NetworkCredential(userName, userPwd);
                smtp.Port = port;
                //smtp.EnableSsl = true;
                smtp.Send(mes);
                return true;
            }
            catch (Exception ex)
            {
                Log4netHelper.Log4netHelper.LoadADONetAppender();
                ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                log.Info("SendMail方法出错。Author:mail sender", ex);
                return false;
            }
        }

        /// <summary> 
        /// 发送邮件 
        /// </summary> 
        /// <param name="sender">发件人邮件地址</param> 
        /// <param name="reciver">收件人邮件地址</param> 
        /// <param name="subject">邮件主题</param> 
        /// <param name="body">邮件内容</param> 
        /// <param name="username">登录smtp主机时用到的用户名,注意是邮件地址'@'以前的部分</param> 
        /// <param name="password">登录smtp主机时用到的用户密码</param> 
        public static bool SendMail(string sender, string reciver, string subject, string body, string userName, string userPwd, string smtpHost, int port,out string errorMsg)
        {
            errorMsg = "";
            try
            {
                MailAddress from = new MailAddress(sender);
                MailMessage mes = new MailMessage();
                mes.From = from;            //发件人
                mes.To.Add(reciver);        //收件人
                mes.Subject = subject;      //主题
                mes.IsBodyHtml = true;      //设置为主题用HTML格式
                mes.Body = body;            //内容

                //设置主题与内容编码
                mes.SubjectEncoding = System.Text.Encoding.UTF8;
                mes.BodyEncoding = System.Text.Encoding.UTF8;

                //设置发送人身份验证
                SmtpClient smtp = new SmtpClient(smtpHost);
                smtp.Credentials = new NetworkCredential(userName, userPwd);
                smtp.Port = port;
                smtp.Send(mes);
                return true;
            }
            catch (Exception ex)
            {
                Log4netHelper.Log4netHelper.LoadADONetAppender();
                ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                log.Info("SendMail方法出错。Author:mail sender", ex);
                errorMsg = ex.Message;
                return false;
            }
        }

        #endregion
    }
}
