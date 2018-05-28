using System.Collections.Generic;
using System.Threading.Tasks;

using JobAdderHomework.Areas.Jobs.Models;

namespace JobAdderHomework.Services
{
    public interface IJobsService
    {
        Task<Dictionary<int, JobDescription>> GetJobs();
    }
}