using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tresana.Data.Entities;

namespace Tresana.Web.Services
{
    public interface IUserService : IDisposable
    {
        User GetUserById(int userId);
        IEnumerable<User> GetAllUsers();
        int CreateUser(User user);
        bool UpdateUser(int userId, User user);
        bool DeleteUser(int userId);
    }
}
