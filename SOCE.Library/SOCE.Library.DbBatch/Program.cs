using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOCE.Library.Db;
using System.Linq;
using System.IO;
using SOCE.Library.Excel;

namespace SOCE.Library.DbBatch
{
    class Program
    {
        static void Main(string[] args)
        {
            //RunProgram();
            //RunBudgetProgram();
            //RunUpdateScheduling();
            //RunUpdateTimesheetData();
            //RunSummary();

            RunBillableHours();
        }

        public static void RunBillableHours()
        {
            List<ProjectDbModel> projects = SQLAccess.LoadProjects();
            List<EmployeeDbModel> employees = SQLAccess.LoadAllEmployees();
            List<ClientDbModel> allclient = SQLAccess.LoadClients();

            DateTime firstdate = new DateTime(2022, 1, 1);
            DateTime lastdate = new DateTime(2023, 12, 31);
            List<EmployeeHours> employeelist = new List<EmployeeHours>();
            foreach (EmployeeDbModel em in employees)
            {
                List<TimesheetRowDbModel> rows =  SQLAccess.LoadTimeSheet(firstdate, lastdate, em.Id);
                double hours = 0;
                double generalhours = 0;
                foreach (TimesheetRowDbModel row in rows)
                {
                    ProjectDbModel dbmod = projects.Where(x => x.Id == row.ProjIdRef).FirstOrDefault();

                    if (dbmod != null)
                    {
                        ClientDbModel client = allclient.Where(x => x.Id == dbmod.ClientId).FirstOrDefault();

                        if (client != null)
                        {
                            if (client.ClientName.ToUpper() != "GENERAL")
                            {
                                hours += row.TimeEntry;
                            }
                            else
                            {
                                generalhours += row.TimeEntry;
                            }
                        }
                    }
                }

                EmployeeHours employee = new EmployeeHours() { Hours = hours, GeneralHours = generalhours, Name = em.FullName };
                employeelist.Add(employee);
            }

            string generalpath = "C:\\Users\\nnoe\\Documents\\EmployeeHourSummary.xlsx";
            Excel.Excel exinst = new Excel.Excel();
            int i = 0;
            List<object> row2 = new List<object>() { "Name", "Billable Hours", "Overhead Hours"};
            exinst.WriteRow<object>(i + 1, 1, row2);
            i++;
            foreach (EmployeeHours emhours in employeelist)
            {
                List<object> row = new List<object>() { emhours.Name, emhours.Hours, emhours.GeneralHours };
                exinst.WriteRow<object>(i+1,1, row);
                i++;
            }
            exinst.SaveAs(generalpath);
            exinst.Close();
            //File.Create(generalpath);
        }

        public static void RunSummary()
        {
            List<ProjectDbModel> projects = SQLAccess.LoadProjects();
            List<TimesheetRowDbModel> alltimesheetdata = SQLAccess.LoadAllTimeSheetData();
            List<ClientDbModel> allclient = SQLAccess.LoadClients();
            List<MarketDbModel> allmarket = SQLAccess.LoadMarkets();

            List<ClientFoundInfo> clientinfo = new List<ClientFoundInfo>();
            List<MarketFoundInfo> marketinfo = new List<MarketFoundInfo>();

            foreach (TimesheetRowDbModel time in alltimesheetdata)
            {
                if (time.Date > 20230000 && time.Date < 20240000)
                {
                    time.Invoiced = 0;

                    SubProjectDbModel sub = SQLAccess.LoadSubProjectsById(time.SubProjectId);

                    ProjectDbModel projfound = projects.Where(x => x.Id == sub.ProjectId).FirstOrDefault();

                    if (projfound != null)
                    {
                        ClientDbModel clienta = allclient.Where(x => x.Id == projfound.ClientId).FirstOrDefault();

                        if (clienta != null)
                        {
                            ClientFoundInfo cfi = clientinfo.Where(x => x.client.Id == clienta.Id).FirstOrDefault();

                            if (cfi == null)
                            {
                                clientinfo.Add(new ClientFoundInfo() { client = clienta, Budget = time.BudgetSpent, Hours = time.TimeEntry });
                            }
                            else
                            {
                                cfi.Hours += time.TimeEntry;
                                cfi.Budget += time.BudgetSpent;
                            }
                        }

                        MarketDbModel marketa = allmarket.Where(x => x.Id == projfound.MarketId).FirstOrDefault();

                        if (clienta != null)
                        {
                            MarketFoundInfo mfi = marketinfo.Where(x => x.market.Id == marketa.Id).FirstOrDefault();

                            if (mfi == null)
                            {
                                marketinfo.Add(new MarketFoundInfo() { market = marketa, Budget = time.BudgetSpent, Hours = time.TimeEntry });
                            }
                            else
                            {
                                mfi.Hours += time.TimeEntry;
                                mfi.Budget += time.BudgetSpent;
                            }
                        }

                    }
                }
            }

            // Set a variable to the Documents path.
            string docPath =
              Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "WriteLines.txt")))
            {
                outputFile.WriteLine("Clients");
                clientinfo.OrderBy(x => x.client.ClientNumber);
                foreach (ClientFoundInfo clientline in clientinfo)
                {
                    string line = $"{clientline.client.ClientNumber}  {clientline.client.ClientName}  {clientline.Hours:n} hrs.  ${clientline.Budget:n}";
                    Console.WriteLine(line);
                    outputFile.WriteLine(line);
                }

                outputFile.WriteLine("Markets");
                foreach (MarketFoundInfo marketline in marketinfo)
                {
                    string line = $"{marketline.market.MarketName}  {marketline.Hours:n} hrs.  ${marketline.Budget:n}";
                    Console.WriteLine(line);
                    outputFile.WriteLine(line);
                }
            }

        }

        public class EmployeeHours
        {
            public double Hours;
            public double GeneralHours;
            public string Name;
        }

        //public static void RunUpdateTimesheetData()
        //{
        //    List<ProjectDbModel> projects = SQLAccess.LoadProjects();
        //    List<TimesheetRowDbModel> alltimesheetdata = SQLAccess.LoadAllTimeSheetData();

        //    foreach (TimesheetRowDbModel time in alltimesheetdata)
        //    {
        //        time.Invoiced = 0;

        //        SubProjectDbModel sub = SQLAccess.LoadSubProjectsById(time.SubProjectId);

        //        ProjectDbModel projfound = projects.Where(x => x.Id == sub.ProjectId).FirstOrDefault();

        //        if (projfound != null)
        //        {
        //            time.ProjIdRef = projfound.Id;
        //            SQLAccess.AddTimesheetData(time);
        //            Console.WriteLine($"Updated Timesheet Id: {time.Id}");
        //        }
        //    }
        //}

        //public static void RunUpdateScheduling()
        //{
        //    int id = 0;
        //    DateTime date = DateTime.Now.AddYears(-1);
        //    List<SchedulingDataDbModel> schedulingdata = SQLAccess.LoadSchedulingDataByAboveDate(date);
        //    List<EmployeeDbModel> employeeData = SQLAccess.LoadEmployees();
        //    var grouped = schedulingdata.GroupBy(x => x.PhaseId);

        //    foreach (var schedlist in grouped)
        //    {
        //        SchedulingDataDbModel first = schedlist.First();
        //        SubProjectDbModel subdb = SQLAccess.LoadSubProjectsBySubProject(first.PhaseId);

        //        if (subdb != null)
        //        {

        //            ProjectDbModel projdb = SQLAccess.LoadProjectsById(subdb.ProjectId);

        //            foreach (SchedulingDataDbModel sched in schedlist)
        //            {
        //                EmployeeDbModel em = employeeData.Where(x => x.Id == sched.EmployeeId).FirstOrDefault();

        //                if (em != null)
        //                {
        //                    sched.EmployeeName = em.FullName;
        //                }

        //                sched.ManagerId = projdb.ManagerId;
        //                sched.PhaseName = subdb.PointNumber;
        //                sched.ProjectName = projdb.ProjectName;
        //                sched.ProjectNumber = projdb.ProjectNumber;
        //                SQLAccess.AddSchedulingData(sched);
        //                Console.WriteLine($"{sched.Id}, { sched.PhaseName}, {sched.ProjectName}");
        //            }
        //        }
        //    }
        //}

        //public static void RunBudgetProgram()
        //{
        //    List<EmployeeDbModel> employees = SQLAccess.LoadEmployees();
        //    List<TimesheetRowDbModel> time = SQLAccess.LoadAllTimeSheetData();

        //    foreach (TimesheetRowDbModel trm in time)
        //    {
        //        if (trm.BudgetSpent <= 0)
        //        {
        //            EmployeeDbModel emdb = employees.Where(x => x.Id == trm.EmployeeId).FirstOrDefault();

        //            if (emdb != null)
        //            {
        //                trm.BudgetSpent = emdb.Rate * trm.TimeEntry;
        //                SQLAccess.UpdateTimesheetData(trm);
        //            }
        //        }
        //    }
        //}

        //public static void RunProgram()
        //{
        //    List<ClientSummary> clientsumamry = new List<ClientSummary>();

        //    List<EmployeeDbModel> employees = SQLAccess.LoadEmployees();
        //    List<ProjectDbModel> projects = SQLAccess.LoadProjects();
        //    List<ClientDbModel> clients = SQLAccess.LoadClients();

        //    List<SubProjectDbModel> subs = new List<SubProjectDbModel>();


        //    foreach (ProjectDbModel pdb in projects)
        //    {
        //        List<SubProjectDbModel> sub = SQLAccess.LoadAllSubProjectsByProject(pdb.Id);
        //        subs.AddRange(sub);
        //    }


        //    foreach (EmployeeDbModel em in employees)
        //    {
        //        DateTime newdate = new DateTime(2022, 1, 1);
        //        DateTime enddate = new DateTime(2022, 12, 31);

        //        List<TimesheetRowDbModel> timesheets = SQLAccess.LoadTimeSheet(newdate, enddate, em.Id);

        //        foreach (TimesheetRowDbModel times in timesheets)
        //        {
        //            int subid = times.SubProjectId;

        //            SubProjectDbModel sub = subs.Where(x => x.Id == subid).FirstOrDefault();

        //            ProjectDbModel project = projects.Where(x => x.Id == sub.ProjectId).FirstOrDefault();

        //            ClientDbModel client = clients.Where(x => x.Id == project.ClientId).FirstOrDefault();

        //            ClientSummary check = clientsumamry.Where(x => x.client == client).FirstOrDefault();

        //            if (check == null)
        //            {
        //                clientsumamry.Add(new ClientSummary()
        //                {
        //                    client = client,
        //                    hours = times.TimeEntry,
        //                    budget = times.BudgetSpent,
        //                });
        //            }
        //            else
        //            {
        //                check.hours += times.TimeEntry;
        //                check.budget += times.BudgetSpent;
        //            }
        //        }
        //    }

        //    StringBuilder csv = new StringBuilder();

        //    foreach (ClientSummary clientsum in clientsumamry)
        //    {
        //        string newLine = string.Format($"{clientsum.client.ClientNumber.ToString()}, {clientsum.client.ClientName}, {clientsum.hours}, {clientsum.budget}");
        //        csv.AppendLine(newLine);
        //    }


        //    string filepath = "T:\\2022summary.csv";
        //    File.WriteAllText(filepath, csv.ToString());
        //}
        //public class ClientSummary
        //{
        //    public ClientDbModel client;

        //    public double hours;

        //    public double budget;

        //    ////client add batch file
        //    //string[] lines = System.IO.File.ReadAllLines(@"T:\PortalImport\ClientUnedited.txt", Encoding.Default);

        //    //foreach (string line in lines)
        //    //{
        //    //    char[] sep = new char[] { '\t' };
        //    //    string[] splitContent = line.Split(sep);

        //    //    ClientDbModel client = new ClientDbModel() { ClientNumber = Convert.ToInt32(splitContent[0]), ClientName = splitContent[1], IsActive = 1 };
        //    //    SQLAccess.AddClient(client);
        //    //    Console.WriteLine("\t" + line);
        //    //}


        //    //string[] employeelines = System.IO.File.ReadAllLines(@"T:\PortalImport\Employees.txt", Encoding.Default);
        //    //int count1 = 0;
        //    //foreach (string line in employeelines)
        //    //{
        //    //    if (count1 > 0)
        //    //    {
        //    //        string sep = "\t";
        //    //        string[] splitContent = line.Split(sep.ToCharArray());

        //    //        EmployeeDbModel employee = new EmployeeDbModel()
        //    //        {
        //    //            FirstName = splitContent[0],
        //    //            LastName = splitContent[1],
        //    //            Title = splitContent[2],
        //    //            Extension = splitContent[3],
        //    //            AuthId = Convert.ToInt32(splitContent[4]),
        //    //            Rate = Convert.ToDouble(splitContent[5]),
        //    //            DefaultRoleId = Convert.ToInt32(splitContent[6]),
        //    //            Email = splitContent[7],
        //    //            PTORate = Convert.ToDouble(splitContent[8]),
        //    //            SickRate = Convert.ToDouble(splitContent[9]),
        //    //            Password = "password",
        //    //            PTOCarryover = 0,
        //    //            IsActive = 1
        //    //        };
        //    //        SQLAccess.AddEmployee(employee);
        //    //        Console.WriteLine("\t" + line);
        //    //    }
        //    //    count1++;
        //    //}

        //    //string[] projectlines = System.IO.File.ReadAllLines(@"T:\PortalImport\ProjectsNew.txt", Encoding.Default);
        //    //int count2 = 0;

        //    //List<ClientDbModel> clients = SQLAccess.LoadClients();
        //    //List<EmployeeDbModel> pms = SQLAccess.LoadProjectManagers();

        //    //foreach (string line in projectlines)
        //    //{
        //    //    if (count2 > 0)
        //    //    {
        //    //        string sep = "\t";
        //    //        string[] splitContent = line.Split(sep.ToCharArray());

        //    //        int clientnum = Convert.ToInt32(splitContent[2]);

        //    //        ClientDbModel client = clients.Where(x => x.ClientNumber == clientnum).FirstOrDefault();
        //    //        EmployeeDbModel pm = pms.Where(x => x.FirstName == splitContent[3].Trim()).FirstOrDefault();

        //    //        if (client != null && pm != null)
        //    //        {
        //    //            string s = splitContent[0].Replace("\"", "");

        //    //            string startdate = "0";

        //    //            if (splitContent[1].Length > 2)
        //    //            {
        //    //                startdate = splitContent[1].Substring(0, 2);
        //    //            }

        //    //            int intdate = 2000 + Convert.ToInt32(startdate);

        //    //            DateTime dateconvert = new DateTime(intdate, 1, 1);

        //    //            ProjectDbModel project = new ProjectDbModel()
        //    //            {
        //    //                ProjectName = s,
        //    //                ProjectNumber = Convert.ToInt32(splitContent[1]),
        //    //                ClientId = client.Id,
        //    //                ManagerId = pm.Id,
        //    //                MarketId = 1,
        //    //                Fee = 2000,
        //    //                PercentComplete = 0,
        //    //                ProjectStart = (int)long.Parse(dateconvert.ToString("yyyyMMdd")),
        //    //                IsActive = 1,
        //    //                IsCurrActive = 1
        //    //            };

        //    //            int projectnumber = SQLAccess.AddProject(project);

        //    //            if (splitContent[0].ToUpper().Contains("General"))
        //    //            {
        //    //                SubProjectDbModel miscsub = new SubProjectDbModel()
        //    //                {
        //    //                    ProjectId = projectnumber,
        //    //                    PercentComplete = 0,
        //    //                    PointNumber = "MISC",
        //    //                    IsActive = 1,
        //    //                    IsCurrActive = 1,
        //    //                    Description = "Miscellaneous",
        //    //                    Fee = 0,
        //    //                    PercentBudget = 100,
        //    //                    IsInvoiced = 0,
        //    //                };

        //    //                SQLAccess.AddSubProject(miscsub);

        //    //            }
        //    //            else
        //    //            {
        //    //                SubProjectDbModel CDsub = new SubProjectDbModel()
        //    //                {
        //    //                    ProjectId = projectnumber,
        //    //                    PercentComplete = 0,
        //    //                    PointNumber = "CD",
        //    //                    IsActive = 1,
        //    //                    IsCurrActive = 1,
        //    //                    Description = "Construction Documents",
        //    //                    Fee = 1000,
        //    //                    PercentBudget = 50,
        //    //                    IsInvoiced = 0,
        //    //                };

        //    //                SubProjectDbModel CAsub = new SubProjectDbModel()
        //    //                {
        //    //                    ProjectId = projectnumber,
        //    //                    PercentComplete = 0,
        //    //                    PointNumber = "CA",
        //    //                    IsActive = 1,
        //    //                    IsCurrActive = 1,
        //    //                    Description = "Construction Administration",
        //    //                    Fee = 1000,
        //    //                    PercentBudget = 50,
        //    //                    IsInvoiced = 0,
        //    //                };

        //    //                SQLAccess.AddSubProject(CDsub);
        //    //                SQLAccess.AddSubProject(CAsub);
        //    //            }






        //    //            Console.WriteLine("\t" + line);
        //    //        }






        //    //    }
        //    //    count2++;
        //    //}



        //}
    }
}
