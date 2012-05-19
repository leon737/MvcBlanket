/*
MVC Blanket Library Copyright (C) 2009-2012 Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using System.Net.Mail;
using System.Configuration;
using MvcBlanketLib.Mail.Configuration;
using MvcBlanketLib.Mail.Factories;
using MvcBlanketLib.Mail.TemplateLocators;

namespace MvcBlanketLib.Mail
{
	public class MailService
	{
		private MailService()
		{

		}


// ReSharper disable InconsistentNaming
		static readonly MailService instance = new MailService();
// ReSharper restore InconsistentNaming

        private IMailTemplateLocator templateLocator;
		private IMailStorage storage;
	    private IMailSenderFactory factory;
	    private IConfiguration configuration;

		public static MailService Instance
		{
			get
			{
				return instance;
			}
		}


// ReSharper disable ParameterHidesMember
        public MailService RegisterTemplateLocator(IMailTemplateLocator templateLocator)
        {
            this.templateLocator = templateLocator;
            return this;
        }

		public MailService RegisterStorage(IMailStorage storage)
		{
			this.storage = storage;
			return this;
		}

        public MailService RegisterMailSenderFactory(IMailSenderFactory factory)
        {
            this.factory = factory;
            return this;
        }

        public MailService RegisterConfiguration(IConfiguration configuration)
        {
            this.configuration = configuration;
            return this;
        }

// ReSharper restore ParameterHidesMember

        public Mail RegisterMail(string recipientEmail, string templateName)
        {
            if (storage == null) return null;
            var mail = new Mail { Storage = storage, RecipientEmail = recipientEmail, TemplateName = templateName };
            mail.AddVariable("Domain", ConfigurationManager.AppSettings["Domain"]);
            return mail;
        }

		public void ProcessQueue()
		{
			if (storage == null || templateLocator == null || factory == null || configuration == null) return;
			for (; ; )
			{
				var mail = storage.DeserializeMail();
				if (mail == null) return;
				var sender =  factory.GetMailSender(templateLocator, configuration, storage.TemplatesPath + mail.TemplateName + ".txt", mail.Variables);
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
