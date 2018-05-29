using System;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using JobAdderHomework.Areas.Jobs.Controllers;
using JobAdderHomework.ControllerFactory;
using JobAdderHomework.Services;

namespace JobAdderHomework
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            const string BASE_ADDRESS = "http://private-76432-jobadder1.apiary-mock.com/";
            var httpClient = new HttpClient { BaseAddress = new Uri(BASE_ADDRESS) };


            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(JobMatcherController).Assembly);

            builder.RegisterInstance(httpClient).AsSelf();

            builder.RegisterType<JobsService>().As<IJobsService>();
            builder.RegisterType<CandidatesService>().As<ICandidatesService>();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            RegisterCustomConrollerFactory(container);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void RegisterCustomConrollerFactory(IContainer container)
        {
            IControllerFactory factory = new CustomControllerFactory(container);
            ControllerBuilder.Current.SetControllerFactory(factory);
        }
    }
}
