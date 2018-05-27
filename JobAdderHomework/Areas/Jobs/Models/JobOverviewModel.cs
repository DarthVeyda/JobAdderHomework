using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobAdderHomework.Areas.Jobs.Models
{
    public class JobOverviewModel
    {
        public Dictionary<int, JobDescription> Jobs { get; set; }
        public Dictionary<int, Candidate> Candidates { get; set; }
    }

    public class Candidate
    {
        public int CandidateId { get; set; }
        public string Name { get; set; }
        public string SkillTags { get; set; }
        public Dictionary<string, double> WeightedSkills { get; set; }
    }

    public class JobDescription
    {
        public int JobId { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Skills { get; set; }
        public Dictionary<string, double> WeightedSkills { get; set; }
    }
}