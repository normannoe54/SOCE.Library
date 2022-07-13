using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using SOCE.Library.Models.Accounts;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using SOCE.Library.Models;
using System.Windows;

namespace SOCE.Library.UI.ViewModels
{ 
    public class LoginVM : BaseVM
    {
        private AuthenticateRequest _loginRequest = new AuthenticateRequest();
        public AuthenticateRequest LoginRequest
        {
            get
            {
                return _loginRequest;
            }
            set
            {
                _loginRequest = value;
                RaisePropertyChanged(nameof(LoginRequest));
            }
        }

        public ICommand GoToNewViewCommand { get; set; }

        public ICommand LoginCommand { get; set; }

        //Default message
        public string LoginMessage { get; set; }

        public LoginVM()
        {
            //LoginRequest.Password = "pass123";
            //LoginMessage = "";
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

            Task<HttpResponseMessage> loginresponse = APIHelper.ApiCall("Accounts/authenticate", HttpMethod.Post, sinput);

            HttpResponseMessage response = loginresponse.Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                AuthenticateResponse authresp = response.Content.ReadAsAsync<AuthenticateResponse>().Result;

                if (authresp.IsVerified)
                {
                    //User is authenticated


                    //close login window
                    Application.Current.MainWindow.Close();
                }
                else
                {
                    LoginMessage = $"The account was registered {Environment.NewLine} but never verified";
                    //Resend verification email?
                }
            }
            else
            {
                LoginMessage = $"Review username and password {Environment.NewLine} account was not found";
            }
        }
    }
}
