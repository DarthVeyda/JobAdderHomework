using System;
using System.Web.Mvc;
using System.Web.Routing;

using Autofac;
using Autofac.Integration.Mvc;

namespace JobAdderHomework.ControllerFactory
{
    public class CustomControllerFactory : DefaultControllerFactory
    {
        private readonly IContainer _Container;

        public CustomControllerFactory(IContainer container)
        {
            _Container = container ?? throw new ArgumentNullException("container");
        }

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            Type controllerType = GetControllerType(requestContext, controllerName);
            
            var controller = ((AutofacDependencyResolver)DependencyResolver.Current).RequestLifetimeScope.Resolve(controllerType);
            return controller as IController;
        }

        public override void ReleaseController(IController controller)
        {
            IDisposable dispose = controller as IDisposable;
            if (dispose != null)
            {
                dispose.Dispose();
            }
        }
    }
}