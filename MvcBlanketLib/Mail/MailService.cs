/*
MVC Blanket Library Copyright (C) 2009-2012 Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using MvcBlanketLib.Helpers;
using System.Reflection;
using System.Net.Mail;
using System.Web;
using System.Configuration;

namespace MvcBlanketLib.Mail
{
	public class MailService
	{
		private MailService()
		{

		}

		static MailService instance = new MailService();
		IMailStorage storage;
		Assembly asm;

		public Mail RegisterMail(string recipientEmail, string templateName)
		{
			if (storage == null) return null;
			var mail = new Mail { Storage = storage, RecipientEmail = recipientEmail, TemplateName = templateName };
			mail.AddVariable("Domain", ConfigurationManager.AppSettings["Domain"]);
			return mail;
		}

		public static MailService Instance
		{
			get
			{
				return instance;
			}
		}

		public MailService RegisterStorage(IMailStorage storage)
		{
			this.storage = storage;
			return this;
		}

		public MailService RegisterAssembly(Assembly asm)
		{
			this.asm = asm;
			return this;
		}

		public void ProcessQueue()
		{
			if (storage == null || asm == null) return;
			for (; ; )
			{
				var mail = storage.DeserializeMail();
				if (mail == null) return;
				var sender = MailSender.Create(storage.TemplatesDirectory + "/" + mail.TemplateName + ".txt", mail.Variables);
				if (sender == null)
				{
					mail.Failed = true;
				}
				else
				{
					try
					{
						sender.Send(mail.RecipientEmail);
						mail.Sent = true;
					}
					catch (SmtpException)
					{
						mail.Failed = true;
					}
				}
				storage.SerializeMail(mail);
			}
		}

	}
}
