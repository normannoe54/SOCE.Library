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
            var output = sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", new { Id }, "DefaultConnection");
            return output;
        }
    }
}
