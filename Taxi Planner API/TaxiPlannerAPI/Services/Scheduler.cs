using Quartz;
using Quartz.Impl;

namespace TaxiPlannerAPI.Services
{
    public class Scheduler
    {

        public static void Start()
        {
            IScheduler scheduler = (IScheduler)StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail notifyJob = JobBuilder.Create<NotifyEmailJobService>().Build();

            // on weeekdays at 15:00
            ITrigger notifyTrigger = TriggerBuilder
                .Create()
                //.WithCronSchedule("0 21 15 ? * 1-5 *")
                .WithCronSchedule("0 35 15 ? * MON-FRI *")
                .Build();

            IJobDetail sendListJob = JobBuilder.Create<SendListEmailJobService>().Build();

            // on weekdays at 17:00
            ITrigger sendListTrigger = TriggerBuilder
                .Create()
                .WithCronSchedule("0 0 17 ? * MON-FRI *")
                //.WithCronSchedule("0 30 14 ? * MON-FRI *")
                .Build();

            scheduler.ScheduleJob(notifyJob, notifyTrigger);
            scheduler.ScheduleJob(sendListJob, sendListTrigger);
        }
    }
}