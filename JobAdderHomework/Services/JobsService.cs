using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using JobAdderHomework.Areas.Jobs.Models;

namespace JobAdderHomework.Services
{
    public class JobsService : IJobsService
    {
        private HttpClient _HttpClient;
        private readonly ObjectCache cache;

        public JobsService(HttpClient httpClient)
        {
            _HttpClient = httpClient;
            cache = MemoryCache.Default;
        }

        public async Task<Dictionary<int, JobDescription>> GetJobs()
        {
            if (cache.Contains("Jobs")) return cache["Jobs"] as Dictionary<int, JobDescription>;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            string responseData;
            using (var response = await _HttpClient.GetAsync("jobs"))
            {

                responseData = await response.Content.ReadAsStringAsync();
            }
            if (responseData == null) return new Dictionary<int, JobDescription>();
            var serializer = new JavaScriptSerializer();
            var result = serializer.Deserialize<List<JobDescription>>(responseData);
            result.ForEach(entry => entry.Skills = entry.Skills.Replace("ahpra", "aphra"));
            var resultDict = result.ToDictionary(x => x.JobId, x => x);

            cache.Add("Jobs", resultDict, DateTimeOffset.MaxValue);
            return resultDict;
        }
    }
}