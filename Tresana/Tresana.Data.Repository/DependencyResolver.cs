using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tresana.Resolver;

namespace Tresana.Data.Repository
{
    [Export(typeof(IComponent))]
    public class DependencyResolver:IComponent
    {
        public void SetUp(IRegisterComponent registerComponent)
        {
            registerComponent.RegisterTypeWithControlledLifeTime<IUnitOfWork, UnitOfWork>();
        }
    }
}
