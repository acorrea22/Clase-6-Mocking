using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Tresana.Data.Entities;
using Tresana.Data.Repository;

namespace Tresana.Web.Services
{
    public class TaskService : ITaskService
    {

        private readonly IUnitOfWork unitOfWork;

        public TaskService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public int CreateTask(Task task)
        {
            unitOfWork.TaskRepository.Insert(task);
            unitOfWork.Save();
            return task.Id;
        }

        public bool DeleteTask(int taskId)
        {
            if (ExistsTask(taskId))
            {
                unitOfWork.TaskRepository.Delete(taskId);
                unitOfWork.Save();
                return true;
            }
            else
            {
                return false;
            }
        }

       

        public IEnumerable<Task> GetAllTasks()
        {
            return unitOfWork.TaskRepository.Get();
        }

        public Task GetTaskById(int taskId)
        {
            return unitOfWork.TaskRepository.GetByID(taskId);
        }

        public bool UpdateTask(int taskId, Task task)
        {
            if (ExistsTask(taskId))
            {
                Task existingTask = unitOfWork.TaskRepository.GetByID(taskId);
                existingTask.Name = task.Name;
                existingTask.Description = task.Description;
                existingTask.Priority = task.Priority;
                existingTask.Estimation = task.Estimation;
                existingTask.DueDate = task.DueDate;
                existingTask.FinishDate = task.FinishDate;
                existingTask.ResponsibleUsers = task.ResponsibleUsers;
                existingTask.StartDate = task.StartDate;
                existingTask.Status = task.Status;
                unitOfWork.TaskRepository.Update(task);
                unitOfWork.Save();
                return true;
            }
            return false;               
        }

        private bool ExistsTask(int taskId)
        {
            Task task = unitOfWork.TaskRepository.GetByID(taskId);
            return task != null;
        }

        public void Dispose()
        {
            unitOfWork.Dispose();
        }
    }
}
