using SOCE.Library.Db;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;


namespace SOCE.Library.UI.ViewModels
{
    public class CodeInsertVM : BaseVM
    {
        public string _code { get; set; } = "";
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
                RaisePropertyChanged(nameof(Code));
            }
        }

        public string CodeToExpect;

        public EmployeeDbModel LoggedInEmployee;

        public ICommand GoToNewViewCommand { get; set; }

        public ICommand GoToNextPageCommand { get; set; }

        private string _message { get; set; }
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                RaisePropertyChanged(nameof(Message));
            }
        }

        public CodeInsertVM(string codetofind, EmployeeDbModel em)
        {
            CodeToExpect = codetofind;
            LoggedInEmployee = em;
            this.GoToNewViewCommand = new RelayCommand(IoCLogin.Application.ForgotPassword);
            this.GoToNextPageCommand = new RelayCommand(GoToResetPasswordView);
        }

        public void GoToResetPasswordView()
        {

            if (Code == CodeToExpect && LoggedInEmployee != null)
            {
                //go to new view.
                IoCLogin.Application.ResetPassword(LoggedInEmployee);

            }
            else
            {
                Message = $"The code you entered was incorrect. {Environment.NewLine}Please try again.";

            }

            return;

        }

    }
}
