using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOCE.Library.Db;
using System.Linq;

namespace SOCE.Library.DbBatch
{
    class Program
    {
        static void Main(string[] args)
        {
            //client add batch file
            //string[] lines = System.IO.File.ReadAllLines(@"T:\PortalImport\ClientUnedited.txt", Encoding.Default);

            //foreach (string line in lines)
            //{
            //    string sep = "\t";
            //    string[] splitContent = line.Split(sep.ToCharArray());

            //    ClientDbModel client = new ClientDbModel() { ClientNumber = splitContent[0], ClientName = splitContent[1], IsActive= 1 };
            //    SQLAccess.AddClient(client);
            //    Console.WriteLine("\t" + line);
            //}


            //string[] employeelines = System.IO.File.ReadAllLines(@"T:\PortalImport\Employees.txt", Encoding.Default);
            //int count = 0;
            //foreach (string line in employeelines)
            //{
            //    if (count >0)
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
            //            SickCarryover = 0,
            //            PTOCarryover =0,
            //            IsActive = 1
            //        };
            //        SQLAccess.AddEmployee(employee);
            //        Console.WriteLine("\t" + line);
            //    }
            //    count++;
            //}

            string[] projectlines = System.IO.File.ReadAllLines(@"T:\PortalImport\Projects.txt", Encoding.Default);
            int count = 0;

            List<ClientDbModel> clients = SQLAccess.LoadClients();
            List<EmployeeDbModel> pms = SQLAccess.LoadProjectManagers();

            foreach (string line in projectlines)
            {
                if (count > 0)
                {
                    string sep = "\t";
                    string[] splitContent = line.Split(sep.ToCharArray());

                    int clientnum = Convert.ToInt32(splitContent[2]);

                    ClientDbModel client = clients.Where(x => x.ClientNumber == clientnum).FirstOrDefault();
                    EmployeeDbModel pm = pms.Where(x => x.FirstName == splitContent[3].Trim()).FirstOrDefault();

                    ProjectDbModel project = new ProjectDbModel()
                    {

                        ProjectName = splitContent[0],
                        ProjectNumber = Convert.ToInt32(splitContent[1]),
                        ClientId = client.Id,
                        ManagerId = pm.Id,
                        MarketId = 0,
                        Fee = 0,
                        PercentComplete = 0,
                        IsActive = 1,
                        IsCurrActive = 1
                    };
                    SQLAccess.AddProject(project);
                    Console.WriteLine("\t" + line);
                }
                count++;
            }



        }
    }
}
