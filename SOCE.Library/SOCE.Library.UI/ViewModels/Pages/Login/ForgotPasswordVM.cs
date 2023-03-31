using MailKit.Net.Smtp;
using MimeKit;
using SOCE.Library.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;


namespace SOCE.Library.UI.ViewModels
{
    public class ForgotPasswordVM : BaseVM
    {
        public string _email { get; set; } = "";
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

        public ICommand GoToNewViewCommand { get; set; }

        public ICommand SendEmailCommand { get; set; }

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

        public ForgotPasswordVM()
        {
            this.GoToNewViewCommand = new RelayCommand(IoCLogin.Application.GoToLogin);
            this.SendEmailCommand = new RelayCommand(SendForgotEmail);
        }

        public async void SendForgotEmail()
        {
            string emailinput = Email + "@shirkodonovan.com";

            //tbd
            EmployeeDbModel em = SQLAccess.LoadEmployeeByUser(emailinput);

            if (em != null)
            {
                //send email
                string code = RandomString(5);

                MimeMessage mailMessage = new MimeMessage();
                mailMessage.From.Add(new MailboxAddress("Norm", "portalhelpdesk@shirkodonovan.com"));
                mailMessage.To.Add(new MailboxAddress(em.FirstName, emailinput));
                mailMessage.Subject = "SOCE Portal Reset Password";
                mailMessage.Body = new TextPart("plain")
                {
                    Text = $"Hello {em.FirstName}, {Environment.NewLine}" +
                    $"To reset your password enter in the code below: {Environment.NewLine}" +
                    $"{Environment.NewLine} {code} {Environment.NewLine} {Environment.NewLine}" +
                    $"Thanks,{Environment.NewLine}" +
                    $"SOCE Portal Dev Team."

                };

                using (var client = new SmtpClient())
                {
                    //Have tried both false and true    
                    client.Connect("smtp-mail.outlook.com", 587, false);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate("normnoe@shirkodonovan.com", "Barry553");
                    await client.SendAsync(mailMessage);
                    client.Disconnect(true);
                }


                //insert code
                IoCLogin.Application.InsertCode(code, em);

                //EmployeeModel employee = new EmployeeModel(em);
                //CoreAI globalwindow = (CoreAI)IoCCore.Application;
                //globalwindow.WindowType = WindowState.Maximized;
                //globalwindow.GoToPortal(employee);
            }
            else
            {
                LoginMessage = $"Could not find username,{Environment.NewLine}please try again";
                return;
            }

        }

        private  Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
