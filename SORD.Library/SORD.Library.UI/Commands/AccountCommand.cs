using System;
using System.Collections.Generic;
using System.Text;
using SORD.Library.Models.Accounts;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using SORD.Library.UI.Helpers;

namespace SORD.Library.UI
{
    public static class AccountCommand
    {
        /// <summary>
        /// Login command
        /// </summary>
        /// <param name="loginrequest"></param>
        public static void Login(AuthenticateRequest loginrequest)
        {
            //serialized input
            string sinput = JsonSerializer.Serialize(loginrequest);

            Task<AuthenticateResponse> loginresponse = APIHelper.ApiCall<AuthenticateResponse>("Accounts/authenticate", HttpMethod.Post, sinput);

            //send output
        }

        /// <summary>
        /// Register command
        /// </summary>
        /// <param name="loginrequest"></param>
        public static void Register(RegisterRequest registerrequest)
        {
            //serialized input
            string sinput = JsonSerializer.Serialize(registerrequest);

            Task<AuthenticateResponse> loginresponse = APIHelper.ApiCall<AuthenticateResponse>("Accounts/register", HttpMethod.Post, sinput);

            //send output
        }

        /// <summary>
        /// Register command
        /// </summary>
        /// <param name="loginrequest"></param>
        public static void SendForgotEmail(ForgotPasswordRequest fprequest)
        {
            //serialized input
            string sinput = JsonSerializer.Serialize(fprequest);

            Task<string> response = APIHelper.ApiCall<string>("Accounts/verify-email", HttpMethod.Post, sinput);

            //send output
        }
    }
}
