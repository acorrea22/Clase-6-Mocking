using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tresana.Resolver
{
    public interface IRegisterComponent
    {
        void RegisterType<TFrom, TTo>(bool withInterception = false) where TTo : TFrom;
        void RegisterTypeWithControlledLifeTime<TFrom, TTo>(bool withInterception = false) where TTo : TFrom;
    }
}
