using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tresana.Data.Entities;

namespace Tresana.Web.Services
{
    public interface ITaskService : IDisposable
    {
        Task GetTaskById(int taskId);
        IEnumerable<Task> GetAllTasks();
        int CreateTask(Task task);
        bool UpdateTask(int taskId, Task task);
        bool DeleteTask(int taskId);
    }
}
