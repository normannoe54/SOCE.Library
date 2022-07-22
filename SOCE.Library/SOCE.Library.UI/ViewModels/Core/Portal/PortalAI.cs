using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace SOCE.Library.UI.ViewModels
{
    public class PortalAI : BaseAI, IPortalAI
    {
        public PortalAI()
        {
            CurrentPage = new LoginVM();
        }

        public void GoToPage(PortalPage page)
        {
            switch (page)
            {
                case PortalPage.Login:
                    CurrentPage = new LoginVM();
                    break;
                case PortalPage.ForgotPassword:
                    CurrentPage = new ForgotPasswordVM();
                    break;
                case PortalPage.Signup:
                    CurrentPage = new SignupVM();
                    break;
            }
        }
    }
}
