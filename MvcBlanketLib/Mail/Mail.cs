using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcBlanketLib.Mail
{
	public class Mail
	{
		public Guid MailId { get; set; }
		public string RecipientEmail { get; set; }
		public string TemplateName { get; set; }
		public Dictionary<string, object> Variables { get; set; }
		public bool Sent { get; set; }
		public bool Failed { get; set; }
		internal IMailStorage Storage { get; set; }

		public Mail()
		{
			MailId = Guid.NewGuid();
			Variables = new Dictionary<string, object>();
		}

		public Mail AddVariable(string name, object value)
		{
			if (!Variables.ContainsKey(name))
				Variables.Add(name, value);
			return this;
		}

		public void Save()
		{
			Storage.SerializeMail(this);
		}
	}
}
