using System;
using System.Collections.Generic;
using System.Text;

namespace SOCE.Library.UI
{
    public class ForgotPassRequestModel : PropertyChangedBase
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

        //public ForgotPasswordRequest ConvertAPIModel()
        //{
        //    return new ForgotPasswordRequest()
        //    {
        //        Email = Email,
        //    };
        //}
    }
}
