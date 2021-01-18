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
using SORD.Library.Models;
using System.Windows;
using Microsoft.AspNetCore.Mvc;

namespace SORD.Library.UI.ViewModels
{
    public class SignupVM :BaseVM
    {
        public RegisterRequest Register { get; set; } = new RegisterRequest();
        public ICommand GoToNewViewCommand { get; set; }

        public ICommand RegisterCommand { get; set; }

        public string SignUpMessage { get; set; }

        public SignupVM()
        {
            Register.Password = "pass123";
            Register.ConfirmPassword = "pass123";
            Register.AcceptTerms = true;
            this.GoToNewViewCommand = new RelayCommand<ApplicationPage>(GoToViewCommand.GoToPageWrapper);
            this.RegisterCommand = new RelayCommand<RegisterRequest>(RegisterCom);
        }

        /// <summary>
        /// Register command
        /// </summary>
        /// <param name="loginrequest"></param>
        public void RegisterCom(RegisterRequest registerrequest)
        {
            //serialized input
            string sinput = JsonSerializer.Serialize(registerrequest);

            Task<HttpResponseMessage> loginresponse = APIHelper.ApiCall("Accounts/register", HttpMethod.Post, sinput);

            //if not verified -> one message
            //if username or password incorrect -> another message

            //Server error most like if caught - TODO in future
            HttpResponseMessage response = loginresponse.Result;

            if (response == null)
            {
                SignUpMessage = $"Review username and password {Environment.NewLine} account was not found";
            }
            else
            {
                //Launch another window here!
                AuthenticateResponse authresp = response.Content.ReadAsAsync<AuthenticateResponse>().Result;

                if (authresp.IsVerified)
                {
                    //User is authenticated

                    //close login window
                    Application.Current.MainWindow.Close();
                }
                else
                {
                    SignUpMessage = $"The account was registered {Environment.NewLine} but never verified";
                    //Resend verification email?
                }
            }
        }
    }
}
