using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using System.Diagnostics;
using System.Timers;

namespace MasterNode
{
    public class date
    {
        public static string[][] HostInfo = {new string[] { "host1", "jobname1", "2023-01-26T16:20:00", "2023-01-26T16:20:50", "0/5 * * * * ?", "1.sh" },
                                             new string[] { "host2", "jobname2", "2023-01-26T16:21:00", "2023-01-26T16:21:20", "* * * * * ?", "2.sh" },
                                             new string[] { "host3", "jobname3", "2023-01-26T16:22:00", "2023-01-26T16:22:30", "0/10 * * * * ?", "3.sh" } };

    }
    //public record job
    //{
    //    public string hostName;
    //    public string jobName;
    //    public string startdate;
    //    public string enddate;
    //    public string cronexpression;
    //}
    public class Program
    {
        private static async Task Main(string[] args)
        {
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
            // Grab the Scheduler instance from the Factory
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();
            await scheduler.Start();

            foreach (string[] arr in date.HostInfo)
            {

                
                string startDT = arr[2];
                string endDT = arr[3];

                //Console.WriteLine(startDT);
                //Console.WriteLine(endDT);

                string[] dates = startDT.Split("T");
                string[] datee = endDT.Split("T");

                //Console.WriteLine(dates[0]);
                //Console.WriteLine(dates[1]);

                string[] yearMonthDays = dates[0].Split("-");
                string[] Times = dates[1].Split(":");
                int secs = Int32.Parse(Times[2]);
                int mins = Int32.Parse(Times[1]);
                int Hours = Int32.Parse(Times[0]);

                int days = Int32.Parse(yearMonthDays[2]);
                int months = Int32.Parse(yearMonthDays[1]);
                int years = Int32.Parse(yearMonthDays[0]);

                //Console.WriteLine(secs);
                //Console.WriteLine(mins);
                //Console.WriteLine(Hours);
                //Console.WriteLine(days);
                //Console.WriteLine(months);
                //Console.WriteLine(years);

                string[] yearMonthDaye = datee[0].Split("-");
                string[] Timee = datee[1].Split(":");
                int sece = Int32.Parse(Timee[2]);
                int mine = Int32.Parse(Timee[1]);
                int Houre = Int32.Parse(Timee[0]);

                int daye = Int32.Parse(yearMonthDaye[2]);
                int monthe = Int32.Parse(yearMonthDaye[1]);
                int yeare = Int32.Parse(yearMonthDaye[0]);

                //Console.WriteLine(sece);
                //Console.WriteLine(mine);
                //Console.WriteLine(Houre);
                //Console.WriteLine(daye);
                //Console.WriteLine(monthe);
                //Console.WriteLine(yeare);

                IJobDetail job = JobBuilder.Create<HelloJob>()
                    .WithIdentity(arr[0], "group1")
                    .UsingJobData("name",arr[5])
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(arr[1], "group1")
                    .StartAt(DateBuilder.DateOf(Hours, mins, secs, days, months, years))
                    .WithCronSchedule(arr[4])
                    //.WithSimpleSchedule(x => x
                    //    .WithIntervalInSeconds(1)
                    //    .RepeatForever())
                    .EndAt(DateBuilder.DateOf(Houre, mine, sece, daye, monthe, yeare))
                    .Build();

                await scheduler.ScheduleJob(job, trigger);
                //await scheduler.ScheduleJob(job, trigger);
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

            string cmd = "sudo dotnet WorkHost.dll blob/" + jobName;
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.UseShellExecute = false;
            psi.FileName = "bash";
            psi.Arguments = "-c  \"" + cmd + "\"";
            try
            {
                Process child = Process.Start(psi);
                child.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}


