using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Text;
using SOCE.Library.UI.ViewModels;

namespace SOCE.Library.UI
{
    public static class IoCCore
    {
        public static WindsorContainer Container { get; set; }
        public static ICoreAI Application { get; set; }
        static IoCCore()
        {
            Container = new WindsorContainer();
            Container.Register(Component.For<ICoreAI>().ImplementedBy<CoreAI>());
            Application = Container.Resolve<ICoreAI>();
        }
    }
}
