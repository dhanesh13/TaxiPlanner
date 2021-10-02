using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using TaxiPlannerAPI.Controllers;
using TaxiPlannerAPI.Data;

namespace TaxiPlannerAPI.Services
{
    public class NotifyEmailJobService : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            System.Diagnostics.Debug.WriteLine("sent");
            EmailServer.EmailActions.NotifyAllocators("isfaaqg@gmail.com");
        }

        Task IJob.Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class SendListEmailJobService : IJob
    {

        public async void Execute(IJobExecutionContext context)
        {
            Dictionary<String, String> kv = new Dictionary<string, string>();

            // report 1 has not been created (it means report 2 is also inexistant)
            // create the reports and then mail
            string p1 = ReportService.getPath(1);
            string p2 = ReportService.getPath(2);
            if (!File.Exists(p1))
            {
                ReportService rs = new ReportService();
                List<Transport> transports = await AllocationController.GenerateAll();
                rs.GenerateReport1(transports);
                rs.GenerateReport2(transports);
            } 
            kv.Add(ReportService.getFileName(1), Convert.ToBase64String(File.ReadAllBytes(p1)));
            kv.Add(ReportService.getFileName(2), Convert.ToBase64String(File.ReadAllBytes(p2)));

            EmailServer.EmailActions.SendReport("isfaaqg@gmail.com", kv);
        }

        Task IJob.Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}