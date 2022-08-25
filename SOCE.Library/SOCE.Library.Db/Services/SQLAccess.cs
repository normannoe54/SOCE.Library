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

        public static List<EmployeeDbModel> LoadEmployees()
        {
            using(IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<EmployeeDbModel>("SELECT * FROM Employees", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void AddEmployee(EmployeeDbModel employee)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("INSERT INTO Employees (FirstName, LastName, AuthId, Title, Email, PhoneNumber, Extension)" +
                    "VALUES (@FirstName, @LastName, @AuthId, @Title, @Email, @PhoneNumber, @Extension)", employee);
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

        public static void AddProject(ProjectDbModel project)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("INSERT INTO Projects (ProjectName, ProjectNumber, Client, Fee)" +
                    "VALUES (@ProjectName, @ProjectNumber, @Client, @Fee)", project);
            }
        }

        public static List<SubProjectDbModel> LoadSubProjects(int projectId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<SubProjectDbModel>("SELECT * FROM SubProjects WHERE ProjectId = @projectId", new { projectId});
                return output.ToList();
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
    }
}
