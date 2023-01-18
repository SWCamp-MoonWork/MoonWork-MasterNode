using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using System.Diagnostics;
using System.Timers;

namespace MasterNode
{
    //public static class global
    //{
    //    public static string jobName;
    //}
    //public static class time
    //{
    //    public static DateTime timer;
    //}
    public class Program
    {

        private static async Task Main(string[] args)
        {
            //    Console.WriteLine("원하는 job의 이름과 확장자를 입력하세요");

            //    global.jobName = Console.ReadLine();

            //    Console.WriteLine("실행 시킬 시각을 입력 : yyyy-MM-dd HH:mm:ss");
            //    string date = Console.ReadLine();
            //    DateTime da =Convert.ToDateTime(date);
            //    time.timer = da;
            int SchduleType = 1;
            
            string startDT = "2023-01-18T15:08:00";

            string[] date = startDT.Split("T");

            Console.WriteLine(date[0]);
            Console.WriteLine(date[1]);

            string[] yearMonthDay = date[0].Split("-");
            string[] Time = date[1].Split(":");
            int sec = Int32.Parse(Time[2]);
            int min = Int32.Parse(Time[1]);
            int Hour = Int32.Parse(Time[0]);

            int day = Int32.Parse(yearMonthDay[2]);
            int month = Int32.Parse(yearMonthDay[1]);
            int year = Int32.Parse(yearMonthDay[0]);

            Console.WriteLine(sec);
            Console.WriteLine(min);
            Console.WriteLine(Hour);
            Console.WriteLine(day);
            Console.WriteLine(month);
            Console.WriteLine(year);


            if (SchduleType == 0) 
            {
                LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
                // Grab the Scheduler instance from the Factory
                StdSchedulerFactory factory = new StdSchedulerFactory();
                IScheduler scheduler = await factory.GetScheduler();

                await scheduler.Start();

                IJobDetail job = JobBuilder.Create<HelloJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("onetimeTrigger")
                    .StartAt(DateBuilder.DateOf(Hour, min, sec, day, month, year))
                    .Build();

                await scheduler.ScheduleJob(job, trigger);

                Console.WriteLine("Job is Done");
            }
            else if(SchduleType == 1)
            {
                LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
                // Grab the Scheduler instance from the Factory
                StdSchedulerFactory factory = new StdSchedulerFactory();
                IScheduler scheduler = await factory.GetScheduler();

                await scheduler.Start();

                IJobDetail job = JobBuilder.Create<HelloJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartAt(DateBuilder.DateOf(Hour, min, sec, day, month, year))
                    .WithCronSchedule("* * * * * ?")
                    .EndAt(DateBuilder.DateOf(15, 9, 0))
                    .Build();

                await scheduler.ScheduleJob(job, trigger);

                Console.WriteLine("Press any key to close the application");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("this job has error please Check your scheduler again");
            }
            
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

        public async Task Execute(IJobExecutionContext context)
        {
            DateTime times = DateTime.Now;
            await Console.Out.WriteLineAsync("현재 시각 : " + times);
            //if(times >= time.timer)
            //{
            //    await Console.Out.WriteLineAsync("파일 이름" + global.jobName);
            //};
        }
    }
}
