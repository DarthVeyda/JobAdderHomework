using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobAdderHomework.Areas.Jobs.Models
{
    public class JobOverviewModel
    {
        public List<JobDescription> Jobs { get; set; }
        public List<Candidate> Candidates { get; set; }
    }

    public class Candidate
    {
        public int CandidateId { get; set; }
        public string Name { get; set; }
        public string SkillTags { get; set; }
        public HashSet<(double weight, string name)> WeightedSkills { get; set; }
    }

    public class JobDescription
    {
        public int JobId { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Skills { get; set; }
        public HashSet<(double weight, string name)> WeightedSkills { get; set; }
    }
}