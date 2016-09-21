using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tresana.Data.Entities;
using Tresana.Data.Repository;

namespace Tresana.Web.Services
{
    public class UserService : IUserService
    {

        private readonly IUnitOfWork unitOfWork;

        //public UserService()
        //{
        //    unitOfWork = new UnitOfWork();
        //}

        public UserService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public int CreateUser(User user)
        {
            unitOfWork.UserRepository.Insert(user);
            unitOfWork.Save();
            return user.Id;
        }

        public bool DeleteUser(int userId)
        {
            if (ExistsUser(userId))
            {
                unitOfWork.UserRepository.Delete(userId);
                unitOfWork.Save();
                return true;
            }
            return false;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return unitOfWork.UserRepository.Get(null, null, "");
        }

        public User GetUserById(int userId)
        {
            User user = unitOfWork.UserRepository.GetByID(userId);
            return user;
        }

        public bool UpdateUser(int userId, User user)
        {
            if (ExistsUser(userId))
            {
                User userEntity = unitOfWork.UserRepository.GetByID(userId);
                userEntity.LastName = user.LastName;
                userEntity.Name = user.Name;
                userEntity.Mail = user.Mail;
                userEntity.UserName = user.UserName;
                unitOfWork.UserRepository.Update(userEntity);
                unitOfWork.Save();
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            unitOfWork.Dispose();
        }

        private bool ExistsUser(int userId)
        {
            User user = unitOfWork.UserRepository.GetByID(userId);
            return user != null;
        }
    }
}
