using System;
using System.Collections.Generic;
using System.Text;

namespace SORD.Library.UI
{
    public class ApplicationInstance : BaseVM, IApplicationInstance
    {
        public BaseVM CurrentPage { get; set; }

        public ICommand UpdateMWCommand { get; set; }

        public ApplicationInstance()
        {
            this.CurrentPage = new ProjectVM();
            this.UpdateMWCommand = new RelayCommand<ApplicationPage>(this.GoToPage);
        }

        public void GoToPage(ApplicationPage page)
        {
            switch (page)
            {
                case ApplicationPage.Project:
                    CurrentPage = new ProjectVM();
                    break;
                case ApplicationPage.Account:
                    CurrentPage = new AccountVM();
                    break;
                case ApplicationPage.Docu:
                    CurrentPage = new DocuVM();
                    break;
                case ApplicationPage.Contact:
                    CurrentPage = new ContactVM();
                    break;
                case ApplicationPage.NewProj:
                    CurrentPage = new NewProjectVM();
                    break;
            }
        }
    }
}
