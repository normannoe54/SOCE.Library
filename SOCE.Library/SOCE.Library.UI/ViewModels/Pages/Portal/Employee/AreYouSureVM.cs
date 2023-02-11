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
    public class AreYouSureVM : BaseVM
    {
        public bool Result { get; set; }

        private ProjectSummaryVM ProjectSummary { get; set; }

        public string TopLine { get; set; } = "Are you sure you want to delete:";

        public string BottomLine { get; set; }
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
            TopLine = "Are you sure you want to archive:";
            BottomLine = em.FirstName + " " + em.LastName;
        }

        public AreYouSureVM(ClientModel cm)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            BottomLine = cm.ClientName;
            TopLine = "Are you sure you want to archive:";
        }

        public AreYouSureVM(MarketModel mm)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            BottomLine = mm.MarketName;
        }

        public AreYouSureVM(SubProjectModel spm, ProjectSummaryVM psm)
        {
            ProjectSummary = psm;
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            BottomLine = $"Phase: {spm.PointNumber}";
        }

        public AreYouSureVM(RolePerSubProjectModel rpspm, ProjectSummaryVM psm)
        {
            ProjectSummary = psm;
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            string description = GetEnumDescription((DefaultRoleEnum)rpspm.Role);
            BottomLine = $"Role: {description} {Environment.NewLine}Employee: {rpspm.Employee.FullName}";
        }

        public AreYouSureVM(ProjectModel pm)
        {
            Result = false;
            this.YesCommand = new RelayCommand(this.YesDoTheAction);
            this.CloseCommand = new RelayCommand(this.CancelCommand);
            BottomLine = pm.ProjectName;
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
                                SQLAccess.ArchiveSubProject(sub.Id);
                                ProjectSummary.SubProjects.Remove(sub);
                                ProjectSummary.BaseProject.UpdateSubProjects();
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
