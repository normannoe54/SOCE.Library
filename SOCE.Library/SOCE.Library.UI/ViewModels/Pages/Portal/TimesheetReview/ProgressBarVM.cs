using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Reflection;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace SOCE.Library.UI.ViewModels
{
    public class ProgressBarVM : BaseVM
    {
        private List<TimesheetSubmissionModel> TimesheetSubmissions = new List<TimesheetSubmissionModel>();

        private string _textToShow = "";
        public string TextToShow
        {
            get
            {
                return _textToShow;
            }
            set
            {
                _textToShow = value;
                RaisePropertyChanged(nameof(TextToShow));
            }
        }

        private CancellationTokenSource _cts = new CancellationTokenSource();
        public ICommand CancelCommand { get; set; }

        private double _increment = 0;
        public double Increment
        {
            get
            {
                return _increment;
            }
            set
            {
                _increment = value;
                RaisePropertyChanged(nameof(Increment));
            }
        }

        private DateTime startdate;
        private DateTime enddate;

        public ProgressBarVM(List<TimesheetSubmissionModel> timesheetsubmissions, DateTime first, DateTime last)
        {
            startdate = first;
            last = enddate;
            TimesheetSubmissions = timesheetsubmissions;
            DoStuff();
            this.CancelCommand = new RelayCommand(this.CancelAction);
        }

        private void CancelAction()
        {
            _cts.Cancel();
        }

        private async void DoStuff()
        {
            //Thread.Sleep(200);
            await Task.Run(() => RunProgress());
            //Thread.Sleep(200);
            DialogHost.Close("RootDialog");
        }

        private void RunProgress()
        {
            //Thread.Sleep(200);
            int doubleinc = 0;
            if (TimesheetSubmissions.Count > 0)
            {
                try
                {
                    //approve all
                    foreach (TimesheetSubmissionModel tsm in TimesheetSubmissions)
                    {
                        _cts.Token.ThrowIfCancellationRequested();

                        if (!tsm.Missing)
                        {
                            //load row data
                            List<TimesheetRowModel> trms = new List<TimesheetRowModel>();
                            DateTime datestart = startdate;
                            DateTime dateend = enddate;

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
                                PTOAdded = tsm.PTOAdded,
                                //SickHours = tsm.SickHours,
                                HolidayHours = tsm.HolidayHours,
                                Approved = 1
                            };

                            SQLAccess.UpdateTimesheetSubmission(dbmodel2);
                            Increment = (Convert.ToDouble(doubleinc) / TimesheetSubmissions.Count) * 100;
                            TextToShow = $"Updating {tsm.Employee.FullName} time sheet";
                        }

                        doubleinc++;
                    }

                }
                catch
                {

                }

            }
        }



        //private void DoSomeCounting(int x)
        //{
        //    for (var i = 0; i < x; i++)
        //    {
        //        var value = i.ToString();

        //        Text = value;
        //        Thread.Sleep(100);

        //        if (_cts != null) continue;

        //        Text = "";
        //        return;
        //    }
        //}

        //private async Task DoSomethingAsync(CancellationToken token, int size)
        //{
        //    while (_cts != null)
        //    {
        //        token.ThrowIfCancellationRequested();
        //        await Task.Run(() => DoSomeCounting(size), token);
        //    }
        //}
    }
}
