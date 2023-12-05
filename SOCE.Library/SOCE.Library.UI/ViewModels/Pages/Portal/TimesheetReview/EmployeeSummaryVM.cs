using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using SOCE.Library.Db;
using System.Globalization;
using MaterialDesignThemes.Wpf;
using SOCE.Library.UI.Views;
using MimeKit;
using MailKit.Net.Smtp;
using System.Windows;
using System.Threading.Tasks;

namespace SOCE.Library.UI.ViewModels
{
    public class EmployeeSummaryVM : BaseVM
    {

        private ObservableCollection<TimesheetSubmissionModel> _timesheetSubmissions = new ObservableCollection<TimesheetSubmissionModel>();
        public ObservableCollection<TimesheetSubmissionModel> TimesheetSubmissions
        {
            get { return _timesheetSubmissions; }
            set
            {
                _timesheetSubmissions = value;
                RaisePropertyChanged(nameof(TimesheetSubmissions));
            }
        }

        public ICommand OpenTimesheetSubmission { get; set; }

        public ICommand ApproveAllCommand { get; set; }
        public ICommand EmailReminder { get; set; }

        public TimesheetReviewVM basevm;

        public EmployeeSummaryVM(TimesheetReviewVM vmbase)
        {
            this.OpenTimesheetSubmission = new RelayCommand<TimesheetSubmissionModel>(OpenTimeSheet);
            this.EmailReminder = new RelayCommand<TimesheetSubmissionModel>(SendEmail);
            this.ApproveAllCommand = new RelayCommand(ApproveAll);

            basevm = vmbase;

            LoadTimesSheetSubmissionData(vmbase.firstdate);
        }

        private async void ApproveAll()
        {
            YesNoView view = new YesNoView();
            YesNoVM aysvm = new YesNoVM();

            aysvm.Message = "Are you sure?";
          

            view.DataContext = aysvm;
            var result = await DialogHost.Show(view, "RootDialog");
            aysvm = view.DataContext as YesNoVM;

            if (aysvm.Result)
            {
                //ProgressBarView ecv = new ProgressBarView();
                //ProgressBarVM ecvm = new ProgressBarVM(TimesheetSubmissions.ToList(), basevm.firstdate, basevm.lastdate);
                ////show progress bar and do stuff
                //ecv.DataContext = ecvm;
                //var newres = await DialogHost.Show(ecv, "RootDialog");

                //approve all
                foreach (TimesheetSubmissionModel tsm in TimesheetSubmissions)
                {
                    if (!tsm.Missing)
                    {
                        //load row data
                        List<TimesheetRowModel> trms = new List<TimesheetRowModel>();
                        DateTime datestart = basevm.DateSummary.First().Value;
                        DateTime dateend = basevm.DateSummary.Last().Value;

                        //update employee Id
                        List<TimesheetRowDbModel> dbtimesheetdata = SQLAccess.LoadTimeSheet(datestart, dateend, tsm.Employee.Id);

                        foreach (TimesheetRowDbModel trm in dbtimesheetdata)
                        {
                            trm.Approved = 1;
                            SQLAccess.UpdateTimesheetData(trm);
                        }

                        TimesheetSubmissionDbModel dbmodel2 = new TimesheetSubmissionDbModel()
                        {
                            Id = tsm.Id,
                            EmployeeId = tsm.Employee.Id,
                            Date = (int)long.Parse(tsm.Date.ToString("yyyyMMdd")),
                            TotalHours = tsm.TotalHours,
                            PTOHours = tsm.PTOHours,
                            OTHours = tsm.OTHours,
                            SickHours = tsm.SickHours,
                            HolidayHours = tsm.HolidayHours,
                            Approved = 1
                        };

                        SQLAccess.UpdateTimesheetSubmission(dbmodel2);

                    }
                }
                LoadTimesSheetSubmissionData(basevm.firstdate);
            }
        }

        private async void OpenTimeSheet(TimesheetSubmissionModel timesheetsubmission)
        {
            CoreAI CurrentPage = IoCCore.Application as CoreAI;
            CurrentPage.MakeBlurry();
            await Task.Run(() => Task.Delay(600));
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                TimesheetViewerVM tvvm = new TimesheetViewerVM(basevm, timesheetsubmission);
                basevm.ReviewVM = tvvm;
            }));
            await Task.Run(() => Task.Delay(600));
            CurrentPage.MakeClear();
        }

        private void SendEmail(TimesheetSubmissionModel tsm)
        {
            string MonthYearString = $"{basevm.firstdate.ToString("MMMM")} {basevm.firstdate.Year}";
            string DateString = $"[{basevm.firstdate.Day} - {basevm.lastdate.Day}]";
            TextPart txt = new TextPart("plain")
            {
                Text = $"Hello {tsm.Employee.FirstName}, {Environment.NewLine}" +
                $"Please submit your timesheet for the pay period: {Environment.NewLine}" +
                $"{Environment.NewLine} {MonthYearString} {DateString} {Environment.NewLine} {Environment.NewLine}" +
                $"Thanks,{Environment.NewLine}" +
                $"SOCE Portal Dev Team."

            };

            SendEmailMessage(txt, tsm.Employee, "SOCE Portal Timesheet Reminder");

        }

        public void LoadTimesSheetSubmissionData(DateTime startdate)
        {
            List<EmployeeModel> allemployees = LoadActiveEmployees();
            TimesheetSubmissions.Clear();
            int DateTimesheet = (int)long.Parse(startdate.Date.ToString("yyyyMMdd"));
            foreach (EmployeeModel em in allemployees)
            {
                TimesheetSubmissionDbModel tsdbm = SQLAccess.LoadTimeSheetSubmissionData(DateTimesheet, em.Id);

                if (tsdbm == null)
                {
                    //not submitted yet
                    if (em.IsActive)
                    {
                        TimesheetSubmissions.Add(TimesheetSubmissionModel.Didnotsubmityet(em));
                    }
                }
                else
                {
                    TimesheetSubmissions.Add(new TimesheetSubmissionModel(tsdbm, em));
                }

            }
            //NumSubmittedEmployees = TimesheetSubmissions.Where(x => x.Missing == false).Count();
            //NumApprovedEmployees = TimesheetSubmissions.Where(x => x.Approved).Count();
            //NumTotalEmployees = TimesheetSubmissions.Count();
        }

        private List<EmployeeModel> LoadActiveEmployees()
        {
            List<EmployeeDbModel> dbemployees = SQLAccess.LoadEmployees();

            List<EmployeeModel> members = new List<EmployeeModel>();

            foreach (EmployeeDbModel emdb in dbemployees)
            {
                //add is active here
                EmployeeModel em = new EmployeeModel(emdb);
                members.Add(em);
            }

            return members;
        }

        private async void SendEmailMessage(TextPart textbody, EmployeeModel employeetosendto, string subject)
        {
            SmtpClient client = new SmtpClient();

            client.Connect("smtp-mail.outlook.com", 587, false);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate("normnoe@shirkodonovan.com", "Barry553");
            MimeMessage mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Portal Help Desk", "portalhelpdesk@shirkodonovan.com"));
            mailMessage.To.Add(new MailboxAddress(employeetosendto.FirstName, employeetosendto.Email));
            mailMessage.Subject = subject;
            mailMessage.Body = textbody;


            await client.SendAsync(mailMessage);
            client.Disconnect(true);
        }

    }
}
