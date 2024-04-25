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
    public class ExportConfirmProposalVM : BaseVM
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
        private List<ProposalViewResModel> Proposals;

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

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public ExportConfirmProposalVM(List<ProposalViewResModel> proposals, DateTime dateStart, DateTime dateEnd)
        {
            DateStart = dateStart;
            DateEnd = dateEnd;

            Proposals = proposals;
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
        }

        private void RunProgress()
        {
            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string pathDownload = Path.Combine(pathUser, "Downloads\\ProposalDataExport.xlsx");
            File.WriteAllBytes(pathDownload, Properties.Resources.ProposalDataExport);
            Excel.Excel exinst = new Excel.Excel(pathDownload);
            int basenum = 6;
            int doubleinc = 0;
            Thread.Sleep(200);
            if (Proposals.Count > 0)
            {
                try
                {
                    exinst.WriteCell(3, 5, DateStart.ToString("yyyy/MM/dd"));
                    exinst.WriteCell(3, 7, DateEnd.ToString("yyyy/MM/dd"));
                    foreach (ProposalViewResModel pm in Proposals)
                    {
                        _cts.Token.ThrowIfCancellationRequested();
                        //pm.FormatData(true);

                        List<object> rowinputs = new List<object>();
                        rowinputs.Add(pm.ProposalName);
                        rowinputs.Add(pm.Status.ToString());
                        rowinputs.Add(pm.Client.ClientName);
                        rowinputs.Add(pm.Market.MarketName);
                        rowinputs.Add((pm.Sender != null) ? pm.Sender.FullName : "");
                        rowinputs.Add(pm.Fee);
                        rowinputs.Add(pm.DateSent);
                        rowinputs.Add(pm.CostMetricValue);
                        rowinputs.Add(pm.CostMetric);
                        rowinputs.Add(pm.Remarks);
                        rowinputs.Add(pm.LinkFolder);

                        Increment = (Convert.ToDouble(doubleinc) / Proposals.Count) * 100;
                        TextToShow = $"Exporting {pm.ProposalName}";
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
    }
}
