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
        private AuthRequestModel _loginRequest = new AuthRequestModel();
        public AuthRequestModel LoginRequest
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

        private string _loginMessage { get; set; }
        public string LoginMessage
        {
            get
            {
                return _loginMessage;
            }
            set
            {
                _loginMessage = value;
                RaisePropertyChanged(nameof(LoginMessage));
            }
        }

        public LoginVM()
        {
            //LoginRequest.Password = "pass123";
            //LoginMessage = "";
            this.GoToNewViewCommand = new RelayCommand<LoginPage>(GoToViewCommand.GoToPageWrapper);
            this.LoginCommand = new RelayCommand<AuthRequestModel>(LoginCom);
        }

        /// <summary>
        /// Login command
        /// </summary>
        /// <param name="loginrequest"></param>
        public void LoginCom(AuthRequestModel loginrequest)
        {
            IoCCore.Application.CurrentPage = IoCPortal.Application as BaseAI;

            //check email
            //string emailcheck = loginrequest.Email.Substring(loginrequest.Email.LastIndexOf('@') + 1);

            //if (emailcheck != "email.com")
            //{
            //    LoginMessage = $"Shirk & O'Donovan email must be {Environment.NewLine}included to login to the application";
            //    return;
            //}

            //AuthenticateRequest convertedinput = loginrequest.ConvertAPIModel();

            ////serialized input
            //string sinput = JsonSerializer.Serialize(convertedinput);

            //Task<HttpResponseMessage> loginresponse = APIHelper.ApiCall("Accounts/authenticate", HttpMethod.Post, sinput);

            //try
            //{
            //    HttpResponseMessage response = loginresponse.Result;

            //    if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //    {
            //        AuthenticateResponse authresp = response.Content.ReadAsAsync<AuthenticateResponse>().Result;

            //        if (authresp.IsVerified)
            //        {
            //            //User is authenticated


            //            //close login window
            //            Application.Current.MainWindow.Close();
            //        }
            //        else
            //        {
            //            LoginMessage = $"The account was registered {Environment.NewLine} but never verified";
            //            //Resend verification email?
            //        }
            //    }
            //    else
            //    {
            //        LoginMessage = $"Review username and password {Environment.NewLine} account was not found";
            //    }
            //}
            //catch
            //{
            //    LoginRequest.Email = "";
            //    LoginRequest.Password = "";

            //    LoginMessage = $"Review username and password {Environment.NewLine} account was not found";
            //}

        }
    }
}