using System;
using System.Collections.Generic;
using System.Text;

namespace SOCE.Library.Db
{
    public class EmployeeDbModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public int AuthId { get; set; }
        public int DefaultRoleId { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Extension { get; set; }

        public double PTORate { get; set; }

        public double PTOCarryover { get; set; }

        public double SickRate { get; set; }
        public double SickCarryover { get; set; }

        public double HolidayHours { get; set; }
        public double Rate { get; set; }
        public int IsActive { get; set; }
        public int StartDate { get; set; }
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

    }
}
