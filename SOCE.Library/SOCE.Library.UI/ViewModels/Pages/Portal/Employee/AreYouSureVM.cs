using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;

namespace SOCE.Library.UI.ViewModels
{
    public class AreYouSureVM : BaseVM
    {
        public bool Result { get; set; }

        public string WordNeeded { get; set; } = "delete: ";
        public string TexttoDisplay { get; set; }
        public ICommand YesCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public AreYouSureVM()
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
        }

        public AreYouSureVM(EmployeeModel em)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            TexttoDisplay = em.FirstName + " " + em.LastName;
        }

        public AreYouSureVM(ClientModel cm)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            TexttoDisplay = cm.ClientName;
        }

        public AreYouSureVM(MarketModel mm)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            TexttoDisplay = mm.MarketName;
        }

        public AreYouSureVM(SubProjectModel spm)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            TexttoDisplay = $"Phase: {spm.PointNumber}";
        }

        public AreYouSureVM(ProjectModel pm)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            TexttoDisplay = pm.ProjectName;
        }

        public void YesDoTheAction()
        {
            Result = true;
            //do stuff
            CloseWindow();
        }

        private void CancelCommand()
        {
            Result = false;
            //do stuff
            CloseWindow();
        }

        private void CloseWindow()
        {
            DialogHost.Close("RootDialog");
        }
    }
}
