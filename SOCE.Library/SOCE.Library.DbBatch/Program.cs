using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOCE.Library.Db;
using System.Linq;
using System.IO;

namespace SOCE.Library.DbBatch
{
    class Program
    {
        static void Main(string[] args)
        {
            //RunProgram();
            RunBudgetProgram();
        }

        public static void RunBudgetProgram()
        {
            List<EmployeeDbModel> employees = SQLAccess.LoadEmployees();
            List<TimesheetRowDbModel> time = SQLAccess.LoadAllTimeSheetData();

            foreach(TimesheetRowDbModel trm in time)
            {
                if (trm.BudgetSpent <= 0)
                {
                    EmployeeDbModel emdb = employees.Where(x => x.Id == trm.EmployeeId).FirstOrDefault();

                    if (emdb != null)
                    {
                        trm.BudgetSpent = emdb.Rate * trm.TimeEntry;
                        SQLAccess.UpdateTimesheetData(trm);
                    }
                }
            }
        }
        public static void RunProgram()
        {
            List<ClientSummary> clientsumamry = new List<ClientSummary>();

            List<EmployeeDbModel> employees = SQLAccess.LoadEmployees();
            List<ProjectDbModel> projects = SQLAccess.LoadProjects();
            List<ClientDbModel> clients = SQLAccess.LoadClients();

            List<SubProjectDbModel> subs = new List<SubProjectDbModel>();


            foreach (ProjectDbModel pdb in projects)
            {
                List<SubProjectDbModel> sub = SQLAccess.LoadAllSubProjectsByProject(pdb.Id);
                subs.AddRange(sub);
            }


            foreach (EmployeeDbModel em in employees)
            {
                DateTime newdate = new DateTime(2022, 1, 1);
                DateTime enddate = new DateTime(2022, 12, 31);

                List<TimesheetRowDbModel> timesheets = SQLAccess.LoadTimeSheet(newdate, enddate, em.Id);

                foreach (TimesheetRowDbModel times in timesheets)
                {
                    int subid = times.SubProjectId;

                    SubProjectDbModel sub = subs.Where(x => x.Id == subid).FirstOrDefault();

                    ProjectDbModel project = projects.Where(x => x.Id == sub.ProjectId).FirstOrDefault();

                    ClientDbModel client = clients.Where(x => x.Id == project.ClientId).FirstOrDefault();

                    ClientSummary check = clientsumamry.Where(x => x.client == client).FirstOrDefault();

                    if (check == null)
                    {
                        clientsumamry.Add(new ClientSummary()
                        {
                            client = client,
                            hours = times.TimeEntry,
                            budget = times.BudgetSpent,
                        });
                    }
                    else
                    {
                        check.hours += times.TimeEntry;
                        check.budget += times.BudgetSpent;
                    }
                }
            }

            StringBuilder csv = new StringBuilder();

            foreach (ClientSummary clientsum in clientsumamry)
            {
                string newLine = string.Format($"{clientsum.client.ClientNumber.ToString()}, {clientsum.client.ClientName}, {clientsum.hours}, {clientsum.budget}");
                csv.AppendLine(newLine);
            }


            string filepath = "T:\\2022summary.csv";
            File.WriteAllText(filepath, csv.ToString());
        }
        public class ClientSummary
        {
            public ClientDbModel client;

            public double hours;

            public double budget;

            ////client add batch file
            //string[] lines = System.IO.File.ReadAllLines(@"T:\PortalImport\ClientUnedited.txt", Encoding.Default);

            //foreach (string line in lines)
            //{
            //    char[] sep = new char[] { '\t' };
            //    string[] splitContent = line.Split(sep);

            //    ClientDbModel client = new ClientDbModel() { ClientNumber = Convert.ToInt32(splitContent[0]), ClientName = splitContent[1], IsActive = 1 };
            //    SQLAccess.AddClient(client);
            //    Console.WriteLine("\t" + line);
            //}


            //string[] employeelines = System.IO.File.ReadAllLines(@"T:\PortalImport\Employees.txt", Encoding.Default);
            //int count1 = 0;
            //foreach (string line in employeelines)
            //{
            //    if (count1 > 0)
            //    {
            //        string sep = "\t";
            //        string[] splitContent = line.Split(sep.ToCharArray());

            //        EmployeeDbModel employee = new EmployeeDbModel()
            //        {
            //            FirstName = splitContent[0],
            //            LastName = splitContent[1],
            //            Title = splitContent[2],
            //            Extension = splitContent[3],
            //            AuthId = Convert.ToInt32(splitContent[4]),
            //            Rate = Convert.ToDouble(splitContent[5]),
            //            DefaultRoleId = Convert.ToInt32(splitContent[6]),
            //            Email = splitContent[7],
            //            PTORate = Convert.ToDouble(splitContent[8]),
            //            SickRate = Convert.ToDouble(splitContent[9]),
            //            Password = "password",
            //            PTOCarryover = 0,
            //            IsActive = 1
            //        };
            //        SQLAccess.AddEmployee(employee);
            //        Console.WriteLine("\t" + line);
            //    }
            //    count1++;
            //}

            //string[] projectlines = System.IO.File.ReadAllLines(@"T:\PortalImport\ProjectsNew.txt", Encoding.Default);
            //int count2 = 0;

            //List<ClientDbModel> clients = SQLAccess.LoadClients();
            //List<EmployeeDbModel> pms = SQLAccess.LoadProjectManagers();

            //foreach (string line in projectlines)
            //{
            //    if (count2 > 0)
            //    {
            //        string sep = "\t";
            //        string[] splitContent = line.Split(sep.ToCharArray());

            //        int clientnum = Convert.ToInt32(splitContent[2]);

            //        ClientDbModel client = clients.Where(x => x.ClientNumber == clientnum).FirstOrDefault();
            //        EmployeeDbModel pm = pms.Where(x => x.FirstName == splitContent[3].Trim()).FirstOrDefault();

            //        if (client != null && pm != null)
            //        {
            //            string s = splitContent[0].Replace("\"", "");

            //            string startdate = "0";

            //            if (splitContent[1].Length > 2)
            //            {
            //                startdate = splitContent[1].Substring(0, 2);
            //            }

            //            int intdate = 2000 + Convert.ToInt32(startdate);

            //            DateTime dateconvert = new DateTime(intdate, 1, 1);

            //            ProjectDbModel project = new ProjectDbModel()
            //            {
            //                ProjectName = s,
            //                ProjectNumber = Convert.ToInt32(splitContent[1]),
            //                ClientId = client.Id,
            //                ManagerId = pm.Id,
            //                MarketId = 1,
            //                Fee = 2000,
            //                PercentComplete = 0,
            //                ProjectStart = (int)long.Parse(dateconvert.ToString("yyyyMMdd")),
            //                IsActive = 1,
            //                IsCurrActive = 1
            //            };

            //            int projectnumber = SQLAccess.AddProject(project);

            //            if (splitContent[0].ToUpper().Contains("General"))
            //            {
            //                SubProjectDbModel miscsub = new SubProjectDbModel()
            //                {
            //                    ProjectId = projectnumber,
            //                    PercentComplete = 0,
            //                    PointNumber = "MISC",
            //                    IsActive = 1,
            //                    IsCurrActive = 1,
            //                    Description = "Miscellaneous",
            //                    Fee = 0,
            //                    PercentBudget = 100,
            //                    IsInvoiced = 0,
            //                };

            //                SQLAccess.AddSubProject(miscsub);

            //            }
            //            else
            //            {
            //                SubProjectDbModel CDsub = new SubProjectDbModel()
            //                {
            //                    ProjectId = projectnumber,
            //                    PercentComplete = 0,
            //                    PointNumber = "CD",
            //                    IsActive = 1,
            //                    IsCurrActive = 1,
            //                    Description = "Construction Documents",
            //                    Fee = 1000,
            //                    PercentBudget = 50,
            //                    IsInvoiced = 0,
            //                };

            //                SubProjectDbModel CAsub = new SubProjectDbModel()
            //                {
            //                    ProjectId = projectnumber,
            //                    PercentComplete = 0,
            //                    PointNumber = "CA",
            //                    IsActive = 1,
            //                    IsCurrActive = 1,
            //                    Description = "Construction Administration",
            //                    Fee = 1000,
            //                    PercentBudget = 50,
            //                    IsInvoiced = 0,
            //                };

            //                SQLAccess.AddSubProject(CDsub);
            //                SQLAccess.AddSubProject(CAsub);
            //            }






            //            Console.WriteLine("\t" + line);
            //        }






            //    }
            //    count2++;
            //}



        }
    }
}
