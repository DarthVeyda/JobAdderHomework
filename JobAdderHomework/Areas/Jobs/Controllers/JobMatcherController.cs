using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using JobAdderHomework.Areas.Jobs.Models;

namespace JobAdderHomework.Areas.Jobs.Controllers
{
    public class JobMatcherController : Controller
    {
        private ObjectCache cache = MemoryCache.Default;
        private readonly Uri baseAddress = new Uri("http://private-76432-jobadder1.apiary-mock.com/");

        public async Task<ActionResult> Index()
        {
            var model = new JobOverviewModel();
            model.Candidates = await GetCandidates();
            model.Jobs = await GetJobs();
            return View(model);
        }

        public async Task<ActionResult> FindMatch(JobDescription job)
        {
            var model = new JobMatcherModel();
            var candidates = await GetCandidates();

            return View(model);
        }

        private async Task<List<JobDescription>> GetJobs()
        {
            if (cache.Contains("Jobs")) return cache["Jobs"] as List<JobDescription>;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            string responseData;
            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {

                using (var response = await httpClient.GetAsync("jobs"))
                {

                    responseData = await response.Content.ReadAsStringAsync();
                }
            }
            if (responseData == null) return new List<JobDescription>();
            var serializer = new JavaScriptSerializer();
            var result = serializer.Deserialize<List<JobDescription>>(responseData);
            cache.Add("Jobs", result, DateTimeOffset.MaxValue);
            return result;
        }

        private async Task<List<Candidate>> GetCandidates()
        {
            if (cache.Contains("Candidates")) return cache["Candidates"] as List<Candidate>;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            string responseData;
            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {

                using (var response = await httpClient.GetAsync("candidates"))
                {

                    responseData = await response.Content.ReadAsStringAsync();
                }
            }
            if (responseData == null) return new List<Candidate>();
            var serializer = new JavaScriptSerializer();
            var result = serializer.Deserialize<List<Candidate>>(responseData);
            cache.Add("Candidates", result, DateTimeOffset.MaxValue);
            return result;
        }
    }
}