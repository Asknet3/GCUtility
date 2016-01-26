using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GCU
{
    /// <summary>
    /// Classe per la gestione di metodi da usare nel Web
    /// </summary>
    public class UtilityWeb
    {
        #region INVIA EMAIL
        /// <summary>
        /// Permette l'invio Email
        /// </summary>
        /// <param name="from">Mittente</param>
        /// <param name="toEmail">Destinatario</param>
        /// <param name="subject">Oggetto</param>
        /// <param name="msg">Corpo del messaggio</param>
        /// <param name="username">Username con cui accedere al server SMTP</param>
        /// <param name="password">Password con cui accedere al server SMTP</param>
        /// <param name="useDefaultcredential">Default: False</param>
        /// <param name="port">Porta SMTP. Settare a 0 per usare quella di default.</param>
        /// <param name="enableSsl">Default: True</param>
        /// <param name="timeout">Default: 1000</param>
        /// <returns></returns>
        public static bool SendMail(MailAddress from, MailAddress toEmail, String subject, String msg, String username, String password, Boolean useDefaultcredential = false, int port = 0, Boolean enableSsl = true, int timeout = 1000)
        {
            using (MailMessage message = new MailMessage())
            {
                message.From = from;
                message.To.Add(toEmail);
                message.Subject = subject;
                message.Body = msg;
                message.IsBodyHtml = true;
                NetworkCredential credential = new NetworkCredential(username, password);
                using (SmtpClient smtp = new SmtpClient())
                {
                    try
                    {
                        smtp.UseDefaultCredentials = useDefaultcredential;
                        if (port != 0) smtp.Port = port;
                        smtp.EnableSsl = enableSsl;
                        smtp.Timeout = timeout;
                        smtp.Credentials = credential;

                        smtp.Send(message);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        var log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                        log.Error(ex.Message, ex);
                        return false;
                    }
                }
            }
        }
        #endregion
    }
}
