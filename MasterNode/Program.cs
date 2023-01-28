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

namespace MasterNode
{
    public class date
    {
        public static string[][] HostInfo = {new string[] { "host1", "jobname1", "2023-01-27T15:01:00", "2023-01-27T20:00:40", "0/5 * * * * ?", "1.sh", "1" },
                                             new string[] { "host2", "jobname2", "2023-01-27T15:01:00", "2023-01-27T20:00:40", "* * * * * ?", "2.sh", "0" },
                                             new string[] { "host3", "jobname3", "2023-01-27T15:01:00", "2023-01-27T20:00:40", "0/10 * * * * ?", "3.sh", "0" } };

    }

    public record JobModel
    {
        long JobId;
        string? JobName;
        int IsUse;
        string? WorkflowName;
        byte[]? WorkflowBlob;
        string? Note;
        DateTime SaveDate;
        long UserId;
    }

    //public class Job2
    //{
    //    public long JobId { get; set; }
    //    public string? JobName { get; set; }
    //    public int? IsUse { get; set; }
    //    public string? WorkflowName { get; set; }
    //    public byte[]? WorkflowBlob { get; set; }
    //    public string? Note { get; set; }
    //    public DateTime? SaveDate { get; set; }
    //    public long? UserId { get; set; }
    //}

    public class Program
    {
        static string targetURL = "http://20.39.194.244:5000/v1/job/list";
        private static async Task Main(string[] args)
        {
            string webclientResult = callWebClient();
            //var r = JObject.Parse(webclientResult);
            //var r = JArray.Parse(webclientResult);
            //var list = r["list"];

            //Console.WriteLine(r);

            string r = JsonConvert.SerializeObject(webclientResult);
            
            r = r.Replace("\r\n", "").Replace("\n", "").Replace(@"\", "");
            
            if (r.Substring(0, 2) == @"""[")
                r = r.Substring(1, r.Length - 1);
            if (r.Substring(r.Length - 2, 2) == @"]""")
                r = r.Substring(0, r.Length - 1);

            Console.WriteLine(r);

            var Jobs = System.Text.Json.JsonSerializer.Deserialize<List<JobModel>>(r);

            //var model = JsonConvert.DeserializeObject<List<JobModel>>(r);

            Console.WriteLine(Jobs);

            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
            // Grab the Scheduler instance from the Factory
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();
            await scheduler.Start();

            foreach (string[] arr in date.HostInfo)
            {
                if (arr[6] == "1") continue;
                string startDT = arr[2];
                string endDT = arr[3];

                string[] dates = startDT.Split("T");
                string[] datee = endDT.Split("T");

                string[] yearMonthDays = dates[0].Split("-");
                string[] Times = dates[1].Split(":");
                int secs = Int32.Parse(Times[2]);
                int mins = Int32.Parse(Times[1]);
                int Hours = Int32.Parse(Times[0]);

                int days = Int32.Parse(yearMonthDays[2]);
                int months = Int32.Parse(yearMonthDays[1]);
                int years = Int32.Parse(yearMonthDays[0]);

                string[] yearMonthDaye = datee[0].Split("-");
                string[] Timee = datee[1].Split(":");
                int sece = Int32.Parse(Timee[2]);
                int mine = Int32.Parse(Timee[1]);
                int Houre = Int32.Parse(Timee[0]);

                int daye = Int32.Parse(yearMonthDaye[2]);
                int monthe = Int32.Parse(yearMonthDaye[1]);
                int yeare = Int32.Parse(yearMonthDaye[0]);

                IJobDetail job = JobBuilder.Create<HelloJob>()
                    .WithIdentity(arr[0], "group1")
                    .UsingJobData("name", arr[5])
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(arr[1], "group1")
                    .StartAt(DateBuilder.DateOf(Hours, mins, secs, days, months, years))
                    .WithCronSchedule(arr[4])
                    .EndAt(DateBuilder.DateOf(Houre, mine, sece, daye, monthe, yeare))
                    .Build();

                await scheduler.ScheduleJob(job, trigger);
            }
            Console.WriteLine("Press to stop the Schedule");
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
        public static string callWebClient()
        {
            string result = string.Empty;
            try
            {
                WebClient client = new WebClient();

                using (Stream data = client.OpenRead(targetURL))
                {
                    using (StreamReader reader = new StreamReader(data))
                    {
                        string s = reader.ReadToEnd();
                        result = s;

                        reader.Close();
                        data.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return result;
        }
        public static string callWebRequest()
        {
            string responseFromServer = string.Empty;

            try
            {
                WebRequest request = WebRequest.Create(targetURL);
                request.Method = "GET";
                request.ContentType = "application/json";

                using (WebResponse response = request.GetResponse())
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    responseFromServer = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return responseFromServer;
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

            DateTime times = DateTime.Now;

            await Console.Out.WriteLineAsync("현재 시각 : " + times + " / job Name : " + jobName);

            //string cmd = "sudo dotnet WorkHost.dll blob/" + jobName;
            //ProcessStartInfo psi = new ProcessStartInfo();
            //psi.UseShellExecute = false;
            //psi.FileName = "bash";
            //psi.Arguments = "-c  \"" + cmd + "\"";
            //try
            //{
            //    Process child = Process.Start(psi);
            //    child.WaitForExit();
            //}

            //catch (Exception e)
            //{
            //    Console.WriteLine(e.ToString());
            //}
        }
    }
}


