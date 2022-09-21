using System;
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
                var output = cnn.Query<MarketDbModel>("SELECT * FROM Markets", new DynamicParameters());
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
                cnn.Execute("INSERT INTO Markets (MarketName) VALUES (@MarketName)", market);
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
                var output = cnn.Query<ClientDbModel>("SELECT * FROM Clients", new DynamicParameters());
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
                cnn.Execute("INSERT INTO Employees (ClientName, ClientNumber)" +
                    "VALUES (@ClientName, @ClientNumber)", client);
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
                cnn.Execute("UPDATE Clients SET ClientName = @ClientName, ClientNumber = @ClientNumber WHERE Id = @Id", new { client.ClientName, client.ClientNumber, client.Id });
            }
        }
        #endregion

        #region Employees
        public static List<EmployeeDbModel> LoadEmployees()
        {
            using(IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<EmployeeDbModel>("SELECT * FROM Employees", new DynamicParameters());
                return output.ToList();
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
                cnn.Execute("INSERT INTO Employees (FirstName, LastName, AuthId, Title, Email, PhoneNumber, Extension,PTOHours, SickHours, HolidayHours, Rate)" +
                    "VALUES (@FirstName, @LastName, @AuthId, @Title, @Email, @PhoneNumber, @Extension, @PTOHours, @SickHours, @HolidayHours, @Rate)", employee);
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

        public static void UpdateEmployee(EmployeeDbModel employee)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Employees SET FirstName = @FirstName, LastName = @LastName, Title = @Title, AuthId = @AuthId, Email = @Email, PhoneNumber = @PhoneNumber, Extension = @Extension, Rate = @Rate, PTOHours = @PTOHours, HolidayHours = @HolidayHours, SickHours = @SickHours WHERE Id = @Id",
                        new { employee.FirstName, employee.LastName, employee.Title, employee.AuthId, employee.Email, employee.PhoneNumber, employee.Extension, employee.Rate,employee.PTOHours, employee.HolidayHours, employee.SickHours, employee.Id, });
            }
        }
        #endregion

        #region Projects

        public static void UpdateProjects(ProjectDbModel project)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Projects SET ProjectName = @ProjectName, ProjectNumber = @ProjectNumber, ClientId = @ClientId, Fee = @Fee, MarketId = @MarketId, ManagerId = @ManagerId, IsActive = @IsActive, PercentComplete = @PercentComplete WHERE Id = @Id",
                        new { project.ProjectName, project.ProjectNumber, project.ClientId, project.Fee, project.MarketId, project.ManagerId, project.IsActive, project.PercentComplete, project.Id});
            }
        }

        public static List<ProjectDbModel> LoadProjects()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ProjectDbModel>("SELECT * FROM Projects", new DynamicParameters());
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

        public static void AddProject(ProjectDbModel project)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("INSERT INTO Projects (ProjectName, ProjectNumber, ClientId, Fee, MarketId, ManagerId, IsActive, PercentComplete)" +
                    "VALUES (@ProjectName, @ProjectNumber, @ClientId, @Fee, @MarketId, @ManagerId, @IsActive, @PercentComplete)", project);
            }
        }
        #endregion

        #region Subprojects
        public static List<SubProjectDbModel> LoadSubProjectsByProject(int projectId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<SubProjectDbModel>("SELECT * FROM SubProjects WHERE ProjectId = @projectId", new { projectId});
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

        public static void AddSubProject(SubProjectDbModel subproject)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                //cnn.Open();
                //using (var command = cnn.CreateCommand())
                //{
                //    command.CommandText = "INSERT INTO SubProjects (ProjectId, PointNumber, Description, Fee)" +
                //                         "VALUES (@ProjectId, @PointNumber, @Description, @Fee)";
                //    command.Parameters.Add(new SQLiteParameter("@ProjectId", subproject.ProjectId));
                //    command.Parameters.Add(new SQLiteParameter("@PointNumber", subproject.PointNumber));
                //    command.Parameters.Add(new SQLiteParameter("@Description", subproject.Description));
                //    command.Parameters.Add(new SQLiteParameter("@Fee", subproject.Fee));
                //    command.ExecuteNonQuery();
                //}
                cnn.Execute("INSERT INTO SubProjects (ProjectId, PointNumber, Description, Fee)" +
                    "VALUES (@ProjectId, @PointNumber, @Description, @Fee)", subproject);
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
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<TimesheetSubmissionDbModel>("SELECT * FROM SubmittedTimesheets WHERE EmployeeId = @employeeId"
                    , new { employeeId });

                return output.ToList();
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
    }
}
