/*******************************************************************\
* Module Name: LncaDataTier
* 
* File Name: Helpers/MailSender.cs
*
* Warnings:
*
* Issues:
*
* Created:  10 Jul 2011
* Author:   Leonid Gordo  [ leonardpt@gmail.com ]
*
\***********************************************************************/


using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using MailMessage = System.Net.Mail.MailMessage;
using System.Net;
using System.Web;
using System.Web.Hosting;

namespace MvcBlanketLib.Helpers
{
	public class MailSender
	{

		string body;

		public static MailSender Create(Type t, string templatePath, IDictionary<string, object> data)
		{
			MailSender sender = new MailSender();
			var repository = new NVelocityTemplateRepository(".");
			var stream = Assembly.GetAssembly(t).GetManifestResourceStream(templatePath);
			using (StreamReader sr = new StreamReader(stream))
			{
				string templateContent = sr.ReadToEnd();
				sender.body = repository.RenderTemplateContent(templateContent, data);
			}

			return sender;
		}

		public static MailSender Create(Assembly asm, string templatePath, IDictionary<string, object> data)
		{
			MailSender sender = new MailSender();
			var repository = new NVelocityTemplateRepository(".");
			var stream = asm.GetManifestResourceStream(templatePath);
			if (stream == null)
				return null;
			using (StreamReader sr = new StreamReader(stream))
			{
				string templateContent = sr.ReadToEnd();
				sender.body = repository.RenderTemplateContent(templateContent, data);
			}

			return sender;
		}

		public static MailSender Create(string templatePath, IDictionary<string, object> data)
		{
			MailSender sender = new MailSender();
			var repository = new NVelocityTemplateRepository(".");
			var fileName =  HostingEnvironment.MapPath(templatePath);
			string templateContent;
			using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			using (var sr = new StreamReader(fs))
			{
				templateContent = sr.ReadToEnd();
			}
			sender.body = repository.RenderTemplateContent(templateContent, data);
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
