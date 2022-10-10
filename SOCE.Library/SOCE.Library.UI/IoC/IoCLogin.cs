using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Text;
using SOCE.Library.UI.ViewModels;

namespace SOCE.Library.UI
{
    public static class IoCLogin
    {
        public static WindsorContainer Container { get; set; }
        public static ILoginAI Application { get; set; } 
        static IoCLogin()
        {
            Container = new WindsorContainer();
            Container.Register(Component.For<ILoginAI>().ImplementedBy<LoginAI>());
            Application = Container.Resolve<ILoginAI>();
        }
    }
}
