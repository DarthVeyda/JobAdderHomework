using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using JobAdderHomework.Areas.Jobs.Controllers;
using JobAdderHomework.Areas.Jobs.Models;
using NUnit.Framework;

namespace JobAdderHomework.Tests.Controllers
{
    [TestFixture]
    public class JobMatcherControllerTest
    {
        JobMatcherController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new JobMatcherController();
        }

        [Test]
        public async Task FindMatch_ValidID_ReturnsUser()
        {
            var controller = new JobMatcherController();

            var result = (await controller.FindMatch(15)) as ViewResult;
            var model = result.Model as JobMatcherModel;

            Assert.That(model.Candidate.Name, Is.EqualTo("Rosalinda Barish"));
        }

        [Test]
        public async Task Find_InvalidID_EmptyResult()
        {
            var controller = new JobMatcherController();

            var result = (await controller.FindMatch(0)) as ViewResult;
            var model = result.Model as JobMatcherModel;

            Assert.That(model.Candidate, Is.Null);
            Assert.That(controller.ViewBag.Message, Is.EqualTo("Invalid job ID."));
        }
    }
}
