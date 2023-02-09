using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MasterNode.Net
{
   
    public static class RAPI
    {
        static string host = "http://20.249.17.147:5000";
        static string version = "v1";
        static string schduleList = $"{host}/{version}/job/Schedule_IsUseSelect";
        static string hostList = $"{host}/{version}/host/isusetrue";

        public static string JSONrefactor(string json)
        {
            json = json.Replace("\r\n", "").Replace("\n", "").Replace(@"\", "");

            if (json.Substring(0, 2) == @"""[")
                json = json.Substring(1, json.Length - 1);
            if (json.Substring(json.Length - 2, 2) == @"]""")
                json = json.Substring(0, json.Length - 1);
            return json;
        }
        //JobSchedule 정보 가지고 오는 API 호출
        public static List<Schedule_IsUseSelectModel> GetJobSchedule()
        {
            string result = string.Empty;

            try
            {
                WebClient client = new WebClient();

                using (Stream data = client.OpenRead(schduleList))
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
            
            //string allSchduleList = callSchduleList();
            string r = JsonConvert.SerializeObject(result);

            r = JSONrefactor(r);

            return System.Text.Json.JsonSerializer.Deserialize<List<Schedule_IsUseSelectModel>>(r);
        }

        //Host 정보 불러오는 API 호출
        public static List<HostModel> GetHostList()
        {
            string result = string.Empty;

            try
            {
                WebClient client = new WebClient();

                using (Stream data = client.OpenRead(hostList))
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

            //string allSchduleList = callSchduleList();
            string r = JsonConvert.SerializeObject(result);

            r = JSONrefactor(r);

            return System.Text.Json.JsonSerializer.Deserialize<List<HostModel>>(r);
        }
    }
}