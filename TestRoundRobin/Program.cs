using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Numerics;
using System.Text;
using MasterNode.Net;
using System.Web;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using Quartz.Logging;
using Quartz.Impl;
using Quartz;
using System.Diagnostics;

namespace MasterNode
{
    public class Data
    {
        public long recentHostId; // 최근에 일을 시킨 HostId
    }
    public class WorkerHostModel
    {
        public string? WorkflowName { get; set; }
        public long JobId { get; set; }
        public long HostId { get; set; }
    }

    public class Schedule_IsUseSelectModel
    {
        public long ScheduleId { get; set; }
        public long JobId { get; set; }
        public string? ScheduleName { get; set; }
        public Boolean IsUse { get; set; }
        public Boolean ScheduleType { get; set; }
        public DateTime? OneTimeOccurDT { get; set; }
        public String? CronExpression { get; set; }
        public DateTime? ScheduleStartDT { get; set; }
        public DateTime? ScheduleEndDT { get; set; }
        public DateTime SaveDate { get; set; }
        public long UserId { get; set; }
        public Boolean JobIsUse { get; set; }
        public string? WorkflowName { get; set; }
    }

    public class HostModel
    {
        public long HostId { get; set; }
        public string? HostName { get; set; }
        public string? HostIp { get; set; }
        public Boolean IsUse { get; set; }
        public string? Role { get; set; }
        public string? EndPoint { get; set; }
        public string? Note { get; set; }
        public DateTime SaveDate { get; set; }
        public long UserId { get; set; }
    }

    public class Program
    {
        private static async Task Main(string[] args)
        {
            var psi = new ProcessStartInfo();
            psi.UseShellExecute = false;
            //Worker 인 부모 에서 blob 안의 자식이 실행될때 결과값과 에러를 부모 클래스로 가지고 올것이냐 를 결정하는 것
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;
            psi.FileName = "dawd";
            psi.Arguments = "Dawdwa";

            Console.WriteLine(psi.ToString());

            var Host = Net.RAPI.GetHostList();
            var schedules = Net.RAPI.GetJobSchedule();

            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
            // Grab the Scheduler instance from the Factory
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();
            await scheduler.Start();
            int k = 0;
            for (int i = 0; i < Host.Count; i++)
            {
                Console.WriteLine(i);
                k++;
            }
            Console.WriteLine(k);

            int u = 0;
            int s = 0;
            foreach (var sc in schedules)
            {
                IJobDetail job = JobBuilder.Create<HelloJob>()
                .WithIdentity(schedules[s].WorkflowName, "group1")
                .UsingJobData("name", schedules[s].WorkflowName)
                .UsingJobData("jobid", schedules[s].JobId)
                .UsingJobData("hostid", Host[u].HostId)
                .UsingJobData("hostip", Host[u].HostIp)
                .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(schedules[s].WorkflowName, "group1")
                    .StartAt(DateBuilder.DateOf(10, 0, 0))
                    .WithCronSchedule(schedules[s].CronExpression)
                    .EndAt(DateBuilder.DateOf(23, 0, 0))
                    .Build();

                await scheduler.ScheduleJob(job, trigger);

                u++;
                s++;
                if (u >= k)
                {
                    u = 0;
                }
            }
            Console.ReadKey();
        }
        private class ConsoleLogProvider : ILogProvider
        {
            public Logger GetLogger(string name)
            {
                return (level, func, exception, parameters) =>
                {
                    if (level >= LogLevel.Info && func != null)
                    {
                        Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
                    }
                    return true;
                };
            }

            public IDisposable OpenNestedContext(string message)
            {
                throw new NotImplementedException();
            }
            public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
            {
                throw new NotImplementedException();
            }
        }
    }
    public class HelloJob : IJob
    {
        public JobKey JobKey { get; private set; }

        public async Task Execute(IJobExecutionContext context)
        {
            JobKey = context.JobDetail.Key;

            JobDataMap dataMap = context.JobDetail.JobDataMap;

            string jobName = dataMap.GetString("name");
            long hostid = dataMap.GetInt("hostid");
            string hostip = dataMap.GetString("hostip");
            long jobid = dataMap.GetInt("jobid");

            DateTime times = DateTime.Now;
            await Console.Out.WriteLineAsync("===========================");
            await Console.Out.WriteLineAsync(jobName);
            await Console.Out.WriteLineAsync(jobid.ToString());
            await Console.Out.WriteLineAsync(hostid.ToString());
            await Console.Out.WriteLineAsync(hostip);
            await Console.Out.WriteLineAsync(times.ToString());
            await Console.Out.WriteLineAsync("===========================");

        }
    }
}




