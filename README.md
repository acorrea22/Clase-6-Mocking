# Mocking y Testing de Web API

Vamos a estudiar cómo podemos probar nuestro código evitando probar también sus dependencias, asegurándonos que los errores se restringen únicamente a la sección de código que efectivamente queremos probar. Para ello, utilizaremos una herramienta que nos permitirá crear Mocks. La herramienta será [Moq](https://github.com/moq/moq4).

## ¿Qué son los Mocks?

Los mocks son una de las varios "test doubles" (es decir, objetos que no son reales respecto a nuestro dominio, y que se usan con finalidades de testing) que existen para probar nuestros sistemas. Los más conocidos son los Mocks y los Stubs, siendo la principal diferencia en ellos, el foco de lo que se está testeando.

Antes de hacer énfasis en tal diferencia, es importante aclarar que nos referiremos a la sección del sistema a probar como SUT -System under test-). Los Mocks, nos permiten verificar la interacción del SUT con sus dependencias. Los Stubs, nos permiten verificar el estado de los objetos que se pasan. Como queremos testear el comportamiento de nuestro código, utilizaremos los primeros.

## ¿Por qué los queremos usar?

Porque queremos probar objetos y la forma en que estos interactúan con otros objetos. Para ello crearemos instancias de Mocks, es decir, objetos que simulen el comportamiento externo (es decir, la interfaz), de un cierto objeto. Son objetos tontos, que no dependen de nadie, siendo útiles para aislar una cierta parte de la aplicación que queramos probar. En este caso, utilizaremos el proyecto de Servicios, el cual contiene la lógica fundamental de la Web Api.

## Empezando con Moq

Para comenzar a utilizar Moq, comenzaremos probando nuestro paquete de servicios. Para ello, debemos crear un nuevo proyecto de tipo Librería de Clases (Tresana.Web.Services.Tests) e instalarle Moq, utilizando el manejador de paquetes Nuget. Se deberán agregar también las referencias al proyecto de nuestras entidades, al de los servicios, y al de los repositorios.

Una vez que estos pasos estén prontos, podemos comenzar a realizar nuestro primer test. Creamos entonces la clase UserServiceTests, y en ella escribimos el primer `Fact`. 

```C#

[Fact]
public void CreateBreedTest()
{
    //Arrange
    
    //Act
    
    //Assert
}

```

Para ello seguiremos la metodología **AAA: Arrange, Act, Assert**.
En la sección de **Arrange**, construiremos los el objeto mock y se lo pasaremos al sistema a probar. En la sección de **Act**, ejecutaremos el sistema a probar. Por último, en la sección de **Assert**, verificaremos la interacción del SUT con el objeto mock.

Ahora, podemos comenzar a probar. Nuestros servicios interactúan con la clase UnitOfWork, siendo esa la implementación que debemos mockear. Para ello debemos generar un mock de IUnitOfWork y pasarlo por parámetro al servicio.

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
