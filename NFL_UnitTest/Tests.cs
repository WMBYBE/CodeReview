using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.EntityFrameworkCore;

using SportsPro.Controllers;
using SportsPro.Models;

namespace NFL_UnitTest {
    public class Incident_Controller_ControllerTests {
        [Fact]
        public void List_Action_Test() {
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
        public List<Customer> Customers { get; set; }
        public List<Country> Countries { get; set; }
        public Mock<IRepository<Customer>> MockCustomerRepo { get; set; }
        public Mock<IRepository<Country>> MockCountryRepo { get; set; }
        public CustomerController Controller { get; set; }
        public Customer_Controller_ControllerTests() {
            // Set up common dummy data.
            Customers = new List<Customer>
            {
                new Customer { CustomerID = 1, FirstName = "John", LastName = "Doe" },
                new Customer { CustomerID = 2, FirstName = "John", LastName = "Doe" }
            };

            Countries = new List<Country>
            {
                new Country { CountryID = "USA", Name = "United States of America" },
                new Country { CountryID = "CA", Name = "Canada" }
            };

            // Set up repository mocks.
            MockCustomerRepo = new Mock<IRepository<Customer>>();
            MockCustomerRepo.Setup(repo => repo.GetAll()).Returns(Customers);
            // For the Edit action, GetById(1) should return the first customer.
            MockCustomerRepo.Setup(repo => repo.GetById(1)).Returns(Customers[0]);

            MockCountryRepo = new Mock<IRepository<Country>>();
            MockCountryRepo.Setup(repo => repo.GetAll()).Returns(Countries);

            // Initialize the controller with the mocks.
            Controller = new CustomerController(MockCustomerRepo.Object, MockCountryRepo.Object);
        }

        [Fact]
        public void List_Action_Tests() {

            // Act
            var result = Controller.List();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Customer>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void Add_Action_Test() {
            // Arrange
            Customers.Clear();
            MockCustomerRepo.Setup(repo => repo.GetAll()).Returns(Customers);

            Controller = new CustomerController(MockCustomerRepo.Object, MockCountryRepo.Object);

            // Act
            var result = Controller.Add();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            // The Add action returns the "Edit" view.
            Assert.Equal("Edit", viewResult.ViewName);
            var model = Assert.IsAssignableFrom<Customer>(viewResult.Model);
            Assert.Equal(0, model.CustomerID);
            // Check that ViewBag.Countries contains our countries.
            var viewBagCountries = Controller.ViewBag.Countries as List<Country>;
            Assert.NotNull(viewBagCountries);
            Assert.Equal(2, viewBagCountries.Count);
        }

        [Fact]
         public void Edit_Get_Action_Test() {
            Countries = new List<Country>
            {
                new Country { CountryID = "USA", Name = "United States of America" }
            };
            MockCountryRepo.Setup(repo => repo.GetAll()).Returns(Countries);

            Controller = new CustomerController(MockCustomerRepo.Object, MockCountryRepo.Object);


            // Act
            var result = Controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Customer>(viewResult.Model);
            Assert.Equal(1, model.CustomerID);
            // Check ViewBag.Action
            Assert.Equal("Edit", Controller.ViewBag.Action);
            // Ensure that ViewBag.Countries is set.
            var viewBagCountries = Controller.ViewBag.Countries as List<Country>;
            Assert.NotNull(viewBagCountries);
            Assert.Single(viewBagCountries);
        }
        [Fact]
        public void Edit_Post_Action_New_Customer_Test() {

            // Arrange: Create a new customer (CustomerID == 0).
            var newCustomer = new Customer { CustomerID = 0, FirstName = "New", LastName = "Customer" };

            // Ensure model state is valid.
            Controller.ModelState.Clear();

            // Act
            var result = Controller.Edit(newCustomer);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
            // Verify that the repository's Add method was called.
            MockCustomerRepo.Verify(repo => repo.Add(newCustomer), Times.Once);
        }
        [Fact]
        public void Edit_Post_Action_Existing_Customer_Test() {
            // Arrange: Use an existing customer (CustomerID != 0).
            var existingCustomer = Customers.First(c => c.CustomerID == 1);
            existingCustomer.FirstName = "Updated";
            Controller.ModelState.Clear();

            // Act
            var result = Controller.Edit(existingCustomer);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
            // Verify that the repository's Update method was called.
            MockCustomerRepo.Verify(repo => repo.Update(existingCustomer), Times.Once);
        }
        [Fact]
        public void Edit_Post_Action_InvalidModelState_Test() {
            // Arrange: Create a customer and add an error to the ModelState.
            var customer = new Customer { CustomerID = 1, FirstName = "Invalid", LastName = "Test" };
            Controller.ModelState.AddModelError("error", "error message");

            // Act
            var result = Controller.Edit(customer);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Customer>(viewResult.Model);
            Assert.Equal(customer.CustomerID, model.CustomerID);
            // For an existing customer, the ViewBag.Action should be "Edit".
            Assert.Equal("Edit", Controller.ViewBag.Action);
            // Ensure that ViewBag.Countries is set.
            var viewBagCountries = Controller.ViewBag.Countries as List<Country>;
            Assert.NotNull(viewBagCountries);
            Assert.Equal(2, viewBagCountries.Count);
        }
        [Fact]
        public void Delete_Get_Action_Test() {
            // Act
            var result = Controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Customer>(viewResult.Model);
            Assert.Equal(1, model.CustomerID);
            // Check that ViewBag.Action is set to "Delete".
            Assert.Equal("Delete", Controller.ViewBag.Action);
        }
        [Fact]
        public void Delete_Post_Action_Test() {

            var customerToDelete = Customers.First(c => c.CustomerID == 1);

            var remainingCustomers = Customers.Where(c => c.CustomerID != 1).ToList();
            MockCustomerRepo.Setup(repo => repo.GetAll()).Returns(remainingCustomers);

            // Act
            var result = Controller.Delete(customerToDelete);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            // The POST Delete returns the "list" view.
            Assert.Equal("list", viewResult.ViewName.ToLower());
            var model = Assert.IsAssignableFrom<List<Customer>>(viewResult.Model);
            Assert.Equal(remainingCustomers.Count, model.Count);
            // Verify that Delete was called.
            MockCustomerRepo.Verify(repo => repo.Delete(customerToDelete.CustomerID), Times.Once);
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
