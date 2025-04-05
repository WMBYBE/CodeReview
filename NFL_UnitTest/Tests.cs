using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.EntityFrameworkCore;

using SportsPro.Controllers;
using SportsPro.Models;

namespace NFL_UnitTest {
    public class Incident_Controller_ControllerTests {
        [Fact]
        public void List_Returns_ViewResult_With_Filtered_Customer_List() {
            // Arrange

            // Create dummy data for repositories.
            var dummyIncident = new Incident
            {
                IncidentID = 1,
                Title = "Test Incident",
                Customer = new Customer { CustomerID = 1, FirstName = "John", LastName = "Doe" },
                Product = new Product { ProductID = 1, Name = "Test Product" },
                DateOpened = DateTime.Now
            };
            var incidents = new List<Incident> { dummyIncident };

            var customers = new List<Customer>
            {
                new Customer { CustomerID = 1, FirstName = "John", LastName = "Doe" }
            };

            var products = new List<Product>
            {
                new Product { ProductID = 1, Name = "Test Product" }
            };

            var technicians = new List<Technician>
            {
                new Technician { TechnicianID = 1, Name = "Tech One" }
            };

            // Set up repository mocks.
            var mockIncidentRepo = new Mock<IRepository<Incident>>();
            mockIncidentRepo.Setup(repo => repo.GetAll()).Returns(incidents);

            var mockCustomerRepo = new Mock<IRepository<Customer>>();
            mockCustomerRepo.Setup(repo => repo.GetAll()).Returns(customers);

            var mockProductRepo = new Mock<IRepository<Product>>();
            mockProductRepo.Setup(repo => repo.GetAll()).Returns(products);

            var mockTechnicianRepo = new Mock<IRepository<Technician>>();
            mockTechnicianRepo.Setup(repo => repo.GetAll()).Returns(technicians);


            var sessionMock = new Mock<ISession>();

            sessionMock.Setup(s => s.TryGetValue("TechnicianID", out It.Ref<byte[]>.IsAny))
                       .Returns(false);

            sessionMock.Setup(s => s.Remove(It.IsAny<string>()));

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(ctx => ctx.Session).Returns(sessionMock.Object);

            var requestCookiesMock = new Mock<IRequestCookieCollection>();
            var responseCookiesMock = new Mock<IResponseCookies>();
            httpContextMock.Setup(ctx => ctx.Request.Cookies).Returns(requestCookiesMock.Object);
            httpContextMock.Setup(ctx => ctx.Response.Cookies).Returns(responseCookiesMock.Object);

            // Set up IHttpContextAccessor to return our mock HttpContext.
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(acc => acc.HttpContext).Returns(httpContextMock.Object);

            // Create the IncidentController.
            var controller = new IncidentController(
                mockIncidentRepo.Object,
                mockCustomerRepo.Object,
                mockProductRepo.Object,
                mockTechnicianRepo.Object,
                httpContextAccessorMock.Object
            );

            // Act: Call the List action with the default "all" filter.
            var result = controller.List("all");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IncidentListViewModel>(viewResult.Model);

            // Verify that the model contains the expected data.
            Assert.Equal("all", model.Filter);
            Assert.Single(model.Incidents);
            Assert.Equal("Test Incident", model.Incidents.First().Title);
            Assert.Equal("John Doe", model.Incidents.First().CustomerName);
            Assert.Equal("Test Product", model.Incidents.First().ProductName);
        }
    }
    public class Customer_Controller_ControllerTests {

        [Fact]
        public void List_Action_Tests() {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer { CustomerID = 1, FirstName = "John", LastName = "Doe" },
                new Customer { CustomerID = 2, FirstName = "John", LastName = "Doe" }
            };

            var countries = new List<Country>
            {
                new Country { CountryID = "USA", Name = "United States of America" },
                new Country { CountryID = "CA", Name = "Canada" }
            };

            // Create a mock context and set up its DbSet properties.
            var mockContext = new Mock<SportsProContext>();
            mockContext.Setup(c => c.Customers).ReturnsDbSet(customers);
            mockContext.Setup(c => c.Countries).ReturnsDbSet(countries);

            // The repository is not used in these actions, but is required by the constructor.
            var mockRepo = new Mock<IRepository<Customer>>();

            var controller = new CustomerController(mockContext.Object, mockRepo.Object);

            // Act
            var result = controller.List();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Customer>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void Add_Returns_Edit_View_With_New_Customer_And_Countries() {
            // Arrange
            var customers = new List<Customer>(); // empty customer list
            var countries = new List<Country>
            {
                new Country { CountryID = "USA", Name = "United States of America" },
                new Country { CountryID = "CA", Name = "Canada" }
            };


            var mockCountryRepo = new Mock<IRepository<Country>>();
            mockCountryRepo.Setup(repo => repo.GetAll()).Returns(countries);

            var mockCustomerRepo = new Mock<IRepository<Customer>>();
            mockCustomerRepo.Setup(repo => repo.GetAll()).Returns(customers);

            var controller = new CustomerController(mockContext.Object, mockCustomerRepo.Object);

            // Act
            var result = controller.Add();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            // The Add action returns the "Edit" view.
            Assert.Equal("Edit", viewResult.ViewName);
            var model = Assert.IsAssignableFrom<Customer>(viewResult.Model);
            Assert.Equal(0, model.CustomerID);
            // Check that ViewBag.Countries contains our countries.
            var viewBagCountries = controller.ViewBag.Countries as List<Country>;
            Assert.NotNull(viewBagCountries);
            Assert.Equal(2, viewBagCountries.Count);
        }

        [Fact]
         public void Edit_Get_Returns_Edit_View_With_Customer() {
            // Arrange
            var customer = new Customer { CustomerID = 1, FirstName = "John", LastName = "Doe" };
            var customers = new List<Customer> { customer };
            var countries = new List<Country>
            {
                new Country { CountryID = "USA", Name = "United States of America" }
            };

            var mockContext = new Mock<SportsProContext>();
            mockContext.Setup(c => c.Customers).ReturnsDbSet(customers);
            mockContext.Setup(c => c.Countries).ReturnsDbSet(countries);

            var mockRepo = new Mock<IRepository<Customer>>();
            var controller = new CustomerController(mockContext.Object, mockRepo.Object);

            // Act
            var result = controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Customer>(viewResult.Model);
            Assert.Equal(1, model.CustomerID);
            // Check ViewBag.Action
            Assert.Equal("Edit", controller.ViewBag.Action);
            // Ensure that ViewBag.Countries is set.
            var viewBagCountries = controller.ViewBag.Countries as List<Country>;
            Assert.NotNull(viewBagCountries);
            Assert.Single(viewBagCountries);
        }


    }
    public class Product_Controller_Tests {
        [Fact]
        public void List_Returns_ViewResult_With_Product_List() {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductID = 1, Name = "Product A", ReleaseDate = new DateTime(2020, 1, 1) },
                new Product { ProductID = 2, Name = "Product B", ReleaseDate = new DateTime(2021, 1, 1) }
            };

            var mockRepo = new Mock<IRepository<Product>>();
            mockRepo.Setup(r => r.GetAll()).Returns(products);

            var controller = new ProductController(mockRepo.Object);

            // Act
            var result = controller.List();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            Assert.Equal(2, model.Count);
            Assert.Equal("Product A", model.First().Name);
        }

        [Fact]
        public void Add_Returns_AddEdit_View_With_New_Product() {
            // Arrange
            var mockRepo = new Mock<IRepository<Product>>();
            var controller = new ProductController(mockRepo.Object);

            // Act
            var result = controller.Add();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AddEdit", viewResult.ViewName);
            var model = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(0, model.ProductID);
            Assert.Equal("Add", controller.ViewBag.Action);
        }

        [Fact]
        public void Edit_Get_Returns_AddEdit_View_With_Product() {
            // Arrange
            var product = new Product { ProductID = 1, Name = "Product A", ReleaseDate = new DateTime(2020, 1, 1) };
            var mockRepo = new Mock<IRepository<Product>>();
            mockRepo.Setup(r => r.GetById(1)).Returns(product);

            var controller = new ProductController(mockRepo.Object);

            // Act
            var result = controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AddEdit", viewResult.ViewName);
            var model = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(1, model.ProductID);
            Assert.Equal("Edit", controller.ViewBag.Action);
        }

        [Fact]
        public void Edit_Post_Redirects_To_List_When_ModelState_Is_Valid() {
            // Arrange
            var product = new Product { ProductID = 0, Name = "New Product", ReleaseDate = new DateTime(2022, 1, 1) };
            var mockRepo = new Mock<IRepository<Product>>();
            // Setup repository Add as a no-op.
            mockRepo.Setup(r => r.Add(product));
            var controller = new ProductController(mockRepo.Object);
            controller.ModelState.Clear(); // Ensure ModelState is valid

            // Act
            var result = controller.Edit(product);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
        }

        [Fact]
        public void Delete_Get_Returns_View_With_Product() {
            // Arrange
            var product = new Product { ProductID = 1, Name = "Product A", ReleaseDate = new DateTime(2020, 1, 1) };
            var mockRepo = new Mock<IRepository<Product>>();
            mockRepo.Setup(r => r.GetById(1)).Returns(product);
            var controller = new ProductController(mockRepo.Object);

            // Act
            var result = controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(1, model.ProductID);
        }

        [Fact]
        public void Delete_Post_Redirects_To_List() {
            // Arrange
            var product = new Product { ProductID = 1, Name = "Product A", ReleaseDate = new DateTime(2020, 1, 1) };
            var mockRepo = new Mock<IRepository<Product>>();
            mockRepo.Setup(r => r.Delete(product.ProductID));
            var controller = new ProductController(mockRepo.Object);

            // Act
            var result = controller.Delete(product);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
        }
    }
}
