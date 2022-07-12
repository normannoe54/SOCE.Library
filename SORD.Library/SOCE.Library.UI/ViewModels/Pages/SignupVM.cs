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
using Microsoft.AspNetCore.Mvc;

namespace SOCE.Library.UI.ViewModels
{
    public class SignupVM : BaseVM
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
            //SignUpMessage = "Testing Message";
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

            //Server error most like if caught - TODO in future
            HttpResponseMessage response = loginresponse.Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //Launch login page with message saying "Account successfully registered, sign in here" - TODO


            }
            else
            {
                SignUpMessage = $"This account already exists {Environment.NewLine} use the login screen";
            }
        }
    }
}
