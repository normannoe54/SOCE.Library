using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SORD.Library.UI
{
    public interface IApplicationInstance
    {
        BaseVM CurrentPage { get; set; }
        void GoToPage(ApplicationPage page);
        ICommand UpdateMWCommand { get; set; }
    }
}
