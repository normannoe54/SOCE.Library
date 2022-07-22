using System;
using System.Collections.Generic;
using System.Text;
using SOCE.Library.UI.ViewModels;

namespace SOCE.Library.UI
{
    public static class GoToViewCommand
    {
        public static void GoToPageWrapper(LoginPage page)
        {
            IoCLogin.Application.GoToPage(page);
        }
    }
}
