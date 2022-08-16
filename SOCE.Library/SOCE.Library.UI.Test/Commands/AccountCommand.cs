using System;
using System.Collections.Generic;
using System.Text;
using SOCE.Library.Models.Accounts;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using SOCE.Library.Models ;

namespace SOCE.Library.UI
{
    public static class AccountCommand
    {

        /// <summary>
        /// Register command
        /// </summary>
        /// <param name="loginrequest"></param>
        public static void SendForgotEmail(ForgotPassRequestModel fprequest)
        {
            ForgotPasswordRequest convertedinput = fprequest.ConvertAPIModel();

            //serialized input
            string sinput = JsonSerializer.Serialize(convertedinput);

            Task<HttpResponseMessage> response = APIHelper.ApiCall("Accounts/verify-email", HttpMethod.Post, sinput);

            //send output
        }
    }
}
