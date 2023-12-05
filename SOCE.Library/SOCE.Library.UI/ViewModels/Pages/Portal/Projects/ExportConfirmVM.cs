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
using System.Linq;

namespace SOCE.Library.UI.ViewModels
{
    public class ExportConfirmVM : BaseVM
    {

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
        private List<ProjectViewResModel> Projects;

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

        public ExportConfirmVM(List<ProjectViewResModel> projects)
        {
            Projects = projects;
            DoStuff();
            this.CancelCommand = new RelayCommand(this.CancelAction);
        }

        private void CancelAction()
        {
            _cts.Cancel();
        }

        private async void DoStuff()
        {
            Thread.Sleep(200);
            await Task.Run(() => RunProgress());
            Thread.Sleep(200);
            DialogHost.Close("RootDialog");



            //if (_cts == null)
            //{
            //    _cts = new CancellationTokenSource();
            //    try
            //    {
            //        await DoSomethingAsync(_cts.Token, 1000000);
            //    }
            //    catch (OperationCanceledException)
            //    {
            //    }
            //    finally
            //    {
            //        _cts = null;
            //    }
            //}
            //else
            //{
            //    _cts.Cancel();
            //    _cts = null;
            //}
        }

        private void RunProgress()
        {
            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string pathDownload = Path.Combine(pathUser, "Downloads\\ProjectDataExport.xlsx");
            File.WriteAllBytes(pathDownload, Properties.Resources.ProjectDataExport);
            Excel.Excel exinst = new Excel.Excel(pathDownload);
            int basenum = 6;
            int doubleinc = 0;
            Thread.Sleep(200);
            if (Projects.Count > 0)
            {
                try
                {
                    foreach (ProjectViewResModel pm in Projects)
                    {
                        _cts.Token.ThrowIfCancellationRequested();
                        //pm.FormatData(true);

                        List<object> rowinputs = new List<object>();
                        rowinputs.Add(pm.ProjectNumber);
                        rowinputs.Add(pm.ProjectName);
                        rowinputs.Add(pm.Client.ClientName);
                        rowinputs.Add(pm.Market.MarketName);
                        rowinputs.Add((pm.ProjectManager != null) ? pm.ProjectManager.FullName : "");
                        rowinputs.Add(pm.PercentComplete/100);
                        rowinputs.Add(pm.Fee);

                        double totalhours = 0;
                        double totalreg = 0;
                        double budgetspent = 0;
                        //double budgetleft = 0;
                        double hoursspent = 0;
                        //double hoursleft = 0;
                        //double percentbudgetspent = 0;

                        List<SubProjectDbModel> subs = SQLAccess.LoadSubProjectsByProject(pm.Id);

                        foreach(SubProjectDbModel sub in subs)
                        {
                            List<RolePerSubProjectDbModel> roles = SQLAccess.LoadRolesPerSubProject(sub.Id);
                            
                            foreach (RolePerSubProjectDbModel role in roles)
                            {
                                totalreg += role.BudgetHours * role.Rate;
                                List<TimesheetRowDbModel> time = SQLAccess.LoadTimeSheetDataByIds(role.EmployeeId, sub.Id);
                                budgetspent += time.Sum(x => x.BudgetSpent);
                                hoursspent += time.Sum(x => x.TimeEntry);
                                totalhours += role.BudgetHours;
                            }
                        }

                        rowinputs.Add(totalreg);
                        rowinputs.Add(budgetspent);
                        rowinputs.Add(totalreg - budgetspent);
                        rowinputs.Add(hoursspent);
                        rowinputs.Add(totalhours - hoursspent);
                        rowinputs.Add((budgetspent/totalreg) / 100);
                        Increment = (Convert.ToDouble(doubleinc) / Projects.Count) * 100;
                        TextToShow = $"Exporting [{pm.ProjectNumber}] {pm.ProjectName}";
                        exinst.WriteRow<object>(basenum, 1, rowinputs);

                        basenum++;
                        doubleinc++;
                        //foreach (SubProjectModel spm in pm.SubProjects)
                        //{
                        //write sub rows
                        //}
                    }

                    exinst.Save();
                    //exinst.FitColumns();
                    Process.Start(pathDownload);

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
