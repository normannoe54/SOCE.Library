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
    public class SignupVM :BaseVM
    {
        public RegisterRequest Register { get; set; } = new RegisterRequest();
        public ICommand GoToNewViewCommand { get; set; }

        public ICommand RegisterCommand { get; set; }

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
        public static void RegisterCom(RegisterRequest registerrequest)
        {
            //serialized input
            string sinput = JsonSerializer.Serialize(registerrequest);

            Task<AuthenticateResponse> loginresponse = APIHelper.ApiCall<AuthenticateResponse>("Accounts/register", HttpMethod.Post, sinput);

            //if not verified -> one message
            //if username or password incorrect -> another message
            
        }
    }
}
