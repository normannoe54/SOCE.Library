using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SORD.Library.API.Test.Data;
using SORD.Library.API.Test.Dtos;
using SORD.Library.API.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

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
        /// Mapper for objects
        /// </summary>
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
        [HttpGet("{id}", Name = "GetUser")]
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

        /// <summary>
        /// POST api/commands/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<UserReadDto> CreateUser(UserCreateDto usercreatedto)
        {
            User user = _mapper.Map<User>(usercreatedto);
            _userController.CreateUser(user);         
            _userController.SaveChanges();
            UserReadDto userreaddto = _mapper.Map<UserReadDto>(user);

            //201 result
            return CreatedAtRoute(nameof(GetUser), new { Id = userreaddto.Id }, userreaddto);
        }

        /// <summary>
        /// PUT api/commands/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public ActionResult UpdateUser(int id, UserUpdateDto userupdatedto)
        {
            User user = _userController.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            _mapper.Map(userupdatedto, user);
            _userController.UpdateUser(user);
            _userController.SaveChanges();
            return NoContent();
        }

        /// <summary>
        /// PATCH api/commands/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public ActionResult PatchUser(int id, JsonPatchDocument<UserUpdateDto> patchdoc)
        {
            User user = _userController.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            UserUpdateDto usertopatch = _mapper.Map<UserUpdateDto>(user);
            patchdoc.ApplyTo(usertopatch, ModelState);

            if (!TryValidateModel(usertopatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(usertopatch, user);
            _userController.UpdateUser(user);
            _userController.SaveChanges();
            return NoContent();
        }

        /// <summary>
        /// DELETE api/commands/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            User user = _userController.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            _userController.DeleteUser(user);
            _userController.SaveChanges();
            return NoContent();
        }

    }
}
