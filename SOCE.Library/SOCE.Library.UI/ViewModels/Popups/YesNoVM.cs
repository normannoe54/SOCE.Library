using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SOCE.Library.Db;
using System.Reflection;
using System.ComponentModel;

namespace SOCE.Library.UI.ViewModels
{
    public class YesNoVM : BaseVM
    {
        public bool Result { get; set; }

        private ProjectSummaryVM ProjectSummary { get; set; }

        public string Message { get; set; } = "";

        private bool _isSubVisible = false;
        public bool IsSubVisible
        {
            get
            {
                return _isSubVisible;
            }
            set
            {
                _isSubVisible = value;
                RaisePropertyChanged(nameof(IsSubVisible));
            }
        }

        private string _subMessage = "";
        public string SubMessage
        {
            get
            {
                return _subMessage;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    IsSubVisible = true;
                }
                _subMessage = value;
                RaisePropertyChanged(nameof(SubMessage));
            }
        }

        public ICommand YesCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public YesNoVM()
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
        }

        public YesNoVM(EmployeeModel em)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            Message = "Are you sure you want to delete:";
            SubMessage = em.FirstName + " " + em.LastName;
        }

        public YesNoVM(ClientModel cm)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            Message = "Are you sure you want to delete:";
            SubMessage = cm.ClientName;
        }

        public YesNoVM(MarketModel mm)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            Message = "Are you sure you want to delete:";
            SubMessage = mm.MarketName;
        }

        public YesNoVM(SubProjectModel spm, ProjectSummaryVM psm)
        {
            ProjectSummary = psm;
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            Message = "Are you sure you want to delete:";
            SubMessage = $"Phase: {spm.PointNumber}";
        }

        public YesNoVM(RolePerSubProjectModel rpspm, ProjectSummaryVM psm)
        {
            ProjectSummary = psm;
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            string description = GetEnumDescription((DefaultRoleEnum)rpspm.Role);
            Message = "Are you sure you want to delete:";
            SubMessage = $"Role: {description} {Environment.NewLine}Employee: {rpspm.Employee.FullName}";
        }

        public YesNoVM(ProjectModel pm)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            Message = "Are you sure you want to delete:";
            SubMessage = pm.ProjectName;
        }

        private string GetEnumDescription(Enum value)
        {
            // Get the Description attribute value for the enum value
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
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
            if (ProjectSummary == null)
            {
                bool val = DialogHost.IsDialogOpen("RootDialog");
                DialogHost.Close("RootDialog");
            }
            else
            {
                object itemtodelete = ProjectSummary.ItemToDelete;

                if (Result)
                {
                    switch (itemtodelete)
                    {
                        case SubProjectModel sub:
                            {
                                foreach (RolePerSubProjectModel rpspm in sub.RolesPerSub)
                                {
                                    SQLAccess.DeleteRolesPerSubProject(rpspm.Id);
                                }
                                SQLAccess.DeleteSubProject(sub.Id);
                                ProjectSummary.SubProjects.Remove(sub);
                                ProjectSummary.BaseProject.UpdateSubProjects();
                                ProjectSummary.SubProjects.Renumber(true);
                                break;
                            }
                        case RolePerSubProjectModel role:
                            {
                                if (role.SpentHours == 0)
                                {
                                    SQLAccess.DeleteRolesPerSubProject(role.Id);
                                }
                                ProjectSummary.SelectedProjectPhase.RolesPerSub.Remove(role);
                                role.Subproject.baseproject.FormatData(false);

                                break;
                            }
                        default:
                            break;
                    }
                }
                ProjectSummary.ItemToDelete = null;
                ProjectSummary.LeftDrawerOpen = false;
                
            }
        }
    }
}
