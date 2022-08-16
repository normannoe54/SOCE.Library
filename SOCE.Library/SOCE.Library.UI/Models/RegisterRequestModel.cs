using System;
using System.Collections.Generic;
using System.Text;

namespace SOCE.Library.UI
{
    public class RegisterRequestModel : PropertyChangedBase
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

        private string _confirmpassword = "";
        public string ConfirmPassword
        {
            get
            {
                return _confirmpassword;
            }
            set
            {
                _confirmpassword = value;
                RaisePropertyChanged(nameof(ConfirmPassword));
            }
        }

        private bool _acceptterms = false;
        public bool AcceptTerms
        {
            get
            {
                return _acceptterms;
            }
            set
            {
                _acceptterms = value;
                RaisePropertyChanged(nameof(AcceptTerms));
            }
        }

        //public RegisterRequest ConvertAPIModel()
        //{
        //    return new RegisterRequest()
        //    {
        //        AcceptTerms = AcceptTerms,
        //        Email = Email,
        //        Password = Password,
        //        ConfirmPassword = ConfirmPassword
        //    };
        //}
    }
}
