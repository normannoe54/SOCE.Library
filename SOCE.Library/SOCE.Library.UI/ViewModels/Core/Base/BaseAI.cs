using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace SOCE.Library.UI.ViewModels
{
    public class BaseAI : BaseVM, IBaseAI
    {
        private BaseVM _currentPage { get; set; }
        public BaseVM CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                _currentPage = value;
                RaisePropertyChanged(nameof(CurrentPage));
            }
        }


        public BaseAI()
        {
            //CurrentPage = new LoginVM();
        }
    }
}
