#Moq y Testing de Web API

Vamos a estudiar cómo podemos probar nuestro código evitando probar también sus dependencias, asegurándonos que los errores se restringen únicamente a la sección de código que efectivamente queremos probar. Para ello, utilizaremos una herramienta que nos permitirá crear Mocks. La herramienta será [Moq](https://github.com/moq/moq4).

##¿Qué son los Mocks?

Existen dos objetos que nos permiten probar sistemas. Nos referiremos a la sección del sistema a probar como SUT (System under test). Los Mocks, nos permiten verificar la interacción del SUT con sus dependencias. Los Stubs, nos permiten verificar el estado de los objetos que se pasan. Como queremos testear el comportamiento de nuestro código, utilizaremos los primeros. 

##Empezando con Moq.

Para comenzar a utiliar Moq, comenzaremos probando nuestro paquete de servicios. Para ello, debemos crear un nuevo proyecto de tipo Librería de Clases (Tresana.Web.Services.Tests) e instalarle Moq y XUnit, utilizando el manejador de paquetes Nuget. Se deberán agregar también las referencias al proyecto de nuestras entidades, al de los servicios, y al de los repositorios.

Una vez que estos pasos estén prontos, podemos comenzar a realizar nuestro primer test. Creamos entonces la clase UserServiceTests, y en ella escribimos el primer `Fact`. 

```C#

[Fact]
public void CreateUserTest()
{
    //Arrange
    
    //Act
    
    //Assert
}

```

Para ello seguiremos la metodología AAA: Arrange, Act, Assert.
En la sección de Arrange, construiremos los el objeto mock y se lo pasaremos al sistema a probar. En la sección de Act, ejecutaremos el sistema a probar. Por último, en la sección de Assert, verificaremos la interacción del SUT con el objeto mock.

Ahora, podemos comenzar a probar. Nuestros servicios interactúan con la clase UnitOfWork, y esa implementación la que deseamos mockear. Para ello debemos generar un mock de IUnitOfWork y pasarlo por parámetro al servicio.

```C#

[Fact]
public void CreateUserTest()
{
    //Arrange
    
    //Inicializo un mock de IUnitOfWork con el que interactuará el UserService
    var mockUnitOfWork = new Mock<IUnitOfWork>();
    
    //Paso el mockUnitOfWork por parámetro al constructor del servicio.
    //Para obtener el objeto del tipo que creamos el mock, debemos obtener la property Object del mock,
    //lo que retorna un objeto de tipo IUnitOfWork
    IUserService userService = new UserService(mockUnitOfWork.Object);

    //Act
    
    //Assert
}

```

Sin embargo, nos falta definir el comportamiento que debe tener el mock del unitOfWork. Para ello, debemos hacer el Setup

```C#

[Fact]
public void CreateUserTest()
{
    //Arrange
    
    //Inicializo un mock de IUnitOfWork con el que interactuará el UserService
    var mockUnitOfWork = new Mock<IUnitOfWork>();
    
    //Esperamos que se llame al método Insert del userRepository con un Usuario y luego al Save();
    mockUnitOfWork.Setup(un => un.UserRepository.Insert(It.IsAny<User>()));
    mockUnitOfWork.Setup(un => un.Save());
    
    //Paso el mockUnitOfWork por parámetro al constructor del servicio
    IUserService userService = new UserService(mockUnitOfWork.Object);

    //Act
    
    //Efectuamos la llamada al servicio
    User user = userService.CreateUser(new User() { });
    
    //Assert
}

```

Una vez que ejecutamos el método que queremos probar, también debemos verificar que se hicieron las llamadas pertinentes. Para esto usamos el método VerifyAll del mock.

```C#

[Fact]
public void CreateUserTest()
{
    //Arrange
    
    //Inicializo un mock de IUnitOfWork con el que interactuará el UserService
    var mockUnitOfWork = new Mock<IUnitOfWork>();
    
    //Esperamos que se llame al método Insert del userRepository con un Usuario y luego al Save();
    mockUnitOfWork.Setup(un => un.UserRepository.Insert(It.IsAny<User>()));
    mockUnitOfWork.Setup(un => un.Save());
    
    //Paso el mockUnitOfWork por parámetro al constructor del servicio
    IUserService userService = new UserService(mockUnitOfWork.Object);

    //Act
    
    //Efectuamos la llamada al servicio
    userService.CreateUser(new User() { });
    
    //Assert
    mockUnitOfWork.VerifyAll();
}

```

Y voilá. Si corremos, obtenemos que el test ha pasado. Sin embargo, el método CreateUser() retorna un int.
Más adelante veremos como verificar esto.

Veamos ahora como controlar los valores de retorno de los mocks en nuestros métodos. 
Para ello, probemos el método de updateUser

```C#

[Fact]
public void UpdateExistingUser()
{
    //Arrange 
    var mockUnitOfWork = new Mock<IUnitOfWork>();
    //Para el Update, se utiliza el método ExistsUser(), el cual a su vez utiliza el método GetUserByID del repositorio.
    //En este test, querémos asegurarnos que, en caso que el usuario exista, se ejecute el Update() y el Save() en el repositorio.
    //Por lo tanto, debemos establecer que el GetUserByID devuelva algo distinto de null, de manera que el ExistsUser retorne true.
    mockUnitOfWork
        .Setup(un => un.UserRepository.GetByID(It.IsAny<int>()))
        .Returns(new User() { });

    //Además, seteamos las expectativas para los métodos que deben llamarse luego
    mockUnitOfWork.Setup(un => un.UserRepository.Update(It.IsAny<User>()));
    mockUnitOfWork.Setup(un => un.Save());
    
    IUserService userService = new UserService(mockUnitOfWork);
    
}

```

Una vez que seteamos el retorno esperado, debemos ejecutar el update con un usuario cualquiera y verificar que se realizaron los llamados correspondientes.

```C#

[Fact]
public void UpdateExistingUser()
{
    //Arrange 
    var mockUnitOfWork = new Mock<IUnitOfWork>();
    //Para el Update, se utiliza el método ExistsUser(), el cual a su vez utiliza el método GetUserByID del repositorio.
    //En este test, querémos asegurarnos que, en caso que el usuario exista, se ejecute el Update() y el Save() en el repositorio.
    //Por lo tanto, debemos establecer que el GetUserByID devuelva algo distinto de null, de manera que el ExistsUser retorne true.
    mockUnitOfWork
        .Setup(un => un.UserRepository.GetByID(It.IsAny<int>()))
        .Returns(new User() { });

    //Además, seteamos las expectativas para los métodos que deben llamarse luego
    mockUnitOfWork.Setup(un => un.UserRepository.Update(It.IsAny<User>()));
    mockUnitOfWork.Setup(un => un.Save());
    
    IUserService userService = new UserService(mockUnitOfWork.Object);

    //act
    bool updated = userService.UpdateUser(0, new User() {});

    //Assert
    //En este caso, debemos asegurarnos que el Update y el Save se hayan llamado una vez.
    mockUnitOfWork.Verify(un=> un.UserRepository.Update(It.IsAny<User>()), Times.Exactly(1));
    mockUnitOfWork.Verify(un=> un.Save(), Times.Exactly(1));
    
    //Además, verificamos que retorne true, ya que el update fue realizado.
    Assert.True(updated);
    
}

```

##Ejercicio: 

Probar ahora como funciona el UpdateUser cuando no existe el usuario

#Utilizando Reflection

Para poder invertir las dependencias de nuestra api, utilizaremos reflection. Además, extraeremos la responsabilidad de resolver las dependencias a un componente externo.

Para ello, nos creamos un nuevo proyecto, Tresana.Resolver, y en él instalamos los siguientes paquetes nuget:
- Microsoft.AspNet.WebApi.Core
- Unity
- Unity.WebApi
Además, debemos agregar la referencia a System.ComponentModel.Composition. Para ello, seleccionamos con botón derecho el proyecto Resolver, y seleccionamos Add Reference. Una vez allí, buscamos esta referencia entre los Assemblies (ensamblados).

Una vez instaladas las referencias, podemos eliminar la carpeta de App_start del proyecto, y su contenido.

Ahora, debemos crear 2 interfaces y una clase. Comencemos por las interfaces.

IRegisterComponent:

```C#

namespace Tresana.Resolver
{
    public interface IRegisterComponent
    {
        void RegisterType<TFrom, TTo>(bool withInterception = false) where TTo : TFrom;
        void RegisterTypeWithControlledLifeTime<TFrom, TTo>(bool withInterception = false) where TTo : TFrom;
    }
}
```

IComponent:

```C#

namespace Tresana.Resolver
{
    public interface IComponent
    {
        void SetUp(IRegisterComponent registerComponent);
    }
}

```


Y luego la clase ComponentResolver

```C#

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Tresana.Resolver
{
    public class ComponentLoader
    {
        public static void LoadContainer(IUnityContainer container, string path, string pattern)
        {
            var dirCat = new DirectoryCatalog(path, pattern);
            var importDef = BuildImportDefinition();
            try
            {
                using (var aggregateCatalog = new AggregateCatalog())
                {
                    aggregateCatalog.Catalogs.Add(dirCat);

                    using (var componsitionContainer = new CompositionContainer(aggregateCatalog))
                    {
                        IEnumerable<Export> exports = componsitionContainer.GetExports(importDef);

                        IEnumerable<IComponent> modules = exports.Select(export => export.Value as IComponent).Where(m => m != null);

                        var registerComponent = new RegisterComponent(container);
                        foreach (IComponent module in modules)
                        {
                            module.SetUp(registerComponent);
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException typeLoadException)
            {
                var builder = new StringBuilder();
                foreach (Exception loaderException in typeLoadException.LoaderExceptions)
                {
                    builder.AppendFormat("{0}\n", loaderException.Message);
                }

                throw new TypeLoadException(builder.ToString(), typeLoadException);
            }
        }

        private static ImportDefinition BuildImportDefinition()
        {
            return new ImportDefinition(
                def => true, typeof(IComponent).FullName, ImportCardinality.ZeroOrMore, false, false);
        }
    }

    internal class RegisterComponent : IRegisterComponent
    {
        private readonly IUnityContainer _container;

        public RegisterComponent(IUnityContainer container)
        {
            this._container = container;
            //Register interception behaviour if any
        }

        public void RegisterType<TFrom, TTo>(bool withInterception = false) where TTo : TFrom
        {
            if (withInterception)
            {
                //register with interception
            }
            else
            {
                this._container.RegisterType<TFrom, TTo>();
            }
        }

        public void RegisterTypeWithControlledLifeTime<TFrom, TTo>(bool withInterception = false) where TTo : TFrom
        {
            this._container.RegisterType<TFrom, TTo>(new ContainerControlledLifetimeManager());
        }
    }
}
```

Ahora, nuestro proyecto puede registrar tipos a través de reflection.

Agreguemos entonces la resolución de dependencias en los servicios.

Agregamos la clase DependencyResolver en ese proyecto, que implementa una de las interfaces realizadas previamente:

```C#

using System.ComponentModel.Composition;
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
            registerComponent.RegisterTypeWithControlledLifeTime<IUnitOfWork,UnitOfWork>();

        }
    }
}

```

Ahora lo mismo para UnitOfWork:

```C#

using System.ComponentModel.Composition;
using Tresana.Resolver;

namespace Tresana.Data.Repository
{
    [Export(typeof(IComponent))]
    public class DependencyResolver:IComponent
    {
        public void SetUp(IRegisterComponent registerComponent)
        {
            registerComponent.RegisterType<IUnitOfWork, UnitOfWork>();
        }
    }
}

```

Ya tenemos el 90% del trabajo pronto. 

Hagamos el resto:

En WebApiConfig.cs, cambiamos el método Register para que quede de la siguiente manera:

```C#

public static void Register(HttpConfiguration config)
{
    var container = new UnityContainer();

    ComponentLoader.LoadContainer(container, ".\\bin", "Tresana.Web.*.dll");

    GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
    // Web API routes
    config.MapHttpAttributeRoutes();

    config.Routes.MapHttpRoute(
        name: "DefaultApi",
        routeTemplate: "api/{controller}/{id}",
        defaults: new { id = RouteParameter.Optional }
    );
}

```

Y de esta manera, podemos quitar la referencia al proyecto de Repository desde WebApi, habiendo logrado invertir las dependencias.
