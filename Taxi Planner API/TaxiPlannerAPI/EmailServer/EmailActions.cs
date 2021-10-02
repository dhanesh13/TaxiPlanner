using DocumentFormat.OpenXml.Drawing.Charts;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaxiPlannerAPI.Models;

namespace TaxiPlannerAPI.EmailServer
{
    public class EmailActions
    {
        public static void SendEmailStatusChanged(string emp_name, string emp_mail, BookingStatus status, string time, string date, string approver_name)
        {
            var sendGridClient = new SendGridClient(System.Configuration.ConfigurationManager.AppSettings["SendGridClientId"]);
            var msg = new SendGridMessage();
            msg.SetTemplateId(System.Configuration.ConfigurationManager.AppSettings["EmailTemplateIdstatus_mail"]);
            msg.SetFrom(new EmailAddress(Appsettings.mailFrom));
            msg.AddTo(emp_mail, emp_name);
            var dynamicTemplateData = new MailStruct
            {
                emp_name = emp_name,
                emp_mail = emp_mail,
                approver_name = approver_name,
                status = status.ToString(),
                date = date,
                time = time
            };
            msg.SetTemplateData(dynamicTemplateData);
            var response = sendGridClient.SendEmailAsync(msg).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                Console.WriteLine("Email sent");
            }
        }
        public static void SendMailDelegation(string emp_name, string emp_mail, string approver_name, string start_from, string expiry_date)
        {
            var sendGridClient = new SendGridClient(System.Configuration.ConfigurationManager.AppSettings["SendGridClientId"]);
            var msg = new SendGridMessage();
            msg.SetTemplateId(System.Configuration.ConfigurationManager.AppSettings["EmailTemplateIdmail_delegation"]);
            msg.SetFrom(new EmailAddress(Appsettings.mailFrom));
            msg.AddTo(emp_mail, emp_name);
            var dynamicTemplateData = new MailStruct
            {
                emp_name = emp_name,
                approver_name = approver_name,
                start_from = start_from,
                expiry_date = expiry_date
            };
            msg.SetTemplateData(dynamicTemplateData);
            var response = sendGridClient.SendEmailAsync(msg).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                Console.WriteLine("Email sent");
            }
        }
        public static void SendEmailBookingBySuperior(string emp_name, string emp_mail, string emp_address, string approver_name, string bookingdatetime)
        {
            var sendGridClient = new SendGridClient(System.Configuration.ConfigurationManager.AppSettings["SendGridClientId"]);
            var msg = new SendGridMessage();
            msg.SetTemplateId(System.Configuration.ConfigurationManager.AppSettings["EmailTemplateIdbooking_made_by_superior"]);
            msg.SetFrom(new EmailAddress(Appsettings.mailFrom));
            msg.AddTo(emp_mail, emp_name);
            var dynamicTemplateData = new MailStruct
            {
                emp_name = emp_name,
                emp_mail = emp_mail,
                emp_address = emp_address,
                approver_name = approver_name,
                bookingdatetime = bookingdatetime
            };
            msg.SetTemplateData(dynamicTemplateData);
            var response = sendGridClient.SendEmailAsync(msg).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                Console.WriteLine("Email sent");
            }
        }

        public static void SendEmailBookingByEmployee(string emp_name, string emp_mail, string emp_address, string approver_name, string approver_email, string bookingdatetime)
        {
            var sendGridClient = new SendGridClient(System.Configuration.ConfigurationManager.AppSettings["SendGridClientId"]);
            var msg = new SendGridMessage();
            msg.SetTemplateId(System.Configuration.ConfigurationManager.AppSettings["EmailTemplateIdbooking_made_by_employee"]);
            msg.SetFrom(new EmailAddress(Appsettings.mailFrom));
            msg.AddTo(approver_email, approver_name);
            var dynamicTemplateData = new MailStruct
            {
                emp_name = emp_name,
                emp_address = emp_address,
                approver_name = approver_name,
                bookingdatetime = bookingdatetime
            };
            msg.SetTemplateData(dynamicTemplateData);
            var response = sendGridClient.SendEmailAsync(msg).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                Console.WriteLine("Email sent");
            }
        }

        public static void SendEmailTimeChanged(string emp_name, string emp_mail, string time, string date, string approver_name)
        {
            var sendGridClient = new SendGridClient(System.Configuration.ConfigurationManager.AppSettings["SendGridClientId"]);
            var msg = new SendGridMessage();
            msg.SetTemplateId(System.Configuration.ConfigurationManager.AppSettings["EmailTemplateIdchange_time"]);
            msg.SetFrom(new EmailAddress(Appsettings.mailFrom));
            msg.AddTo(emp_mail, emp_name);
            var dynamicTemplateData = new MailStruct
            {
                emp_name = emp_name,
                approver_name = approver_name,
                date = date,
                time = time
            };
            msg.SetTemplateData(dynamicTemplateData);
            var response = sendGridClient.SendEmailAsync(msg).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                Console.WriteLine("Email sent");
            }
        }

        public static bool NotifyAllocators(string emp_mail)
        {
            var sendGridClient = new SendGridClient(System.Configuration.ConfigurationManager.AppSettings["SendGridClientId"]);
            var msg = new SendGridMessage();
            msg.SetTemplateId(System.Configuration.ConfigurationManager.AppSettings["NotifyTransportAllocators"]);
            msg.SetFrom(new EmailAddress(Appsettings.mailFrom));
            msg.AddTo(emp_mail, "Allocator");
            var response = sendGridClient.SendEmailAsync(msg).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                System.Diagnostics.Debug.WriteLine("Email sent");
                return true;
            }
            return false;
        }

        public static bool SendReport(string emp_mail, Dictionary<String, String> reports )
        {
            var sendGridClient = new SendGridClient(System.Configuration.ConfigurationManager.AppSettings["SendGridClientId"]);
            var msg = new SendGridMessage();
            msg.SetTemplateId(System.Configuration.ConfigurationManager.AppSettings["Gemini"]);
            msg.SetFrom(new EmailAddress(Appsettings.mailFrom));
            msg.AddTo(emp_mail);

            foreach (KeyValuePair<String, String> report in reports)
            {
                msg.AddAttachment(report.Key, report.Value);
            }

            var response = sendGridClient.SendEmailAsync(msg).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                System.Diagnostics.Debug.WriteLine("Sent");
                return true;
            }
            return false;
        }
    }
}
