using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SORD.Library.API.Test.Data;
using SORD.Library.API.Test.Dtos;
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

        private readonly IMapper _mapper;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="userController"></param>
        /// <param name="mapper"></param>
        public UsersController(IUserData userController, IMapper mapper)
        {
            _userController = userController;
            _mapper = mapper;
        }

        /// <summary>
        /// GET api/commands
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult <IEnumerable<User>> GetUsers()
        {
            IEnumerable<User> users = _userController.GetUsers();
            IEnumerable<UserReadDto> usersdto = _mapper.Map<IEnumerable<UserReadDto>>(users);
            return Ok(users);
        }

        /// <summary>
        /// GET api/commands/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<UserReadDto> GetUser(int id)
        {
            User user = _userController.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            UserReadDto userreaddto = _mapper.Map<UserReadDto>(user);

            return Ok(userreaddto);
        }

    }
}
