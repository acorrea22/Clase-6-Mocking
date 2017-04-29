# Mocking y Testing de Web API

Vamos a estudiar cómo podemos probar nuestro código evitando probar también sus dependencias, asegurándonos que los errores se restringen únicamente a la sección de código que efectivamente queremos probar. Para ello, utilizaremos una herramienta que nos permitirá crear Mocks. La herramienta será [Moq](https://github.com/moq/moq4).

## ¿Qué son los Mocks?

Los mocks son una de las varios "test doubles" (es decir, objetos que no son reales respecto a nuestro dominio, y que se usan con finalidades de testing) que existen para probar nuestros sistemas. Los más conocidos son los Mocks y los Stubs, siendo la principal diferencia en ellos, el foco de lo que se está testeando.

Antes de hacer énfasis en tal diferencia, es importante aclarar que nos referiremos a la sección del sistema a probar como SUT -System under test-). Los Mocks, nos permiten verificar la interacción del SUT con sus dependencias. Los Stubs, nos permiten verificar el estado de los objetos que se pasan. Como queremos testear el comportamiento de nuestro código, utilizaremos los primeros.

## ¿Por qué los queremos usar?

Porque queremos probar objetos y la forma en que estos interactúan con otros objetos. Para ello crearemos instancias de Mocks, es decir, objetos que simulen el comportamiento externo (es decir, la interfaz), de un cierto objeto. Son objetos tontos, que no dependen de nadie, siendo útiles para aislar una cierta parte de la aplicación que queramos probar. 

Hay ciertos casos en los que incluso los mocks son realmente la forma más adecuada de llevar a cabo pruebas unitarias. Y afortunadamente, la inyección de dependencias que ya hemos visto nos ayuda a usar mocks de una forma muy transparente. En lugar de crear los objetos dependientes en lugar de nuestros constructores, los estaremos inyectando. Y como dichos objetos de los cuales nuestros objetos dependen tienen una interfaz definida, podremos crear objetos mocks que cumplan con dicha interfaz y se inyecten.

![alt text](http://tutorials.jenkov.com/images/java-unit-testing/testing-with-di-containers.png)

En consecuencia, generamos un **bajo acoplamiento** entre una clase y sus dependencias, lo cual nos facilita utilizar un framework de mocking. Especialmente para aquellos objetos que dependen de un recurso externo (una red, un archivo o una base de datos).

![alt text](https://s-media-cache-ak0.pinimg.com/originals/e8/11/e4/e811e4d1f27b6f70006307ae7dce38d8.gif)


Particularmente estaremos mostrando cómo hacer pruebas unitarias sobre los controllers de nuestra Web Api, realizando mocks sobre las definiciones de nuestra lógica de negocio, por ejemplo: IBreedsBusinessLogic.

## Empezando con Moq

Para comenzar a utilizar Moq, comenzaremos probando nuestro paquete de controllers de la web api. Para ello, debemos crear un nuevo proyecto de tipo Librería de Clases (Tresana.Web.Api.Tests) e instalarle Moq, utilizando el manejador de paquetes Nuget.


![alt text](https://github.com/ORT-DA2/Clase-5/blob/develop/Imagenes/1.png)


![alt text](https://github.com/ORT-DA2/Clase-5/blob/develop/Imagenes/2.png)

Se deberán agregar también las referencias al proyecto de nuestras Entities, al de los BusinessLogic, y obviamente, al de WebApi.

![alt text](https://github.com/ORT-DA2/Clase-5/blob/develop/Imagenes/3.png)

Una vez que estos pasos estén prontos, podemos comenzar a realizar nuestro primer test. Creamos entonces la clase BreedsControllerTests, y en ella escribimos el primer `TestMethod`. 

## Probando los Get

```C#

[TestClass]
public class BreedsControllerTests
{
    [TestMethod]
    public void GetAllBreedsOkTest()
    {
        //Arrange
        
        //Act
        
        //Assert
    }

}
```

Para ello seguiremos la metodología **AAA: Arrange, Act, Assert**.
En la sección de **Arrange**, construiremos los el objeto mock y se lo pasaremos al sistema a probar. En la sección de **Act**, ejecutaremos el sistema a probar. Por último, en la sección de **Assert**, verificaremos la interacción del SUT con el objeto mock.

Ahora, podemos comenzar a probar. Nuestro **BreedsController** interactúan con la clase **IBreedsBusinessLogic** la cual es inyectada en el controller 🤘🏻😎, siendo esa la interfaz que debemos mockear. Para ello debemos generar un mock de IBreedsBusinessLogic y se lo debemos pasar por parmetro por parámetro al Controller.

```C#

[TestMethod]
public void GetAllBreedsOkTest()
{
    //Arrange: Construimos el mock
    var mockBreedsBusinessLogic = new Mock<IBreedsBusinessLogic>();
    var controller = new BreedsController(mockBreedsBusinessLogic.Object);

    //Act: Efectuamos la llamada al controller
    IHttpActionResult obtainedResult = controller.Get();

    //Assert

}
```

Sin embargo, nos falta definir el comportamiento que debe tener el mock del nuestro IBreedsBusinessLogic. Esto es lo que llamamos **expectativas** y lo que vamos asegurarnos que se cumpla al final de la prueba. Recordemos, los mocks simulan el comportamiento de nuestros objetos, siendo ese comportamiento lo que vamos a especificar a partir de expectativas. Para ello, usamos el método **Setup**.

### ¿Cómo saber qué expectativas asignar?

Esto va en función del método de prueba. Las expectativas se corresponden al caso de uso particular que estamos probando dentro de nuestro método de prueba. Si esperamos probar el Get() de nuestro BreedsController, y queremos mockear la clase BreedsBusinessLogic, entonces las expectativas se corresponden a las llamadas que hace BreedsController sobre BreedsBusinessLogic. Veamos el método a probar:


```C#
public IHttpActionResult Get()
{
    IEnumerable<Breed> breeds = breedsBusinessLogic.GetAllBreeds();
    if (breeds == null)
    {
        return NotFound();
    }
    return Ok(breeds);
}
```

La línea que queremos mockear es la de:

```C#
IEnumerable<Breed> breeds = breedsBusinessLogic.GetAllBreeds();
``` 

Entonces:

1) Primero vamos a decirle que esperamos que sobre nuestro Mock que se llame a la función GetAllBreeds().
2) Luego vamos a indicarle que esperamos que tal función se retorne una lista de razas que definimos en otro lado.

```C#

[TestMethod]
public void GetAllBreedsOkTest()
{
    //Arrange: Construimos el mock y seteamos las expectativas
    var expectedBreeds = GetFakeBreeds();
    var mockBreedsBusinessLogic = new Mock<IBreedsBusinessLogic>();
    mockBreedsBusinessLogic
        .Setup(bl => bl.GetAllBreeds())
        .Returns(expectedBreeds);

    //Act: Efectuamos la llamada al controller
    IHttpActionResult obtainedResult = controller.Get();

    //Assert

}

//Función auxiliar
private IEnumerable<Breed> GetFakeBreeds()
{
    return new List<Breed>
    {
        new Breed
        {
            Id = new Guid("e5020d0b-6fce-4b9f-a492-746c6c8a1bfa"),
            Name = "Pug",
            HairType  = "short fur",
            HairColors = new List<string>
            {
                "blonde"
            }
        },
        new Breed
        {
            Id = new Guid("6b718186-fa8c-4e14-9af8-2601e153db71"),
            Name = "Golden Retriever",
            HairType  = "hairy fur",
            HairColors = new List<string>
            {
                "blonde"
            }
        }
    };
}

```

Una vez que ejecutamos el método que queremos probar, también debemos verificar que se hicieron las llamadas pertinentes. Para esto usamos el método VerifyAll del mock.

Además, realizamos asserts (aquí estamos probando estado), para ver que los objetos usados son consistentes de acuerdo al resultado esperado.

```C#

[TestMethod]
public void GetAllBreedsOkTest()
{
    //Arrange
    var expectedBreeds = GetFakeBreeds();

    var mockBreedsBusinessLogic = new Mock<IBreedsBusinessLogic>();
    mockBreedsBusinessLogic
        .Setup(bl => bl.GetAllBreeds())
        .Returns(expectedBreeds);

    var controller = new BreedsController(mockBreedsBusinessLogic.Object);

    //Act
    IHttpActionResult obtainedResult = controller.Get();
    // Casteo el resultado HTTP a un resultado OK
    var contentResult = obtainedResult as OkNegotiatedContentResult<IEnumerable<Breed>>;

    //Assert
    mockBreedsBusinessLogic.VerifyAll();
    Assert.IsNotNull(contentResult);
    Assert.IsNotNull(contentResult.Content);
    Assert.AreEqual(expectedBreeds, contentResult.Content);
}

```

Y voilá. Vemos que nuestro test pasa 😎!

Ahora veamos como probar otros casos particulares, por ejemplo cuando nuestro ```Get()``` del Controller nos devuelve una **BadRequest**.

Particularmente, en el caso que hemos visto antes nuestro Controller retornaba OK para dicho. Ahora, nos interesa probar el caso en el que nuestro Controller retorna una BadRequest. Particularmente esto se da cuando el método ```GetAllBreeds()``` retorna null. Seteamos entonces dichas expectativas y probemos.

```C#
 [TestMethod]
public void GetAllBreedsErrorNotFoundTest()
{
    //Arrange
    List<Breed> expectedBreeds = null;

    var mockBreedsBusinessLogic = new Mock<IBreedsBusinessLogic>();
    mockBreedsBusinessLogic
        .Setup(bl => bl.GetAllBreeds())
        .Returns(expectedBreeds);

    var controller = new BreedsController(mockBreedsBusinessLogic.Object);

    //Act
    IHttpActionResult obtainedResult = controller.Get();

    //Assert
    mockBreedsBusinessLogic.VerifyAll();
    Assert.IsInstanceOfType(obtainedResult, typeof(NotFoundResult));
}

```

Lo que hicimos fue indicar que el retorno esperado será ```null```. En consecuencia, nuestro controller al llamar a este mock, recibirá una lista de razas ```null```. Esto har que nuestro ```Get()``` sobre el Controller retorne una BadRequest.

Finalmente entonces, verificamos que las expectativas se hayan cumplido (con el ```VerifyAll()```), y luego que el resultado obtenido sea un ```NotFoundResult```

## Probando los POST/PUT

Lo que haremos ahora será ver como probar nuestros endpoints que creen recursos, particularmente razas.

Para ello definimos una nueva función, ```CreateNewBreedTest```.

La diferencia respecto a los tests anteriores es que:

1) Usamos una función ```GetAFakeBreed()``` que nos devuelve una raza esperada. Esta ser la raza que agregaremos sobre nuestro mock de la lógica de negocio (```BreedsBusinessLogic```).

2) Nuestro retorno esperado será la ID de la raza que agregamos.

3) Nuestro resultado esperado será una respuesta HTTP del tipo CreatedAtRoute (por eso casteamos a ```CreatedAtRouteNegotiatedContentResult```)

4) Dentro de los asserts que hacemos, no solo verificamos que el objeto retornado sea el correcto, si no que se devuelva la ID, (nuestro método POST del Controller devuelve la id del objeto recién agregado). También verificamos que el template de rutas que estemos usando sea DefaultApi.


```c#
[TestMethod]
public void CreateNewBreedTest()
{
    //Arrange
    var fakeBreed = GetAFakeBreed();

    var mockBreedsBusinessLogic = new Mock<IBreedsBusinessLogic>();
    mockBreedsBusinessLogic
        .Setup(bl => bl.Add(fakeBreed))
        .Returns(fakeBreed.Id);

    var controller = new BreedsController(mockBreedsBusinessLogic.Object);

    //Act
    IHttpActionResult obtainedResult = controller.Post(fakeBreed);
    var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<Breed>;

    //Assert
    mockBreedsBusinessLogic.VerifyAll();
    Assert.IsNotNull(createdResult);
    Assert.AreEqual("DefaultApi", createdResult.RouteName);
    Assert.AreEqual(fakeBreed.Id, createdResult.RouteValues["id"]);
    Assert.AreEqual(fakeBreed, createdResult.Content);
}
```

Ahora probaremos aquellos casos en los que queremos crear un nuevo recurso que es null. Nuestro método del controller espera recibir una excepción del otro lado cuando esto sucede.
```C#
[TestMethod]
public void CreateNullBreedErrorTest()
{
    //Arrange
    Breed fakeBreed = null;

    var mockBreedsBusinessLogic = new Mock<IBreedsBusinessLogic>();
    mockBreedsBusinessLogic
        .Setup(bl => bl.Add(fakeBreed))
        .Throws(new ArgumentNullException());

    var controller = new BreedsController(mockBreedsBusinessLogic.Object);

    //Act
    IHttpActionResult obtainedResult = controller.Post(fakeBreed);

    //Assert
    mockBreedsBusinessLogic.VerifyAll();
    Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
}
```
## Probando DELETE (con HttpResponseMessage)

Probar métodos de un controller que devuelvan HttpResponseMessage es un poco más trabajoso, ya que hay que realizar ciertas configuraciones adicionales sobre el controller (por ejemplo: setearle la Request entrante -que al tratarse de una prueba creada por nosotros debemos crearla-).


```c#
[TestMethod]
public void DeleteBreedOkTest()
{
    //Arrange

    Guid fakeGuid = Guid.NewGuid();

    var mockBreedsBusinessLogic = new Mock<IBreedsBusinessLogic>();
    mockBreedsBusinessLogic
        .Setup(bl => bl.Delete(It.IsAny<Guid>()))
        .Returns(It.IsAny<bool>());

    var controller = new BreedsController(mockBreedsBusinessLogic.Object);
    // Configuramos la Request (dado que estamos utilziando HttpResponseMessage)
    // Y usando el objeto Request adentro.
    ConfigureHttpRequest(controller);

    //Act
    HttpResponseMessage obtainedResult = controller.Delete(fakeGuid);

    //Assert
    mockBreedsBusinessLogic.VerifyAll();
    Assert.IsNotNull(obtainedResult);
}

private void ConfigureHttpRequest(BreedsController controller)
{
    controller.Request = new HttpRequestMessage();
    controller.Configuration = new HttpConfiguration();
    controller.Configuration.Routes.MapHttpRoute(
        name: "DefaultApi",
        routeTemplate: "api/{controller}/{id}",
        defaults: new { id = RouteParameter.Optional });
}
```

Es interesante ver cómo usamos ```It.IsAny<T>()``` lo cual le indica al mock que está recibiendo un parámetro que sea cualquier cosa, pero del tipo T.

## Mockeando Headers

Seguramente para llevar a cabo autenticación en su Web Api les interese obtener los headers en los mensajes HTTP.

<http://stackoverflow.com/questions/21404734/how-to-add-and-get-header-values-in-webapi>

Mockeando los headers:

<http://stackoverflow.com/questions/9263457/how-do-i-make-a-unit-test-to-test-a-method-that-checks-request-headers>

## Documentacion de Moq

Aquí vimos un montón de formas de usar mocks y de funciones que tiene el framework, sin embargo hay muchísimas más. Para leer sobre las mismas, ver la documentación.

<https://github.com/Moq/moq4/wiki/Quickstart>
