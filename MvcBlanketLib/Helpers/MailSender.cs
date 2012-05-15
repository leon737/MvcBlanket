/*
MVC Blanket Library Copyright (C) 2009-2012 Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using System.Reflection;
using MvcBlanketLib.Mail.TemplateLocators;
using MailMessage = System.Net.Mail.MailMessage;
using System.Net;

namespace MvcBlanketLib.Helpers
{
    public class MailSender
    {
        public IMailTemplateLocator TemplateLocator { get; set; }

        string body;

        //TODO: mail service process queue method should be based on the strategies to choose processing pipeline
        //TODO: (one with send invoke, other without, etc)


        private void Initialize(string templatePath, IDictionary<string, object> data)
        {
            var repository = new NVelocityTemplateRepository(".");
            body = repository.RenderTemplateContent(TemplateLocator.GetTemplateContent(templatePath), data);
        }

        public static MailSender Create(IMailTemplateLocator templateLocator, string templatePath, IDictionary<string, object> data)
        {
            var sender = new MailSender {TemplateLocator = templateLocator};
            sender.Initialize(templatePath, data);
            return sender;
        }

        public void Send(string subject, string recipient)
        {
            string sender = ConfigurationManager.AppSettings["MailSenderEmail"];
            string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            string smtpUser = ConfigurationManager.AppSettings["SmtpUser"];
            string smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
            var client = new SmtpClient(smtpHost);
            if (!string.IsNullOrWhiteSpace(smtpUser))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPassword);
            }
            var msg = new MailMessage(sender, recipient, subject, body);
            client.Send(msg);
        }

        public void Send(string recipient)
        {
            string sender = ConfigurationManager.AppSettings["MailSenderEmail"];
            string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            string smtpUser = ConfigurationManager.AppSettings["SmtpUser"];
            string smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
            var client = new SmtpClient(smtpHost);
            if (!string.IsNullOrWhiteSpace(smtpUser))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPassword);
            }
            string[] parts = body.Split(new[] { "--SPLITTER--" }, StringSplitOptions.None);
            string subject = parts[0].Trim();
            body = parts[1].Trim();
            var msg = new MailMessage(sender, recipient, subject, body);
            client.Send(msg);
        }
    }
}
