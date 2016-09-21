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
    public class TasksController : ApiController
    {
        private readonly ITaskService taskService;

        public TasksController(ITaskService service)
        {
            taskService = service;
        }

        // GET: api/Tasks
        public IEnumerable<Task> GetTasks()
        {
            return taskService.GetAllTasks();
        }

        // GET: api/Tasks/5
        [ResponseType(typeof(Task))]
        public IHttpActionResult GetTask(int id)
        {
            Task task = taskService.GetTaskById(id);
            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        // PUT: api/Tasks/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTask(int id, Task task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != task.Id)
            {
                return BadRequest();
            }
            if (!taskService.UpdateTask(id, task))
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Tasks
        [ResponseType(typeof(Task))]
        public IHttpActionResult PostTask(Task task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int id = taskService.CreateTask(task);

            return CreatedAtRoute("DefaultApi", new { id = task.Id }, task);
        }

        // DELETE: api/Tasks/5
        [ResponseType(typeof(Task))]
        public IHttpActionResult DeleteTask(int id)
        {
            if (taskService.DeleteTask(id))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return NotFound();

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                taskService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}