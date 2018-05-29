using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

using JobAdderHomework.Areas.Jobs.Controllers;
using JobAdderHomework.Areas.Jobs.Models;
using JobAdderHomework.Services;

using NUnit.Framework;
using Moq;

namespace JobAdderHomework.Tests.Controllers
{
    [TestFixture]
    public class JobMatcherControllerTest
    {
        [Test]
        public async Task Find_EmptyOrInvalidID_EmptyResult()
        {
            var jobsMock = new Mock<IJobsService>();
            jobsMock.Setup(x => x.GetJobs()).
                Returns(Task.FromResult(new Dictionary<int, JobDescription>()));

            var candidatesMock = new Mock<ICandidatesService>();
            candidatesMock.Setup(x => x.GetCandidates()).
                Returns(Task.FromResult(new Dictionary<int, Candidate>()));

            var controller = new JobMatcherController(jobsMock.Object, candidatesMock.Object);

            var result = (await controller.FindMatch(0)) as ViewResult;
            var model = result.Model as JobMatcherModel;

            Assert.That(model.Candidate, Is.Null);
            Assert.That(controller.ViewBag.Message, Is.EqualTo("Invalid job ID."));
        }

        [Test]
        public async Task FindMatch_ValidID_ReturnsUser()
        {
            var jobsMock = new Mock<IJobsService>();
            jobsMock.Setup(x => x.GetJobs()).Returns(
                Task.FromResult(
                    new Dictionary<int, JobDescription>()
                    {
                        {
                            4, new JobDescription()
                            {
                                Company = "Bellile",
                                Name ="Head Chef",
                                JobId = 4,
                                Skills ="creativity, cooking, ordering, cleanliness, service"
                            }
                        },
                        {
                            15, new JobDescription()
                            {
                                Company = "Jazz",
                                Name = "Truck Driver",
                                JobId = 15,
                                Skills = "driver-license, reliable, communication, maintenance"
                            }
                        }
                    }));

            var candidatesMock = new Mock<ICandidatesService>();
            candidatesMock.Setup(x => x.GetCandidates()).
                Returns(Task.FromResult(
                    new Dictionary<int, Candidate>()
                    {
                        {
                            67, new Candidate()
                            {
                                CandidateId = 67,
                                Name = "Rosalinda Barish",
                                SkillTags = "driver-license, ms-office, communication, ordering, organisation"
                            }
                        }
                    }));

            var controller = new JobMatcherController(jobsMock.Object, candidatesMock.Object);

            var result = (await controller.FindMatch(15)) as ViewResult;
            var model = result.Model as JobMatcherModel;

            Assert.That(model.Candidate.Name, Is.EqualTo("Rosalinda Barish"));
        }
    }
}
