using SORD.Library.Kernel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SORD.Library.Kernel.DataAccess
{
    public class UserData
    {
        public List<UserModel> GetUserById(string Id)
        {
            SQLDataAccess sql = new SQLDataAccess();
            var p = new { Id = Id };
            var output = sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "DefaultConnection");
            return output;
        }
    }
}
