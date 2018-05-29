using System.Collections.Generic;
using System.Threading.Tasks;
using JobAdderHomework.Areas.Jobs.Models;

namespace JobAdderHomework.Services
{
    public interface ICandidatesService
    {
        Task<Dictionary<int, Candidate>> GetCandidates();
    }
}