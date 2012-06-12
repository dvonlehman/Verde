Verde
=======

##Overview

Verde is an integration test and application health verification framework for ASP.Net MVC. Integration tests are intended to automate verifying the different components of your application such as databases, web services, etc. are working together correctly. Unlike test runners integrated into Visual Studio or standalone applications, the Verde framework runs tests right within your web application, leveraging the same configuration settings, security permissions, network topology, etc. This allows exposing issues that may not otherwise be encountered until an end-user actually starts using the application. Verde is intended to complement a robust suite of unit tests; the unit tests ensure all the individual units of work function correctly, while integration tests ensure everything works together.

##Getting Started
Verde provides both a browser based GUI and a programmatic RESTful endpoint which could easily be incorporated into an auto-deployment or monitoring script. The framework is designed to be easily dropped into an existing MVC application with minimal configuration overhead. Basically all you need to do is add a reference to the Verde library and add a small bit to the **Application_Start** event in your global.asax.

```csharp
// Minimal setup
Verde.Setup.Initialize(new Verde.Settings
{
    TestsAssembly = System.Reflection.Assembly.GetExecutingAssembly()
});
```

The minimal setup requires you to only provide a reference to the assembly where your integration tests are defined.  In this case the tests are assumed to reside in the MVC Application itself, but they could just as easily be in a dedicated class library. There are several other settings you can override if you choose, but none are required. You could always wrap this statement in a conditional block so that the Verde framework is enabled based on a custom configuration setting. Additionally you can choose to authorize access to the Verde endpoints by providing a custom AuthorizationCheck delegate:

```csharp
Verde.Setup.Initialize(new Verde.Settings
{
    TestsAssembly = System.Reflection.Assembly.GetExecutingAssembly(),
    AuthorizationCheck = (context) =>
    {
        return context.User.IsInRole("admin");
    }
});
```

The Verde GUI is based on the [QUnit](http://docs.jquery.com/QUnit) JavaScript test framework. Custom ITestGuiRenderer instances are also supported.

![Screenshot](docs/gui_screenshot.png)

Now all that's left is to write some tests.

##Writing Tests
Verde integration tests are simply NUnit tests decorated with the IntegrationTest attribute declared within a class decorated by the IntegrationFixture attribute. When a test executes, it in turn creates a nested handler corresponding to the application URL being tested. After the  method completes, control is returned back to the current request. The bit that makes this possible is the [HttpServerUtility.Execute](http://msdn.microsoft.com/en-us/library/system.web.httpserverutility.execute.aspx) method. The Verde.Executor namespace defines a MvcExectorScope class that is used to define the scope of this nested execution.

The source code includes a fork of [Jon Galloway](http://weblogs.asp.net/jgalloway/)'s [MvcMusicStore sample app](http://mvcmusicstore.codeplex.com/) that has been augmented with Verde integration tests. Here are some examples:

### Basic ViewResult
In this test we get an arbitrary Album and define a MvcExecutorScope for the details page of that album. It is highly recommended that a __using__ statement be implemented to control the lifetime of the ExecutorScope. Inside the __using__ block, the *scope* variable provides us access to some helpful contextual objects like the HttpContext, Controller, ViewData, Action, and ResponseText that the test can make assertions against. The ScrappySharp library, in conjunction with the HtmlAgilityPack (both easily installable via NuGet), provides a CSS selector engine that allows the test to examine the contents of the HTML ResponseText in a more elegant way than brute force string matching.

```csharp
[IntegrationTest]
public void Index_Load_ExpectedHtml()
{
    // Get a product to load the details page for.
    var album = storeDB.Albums
        .Take(1)
        .First();

    using (var scope = new MvcExecutorScope("Store/Details/" + album.AlbumId))
    {
		Assert.AreEqual(200, scope.HttpContext.Response.StatusCode);
        Assert.IsTrue(scope.Controller is StoreController);
        Assert.AreEqual("Details", scope.Action);

        var model = scope.Controller.ViewData.Model as Album;
        Assert.IsNotNull(model);
        Assert.AreEqual(album.AlbumId, model.AlbumId);

        Assert.IsFalse(String.IsNullOrEmpty(scope.ResponseText));

        // Load the ResponseText into an HtmlDocument
        var html = new HtmlDocument();
        html.LoadHtml(scope.ResponseText);

        // Use ScrappySharp CSS selector to make assertions about the rendered HTML
        Assert.AreEqual(album.Title, html.DocumentNode.CssSelect("#main h2").First().InnerText);
    }
}
```
	
### RedirectAction
We can also test action results that do a redirect. 

```csharp
[IntegrationTest]
public void AddToCart_ValidItem_Succeeds()
{
    // Get a product to load the details page for.
    var album = storeDB.Albums
        .Take(1)
        .First();
    
    var settings = new ExecutorSettings("ShoppingCart/AddToCart/" + album.AlbumId) { 
        User = new GenericPrincipal(new GenericIdentity("GenghisKahn"), null) 
    };

    using (var scope = new MvcExecutorScope(settings))
    {
        Assert.AreEqual(302, scope.HttpContext.Response.StatusCode);
        Assert.AreEqual("/ShoppingCart", scope.HttpContext.Response.RedirectLocation);

        // Now verify that the cart contains the item we just added.
        var cart = MvcMusicStore.Models.ShoppingCart.GetCart(scope.HttpContext);
        var cartItems = cart.GetCartItems();
        Assert.AreEqual(1, cartItems.Count);
        Assert.AreEqual(album.AlbumId, cartItems[0].AlbumId);
                        
        // Finally clear the cart.
        cart.EmptyCart();
    }
}
```

### POST Request and JSON Responses
We can also test a POST request and Json action results.

```csharp
[IntegrationTest]
public void RemoveFromCart_ValidJson()
{
    // Add an item to the cart so we have something to remove.
    string userName = "JimmyHendrix";
    MvcMusicStore.Models.ShoppingCart cart = TestUtil.AddItemsToCart(
        userName, storeDB.Albums.Take(1));
    var recordId = cart.GetCartItems().First().RecordId;                       

    var settings = new ExecutorSettings("ShoppingCart/RemoveFromCart/" + recordId) 
    { 
        User = TestUtil.CreateUser(userName), 
        HttpMethod = "POST"
    };

    using (var scope = new MvcExecutorScope(settings))
    {
        Assert.AreEqual("application/json", scope.HttpContext.Response.ContentType);

		// Use JSON.Net to deserialize the response
        var deserializedResponse = JsonConvert.DeserializeObject<ShoppingCartRemoveViewModel>(scope.ResponseText);
        Assert.AreEqual(0.0d, deserializedResponse.CartTotal, "The shopping cart total should be $0.00.");
        Assert.AreEqual(0, deserializedResponse.ItemCount, "The shopping cart should have 0 items left.");
        Assert.AreEqual(recordId, deserializedResponse.DeleteId);
    }
}
```

##More Details

![Sequence Diagram](http://www.websequencediagrams.com/cgi-bin/cdraw?lz=dGl0bGUgVmVyZGUgU2VxdWVuY2UKCnBhcnRpY2lwYW50ICIAFwZHVUkgPGJyb3dzZXI-IiBhcyBndWkAHA1JbnRlZ3JhdGlvblRlc3RIYW5kbGVyIGFzIGgABQYASg1OVW5pdFRlc3RSdW5uACEGcgAFBQA8GEZpeHR1cmUgYXMgZgAFBgCBGg1NdmNFeGVjdXRvclNjb3AAIwVzY29wABURRnJhbWV3b3JrIGFzIG12YwCBYA1Zb3VyQ29udHJvbACBNQdjAAUJCgoKZ3VpLT4AgUcHOiAAagZlIFRlc3RzCgCBXgctPgCBQQYADxAKbG9vcCBmb3JlYWNoAIFFEwoAgXgGLT4AgVYHOiBDcmVhdGUKYWN0aXZhdGUAgW0JADEYVGVzdAA0Ekludm9rZQoAgiwHLT4Agg8FAEsSAIIlBgCCLAUtPm12YzogU2VydmVyLgCBXgcKbXZjLT4AggMKAFEIIEFjdGlvbgpub3RlIHJpZ2h0IG9mAIIpDCAgICAgICAgAIJLC2UAgjIGcyAAFAlqdXN0IGFzIGl0IGRvZXMgd2hlbgAxCWkAgT0FZCBieSBhIACEewcuAFMFZW5kIG5vdGUKAIMbCi0AgTcHcmV0dXJuAIEXB1Jlc3VsdACBPQYAgVcFAIMzBwCBJxUAhFUIICAgTWFrZSBBc3NlcnQgc3RhdGVtZW50cwoAbgkAgkUQRGlzcG9zZQpkZQCCSQ8ABgsAhTQIZW5kIGxvb3AAAQhwCgCDeggAhFcLAIE_BwCGIgUAgT8FAIRdCi0-Z3VpOgCBYAdKU09OIHIAGwcKCgoK&s=vs2010)





