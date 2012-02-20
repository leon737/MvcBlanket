using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcBlanketLib.Mail
{
    public interface IMailStorage
    {
        void SerializeMail(Mail mail);
        Mail DeserializeMail();
        string TemplatesDirectory { get; }
    }
}
