using Microsoft.AspNetCore.Mvc;
using SORD.Library.API.Test.Data;
using SORD.Library.API.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SORD.Library.API.Test.Controllers
{
    [Route("api/commands")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        /// <summary>
        /// User controller
        /// </summary>
        private readonly IUserData _userController;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userController"></param>
        public UsersController(IUserData userController)
        {
            _userController = userController;
        }        

        /// <summary>
        /// GET api/commands
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult <IEnumerable<User>> GetUsers()
        {
            IEnumerable<User> users = _userController.GetUsers();
            return Ok(users);
        }

        /// <summary>
        /// GET api/commands/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            User user = _userController.GetUserById(id);
            return Ok(user);
        }

    }
}
