using System;
using System.Collections.Generic;
using System.Text;
using SORD.Library.UI.ViewModels;

namespace SORD.Library.UI
{
    public static class GoToViewCommand
    {
        public static void GoToPageWrapper(ApplicationPage page)
        {
            IoC.Application.GoToPage(page);
        }
    }
}
