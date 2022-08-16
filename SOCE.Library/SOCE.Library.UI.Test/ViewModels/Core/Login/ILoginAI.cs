using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SOCE.Library.UI.ViewModels
{
    public interface ILoginAI : IBaseAI
    {
        void GoToPage(LoginPage page);
        ICommand UpdateMWCommand { get; set; }
    }
}
