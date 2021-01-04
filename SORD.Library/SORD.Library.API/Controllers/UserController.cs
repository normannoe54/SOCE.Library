using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SORD.Library.Kernel.DataAccess;
using SORD.Library.Kernel.Models;
using Microsoft.AspNetCore.Authorization;

namespace SORD.Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public UserModel GetById()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserData data = new UserData();
            UserModel listdata = data.GetUserById(userId).First();
            return listdata;
        }
    }
}
