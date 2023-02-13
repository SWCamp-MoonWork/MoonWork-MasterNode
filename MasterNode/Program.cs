using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Numerics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Console;
using System.Text;
using MasterNode.Net;
using System.Web;

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
    }
    public class Loop
    {
        public int? startHour { get; set; }
        public int? startMin { get; set; }
        public int? startSec { get; set; }
        public int? startDay { get; set; }
        public int? startMonth { get; set; }
        public int? startYear { get; set; }
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
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
            // Grab the Scheduler instance from the Factory
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();
            await scheduler.Start();

            var Host = Net.RAPI.GetHostList();
            Data data = new Data();
            var schedules = Net.RAPI.GetJobSchedule();
            int k = 0;
            for (int i = 0; i < Host.Count; i++)
            {
                Console.WriteLine(i);
                k++;
            }
            Console.WriteLine(k);

            int u = 0;
            int s = 0;
            foreach (var a in schedules)
            {
                if (schedules[s].ScheduleType == true)
                {
                    string startDT = schedules[s].ScheduleStartDT.ToString();
                    string endDT = schedules[s].ScheduleEndDT.ToString();

                    Console.WriteLine(startDT);
                    Console.WriteLine(endDT);

                    string[] dates = startDT.Split(" ");
                    string[] datee = endDT.Split(" ");

                    string[] yearMonthDays = dates[0].Split("/");
                    string[] Times = dates[1].Split(":");
                    int secs = Int32.Parse(Times[2]);
                    int mins = Int32.Parse(Times[1]);
                    int Hours = Int32.Parse(Times[0]);

                    int days = Int32.Parse(yearMonthDays[1]);
                    int months = Int32.Parse(yearMonthDays[0]);
                    int years = Int32.Parse(yearMonthDays[2]);

                    string[] yearMonthDaye = datee[0].Split("/");
                    string[] Timee = datee[1].Split(":");
                    int sece = Int32.Parse(Timee[2]);
                    int mine = Int32.Parse(Timee[1]);
                    int Houre = Int32.Parse(Timee[0]);

                    int daye = Int32.Parse(yearMonthDaye[1]);
                    int monthe = Int32.Parse(yearMonthDaye[0]);
                    int yeare = Int32.Parse(yearMonthDaye[2]);

                    Console.WriteLine(years);
                    Console.WriteLine(yeare);
                    Console.WriteLine(months);
                    Console.WriteLine(monthe);
                    Console.WriteLine(days);
                    Console.WriteLine(daye);
                    Console.WriteLine(Hours);
                    Console.WriteLine(Houre);
                    Console.WriteLine(mins);
                    Console.WriteLine(mine);
                    Console.WriteLine(secs);
                    Console.WriteLine(sece);
                    Console.WriteLine(schedules[s].CronExpression);

                    IJobDetail job = JobBuilder.Create<HelloJob>()
                    .WithIdentity(schedules[s].WorkflowName, "group1")
                    .UsingJobData("name", schedules[s].WorkflowName)
                    .UsingJobData("jobid", schedules[s].JobId)
                    .UsingJobData("hostid", Host[u].HostId)
                    .UsingJobData("hostip", Host[u].HostIp)
                    .Build();

                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity(schedules[s].WorkflowName, "group1")
                        .StartAt(DateBuilder.DateOf(Hours, mins, secs, days, months, years))
                        .WithCronSchedule(schedules[s].CronExpression)
                        .EndAt(DateBuilder.DateOf(Houre, mine, sece, daye, monthe, yeare))
                        .Build();

                    await scheduler.ScheduleJob(job, trigger);
                }
                else
                {
                    string oneTimeDT = schedules[s].OneTimeOccurDT.ToString();
                    
                    string[] dates = oneTimeDT.Split(" ");

                    string[] yearMonthDays = dates[0].Split("/");
                    string[] Times = dates[1].Split(":");
                    int sec = Int32.Parse(Times[2]);
                    int min = Int32.Parse(Times[1]);
                    int Hour = Int32.Parse(Times[0]);

                    int day = Int32.Parse(yearMonthDays[1]);
                    int month = Int32.Parse(yearMonthDays[0]);
                    int year = Int32.Parse(yearMonthDays[2]);

                    IJobDetail job = JobBuilder.Create<HelloJob>()
                    .WithIdentity(schedules[s].WorkflowName, "group1")
                    .UsingJobData("name", schedules[s].WorkflowName)
                    .UsingJobData("jobid", schedules[s].JobId)
                    .UsingJobData("hostid", Host[u].HostId)
                    .UsingJobData("hostip", Host[u].HostIp)
                    .Build();

                    ITrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartAt(DateBuilder.DateOf(Hour, min, sec, day, month, year)) // some Date 
                    .Build();

                    await scheduler.ScheduleJob(job, trigger);
                }
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
            Console.WriteLine("===========================");
            Console.WriteLine(jobName);
            Console.WriteLine(hostid);
            Console.WriteLine(hostip);
            Console.WriteLine(jobid);
            Console.WriteLine(times);
            Console.WriteLine("===========================");

            await Console.Out.WriteLineAsync("현재 시각 : " + times + " / job Name : " + jobName);

            string sendInfoUrl = $"http://{hostip}:5000/worker/{hostid}";
            Console.WriteLine(sendInfoUrl);
            string responseText = string.Empty;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(sendInfoUrl);
            webRequest.Method = "POST";
            webRequest.Timeout = 30 * 1000;
            webRequest.ContentType = "application/json";

            var obj = new WorkerHostModel
            {
                WorkflowName = jobName,
                JobId = jobid
            };
            //Data 입력받아 requset해서 data를 api로 전송하는 구간
            //Json을 string type으로 입력해준다.
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            //보낼 데이터를 byteArray로 바꿔준다.
            byte[] byteArray = Encoding.UTF8.GetBytes(json);

            //요청 Data를 쓰는데 사용할 Stream 개체를 가져온다.
            Stream dataStream = webRequest.GetRequestStream();
            //전송...
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            //Data를 잘 받았는지 확인하는 response 구간
            //응답 받기
            using (HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse())
            {
                HttpStatusCode status = resp.StatusCode;
                Console.WriteLine(status);      // status 가 정상일경우 OK가 입력된다.

                // 응답과 관련된 stream을 가져온다.
                Stream respStream = resp.GetResponseStream();
                using (StreamReader streamReader = new StreamReader(respStream))
                {
                    responseText = streamReader.ReadToEnd();
                }
            }

            Console.WriteLine(responseText);
        }
    }
}




