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
    public class CandidatesService : ICandidatesService
    {
        private readonly ObjectCache cache;
        private HttpClient _HttpClient;

        public CandidatesService(HttpClient httpClient)
        {
            _HttpClient = httpClient;
            cache = MemoryCache.Default;
        }

        public async Task<Dictionary<int, Candidate>> GetCandidates()
        {
            if (cache.Contains("Candidates")) return cache["Candidates"] as Dictionary<int, Candidate>;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            string responseData;
            using (var response = await _HttpClient.GetAsync("candidates"))
            {
                responseData = await response.Content.ReadAsStringAsync();
            }
            if (responseData == null) return new Dictionary<int, Candidate>();
            var serializer = new JavaScriptSerializer();
            var result = serializer.Deserialize<List<Candidate>>(responseData).ToDictionary(x => x.CandidateId, x => x);
            cache.Add("Candidates", result, DateTimeOffset.MaxValue);
            return result;
        }
    }
}