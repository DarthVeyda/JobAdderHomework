using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace JobAdderHomework.ControllerFactory
{
    public class CustomControllerFactory : DefaultControllerFactory
    {
        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            string controllername = requestContext.RouteData.Values["controller"].ToString();
            Type controllerType = Type.GetType(string.Format("CustomControllerFactory.Controllers.{0}", controllername));
            IController controller = Activator.CreateInstance(controllerType) as IController;
            return controller;
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