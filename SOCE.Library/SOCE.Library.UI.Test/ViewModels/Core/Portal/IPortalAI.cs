using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SOCE.Library.UI.ViewModels
{
    public interface IPortalAI: IBaseAI
    {
        ICommand GoToNewViewCommand { get; set; }

        void GoToPage(PortalPage page);
    }
}
