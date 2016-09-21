using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Tresana.Data.Entities;
using Tresana.Web.Services;

namespace Tresana.Web.Api.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IUserService userService;

        //public UsersController()
        //{
        //    userService = new UserService();
        //}

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        // GET: api/Users
        public IHttpActionResult GetUsers()
        {
            IEnumerable<User> users = userService.GetAllUsers();
            return Ok(users);
        }
        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            if(!userService.UpdateUser(id, user))
            {
                return NotFound();
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Users
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int id = userService.CreateUser(user);

            return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            if (userService.DeleteUser(id))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return NotFound();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                userService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}