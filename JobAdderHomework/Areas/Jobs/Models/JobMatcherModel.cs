using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace JobAdderHomework.Areas.Jobs.Models
{
    public class JobMatcherModel
    {
        public int JobId { get; set; }
        public string JobName { get; set; }
        public string CompanyName { get; set; }
        public string Skills { get; set; }
        public ClosestMatchingCandidate Candidate { get; set; }

        public class ClosestMatchingCandidate
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Skills { get; set; }
        }
    }
}