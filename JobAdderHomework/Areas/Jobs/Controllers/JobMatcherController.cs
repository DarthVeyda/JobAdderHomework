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
        private readonly double minimalThreshold = 0.66;
        private readonly double betterThreshold = 0.34;

        public async Task<ActionResult> Index()
        {
            var model = new JobOverviewModel();
            model.Candidates = await GetCandidates();
            model.Jobs = await GetJobs();
            return View(model);
        }

        public async Task<ActionResult> FindMatch(int jobId)
        {
            var model = new JobMatcherModel();
            var job = (await GetJobs())[jobId];
            model.JobId = jobId;
            model.JobName = job.Name;
            model.CompanyName = job.Company;
            model.Skills = job.Skills;

            var candidates = await GetCandidates();
            job.WeightedSkills = InitWeightedSkills(job.Skills, true);         

            foreach (var candidate in candidates)
            {
                candidate.Value.WeightedSkills = InitWeightedSkills(candidate.Value.SkillTags);
            }

            var first = candidates.OrderByDescending(c => job.WeightedSkills.IntersectByName(c.Value.WeightedSkills).Sum(s => s.Value)).First();
            var selectedCandidate = new Candidate();

            selectedCandidate = first.Value;

            model.Candidate = new JobMatcherModel.ClosestMatchingCandidate() { Id = selectedCandidate.CandidateId, Name = selectedCandidate.Name, Skills = selectedCandidate.SkillTags };
            return View(model);
        }


        private Dictionary<string, double> InitWeightedSkills(string rawSkills, bool forJD=false)
        {
            var result = new Dictionary<string, double>();
            var skills = rawSkills.Split(',').Select(s => s.Trim()).Distinct().ToArray();
            var count = skills.Count();
            for (int i = 0; i < count; i++)
            {
                var rawWeight = (double)(count - i) / count;
                var weightToAdd = rawWeight >= minimalThreshold ? 1 : (rawWeight >= betterThreshold ? minimalThreshold : betterThreshold);
                result.Add(skills[i], (forJD ? (count - i) * rawWeight : rawWeight));
            }
            return result;
        }

        private async Task<Dictionary<int, JobDescription>> GetJobs()
        {
            if (cache.Contains("Jobs")) return cache["Jobs"] as Dictionary<int, JobDescription>;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            string responseData;
            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {

                using (var response = await httpClient.GetAsync("jobs"))
                {

                    responseData = await response.Content.ReadAsStringAsync();
                }
            }
            if (responseData == null) return new Dictionary<int, JobDescription>();
            var serializer = new JavaScriptSerializer();
            var result = serializer.Deserialize<List<JobDescription>>(responseData);
            result.ForEach(entry => entry.Skills = entry.Skills.Replace("ahpra", "aphra"));  
            var resultDict = result.ToDictionary(x => x.JobId, x => x);
            
            cache.Add("Jobs", resultDict, DateTimeOffset.MaxValue);
            return resultDict;
        }

        private async Task<Dictionary<int, Candidate>> GetCandidates()
        {
            if (cache.Contains("Candidates")) return cache["Candidates"] as Dictionary<int, Candidate>;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            string responseData;
            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {

                using (var response = await httpClient.GetAsync("candidates"))
                {

                    responseData = await response.Content.ReadAsStringAsync();
                }
            }
            if (responseData == null) return new Dictionary<int, Candidate>();
            var serializer = new JavaScriptSerializer();
            var result = serializer.Deserialize<List<Candidate>>(responseData).ToDictionary(x => x.CandidateId, x => x);
            cache.Add("Candidates", result, DateTimeOffset.MaxValue);
            return result;
        }
    }

    public static class Extensions
    {
        public static Dictionary<string, double> IntersectByName(this Dictionary<string, double> dict, Dictionary<string, double> dictToIntersect)
        {
            var keys = dict.Keys.Intersect(dictToIntersect.Keys);
            keys.Select(key => new { Key = key, Value = dict[key] * dictToIntersect[key] }).ToDictionary(x => x.Key, x => x.Value);
            return keys.Select(key => new { Key = key, Value = dict[key] * dictToIntersect[key] }).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}