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
        public SQLiteConnection connection { get; set; }
        public static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        public void Open()
        {
            connection.Open();
        }

        public void Close()
        {
            connection.Close();
        }

        public SQLAccess()
        {
            connection = new SQLiteConnection(LoadConnectionString());
        }

        public List<ProjectFormatResultDb> LoadProjectSummaryByPhaseIds(List<int> subids)
        {
            List<ProjectFormatResultDb> results = new List<ProjectFormatResultDb>();
            //SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString());
            //using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            //{
            foreach (int subid in subids)
            {
                ProjectFormatResultDb singleform = new ProjectFormatResultDb();
                var output = connection.Query<SubProjectDbModel>("SELECT * FROM SubProjects WHERE Id = @subid", new { subid });
                SubProjectDbModel singleoutput = output.FirstOrDefault();

                if (singleoutput != null)
                {
                    singleform.SelectedSubProject = (SubProjectDbModel)singleoutput;
                    int projectId = singleoutput.ProjectId;
                    var projoutput = connection.Query<ProjectDbModel>("SELECT * FROM Projects WHERE Id = @projectId", new { projectId });
                    ProjectDbModel singleprojoutput = projoutput.FirstOrDefault();

                    singleform.Project = singleprojoutput;

                    var outputlist = connection.Query<SubProjectDbModel>("SELECT * FROM SubProjects WHERE ProjectId = @projectId", new { projectId });
                    List<SubProjectDbModel> ListofSubs = outputlist.ToList();

                    singleform.SubProjects = ListofSubs;

                    results.Add(singleform);
                }
                else
                {
                    break;
                }
            }

            //var output = cnn.Query<ProjectDbModel>("SELECT * FROM Projects", new DynamicParameters());

            ////var output = cnn.Query<ProjectDbModel>("SELECT * FROM Projects WHERE IsCurrActive = 1", new DynamicParameters());
            //return output.ToList();
            //}
            return results;
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

        public static int AddMarket(MarketDbModel market)
        {
            int output = 0;
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    int id = cnn.QuerySingle<int>("INSERT INTO Markets (MarketName, IsActive) VALUES (@MarketName, @IsActive) returning id", market);

                    output = id;
                }
                catch
                {
                }
            }
            return output;
        }

        public static void DeleteMarket(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Execute("DELETE FROM Markets WHERE Id = @id", new { id });
            }
        }

        //public static void ArchiveMarket(int id)
        //{
        //    //check if date and subproject already exist
        //    using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
        //    {
        //        var output = cnn.Execute("UPDATE Markets SET IsActive = 0 WHERE Id = @id"
        //            , new { id });
        //    }
        //}

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

        public static int AddClient(ClientDbModel client)
        {
            int output = 0;
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    int id = cnn.QuerySingle<int>("INSERT INTO Clients (ClientName, ClientNumber, NameOfClient, ClientAddress, ClientCity, IsActive)" +
                    "VALUES (@ClientName, @ClientNumber, @NameOfClient, @ClientAddress, @ClientCity, @IsActive) returning id", client);

                    output = id;
                }
                catch
                {
                }
            }
            return output;
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
                cnn.Execute("UPDATE Clients SET ClientName = @ClientName, ClientNumber = @ClientNumber, NameOfClient = @NameOfClient, ClientAddress = @ClientAddress, ClientCity = @ClientCity WHERE Id = @Id", new { client.ClientName, client.ClientNumber, client.NameOfClient, client.ClientAddress, client.ClientCity, client.Id });
            }
        }
        #endregion

        #region Invoices
        public static List<InvoicingModelDb> LoadInvoices(int projectId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<InvoicingModelDb>("SELECT * FROM Invoicing WHERE ProjectId = @projectId", new { projectId });
                return output.ToList();
            }
        }

        public static int AddInvoice(InvoicingModelDb invoice)
        {
            int output = 0;
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    int id = cnn.QuerySingle<int>("INSERT INTO Invoicing (ProjectId, Date, InvoiceNumber, PreviousSpent, AmountDue, ClientName, ClientCompany, ClientAddress, ClientCity, EmployeeSignedId, TimesheetIds)" +
                     " VALUES (@ProjectId, @Date, @InvoiceNumber, @PreviousSpent, @AmountDue, @ClientName, @ClientCompany, @ClientAddress, @ClientCity, @EmployeeSignedId, @TimesheetIds) returning id;", invoice);
                    output = id;
                }
                catch
                {
                }
            }
            return output;
        }

        public static void DeleteInvoice(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Execute("DELETE FROM Invoicing WHERE Id = @id"
                    , new { id });
            }
        }
        #endregion

        #region InvoiceRows
        public static List<InvoicingRowsDb> LoadInvoiceRows(int InvoiceId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<InvoicingRowsDb>("SELECT * FROM InvoicingRows WHERE InvoiceId = @InvoiceId", new { InvoiceId });
                return output.ToList();
            }
        }

        public static InvoicingRowsDb LoadInvoiceRowsByInvoiceAndSubId(int SubId, int InvoiceId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<InvoicingRowsDb>("SELECT * FROM InvoicingRows WHERE InvoiceId = @InvoiceId AND SubId = @SubId", new { InvoiceId, SubId });
                return output.FirstOrDefault() ;
            }
        }

        public static int AddInvoiceRow(InvoicingRowsDb invoice)
        {
            int output = 0;
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    int id = cnn.QuerySingle<int>("INSERT INTO InvoicingRows (InvoiceId, SubId, PercentComplete, PreviousInvoiced, ThisPeriodInvoiced, ScopeName)" +
                     " VALUES (@InvoiceId, @SubId, @PercentComplete, @PreviousInvoiced, @ThisPeriodInvoiced, @ScopeName) returning id;", invoice);
                    output = id;
                }
                catch
                {
                }
            }
            return output;
        }

        public static void DeleteInvoiceRows(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Execute("DELETE FROM InvoicingRows WHERE Id = @id"
                    , new { id });
            }
        }
        #endregion

        #region Employees
        public static List<EmployeeDbModel> LoadEmployees()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<EmployeeDbModel>("SELECT * FROM Employees WHERE IsActive = 1", new DynamicParameters());
                return output.ToList();
            }
        }

        //public static List<EmployeeDbModel> LoadAllEmployees()
        //{
        //    using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
        //    {
        //        var output = cnn.Query<EmployeeDbModel>("SELECT * FROM Employees", new DynamicParameters());
        //        return output.ToList();
        //    }
        //}

        public static EmployeeDbModel LoadEmployeeByUserandPassword(string email, string password)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<EmployeeDbModel>("SELECT * FROM Employees WHERE Email = @email AND Password = @password AND IsActive = 1", new { email, password });
                return output.FirstOrDefault();
            }
        }

        public static EmployeeDbModel LoadEmployeeByUser(string email)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<EmployeeDbModel>("SELECT * FROM Employees WHERE Email = @email AND IsActive = 1", new { email });
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
                cnn.Execute("INSERT INTO Employees (FirstName, LastName, AuthId, Title, Email, Password, PhoneNumber, Extension, PTORate, PTOCarryover, SickRate, HolidayHours, Rate, StartDate, IsActive, DefaultRoleId, MondayHours, TuesdayHours, WednesdayHours, ThursdayHours, FridayHours, PMSignature)" +
                    "VALUES (@FirstName, @LastName, @AuthId, @Title, @Email, @Password, @PhoneNumber, @Extension, @PTORate, @PTOCarryover, @SickRate, @HolidayHours, @Rate, @StartDate, @IsActive, @DefaultRoleId, @MondayHours, @TuesdayHours, @WednesdayHours, @ThursdayHours, @FridayHours, @PMSignature)", employee);
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
                    , new { id });
            }
        }

        public static List<EmployeeDbModel> LoadProjectManagers()
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<EmployeeDbModel>("SELECT * FROM Employees WHERE AuthId > 1 AND IsActive = 1");
                return output.ToList();
            }
        }

        public static void UpdateEmployee(EmployeeDbModel employee)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Employees SET FirstName = @FirstName, LastName = @LastName, Title = @Title, AuthId = @AuthId, DefaultRoleId = @DefaultRoleId, Email = @Email, PhoneNumber = @PhoneNumber, Extension = @Extension, Rate = @Rate," +
                    " PTORate = @PTORate, PTOCarryover = @PTOCarryover, HolidayHours = @HolidayHours, SickRate = @SickRate, StartDate = @StartDate, IsActive = @IsActive, PMSignature = @PMSignature," +
                    " MondayHours = @MondayHours, TuesdayHours = @TuesdayHours, WednesdayHours = @WednesdayHours, ThursdayHours = @ThursdayHours, FridayHours = @FridayHours WHERE Id = @Id",
                        new
                        {
                            employee.FirstName,
                            employee.LastName,
                            employee.Title,
                            employee.AuthId,
                            employee.DefaultRoleId,
                            employee.Email,
                            employee.PhoneNumber,
                            employee.Extension,
                            employee.Rate,
                            employee.PTORate,
                            employee.PTOCarryover,
                            employee.HolidayHours,
                            employee.SickRate,
                            employee.StartDate,
                            employee.IsActive,
                            employee.PMSignature,
                            employee.MondayHours,
                            employee.TuesdayHours,
                            employee.WednesdayHours,
                            employee.ThursdayHours,
                            employee.FridayHours,
                            employee.Id
                        });

                //cnn.Execute("UPDATE Employees SET FirstName = @FirstName, LastName = @LastName, Title = @Title, AuthId = @AuthId, DefaultRoleId = @DefaultRoleId WHERE Id = @Id",
                //        new { employee.FirstName,employee.LastName, employee.Title, employee.AuthId, employee.DefaultRoleId, employee.Id});
            }
        }

        public static void UpdatePassword(EmployeeDbModel employee)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Employees SET Password = @Password WHERE Id = @Id", new { employee.Password, employee.Id });
            }
        }

        public static void UpdateScheduleWeek(int employeeId, int scheduleweekcheck)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Employees SET ScheduleWeekCheck = @scheduleweekcheck WHERE Id = @employeeId", new { scheduleweekcheck, employeeId });
            }
        }
        #endregion

        #region Projects
        public static void UpdateProjectMain(ProjectDbModel project)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Projects SET ProjectName = @ProjectName, ProjectNumber = @ProjectNumber, ClientId = @ClientId, MarketId = @MarketId,"
                          + "ManagerId = @ManagerId, DueDate = @DueDate, IsActive = @IsActive, PercentComplete = @PercentComplete, Projectfolder = @Projectfolder, Drawingsfolder = @Drawingsfolder, Architectfolder = @Architectfolder, Plotfolder = @Plotfolder," +
                          "ProjectStart = @ProjectStart, ProjectEnd = @ProjectEnd WHERE Id = @Id",
                        new
                        {
                            project.ProjectName,
                            project.ProjectNumber,
                            project.ClientId,
                            project.MarketId,
                            project.ManagerId,
                            project.DueDate,
                            project.IsActive,
                            project.PercentComplete,
                            project.Projectfolder,
                            project.Drawingsfolder,
                            project.Architectfolder,
                            project.Plotfolder,
                            project.ProjectStart,
                            project.ProjectEnd,
                            project.Id
                        });
            }
        }

        public static void UpdateProjects(ProjectDbModel project)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Projects SET ProjectName = @ProjectName, ProjectNumber = @ProjectNumber, ClientId = @ClientId, Fee = @Fee, MarketId = @MarketId,"
                          + "ManagerId = @ManagerId, DueDate = @DueDate, IsActive = @IsActive, PercentComplete = @PercentComplete, Projectfolder = @Projectfolder, Drawingsfolder = @Drawingsfolder,Architectfolder = @Architectfolder,Plotfolder = @Plotfolder," +
                          "ProjectStart = @ProjectStart, ProjectEnd = @ProjectEnd, FinalSpent = @FinalSpent, MiscName = @MiscName, AdserviceFile = @AdserviceFile, Remarks = @Remarks, IsOnHold = @IsOnHold WHERE Id = @Id",
                        new
                        {
                            project.ProjectName,
                            project.ProjectNumber,
                            project.ClientId,
                            project.Fee,
                            project.MarketId,
                            project.ManagerId,
                            project.DueDate,
                            project.IsActive,
                            project.PercentComplete,
                            project.Projectfolder,
                            project.Drawingsfolder,
                            project.Architectfolder,
                            project.Plotfolder,
                            project.ProjectStart,
                            project.ProjectEnd,
                            project.FinalSpent,
                            project.MiscName,
                            project.AdserviceFile,
                            project.Remarks,
                            project.IsOnHold,
                            project.Id
                        });
            }
        }

        //public static void ArchiveProject(int id)
        //{
        //    //check if date and subproject already exist
        //    using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
        //    {
        //        var output = cnn.Execute("UPDATE Projects SET IsCurrActive = 0 WHERE Id = @id"
        //            , new { id });
        //    }
        //}

        public static void UpdateProjectList(int id, int pmid, int? duedate, double percentcomplete, int isonhold, string remarks)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Projects SET Remarks = @remarks, ManagerId = @pmid, DueDate = @duedate, PercentComplete = @percentcomplete, IsOnHold = @isonhold WHERE Id = @id",
                        new
                        {
                            remarks,
                            pmid,
                            duedate,
                            percentcomplete,
                            isonhold,
                            id
                        });
            }
        }

        public static void UpdateFee(int id, double fee)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Projects SET Fee = @fee WHERE Id = @id", new { fee, id });
            }
        }

        public static void UpdateOnHoldStatus(int id, int isonhold)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Projects SET IsOnHold = @isonhold WHERE Id = @id", new { isonhold, id });
            }
        }

        public static List<ProjectDbModel> LoadProjects()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ProjectDbModel>("SELECT * FROM Projects", new DynamicParameters());

                //var output = cnn.Query<ProjectDbModel>("SELECT * FROM Projects WHERE IsCurrActive = 1", new DynamicParameters());
                return output.ToList();
            }
        }



        public static List<ProjectDbModel> LoadActiveProjects(bool active)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ProjectDbModel>("SELECT * FROM Projects WHERE IsActive = @active", new { active });
                return output.ToList();
            }
        }

        public static ProjectDbModel LoadProjectsByProjectNumber(int num)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ProjectDbModel>("SELECT * FROM Projects WHERE ProjectNumber = @num", new { num });
                return output.ToList().FirstOrDefault();
            }
        }

        public static List<ProjectDbModel> LoadActiveNonHoldProjects()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ProjectDbModel>("SELECT * FROM Projects WHERE IsActive = 1 AND IsOnHold = 0");
                return output.ToList();
            }
        }

        public static List<ProjectDbModel> LoadInactiveProjectsByYear()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ProjectDbModel>("SELECT * FROM Projects WHERE IsActive = 0 AND ");
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

        public static List<ProjectDbModel> LoadProjectsByOnHoldScheduling()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ProjectDbModel>("SELECT * FROM Projects WHERE IsOnHold = 1 AND IsActive = 1");
                return output.ToList();
            }
        }

        public static int AddProject(ProjectDbModel project)
        {
            int output = 0;
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                //cnn.Execute("INSERT INTO Projects (ProjectName, ProjectNumber, ClientId, Fee, MarketId, ManagerId, IsActive, PercentComplete, Projectfolder,Drawingsfolder,Architectfolder,Plotfolder)" +
                //    " VALUES (@ProjectName, @ProjectNumber, @ClientId, @Fee, @MarketId, @ManagerId, @IsActive, @PercentComplete, @Projectfolder, @Drawingsfolder, @Architectfolder, @Plotfolder)", project);
                try
                {
                    int id = cnn.QuerySingle<int>("INSERT INTO Projects (ProjectName, ProjectNumber, ClientId, Fee, MarketId, ManagerId, DueDate, IsActive, PercentComplete, Projectfolder,Drawingsfolder,Architectfolder,Plotfolder, ProjectStart, ProjectEnd, FinalSpent, MiscName, AdserviceFile, Remarks, IsOnHold)" +
                     " VALUES (@ProjectName, @ProjectNumber, @ClientId, @Fee, @MarketId, @ManagerId, @DueDate, @IsActive, @PercentComplete, @Projectfolder, @Drawingsfolder, @Architectfolder, @Plotfolder, @ProjectStart, @ProjectEnd, @FinalSpent, @MiscName, @AdserviceFile, @Remarks, @IsOnHold) returning id;", project);
                    output = id;
                }
                catch
                {

                }

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

        public static void UpdateActiveProject(int Id, int IsActive)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Projects SET IsActive = @IsActive WHERE Id = @Id",
                        new
                        {
                            IsActive,
                            Id
                        });
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

        //public static void ArchiveSubProject(int id)
        //{
        //    //check if date and subproject already exist
        //    using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
        //    {
        //        var output = cnn.Execute("UPDATE SubProjects SET IsCurrActive = 0 WHERE Id = @id"
        //            , new { id });
        //    }
        //}

        public static SubProjectDbModel LoadSubProjectsById(int subid)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<SubProjectDbModel>("SELECT * FROM SubProjects WHERE Id = @subid", new { subid });
                return output.FirstOrDefault();
            }
        }

        public static List<SubProjectDbModel> LoadSubProjectsByProject(int projectId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<SubProjectDbModel>("SELECT * FROM SubProjects WHERE ProjectId = @projectId", new { projectId });
                return output.ToList();
            }
        }

        public static List<SubProjectDbModel> LoadSubProjectsByActiveScheduling()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<SubProjectDbModel>("SELECT * FROM SubProjects WHERE IsScheduleActive = 1 AND IsActive = 1");
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
                int id = cnn.QuerySingle<int>("INSERT INTO SubProjects (ProjectId, PointNumber, Description, Fee, PercentComplete, PercentBudget, IsActive, IsInvoiced, ExpandedDescription, IsAdservice, NumberOrder, IsScheduleActive, IsBillable, SubStart, SubEnd, NameOfClient, ClientCompanyName, ClientAddress, ClientCity, EmployeeIdSigned, PersonToAddress, IsHourly)" +
                                                              "VALUES (@ProjectId, @PointNumber, @Description, @Fee, @PercentComplete, @PercentBudget, @IsActive, @IsInvoiced, @ExpandedDescription, @IsAdservice, @NumberOrder, @IsScheduleActive, @IsBillable, @SubStart, @SubEnd, @NameOfClient, @ClientCompanyName, @ClientAddress, @ClientCity, @EmployeeIdSigned, @PersonToAddress, @IsHourly) returning id;", subproject);
                output = id;
            }
            return output;
        }

        public static void UpdateSubProjectSummary(SubProjectDbModel subproject)
        {
            if (subproject.Id != 0)
            {
                //check if date and subproject already exist
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute("UPDATE SubProjects SET PointNumber = @PointNumber, Description = @Description, Fee = @Fee, PercentComplete = @PercentComplete, PercentBudget = @PercentBudget,"
                              + "IsActive = @IsActive, IsHourly = @IsHourly, IsBillable = @IsBillable, ExpandedDescription = @ExpandedDescription, IsAdservice = @IsAdservice, NumberOrder = @NumberOrder, IsScheduleActive = @IsScheduleActive WHERE Id = @Id",
                            new
                            {
                                subproject.PointNumber,
                                subproject.Description,
                                subproject.Fee,
                                subproject.PercentComplete,
                                subproject.PercentBudget,
                                subproject.IsActive,
                                subproject.IsHourly,
                                subproject.IsBillable,
                                subproject.ExpandedDescription,
                                subproject.IsAdservice,
                                subproject.NumberOrder,
                                subproject.IsScheduleActive,
                                subproject.Id
                            });
                }
            }
        }

        public static void UpdateSubAddService(SubProjectDbModel subproject)
        {
            if (subproject.Id != 0)
            {
                //check if date and subproject already exist
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute("UPDATE SubProjects SET PointNumber = @PointNumber, Description = @Description, Fee = @Fee, PercentComplete = @PercentComplete, PercentBudget = @PercentBudget,"
                              + "IsActive = @IsActive, IsHourly = @IsHourly, SubStart = @SubStart, SubEnd = @SubEnd, ExpandedDescription = @ExpandedDescription, IsAdservice = @IsAdservice," +
                              "NumberOrder = @NumberOrder, IsScheduleActive = @IsScheduleActive, NameOfClient = @NameOfClient, ClientCompanyName = @ClientCompanyName, ClientAddress = @ClientAddress," +
                              "ClientCity = @ClientCity, EmployeeIdSigned = @EmployeeIdSigned, IsBillable = @IsBillable, PersonToAddress = @PersonToAddress, DateSent = @DateSent WHERE Id = @Id",
                            new
                            {
                                subproject.PointNumber,
                                subproject.Description,
                                subproject.Fee,
                                subproject.PercentComplete,
                                subproject.PercentBudget,
                                subproject.IsActive,
                                subproject.IsHourly,
                                subproject.SubStart,
                                subproject.SubEnd,
                                subproject.ExpandedDescription,
                                subproject.IsAdservice,
                                subproject.NumberOrder,
                                subproject.IsScheduleActive,
                                subproject.NameOfClient,
                                subproject.ClientCompanyName,
                                subproject.ClientAddress,
                                subproject.ClientCity,
                                subproject.EmployeeIdSigned,
                                subproject.IsBillable,
                                subproject.PersonToAddress,
                                subproject.DateSent,
                                subproject.Id
                            });
                }
            }
        }

        public static void UpdateSubProject(SubProjectDbModel subproject)
        {
            if (subproject.Id != 0)
            {
                //check if date and subproject already exist
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute("UPDATE SubProjects SET ProjectId = @ProjectId, PointNumber = @PointNumber, Description = @Description, Fee = @Fee, PercentComplete = @PercentComplete, PercentBudget = @PercentBudget,"
                              + "IsActive = @IsActive, IsInvoiced = @IsInvoiced, ExpandedDescription = @ExpandedDescription, IsAdservice = @IsAdservice, NumberOrder = @NumberOrder, IsScheduleActive = @IsScheduleActive,"
                              + "IsBillable = @IsBillable, SubStart = @SubStart, SubEnd = @SubEnd, NameOfClient = @NameOfClient, ClientCompanyName = @ClientCompanyName, ClientAddress = @ClientAddress, ClientCity = @ClientCity, EmployeeIdSigned = @EmployeeIdSigned, IsHourly = @IsHourly WHERE Id = @Id",
                            new
                            {
                                subproject.ProjectId,
                                subproject.PointNumber,
                                subproject.Description,
                                subproject.Fee,
                                subproject.PercentComplete,
                                subproject.PercentBudget,
                                subproject.IsActive,
                                subproject.IsInvoiced,
                                subproject.ExpandedDescription,
                                subproject.IsAdservice,
                                subproject.NumberOrder,
                                subproject.IsScheduleActive,
                                subproject.IsBillable,
                                subproject.SubStart,
                                subproject.SubEnd,
                                subproject.NameOfClient,
                                subproject.ClientCompanyName,
                                subproject.ClientAddress,
                                subproject.ClientCity,
                                subproject.EmployeeIdSigned,
                                subproject.IsHourly,
                                subproject.Id
                            });
                }
            }
        }

        public static void UpdateActive(int Id, int IsActive)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE SubProjects SET IsActive = @IsActive WHERE Id = @Id",
                        new
                        {
                            IsActive,
                            Id
                        });
            }
        }

        public static void UpdateScheduleActive(int Id, int IsScheduleActive)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE SubProjects SET IsScheduleActive = @IsScheduleActive WHERE Id = @Id",
                        new
                        {
                            IsScheduleActive,
                            Id
                        });
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

        public static void UpdateNumberOrder(int Id, int NumberOrder)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE SubProjects SET NumberOrder = @NumberOrder WHERE Id = @Id",
                        new
                        {
                            NumberOrder,
                            Id
                        });
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
                    , new { employeeId, startdate });

                return output.FirstOrDefault();
            }
        }

        public static List<TimesheetSubmissionDbModel> LoadTimesheetSubmissionByEmployee(int employeeId)
        {
            DateTime now = DateTime.Now;
            DateTime newdate = now.AddYears(-1);
            int stint = (int)long.Parse(newdate.Date.ToString("yyyyMMdd"));
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<TimesheetSubmissionDbModel>("SELECT * FROM SubmittedTimesheets WHERE EmployeeId = @employeeId AND Date > @stint"
                    , new { employeeId, stint });

                return output.ToList();
            }
        }

        public static List<TimesheetSubmissionDbModel> LoadTimesheetSubmissionByEmployeeByYear(int employeeId, int year)
        {
            DateTime now = DateTime.Now;
            DateTime newdate = new DateTime(year, 1, 1);
            DateTime enddate = new DateTime(year, 12, 31);

            int stint = (int)long.Parse(newdate.Date.ToString("yyyyMMdd"));
            int ent = (int)long.Parse(newdate.Date.ToString("yyyyMMdd"));

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<TimesheetSubmissionDbModel>("SELECT * FROM SubmittedTimesheets WHERE EmployeeId = @employeeId AND Date > @stint AND Date < @ent"
                    , new { employeeId, stint });

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
                //List<TimesheetRowDbModel> output = cnn.Query<TimesheetRowDbModel>("SELECT * FROM Timesheets " +
                //    "WHERE EmployeeId = @EmployeeId AND Date = @Date AND SubProjectId = @SubProjectId AND Invoiced = @Invoiced",
                //    new { timesheetrow.EmployeeId, timesheetrow.Date, timesheetrow.SubProjectId, timesheetrow.Invoiced }).ToList();

                List<TimesheetRowDbModel> output = cnn.Query<TimesheetRowDbModel>("SELECT * FROM Timesheets " +
                    "WHERE EmployeeId = @EmployeeId AND Date = @Date AND SubProjectId = @SubProjectId",
                    new { timesheetrow.EmployeeId, timesheetrow.Date, timesheetrow.SubProjectId}).ToList();

                if (output.Count == 0)
                {
                    //add
                    cnn.Execute("INSERT INTO Timesheets (EmployeeId, Date, SubProjectId, TimeEntry, BudgetSpent, Submitted, Approved, ProjIdRef, Invoiced)" +
                    "VALUES (@EmployeeId, @Date, @SubProjectId, @TimeEntry, @BudgetSpent, @Submitted, @Approved, @ProjIdRef, @Invoiced)", timesheetrow);
                }
                else
                {
                    TimesheetRowDbModel founditem = output.FirstOrDefault();
                    int index = founditem.Id;
                    //replace
                    cnn.Execute("UPDATE Timesheets SET TimeEntry = @TimeEntry, BudgetSpent = @BudgetSpent, Submitted = @Submitted, Approved = @Approved, ProjIdRef = @ProjIdRef, Invoiced = @Invoiced WHERE Id = @index",
                        new { timesheetrow.TimeEntry, timesheetrow.BudgetSpent, timesheetrow.Submitted, timesheetrow.Approved, timesheetrow.ProjIdRef, timesheetrow.Invoiced, index });
                }

            }
        }

        public static void UpdateTimesheetData(TimesheetRowDbModel timesheetrow)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Timesheets SET TimeEntry = @TimeEntry, BudgetSpent = @BudgetSpent, Submitted = @Submitted, Approved = @Approved WHERE Id = @Id",
                        new { timesheetrow.TimeEntry, timesheetrow.BudgetSpent, timesheetrow.Submitted, timesheetrow.Approved, timesheetrow.Id });
            }
        }

        public static void UpdateTimesheetDataApproved(TimesheetRowDbModel timesheetrow)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Timesheets SET Approved = @Approved WHERE Id = @Id",
                        new {timesheetrow.Approved, timesheetrow.Id });
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
                    , new { employeeId, stint, eint });

                return output.ToList();
            }
        }

        public static List<TimesheetRowDbModel> LoadTimeSheetBySub(DateTime startdate, DateTime enddate, int employeeId, int subprojectId)
        {
            int stint = (int)long.Parse(startdate.Date.ToString("yyyyMMdd"));
            int eint = (int)long.Parse(enddate.Date.ToString("yyyyMMdd"));

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<TimesheetRowDbModel>("SELECT * FROM Timesheets WHERE EmployeeId = @employeeId AND SubProjectId = @subprojectId AND Date >= @stint AND Date <= @eint"
                    , new { employeeId, subprojectId, stint, eint });

                return output.ToList();
            }
        }

        public static List<TimesheetRowDbModel> LoadAllTimeSheetData()
        {

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<TimesheetRowDbModel>("SELECT * FROM Timesheets");

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

        public static List<TimesheetRowDbModel> LoadTimeSheetDataByUninvoiced(int projectId)
        {

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<TimesheetRowDbModel>("SELECT * FROM Timesheets WHERE ProjIdRef = @projectId AND Invoiced = 0"
                    , new { projectId});

                return output.ToList();
            }
        }

        public static List<TimesheetRowDbModel> LoadTimeSheetDataByIds(int employeeId, int subprojectId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<TimesheetRowDbModel>("SELECT * FROM Timesheets WHERE EmployeeId = @employeeId AND SubProjectId = @subprojectId"
                    , new { employeeId, subprojectId});

                return output.ToList();
            }
        }

        public static List<TimesheetRowDbModel> LoadTimeSheetByDateandSub(DateTime startdate, DateTime enddate, int subprojectId)
        {
            int stint = (int)long.Parse(startdate.Date.ToString("yyyyMMdd"));
            int eint = (int)long.Parse(enddate.Date.ToString("yyyyMMdd"));

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<TimesheetRowDbModel>("SELECT * FROM Timesheets WHERE SubProjectId = @subprojectId AND Date >= @stint AND Date <= @eint"
                    , new {subprojectId, stint, eint});

                return output.ToList();
            }
        }

        public static List<TimesheetRowDbModel> LoadTimeSheetDatabySubId(int subprojectId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<TimesheetRowDbModel>("SELECT * FROM Timesheets WHERE SubProjectId = @subprojectId"
                    , new { subprojectId });

                return output.ToList();
            }
        }

        public static TimesheetRowDbModel LoadTimeSheetDatabyId(int timeid)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<TimesheetRowDbModel>("SELECT * FROM Timesheets WHERE Id = @timeid"
                    , new { timeid });

                return output.FirstOrDefault();
            }
        }

        public static List<TimesheetRowDbModel> LoadUninvoicedTimeSheetDatabySubIdAndDate(int subprojectId, DateTime date)
        {
            int stint = (int)long.Parse(date.Date.ToString("yyyyMMdd"));
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {

                var output = cnn.Query<TimesheetRowDbModel>("SELECT * FROM Timesheets WHERE SubProjectId = @subprojectId AND Date <= @stint AND Invoiced = 0"
                    , new { subprojectId, stint });

                return output.ToList();
            }
        }

        public static List<TimesheetRowDbModel> LoadUninvoicedTimeSheetDatabySub(int subid)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {

                var output = cnn.Query<TimesheetRowDbModel>("SELECT * FROM Timesheets WHERE SubProjectId = @subid AND Invoiced = 0"
                    , new { subid});

                return output.ToList();
            }
        }

        public static List<TimesheetRowDbModel> LoadTimeSheetDatabyProjId(int projid)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<TimesheetRowDbModel>("SELECT * FROM Timesheets WHERE ProjIdRef = @projid"
                    , new { projid });

                return output.ToList();
            }
        }

        public static void UpdateInvoiceStatusTime(int id, int invoiced)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Timesheets SET Invoiced = @invoiced WHERE Id = @id", new { invoiced, id });
            }
        }
        #endregion

        #region RolesPerProject
        public static int AddRolesPerSubProject(RolePerSubProjectDbModel rolepersubproject)
        {
            int val = 0;
            //check if employee and project already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                List<RolePerSubProjectDbModel> output = cnn.Query<RolePerSubProjectDbModel>("SELECT * FROM RolePerSubProject " +
                    "WHERE EmployeeId = @EmployeeId AND SubProjectId = @SubProjectId",
                    new { rolepersubproject.EmployeeId, rolepersubproject.SubProjectId, rolepersubproject.Role }).ToList();

                //you dont want to update cuz that screws stuff up
                if (output.Count == 0)
                {
                    //add
                    int id = cnn.QuerySingle<int>("INSERT INTO RolePerSubProject (EmployeeId, SubProjectId, Rate, Role, BudgetHours)" +
                    "VALUES (@EmployeeId, @SubProjectId, @Rate, @Role, @BudgetHours) returning id;", rolepersubproject);
                    val = id;
                }

            }

            return val;
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
                cnn.Execute("UPDATE RolePerSubProject SET EmployeeId = @EmployeeId, SubProjectId = @SubProjectId, Rate = @Rate, Role = @Role, BudgetHours = @BudgetHours WHERE Id = @Id",
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

        #region SchedulingData

        public static void AddSchedulingData(SchedulingDataDbModel schedulingdata)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                List<SchedulingDataDbModel> output = cnn.Query<SchedulingDataDbModel>("SELECT * FROM SchedulingData " +
                    "WHERE PhaseId = @PhaseId AND Date = @Date AND EmployeeId = @EmployeeId",
                    new { schedulingdata.PhaseId, schedulingdata.Date, schedulingdata.EmployeeId }).ToList();

                if (output.Count == 0)
                {
                    //add
                    cnn.Execute("INSERT INTO SchedulingData (PhaseId, EmployeeId, Date, PhaseName, ProjectName, EmployeeName, ManagerId, ProjectNumber," +
                        " Hours1, Hours2, Hours3, Hours4, Hours5, Hours6, Hours7, Hours8)" +
                    "VALUES (@PhaseId, @EmployeeId, @Date, @PhaseName, @ProjectName, @EmployeeName, @ManagerId, @ProjectNumber, @Hours1, @Hours2, @Hours3," +
                    " @Hours4, @Hours5, @Hours6, @Hours7, @Hours8)", schedulingdata);
                }
                else
                {
                    SchedulingDataDbModel founditem = output.FirstOrDefault();
                    int index = founditem.Id;
                    //replace
                    cnn.Execute("UPDATE SchedulingData SET Hours1 = @Hours1, Hours2 = @Hours2, Hours3 = @Hours3," +
                        " Hours4 = @Hours4, Hours5 = @Hours5, Hours6 = @Hours6," +
                        " Hours7 = @Hours7, Hours8 = @Hours8, PhaseName = @PhaseName, ProjectName = @ProjectName," +
                        " EmployeeName = @EmployeeName, ManagerId = @ManagerId, ProjectNumber = @ProjectNumber WHERE Id = @index",
                        new
                        {
                            schedulingdata.Hours1,
                            schedulingdata.Hours2,
                            schedulingdata.Hours3,
                            schedulingdata.Hours4,
                            schedulingdata.Hours5,
                            schedulingdata.Hours6,
                            schedulingdata.Hours7,
                            schedulingdata.Hours8,
                            schedulingdata.PhaseName,
                            schedulingdata.ProjectName,
                            schedulingdata.EmployeeName,
                            schedulingdata.ManagerId,
                            schedulingdata.ProjectNumber,
                            index
                        });
                }

            }
        }

        public static List<SchedulingDataDbModel> LoadSchedulingDataByAboveDate(DateTime date)
        {
            int converteddate = (int)long.Parse(date.Date.ToString("yyyyMMdd"));

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<SchedulingDataDbModel>("SELECT * FROM SchedulingData WHERE Date >= @converteddate"
                    , new { converteddate });

                return output.ToList();
            }
        }

        public static List<SchedulingDataDbModel> LoadSchedulingDataByDate(DateTime date)
        {
            int converteddate = (int)long.Parse(date.Date.ToString("yyyyMMdd"));

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<SchedulingDataDbModel>("SELECT * FROM SchedulingData WHERE Date == @converteddate"
                    , new { converteddate });

                return output.ToList();
            }
        }

        public static List<SchedulingDataDbModel> LoadSchedulingDataByDateandPM(DateTime date, int managerId)
        {
            int converteddate = (int)long.Parse(date.Date.ToString("yyyyMMdd"));

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<SchedulingDataDbModel>("SELECT * FROM SchedulingData WHERE Date == @converteddate AND ManagerId == @managerId"
                    , new { converteddate, managerId });

                return output.ToList();
            }
        }

        public static List<SchedulingDataDbModel> LoadSchedulingData(DateTime date, int phaseid)
        {
            int converteddate = (int)long.Parse(date.Date.ToString("yyyyMMdd"));

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<SchedulingDataDbModel>("SELECT * FROM SchedulingData WHERE PhaseId = @phaseid AND Date == @converteddate"
                    , new { phaseid, converteddate });

                return output.ToList();
            }
        }

        public static List<SchedulingDataDbModel> LoadSchedulingDataByEmployee(DateTime date, int employeeId)
        {
            int converteddate = (int)long.Parse(date.Date.ToString("yyyyMMdd"));

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<SchedulingDataDbModel>("SELECT * FROM SchedulingData WHERE EmployeeId = @employeeId AND Date == @converteddate"
                    , new { employeeId, converteddate });

                return output.ToList();
            }
        }


        public static SchedulingDataDbModel LoadSingleSchedulingData(int employeeId, int subprojectId, DateTime date)
        {
            int dateint = (int)long.Parse(date.Date.ToString("yyyyMMdd"));

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<SchedulingDataDbModel>("SELECT * FROM SchedulingData WHERE EmployeeId = @employeeId AND PhaseId = @subprojectId AND Date = @dateint"
                    , new { employeeId, subprojectId, dateint });

                return output.FirstOrDefault();
            }
        }

        public static void DeleteSchedulingData(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Execute("DELETE FROM SchedulingData WHERE Id = @id"
                    , new { id });
            }
        }

        #endregion

        #region Proposals
        public static void UpdateProposal(ProposalDbModel proposal)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Proposals SET ProposalName = @ProposalName, Status = @Status, Fee = @Fee, ClientId = @ClientId,"
                          + "MarketId = @MarketId, SenderId = @SenderId, DateSent = @DateSent, CostMetricValue = @CostMetricValue, " +
                          "Remarks = @Remarks, MiscClient = @MiscClient, LinkFolder = @LinkFolder, CostMetric = @CostMetric WHERE Id = @Id",
                        new
                        {
                            proposal.ProposalName,
                            proposal.Status,
                            proposal.Fee,
                            proposal.ClientId,
                            proposal.MarketId,
                            proposal.SenderId,
                            proposal.DateSent,
                            proposal.CostMetricValue,
                            proposal.Remarks,
                            proposal.MiscClient,
                            proposal.LinkFolder,
                            proposal.CostMetric,
                            proposal.Id
                        });
            }
        }


        public static List<ProposalDbModel> LoadProposals()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ProposalDbModel>("SELECT * FROM Proposals", new DynamicParameters());

                //var output = cnn.Query<ProjectDbModel>("SELECT * FROM Projects WHERE IsCurrActive = 1", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<ProposalDbModel> LoadProposalsBydates(DateTime startdate, DateTime enddate)
        {
            int stint = (int)long.Parse(startdate.Date.ToString("yyyyMMdd"));
            int eint = (int)long.Parse(enddate.Date.ToString("yyyyMMdd"));

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {

                var output = cnn.Query<ProposalDbModel>("SELECT * FROM Proposals WHERE DateSent >= @stint AND DateSent <= @eint", new { stint, eint });

                return output.ToList();
            }
        }

        public static int AddProposal(ProposalDbModel proposal)
        {
            int output = 0;
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    int id = cnn.QuerySingle<int>("INSERT INTO Proposals (ProposalName, Status, Fee, ClientId, MarketId, SenderId, DateSent, CostMetricValue, Remarks, MiscClient, LinkFolder, CostMetric)" +
                     " VALUES (@ProposalName, @Status, @Fee, @ClientId, @MarketId, @SenderId, @DateSent, @CostMetricValue, @Remarks, @MiscClient, @LinkFolder, @CostMetric) returning id;", proposal);
                    output = id;
                }
                catch
                {

                }

            }
            return output;
        }

        public static void DeleteProposal(int id)
        {
            //check if date and subproject already exist
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Execute("DELETE FROM Proposals WHERE Id = @id", new { id });
            }
        }
        #endregion
    }
}
