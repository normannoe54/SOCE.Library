﻿using System;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;
using System.Linq;

namespace SOCE.Library.Db
{
    public class SQLAccess
    {
        public static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        #region Markets
        public static List<MarketDbModel> LoadMarkets()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<MarketDbModel>("SELECT * FROM Markets WHERE IsActive = 1", new DynamicParameters());
                return output.ToList();
            }
        }

        public static MarketDbModel LoadMarketeById(int marketId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<MarketDbModel>("SELECT * FROM Markets WHERE Id = @marketId", new { marketId });
                return output.FirstOrDefault();
            }
        }

        public static void AddMarket(MarketDbModel market)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("INSERT INTO Markets (MarketName, IsActive) VALUES (@MarketName, @IsActive)", market);
            }
        }

        public static void DeleteMarket(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Execute("DELETE FROM Markets WHERE Id = @id", new { id });
            }
        }

        public static void ArchiveMarket(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Execute("UPDATE Markets SET IsActive = 0 WHERE Id = @id"
                    , new { id });
            }
        }

        public static void UpdateMarket(MarketDbModel market)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Markets SET MarketName = @MarketName WHERE Id = @Id", new { market.MarketName, market.Id });
            }
        }
        #endregion

        #region Clients
        public static List<ClientDbModel> LoadClients()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ClientDbModel>("SELECT * FROM Clients WHERE IsActive = 1", new DynamicParameters());
                return output.ToList();
            }
        }

        public static ClientDbModel LoadClientById(int clientid)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ClientDbModel>("SELECT * FROM Clients WHERE Id = @clientid", new { clientid });
                return output.FirstOrDefault();
            }
        }

        public static void AddClient(ClientDbModel client)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("INSERT INTO Clients (ClientName, ClientNumber, IsActive)" +
                    "VALUES (@ClientName, @ClientNumber, @IsActive)", client);
            }
        }

        public static void ArchiveClient(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Execute("UPDATE Clients SET IsActive = 0 WHERE Id = @id"
                    , new { id });
            }
        }

        public static void DeleteClient(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Execute("DELETE FROM Clients WHERE Id = @id", new { id });
            }
        }

        public static void UpdateClient(ClientDbModel client)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Clients SET ClientName = @ClientName AND ClientNumber = @ClientNumber WHERE Id = @Id", new { client.ClientName, client.Id });
            }
        }
        #endregion

        #region Employees
        public static List<EmployeeDbModel> LoadEmployees()
        {
            using(IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<EmployeeDbModel>("SELECT * FROM Employees WHERE IsActive = 1", new DynamicParameters());
                return output.ToList();
            }
        }

        public static EmployeeDbModel LoadEmployeeByUserandPassword(string email, string password)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<EmployeeDbModel>("SELECT * FROM Employees WHERE Email = @email AND Password = @password", new { email, password });
                return output.FirstOrDefault();
            }
        }

        public static EmployeeDbModel LoadEmployeeByUser(string email)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<EmployeeDbModel>("SELECT * FROM Employees WHERE Email = @email", new { email});
                return output.FirstOrDefault();
            }
        }

        public static EmployeeDbModel LoadEmployeeById(int employeeId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<EmployeeDbModel>("SELECT * FROM Employees WHERE Id = @employeeId", new { employeeId });
                return output.FirstOrDefault();
            }
        }

        public static void AddEmployee(EmployeeDbModel employee)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("INSERT INTO Employees (FirstName, LastName, AuthId, Title, Email, Password, PhoneNumber, Extension, PTORate, PTOCarryover, SickRate, SickCarryover, HolidayHours, Rate, StartDate, IsActive)" +
                    "VALUES (@FirstName, @LastName, @AuthId, @Title, @Email, @Password, @PhoneNumber, @Extension, @PTORate, @PTOCarryover, @SickRate, @SickCarryover, @HolidayHours, @Rate, @StartDate, @IsActive)", employee);
            }
        }

        public static void DeleteEmployee(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Execute("DELETE FROM Employees WHERE Id = @id"
                    , new { id });
            }
        }

        public static void ArchiveEmployee(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Execute("UPDATE Employees SET IsActive = 0 WHERE Id = @id"
                    , new { id});
            }
        }

        public static List<EmployeeDbModel> LoadProjectManagers()
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<EmployeeDbModel>("SELECT * FROM Employees WHERE AuthId > 0");
                return output.ToList();
            }
        }

        public static void UpdateEmployee(EmployeeDbModel employee)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Employees SET FirstName = @FirstName, LastName = @LastName, Title = @Title, AuthId = @AuthId, DefaultRoleId = @DefaultRoleId, Email = @Email, PhoneNumber = @PhoneNumber, Extension = @Extension, Rate = @Rate," +
                    " PTORate = @PTORate, PTOCarryover = @PTOCarryover, HolidayHours = @HolidayHours, SickRate = @SickRate, SickCarryover = @SickCarryover, StartDate = @StartDate, IsActive = @IsActive WHERE Id = @Id",
                        new { employee.FirstName, employee.LastName, employee.Title, employee.AuthId, employee.DefaultRoleId, employee.Email, employee.PhoneNumber, employee.Extension, employee.Rate,
                            employee.PTORate, employee.PTOCarryover, employee.HolidayHours, employee.SickRate, employee.SickCarryover, employee.StartDate, employee.IsActive, employee.Id});
            }
        }

        public static void UpdatePassword(EmployeeDbModel employee)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Employees SET Password = @Password WHERE Id = @Id",new { employee.Password, employee.Id });
            }
        }
        #endregion

        #region Projects

        public static void UpdateProjects(ProjectDbModel project)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Projects SET ProjectName = @ProjectName, ProjectNumber = @ProjectNumber, ClientId = @ClientId, Fee = @Fee, MarketId = @MarketId,"
                          + "ManagerId = @ManagerId, IsActive = @IsActive, PercentComplete = @PercentComplete, Projectfolder = @Projectfolder, Drawingsfolder = @Drawingsfolder,Architectfolder = @Architectfolder,Plotfolder = @Plotfolder," +
                          "ProjectStart = @ProjectStart, ProjectEnd = @ProjectEnd, FinalSpent = @FinalSpent, IsCurrActive = @IsCurrActive, MiscName = @MiscName WHERE Id = @Id",
                        new { project.ProjectName, project.ProjectNumber, project.ClientId, project.Fee, project.MarketId, project.ManagerId, project.IsActive, project.PercentComplete, project.Projectfolder,
                            project.Drawingsfolder, project.Architectfolder, project.Plotfolder, project.ProjectStart, project.ProjectEnd, project.FinalSpent, project.IsCurrActive, project.MiscName, project.Id});
            }
        }

        public static void ArchiveProject(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Execute("UPDATE Projects SET IsCurrActive = 0 WHERE Id = @id"
                    , new { id });
            }
        }

        public static void UpdateFee(int id, double fee)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Projects SET Fee = @fee WHERE Id = @id",new { fee, id });
            }
        }

        public static List<ProjectDbModel> LoadProjects()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ProjectDbModel>("SELECT * FROM Projects WHERE IsCurrActive = 1", new DynamicParameters());
                return output.ToList();
            }
        }

        public static ProjectDbModel LoadProjectsById(int projectId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ProjectDbModel>("SELECT * FROM Projects WHERE Id = @projectId", new { projectId });
                return output.FirstOrDefault();
            }
        }

        public static int AddProject(ProjectDbModel project)
        {
            int output = 0;
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                //cnn.Execute("INSERT INTO Projects (ProjectName, ProjectNumber, ClientId, Fee, MarketId, ManagerId, IsActive, PercentComplete, Projectfolder,Drawingsfolder,Architectfolder,Plotfolder)" +
                //    " VALUES (@ProjectName, @ProjectNumber, @ClientId, @Fee, @MarketId, @ManagerId, @IsActive, @PercentComplete, @Projectfolder, @Drawingsfolder, @Architectfolder, @Plotfolder)", project);

                int id = cnn.QuerySingle<int>("INSERT INTO Projects (ProjectName, ProjectNumber, ClientId, Fee, MarketId, ManagerId, IsActive, PercentComplete, Projectfolder,Drawingsfolder,Architectfolder,Plotfolder, ProjectStart, ProjectEnd, FinalSpent, IsCurrActive, MiscName)" +
                     " VALUES (@ProjectName, @ProjectNumber, @ClientId, @Fee, @MarketId, @ManagerId, @IsActive, @PercentComplete, @Projectfolder, @Drawingsfolder, @Architectfolder, @Plotfolder, @ProjectStart, @ProjectEnd, @FinalSpent, @IsCurrActive, @MiscName) returning id;", project);
                output = id;
            }
            return output;
        }

        public static void DeleteProject(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Execute("DELETE FROM Projects WHERE Id = @id", new { id });
            }
        }
        #endregion

        #region Subprojects
        //public static List<SubProjectDbModel> LoadActiveSubProjectsByProject(int projectId, int isActive)
        //{
        //    using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
        //    {
        //        var output = cnn.Query<SubProjectDbModel>("SELECT * FROM SubProjects WHERE ProjectId = @projectId AND IsActive = @isActive", new { projectId, isActive });
        //        return output.ToList();
        //    }
        //}

        public static void ArchiveSubProject(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Execute("UPDATE SubProjects SET IsCurrActive = 0 WHERE Id = @id"
                    , new { id });
            }
        }

        public static List<SubProjectDbModel> LoadSubProjectsByProject(int projectId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<SubProjectDbModel>("SELECT * FROM SubProjects WHERE ProjectId = @projectId AND IsCurrActive = 1", new { projectId});
                return output.ToList();
            }
        }

        public static List<SubProjectDbModel> LoadAllSubProjectsByProject(int projectId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<SubProjectDbModel>("SELECT * FROM SubProjects WHERE ProjectId = @projectId", new { projectId });
                return output.ToList();
            }
        }

        public static SubProjectDbModel LoadSubProjectsBySubProject(int subprojectId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<SubProjectDbModel>("SELECT * FROM SubProjects WHERE Id = @subprojectId", new { subprojectId });
                return output.FirstOrDefault();
            }
        }

        public static int AddSubProject(SubProjectDbModel subproject)
        {
            int output = 0;
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                int id = cnn.QuerySingle<int>("INSERT INTO SubProjects (ProjectId, PointNumber, Description, Fee, PercentComplete, PercentBudget, IsActive, IsInvoiced, IsCurrActive)" +
                    "VALUES (@ProjectId, @PointNumber, @Description, @Fee, @PercentComplete, @PercentBudget, @IsActive, @IsInvoiced, @IsCurrActive) returning id;", subproject);
                output = id;
            }
            return output;
        }

        public static void UpdateSubProject(SubProjectDbModel subproject)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE SubProjects SET ProjectId = @ProjectId, PointNumber = @PointNumber, Description = @Description, Fee = @Fee, PercentComplete = @PercentComplete, PercentBudget = @PercentBudget,"
                          + "IsActive = @IsActive, IsInvoiced = @IsInvoiced, IsCurrActive = @IsCurrActive WHERE Id = @Id",
                        new { subproject.ProjectId, subproject.PointNumber, subproject.Description, subproject.Fee, subproject.PercentComplete, subproject.PercentBudget, subproject.IsActive, subproject.IsInvoiced, subproject.IsCurrActive, subproject.Id });
            }
        }

        public static void DeleteSubProject(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Execute("DELETE FROM SubProjects WHERE Id = @id", new { id });
            }
        }
        #endregion

        #region TimesheetSubmission
        public static void AddTimesheetSubmissionData(TimesheetSubmissionDbModel timesheetsubmission)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("INSERT INTO SubmittedTimesheets (EmployeeId, Date, TotalHours, PTOHours, OTHours, SickHours, HolidayHours, Approved)" +
                    "VALUES (@EmployeeId, @Date, @TotalHours, @PTOHours, @OTHours, @SickHours, @HolidayHours, @Approved)", timesheetsubmission);
            }
        }

        public static TimesheetSubmissionDbModel LoadTimeSheetSubmissionData(int startdate, int employeeId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<TimesheetSubmissionDbModel>("SELECT * FROM SubmittedTimesheets WHERE EmployeeId = @employeeId AND Date = @startdate"
                    , new { employeeId, startdate});

                return output.FirstOrDefault();
            }
        }

        public static List<TimesheetSubmissionDbModel> LoadTimesheetSubmissionByEmployee(int employeeId)
        {
            int year = DateTime.Now.Year;
            DateTime firstDay = new DateTime(year, 1, 1);
            int stint = (int)long.Parse(firstDay.Date.ToString("yyyyMMdd"));
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<TimesheetSubmissionDbModel>("SELECT * FROM SubmittedTimesheets WHERE EmployeeId = @employeeId AND Date > @stint"
                    , new { employeeId, stint});

                return output.ToList();
            }
        }

        public static void UpdateTimesheetSubmission(TimesheetSubmissionDbModel ts)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE SubmittedTimesheets SET EmployeeId = @EmployeeId, Date = @Date, TotalHours = @TotalHours, PTOHours = @PTOHours, OTHours = @OTHours, SickHours = @SickHours, HolidayHours = @HolidayHours, Approved = @Approved WHERE Id = @Id",
                        new { ts.EmployeeId, ts.Date, ts.TotalHours, ts.PTOHours, ts.OTHours, ts.SickHours, ts.HolidayHours, ts.Approved, ts.Id });
            }
        }

        public static void DeleteTimesheetSubmission(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("DELETE FROM SubmittedTimesheets WHERE Id = @Id",
                        new { id });
            }
        }
        #endregion

        #region Timesheet
        public static void AddTimesheetData(TimesheetRowDbModel timesheetrow)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                List<TimesheetRowDbModel> output = cnn.Query<TimesheetRowDbModel>("SELECT * FROM Timesheets " +
                    "WHERE EmployeeId = @EmployeeId AND Date = @Date AND SubProjectId = @SubProjectId",
                    new { timesheetrow.EmployeeId, timesheetrow.Date, timesheetrow.SubProjectId}).ToList();

                if (output.Count == 0)
                {
                    //add
                    cnn.Execute("INSERT INTO Timesheets (EmployeeId, Date, SubProjectId, TimeEntry, Submitted, Approved)" +
                    "VALUES (@EmployeeId, @Date, @SubProjectId, @TimeEntry, @Submitted, @Approved)", timesheetrow);
                }
                else
                {
                    TimesheetRowDbModel founditem = output.FirstOrDefault();
                    int index = founditem.Id;
                    //replace
                    cnn.Execute("UPDATE Timesheets SET TimeEntry = @TimeEntry, Submitted = @Submitted, Approved = @Approved WHERE Id = @index", 
                        new{ timesheetrow.TimeEntry, timesheetrow.Submitted, timesheetrow.Approved, index });
                }

            }
        }

        public static void UpdateTimesheetData(TimesheetRowDbModel timesheetrow)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Timesheets SET TimeEntry = @TimeEntry, Submitted = @Submitted, Approved = @Approved WHERE Id = @Id",
                        new { timesheetrow.TimeEntry, timesheetrow.Submitted, timesheetrow.Approved, timesheetrow.Id });
            }
        }

        public static void DeleteTimesheetData(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Execute("DELETE FROM Timesheets WHERE Id = @id"
                    , new { id });
            }
        }

        public static List<TimesheetRowDbModel> LoadTimeSheet(DateTime startdate, DateTime enddate, int employeeId)
        {
            int stint = (int)long.Parse(startdate.Date.ToString("yyyyMMdd"));
            int eint = (int)long.Parse(enddate.Date.ToString("yyyyMMdd"));

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<TimesheetRowDbModel>("SELECT * FROM Timesheets WHERE EmployeeId = @employeeId AND Date >= @stint AND Date <= @eint"
                    , new {employeeId, stint, eint});

                return output.ToList();
            }
        }

        public static TimesheetRowDbModel LoadTimeSheetData(int employeeId, int subprojectId, DateTime date)
        {
            int dateint = (int)long.Parse(date.Date.ToString("yyyyMMdd"));

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<TimesheetRowDbModel>("SELECT * FROM Timesheets WHERE EmployeeId = @employeeId AND SubProjectId = @subprojectId AND Date = @dateint"
                    , new { employeeId, subprojectId, dateint });

                return output.FirstOrDefault();
            }
        }

        public static List<TimesheetRowDbModel> LoadTimeSheetDatabySubId(int subprojectId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<TimesheetRowDbModel>("SELECT * FROM Timesheets WHERE SubProjectId = @subprojectId"
                    , new { subprojectId});

                return output.ToList();
            }
        }
        #endregion

        #region RatesPerProject
        public static void AddRolesPerSubProject(RolePerSubProjectDbModel rolepersubproject)
        {
            //check if employee and project already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                List<RolePerSubProjectDbModel> output = cnn.Query<RolePerSubProjectDbModel>("SELECT * FROM RolePerSubProject " +
                    "WHERE EmployeeId = @EmployeeId AND SubProjectId = @SubProjectId AND Role = @Role",
                    new { rolepersubproject.EmployeeId, rolepersubproject.SubProjectId, rolepersubproject.Role}).ToList();

                //you dont want to update cuz that screws stuff up
                if (output.Count == 0)
                {
                    //add
                    cnn.Execute("INSERT INTO RolePerSubProject (EmployeeId, SubProjectId, Rate, Role, BudgetHours)" +
                    "VALUES (@EmployeeId, @SubProjectId, @Rate, @Role, @BudgetHours)", rolepersubproject);
                }
            }
        }

        public static List<RolePerSubProjectDbModel> LoadRolesPerSubProject(int subprojectId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<RolePerSubProjectDbModel>("SELECT * FROM RolePerSubProject WHERE SubProjectId = @subprojectId"
                    , new { subprojectId });

                return output.ToList();
            }
        }

        public static void UpdateRolesPerSubProject(RolePerSubProjectDbModel rpp)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE RolePerSubProject SET EmployeeId = @EmployeeId, SubProjectId = @SubProjectId, Rate = @Rate, Role = @Role, @BudgetHours = BudgetHours WHERE Id = @Id",
                        new { rpp.EmployeeId, rpp.SubProjectId, rpp.Rate, rpp.Role, rpp.BudgetHours, rpp.Id });
            }
        }

        public static void DeleteRolesPerSubProject(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("DELETE FROM RolePerSubProject WHERE Id = @Id", new { id });
            }
        }
        #endregion
    }
}
