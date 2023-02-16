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
        private List<ProjectModel> Projects;

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

        public ExportConfirmVM(List<ProjectModel> projects)
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
                    foreach (ProjectModel pm in Projects)
                    {
                        _cts.Token.ThrowIfCancellationRequested();
                        pm.FormatData(true);

                        List<object> rowinputs = new List<object>();
                        rowinputs.Add(pm.ProjectNumber);
                        rowinputs.Add(pm.ProjectName);
                        rowinputs.Add(pm.Client.ClientName);
                        rowinputs.Add(pm.Market.MarketName);
                        rowinputs.Add(pm.ProjectManager.FullName);
                        rowinputs.Add(pm.PercentComplete);
                        rowinputs.Add(pm.Fee);
                        rowinputs.Add(pm.TotalRegulatedBudget);
                        rowinputs.Add(pm.BudgetSpent);
                        rowinputs.Add(pm.BudgetLeft);
                        rowinputs.Add(pm.HoursSpent);
                        rowinputs.Add(pm.HoursLeft);
                        rowinputs.Add(pm.PercentBudgetSpent/100);
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
