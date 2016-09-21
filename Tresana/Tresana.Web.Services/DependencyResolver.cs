using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Tresana.Data.Repository;
using Tresana.Resolver;

namespace Tresana.Web.Services
{
    [Export(typeof(IComponent))]
    public class DependencyResolver:IComponent
    {
        public void SetUp(IRegisterComponent registerComponent)
        {
            registerComponent.RegisterType<IUserService, UserService>();
            registerComponent.RegisterType<ITaskService, TaskService>();
            //registerComponent.RegisterTypeWithControlledLifeTime<IUnitOfWork,UnitOfWork>();

        }
    }
}
