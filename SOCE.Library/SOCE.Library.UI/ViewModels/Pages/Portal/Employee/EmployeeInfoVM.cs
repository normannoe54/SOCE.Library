using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MaterialDesignThemes.Wpf;
using SOCE.Library.Db;
using SOCE.Library.UI.Views;
using System.IO;

namespace SOCE.Library.UI.ViewModels
{
    public class EmployeeInfoVM : BaseVM
    {
        public ICommand SelectPMSignature { get; set; }
        public ICommand GoToTimesheetCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        private EmployeeModel _selectedEmployee = new EmployeeModel();
        public EmployeeModel SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                RaisePropertyChanged(nameof(SelectedEmployee));
            }
        }

        private string _textEmployees;
        public string TextEmployees
        {
            get { return _textEmployees; }
            set
            {
                _textEmployees = value;
                RaisePropertyChanged(nameof(TextEmployees));
            }
        }

        private bool _isPMVisible;
        public bool IsPMVisible
        {
            get { return _isPMVisible; }
            set
            {
                _isPMVisible = value;
                RaisePropertyChanged(nameof(IsPMVisible));
            }
        }

        private bool _rateTitleVisible;
        public bool RateTitleVisible
        {
            get { return _rateTitleVisible; }
            set
            {
                _rateTitleVisible = value;
                RaisePropertyChanged(nameof(RateTitleVisible));
            }
        }

        public EmployeeInfoVM(EmployeeModel employee)
        {
            SelectedEmployee = employee;
            employee.LoadSignature();
            
            //if (employee.SignatureOfPM != null)
            //{
            //    var bi = new BitmapImage();
            //    using (var ms = new MemoryStream())
            //    {
            //        SelectedEmployee.SignatureOfPM.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //        ms.Position = 0;
            //        bi.BeginInit();
            //        bi.CacheOption = BitmapCacheOption.OnLoad;
            //        bi.StreamSource = ms;
            //        bi.EndInit();
            //    }

            //    employee.SignatureOfPMShownforUI = bi;
            //}
            this.CancelCommand = new RelayCommand(CancelSignature);
            this.GoToTimesheetCommand = new RelayCommand<object>(GoToTimesheet);
            this.CloseCommand = new RelayCommand(this.CloseWindow);
            this.SelectPMSignature = new RelayCommand(this.GoFindPMSignature);
        }

        private void CancelSignature()
        {
            SelectedEmployee.SignatureOfPM = null;
            SelectedEmployee.baseemployee.PMSignature = null;
        }


        private void GoFindPMSignature()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"W:\Documnts\Seals & Signatures\Signatures",
                Title = "Browse for Signatures",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "jpg",
                Filter = "jpg files (*.jpg)|*.jpg",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(openFileDialog1.FileName, UriKind.RelativeOrAbsolute);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                SelectedEmployee.SignatureOfPM = src;
            }
        }

        public void GoToTimesheet(object o)
        {
            TimesheetSubmissionModel tsm = (TimesheetSubmissionModel)o;
            BaseAI CurrentPage = IoCPortal.Application as BaseAI;
            PortalAI portAI = (PortalAI)CurrentPage;
            portAI.GoToTimesheetByDate(tsm.Date);

            DialogHost.Close("RootDialog");
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
