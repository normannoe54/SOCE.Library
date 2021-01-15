using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using SORD.Library.Models.Accounts;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using SORD.Library.UI.Helpers;
using System.Windows;

namespace SORD.Library.UI.ViewModels
{ 
    public class LoginVM : BaseVM
    {
        public AuthenticateRequest LoginRequest { get; set; } = new AuthenticateRequest();
        public ICommand GoToNewViewCommand { get; set; }

        public ICommand LoginCommand { get; set; }

        //Default message
        public string LoginMessage { get; set; }

        public LoginVM()
        {
            LoginRequest.Password = "pass123";
            this.GoToNewViewCommand = new RelayCommand<ApplicationPage>(GoToViewCommand.GoToPageWrapper);
            this.LoginCommand = new RelayCommand<AuthenticateRequest>(LoginCom);
        }

        /// <summary>
        /// Login command
        /// </summary>
        /// <param name="loginrequest"></param>
        public void LoginCom(AuthenticateRequest loginrequest)
        {
            //serialized input
            string sinput = JsonSerializer.Serialize(loginrequest);

            Task<AuthenticateResponse> loginresponse = APIHelper.ApiCall<AuthenticateResponse>("Accounts/authenticate", HttpMethod.Post, sinput);

            //Server error most like if caught - TODO in future
            try
            {
                if (loginresponse.Result == null)
                {
                    LoginMessage = $"Review username and password {Environment.NewLine} account was not found";
                }
                else if (loginresponse.Result.IsVerified)
                {
                    //Launch another window here!

                    //close login window
                    Application.Current.MainWindow.Close();
                }
                else
                {
                    LoginMessage = $"The account was registered {Environment.NewLine} but never verified";
                    //Resend verification email?
                }
            }
            catch
            {
                LoginMessage = $"Server error {Environment.NewLine} contact this person";
            }
            
        
        }
    }
}
