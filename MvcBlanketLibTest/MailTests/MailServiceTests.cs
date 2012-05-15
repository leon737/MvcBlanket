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
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcBlanketLib.Mail;
using Moq;
using MvcBlanketLib.Mail.Factories;
using MvcBlanketLib.Mail.TemplateLocators;

namespace MvcBlanketLibTest.MailTests
{
    [TestClass]
    public class MailServiceTests
    {

        const string RecipientEmail = "test@mail.com";
        const string TemplateName = "TestTemplate";

        private readonly Mock<IMailStorage> mailStorage = new Mock<IMailStorage>();

        [TestMethod]
        public void TestRegisterMail()
        {
            var storage = mailStorage.Object;
            var templateLocator = new ResourceLocator(this.GetType().Assembly);
            var mailService = MailService.Instance.RegisterStorage(storage).RegisterTemplateLocator(templateLocator);
            var mail = mailService.RegisterMail(RecipientEmail, TemplateName);
            Assert.IsNotNull(mail);
        }

        [TestMethod]
        public void TestAddVariableToMail()
        {
            var storage = mailStorage.Object;
            var templateLocator = new ResourceLocator(this.GetType().Assembly);
            var mailService = MailService.Instance.RegisterStorage(storage).RegisterTemplateLocator(templateLocator);
            var mail = mailService.RegisterMail(RecipientEmail, TemplateName);
            mail.AddVariable("StringVariable", "StringValue").AddVariable("IntVariable", 10).AddVariable(
                "BoolVariable", true);
            Assert.IsNotNull(mail);
            Assert.AreEqual(4, mail.Variables.Count());
            Assert.AreEqual("StringValue", mail.Variables["StringVariable"]);
            Assert.AreEqual(10, mail.Variables["IntVariable"]);
            Assert.AreEqual(true, mail.Variables["BoolVariable"]);
            Assert.IsTrue(mail.Variables.ContainsKey("Domain"));
        }

        [TestMethod]
        public void TestSaveMailToPipeline()
        {
            var storage = mailStorage.Object;
            var templateLocator = new ResourceLocator(this.GetType().Assembly);
            var mailService = MailService.Instance.RegisterStorage(storage).RegisterTemplateLocator(templateLocator);
            var mail = mailService.RegisterMail(RecipientEmail, TemplateName);
            mail.AddVariable("StringVariable", "StringValue").AddVariable("IntVariable", 10).AddVariable(
                "BoolVariable", true).Save();
            mailStorage.Verify(m => m.SerializeMail(mail));
        }

        [TestMethod]
        public void TestMailServiceProcessEmptyMailQueue()
        {
            var storage = mailStorage.Object;
            var templateLocator = new ResourceLocator(this.GetType().Assembly);
            var mailService = MailService.Instance.RegisterStorage(storage).RegisterTemplateLocator(templateLocator);
            MailService.Instance.ProcessQueue();
            mailStorage.Verify(m => m.DeserializeMail());
        }

        [TestMethod]
        public void TestMailServiceProcessMailQueueWithOneMail()
        {
            var calls = 0;
            mailStorage.Setup(m => m.DeserializeMail()).Returns(() => calls == 0
                                                                          ? new Mail()
                                                                                {
                                                                                    RecipientEmail = RecipientEmail,
                                                                                    TemplateName = TemplateName,
                                                                                    MailId = Guid.NewGuid(),
                                                                                    Storage = mailStorage.Object,
                                                                                    Variables =
                                                                                        new Dictionary<string, object>()
                                                                                }
                                                                          : null)
                                                                          .Callback(() => calls++);
            mailStorage.SetupGet(m => m.TemplatesPath).Returns("MvcBlanketLibTest.MailTests.");
            var storage = mailStorage.Object;
            var templateLocator = new ResourceLocator(this.GetType().Assembly);
            var mailService = MailService.Instance.RegisterStorage(storage).RegisterTemplateLocator(templateLocator).RegisterMailSenderFactory(new MailSenderFactory());
            MailService.Instance.ProcessQueue();
            mailStorage.Verify(m => m.DeserializeMail(), Times.Exactly(2));
        }
    }
}
