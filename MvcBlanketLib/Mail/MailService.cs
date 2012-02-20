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
