using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using AdvanceUMS.Models;

namespace AdvanceUMS.Helper
{
    public class MailHelper
    {
        private Entities db = new Entities();
        public String SendMail(string dto, String Subject, String Body)
        {
            try
            {
                var setting = db.tblSettings.FirstOrDefault();
                if (setting != null)
                {
                    MailMessage m = new MailMessage();
                    m.To.Add(dto);
                    m.From = new MailAddress(setting.SMTPEmail);
                    m.Subject = Subject;
                    m.Body = Body;
                    m.IsBodyHtml = true;
                    SmtpClient c = new SmtpClient();
                    c.UseDefaultCredentials = true;
                    c.Host = setting.SMTPHost;
                    c.Credentials = new System.Net.NetworkCredential(setting.SMTPUserName, setting.SMTPPassword);
                    c.Port = Convert.ToInt32(setting.SMTPPort);
                    c.EnableSsl = false;
                    c.Send(m);
                    return "Successful";
                }
                else
                {
                    return "failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}