using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using SOCE.Library.Db;
using SOCE.Library.UI.ViewModels;
using SOCE.Library.UI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SOCE.Library.UI
{
    public class ProposalViewResModel : PropertyChangedBase
    {
        private int _id { get; set; }
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        private ProposalStatusEnum _status { get; set; }
        public ProposalStatusEnum Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                RaisePropertyChanged(nameof(Status));
            }
        }

        private EmployeeLowResModel _sender { get; set; }
        public EmployeeLowResModel Sender
        {
            get
            {
                return _sender;
            }
            set
            {
                _sender = value;
                RaisePropertyChanged(nameof(Sender));
            }
        }

        private ClientModel _client { get; set; }
        public ClientModel Client
        {
            get
            {
                return _client;
            }
            set
            {
                _client = value;
                RaisePropertyChanged(nameof(Client));
            }
        }

        private MarketModel _market { get; set; }
        public MarketModel Market
        {
            get
            {
                return _market;
            }
            set
            {
                _market = value;
                //Costmetric = (_market.MarketName.ToUpper() != "MULTI-FAMILY") ? "/SQ.FT." : "/Unit";
                RaisePropertyChanged(nameof(Market));
            }
        }

        private DateTime? _dateSent { get; set; }
        public DateTime? DateSent
        {
            get
            {
                return _dateSent;
            }
            set
            {
                _dateSent = value;
                RaisePropertyChanged(nameof(DateSent));
            }
        }

        private string _proposalName { get; set; }
        public string ProposalName
        {
            get
            {
                return _proposalName;
            }
            set
            {
                _proposalName = value;
                RaisePropertyChanged(nameof(ProposalName));
            }
        }

        private double _costMetricValue { get; set; }
        public double CostMetricValue
        {
            get
            {
                return _costMetricValue;
            }
            set
            {
                _costMetricValue = value;
                RaisePropertyChanged(nameof(CostMetricValue));
            }
        }

        private string _costMetric { get; set; } 
        public string CostMetric
        {
            get
            {
                return _costMetric;
            }
            set
            {
                _costMetric = value;
                RaisePropertyChanged(nameof(CostMetric));
            }
        }

        private string _remarks = "";
        public string Remarks
        {
            get
            {
                return _remarks;
            }
            set
            {
                _remarks = value;
                RaisePropertyChanged(nameof(Remarks));
            }
        }

        private double _fee = 0;
        public double Fee
        {
            get { return _fee; }
            set
            {
                _fee = value;
                RaisePropertyChanged(nameof(Fee));
            }
        }

        private bool onstartup = false;

        private bool _editFieldState = true;
        public bool EditFieldState
        {
            get { return _editFieldState; }
            set
            {
                if (!_editFieldState && value)
                {
                    UpdateProjectMainModel();
                }
                _editFieldState = value;
                //ComboFieldState = !_editFieldState;

                RaisePropertyChanged(nameof(EditFieldState));
            }
        }

        public ICommand CopyLinkFolderCommand { get; set; }
        public ICommand SelectLinkFolderCommand { get; set; }
        public ICommand OpenLinkFolderCommand { get; set; }

        private string _linkFolder = "";
        public string LinkFolder
        {
            get { return _linkFolder; }
            set
            {
                _linkFolder = value;
                if (onstartup)
                {
                    UpdateProjectMainModel();
                }
                RaisePropertyChanged(nameof(LinkFolder));
            }
        }

        public ProposalViewResModel()
        {

        }

        public ProposalViewResModel(ProposalDbModel pm)
        {
            onstartup = false;
            this.CopyLinkFolderCommand = new RelayCommand(this.CopyLinkFolder);
            this.SelectLinkFolderCommand = new RelayCommand(this.SelectLinkFolder);
            this.OpenLinkFolderCommand = new RelayCommand(this.OpenLinkFolder);

            Id = pm.Id;
            ProposalName = pm.ProposalName;
            Status = (ProposalStatusEnum)pm.Status;
            Fee = pm.Fee;
            CostMetric = pm.CostMetric;
            CostMetricValue = pm.CostMetricValue;
            Remarks = pm.Remarks;
            LinkFolder = pm.LinkFolder;
            if (pm?.DateSent != null && pm?.DateSent != 0)
            {
                DateSent = DateTime.ParseExact(pm.DateSent.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            onstartup = true;
        }

        public void SelectLinkFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;

            // Process open file dialog box results
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //save file
                LinkFolder = dialog.FileName;
            }
        }

        public void CopyLinkFolder()
        {
            if (LinkFolder != null)
            {
                Clipboard.SetText(LinkFolder);
            }
        }

        public void OpenLinkFolder()
        {
            try
            {
                Process.Start(LinkFolder);
            }
            catch { }
        }

        private void UpdateProjectMainModel()
        {
            int? duedatevar = null;

            if (DateSent != null)
            {
                duedatevar = (int)long.Parse(DateSent?.ToString("yyyyMMdd"));
            }
            int pmid = 0;
            if (Sender != null)
            {
                pmid = Sender.Id;
            }

            ProposalDbModel proposal = new ProposalDbModel()
            {
                Id = Id,
                ProposalName = ProposalName,
                ClientId = Client.Id,
                MarketId = Market.Id,
                SenderId = pmid,
                DateSent = duedatevar,
                Remarks = Remarks,
                CostMetricValue = CostMetricValue,
                CostMetric = CostMetric,
                Status = (int)Status,
                Fee = Fee,
                LinkFolder = LinkFolder,
            };

            SQLAccess.UpdateProposal(proposal);
        }
    }
}
