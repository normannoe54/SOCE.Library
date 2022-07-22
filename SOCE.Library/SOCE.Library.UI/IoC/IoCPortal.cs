using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Text;
using SOCE.Library.UI.ViewModels;

namespace SOCE.Library.UI
{
    public static class IoCPortal
    {
        public static WindsorContainer Container { get; set; }
        public static IPortalAI Application { get; set; }
        static IoCPortal()
        {
            Container = new WindsorContainer();
            Container.Register(Component.For<IPortalAI>().ImplementedBy<PortalAI>());
            Application = Container.Resolve<IPortalAI>();
        }
    }
}
