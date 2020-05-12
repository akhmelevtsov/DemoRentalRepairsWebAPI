using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Core.Services;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Demo.RentalRepairs.Core.Tests.Integration
{
    [TestClass]
    public class MessageFormatterTest
    {
        //[TestMethod]
        //public void CreateEmailsTest()
        //{
           
        //    var message = new DomainMessage(DomainMessageTypeEnum.Property2TenantOnRequestReceivedMessage, "Sender123", "Recipient123")
        //    {
        //        MessageProperties = new Dictionary<DomainMessageParamsEnum, string>()
        //        {
        //            { DomainMessageParamsEnum.SenderEmail,"Tenant.Property.NoReplyEmailAddress" },
        //            { DomainMessageParamsEnum.RecipientEmail, "Tenant.ContactInfo.EmailAddress"},
        //            { DomainMessageParamsEnum.RecipientPhone,"Tenant.ContactInfo.MobilePhone" },
        //            { DomainMessageParamsEnum.TenantFullName, "Tenant.FullName" },
        //            { DomainMessageParamsEnum.RequestDate, "DateCreated.ToLongTimeString()" },
        //            { DomainMessageParamsEnum.PropertyName, "Tenant.Property.Name" }
        //        }
        //    };
        //    var emails = new List<Domain.Entities.DomainMessage> { message }.CreateEmails();
        //    Assert.AreEqual(1, emails.Count);
        //    var email = emails[0];
        //    Assert.AreEqual("Tenant.ContactInfo.EmailAddress", email.RecipientEmail);
        //    Assert.AreEqual("Tenant.Property.NoReplyEmailAddress", email.SenderEmail );

        //    Assert.AreEqual("Tenant.Property.NoReplyEmailAddress", email.SenderEmail);
        //    Assert.IsTrue(email.Body.Contains("Tenant.FullName"));

        //}
 
    }

}
