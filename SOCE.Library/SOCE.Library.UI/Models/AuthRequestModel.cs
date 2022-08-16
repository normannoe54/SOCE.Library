using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Library.UI
{
    public class AuthRequestModel : PropertyChangedBase
    {
        private string _email = "";
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                RaisePropertyChanged(nameof(Email));
            }
        }

        private string _password = "";
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                RaisePropertyChanged(nameof(Password));
            }
        }

        //public AuthenticateRequest ConvertAPIModel()
        //{
        //    return new AuthenticateRequest()
        //    {
        //        Email = Email,
        //        Password = Password,
        //    };
        //}
    }
}
