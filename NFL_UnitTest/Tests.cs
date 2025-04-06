using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Moq.EntityFrameworkCore;

using SportsPro.Controllers;
using SportsPro.Models;

namespace NFL_UnitTest {
    public class Incident_Controller_Tests {
        // Public members available to every test.
        public List<Incident> Incidents { get; set; }
        public List<Customer> Customers { get; set; }
        public List<Product> Products { get; set; }
        public List<Technician> Technicians { get; set; }

        public Mock<IRepository<Incident>> MockIncidentRepo { get; set; }
        public Mock<IRepository<Customer>> MockCustomerRepo { get; set; }
        public Mock<IRepository<Product>> MockProductRepo { get; set; }
        public Mock<IRepository<Technician>> MockTechnicianRepo { get; set; }
        public Mock<IHttpContextAccessor> MockHttpContextAccessor { get; set; }

        public IncidentController Controller { get; set; }

        public Incident_Controller_Tests() {
            Incidents = new List<Incident>
            {
                new Incident
                {
                    IncidentID = 1,
                    Title = "Incident 1",
                    Customer = new Customer { CustomerID = 1, FirstName = "John", LastName = "Doe" },
                    Product = new Product { ProductID = 1, Name = "Product A" },
                    DateOpened = new DateTime(2022, 1, 1),
                    DateClosed = null
                },
                new Incident
                {
                    IncidentID = 2,
                    Title = "Incident 2",
                    Customer = new Customer { CustomerID = 2, FirstName = "Jane", LastName = "Doe" },
                    Product = new Product { ProductID = 2, Name = "Product B" },
                    DateOpened = new DateTime(2022, 2, 1),
                    DateClosed = null
                }
            };

            Customers = new List<Customer>
            {
                new Customer { CustomerID = 1, FirstName = "John", LastName = "Doe" },
                new Customer { CustomerID = 2, FirstName = "Jane", LastName = "Doe" }
            };

            Products = new List<Product>
            {
                new Product { ProductID = 1, Name = "Product A" },
                new Product { ProductID = 2, Name = "Product B" }
            };

            Technicians = new List<Technician>
            {
                new Technician { TechnicianID = 1, Name = "Tech One" },
                new Technician { TechnicianID = 2, Name = "Tech Two" }
            };

            // Set up repository mocks.
            MockIncidentRepo = new Mock<IRepository<Incident>>();
            MockIncidentRepo.Setup(repo => repo.GetAll()).Returns(Incidents);
            MockIncidentRepo.Setup(repo => repo.GetById(1)).Returns(Incidents.First());

            MockCustomerRepo = new Mock<IRepository<Customer>>();
            MockCustomerRepo.Setup(repo => repo.GetAll()).Returns(Customers);

            MockProductRepo = new Mock<IRepository<Product>>();
            MockProductRepo.Setup(repo => repo.GetAll()).Returns(Products);

            MockTechnicianRepo = new Mock<IRepository<Technician>>();
            MockTechnicianRepo.Setup(repo => repo.GetAll()).Returns(Technicians);

            var mockSession = new Mock<ISession>();
            mockSession.Setup(s => s.TryGetValue("TechnicianID", out It.Ref<byte[]>.IsAny)).Returns(false);

            mockSession.Setup(s => s.Remove(It.IsAny<string>()));

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(ctx => ctx.Session).Returns(mockSession.Object);

            var dummyRequestCookies = new Mock<IRequestCookieCollection>().Object;
            var dummyResponseCookies = new Mock<IResponseCookies>().Object;
            mockHttpContext.Setup(ctx => ctx.Request.Cookies).Returns(dummyRequestCookies);
            mockHttpContext.Setup(ctx => ctx.Response.Cookies).Returns(dummyResponseCookies);

            MockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            MockHttpContextAccessor.Setup(acc => acc.HttpContext).Returns(mockHttpContext.Object);

            Controller = new IncidentController(
                MockIncidentRepo.Object,
                MockCustomerRepo.Object,
                MockProductRepo.Object,
                MockTechnicianRepo.Object,
                MockHttpContextAccessor.Object
            );
        }
        [Fact]
        public void List_Action_All_Filter_Test() {
            // Act
            var result = Controller.List("all");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IncidentListViewModel>(viewResult.Model);
            Assert.Equal("all", model.Filter);
            Assert.Equal(Incidents.Count, model.Incidents.Count);
        }
        [Fact]
        public void List_Action_Filter_Unassigned_Test() {
            // Arrange: ensure all incidents have TechnicianID null.
            foreach (var inc in Incidents)
            {
                inc.TechnicianID = null;
            }

            // Act
            var result = Controller.List("unassigned");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IncidentListViewModel>(viewResult.Model);
            Assert.Equal("unassigned", model.Filter);
            Assert.Equal(Incidents.Count, model.Incidents.Count);
        }
        [Fact]
        public void List_Action_Filter_Open_Test() {
            // Arrange: mark first incident as closed.
            Incidents[0].DateClosed = DateTime.Now;

            // Act
            var result = Controller.List("open");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IncidentListViewModel>(viewResult.Model);
            Assert.Equal("open", model.Filter);
            // Only the second incident is open.
            Assert.Single(model.Incidents);
        }

        [Fact]
        public void Add_Action_Test() {
            // Act
            var result = Controller.Add();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AddEdit", viewResult.ViewName);
            var model = Assert.IsAssignableFrom<IncidentEditViewModel>(viewResult.Model);
            Assert.Equal("Add", model.Mode);
            Assert.NotNull(model.Incident);
            Assert.NotNull(model.Customers);
            Assert.NotNull(model.Products);
            Assert.NotNull(model.Technicians);
        }
        [Fact]
        public void Edit_Get_Action_Test() {
            // Act
            var result = Controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AddEdit", viewResult.ViewName);
            var model = Assert.IsAssignableFrom<IncidentEditViewModel>(viewResult.Model);
            Assert.Equal("Edit", model.Mode);
            Assert.NotNull(model.Incident);
            Assert.Equal(1, model.Incident.IncidentID);
        }
        [Fact]
        public void Edit_Post_Action_Valid_NewIncident_Test() {
            // Arrange: create a new incident with ProductID == 0.
            var newIncident = new Incident { IncidentID = 0, Product = new Product { ProductID = 0 } };
            Controller.ModelState.Clear();

            var editModel = new IncidentEditViewModel
            {
                Mode = "Add",
                Incident = newIncident,
                Customers = Customers,
                Products = Products,
                Technicians = Technicians
            };

            // Act
            var result = Controller.Edit(editModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
            // Verify Add was called.
            MockIncidentRepo.Verify(repo => repo.Add(newIncident), Times.Once);
        }

        [Fact]
        public void Edit_Post_Action_Valid_ExistingIncident_Test() {
            // Arrange: use an existing incident (non-zero ProductID).
            var existingIncident = Incidents.First();
            existingIncident.ProductID = 1;
            existingIncident.Product = new Product { ProductID = 1 }; // non-zero
            Controller.ModelState.Clear();

            var editModel = new IncidentEditViewModel
            {
                Mode = "Edit",
                Incident = existingIncident,
                Customers = Customers,
                Products = Products,
                Technicians = Technicians
            };

            // Act
            var result = Controller.Edit(editModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
            // Verify Update was called.
            MockIncidentRepo.Verify(repo => repo.Update(existingIncident), Times.Once);
        }
        [Fact]
        public void Edit_Post_Action_InvalidModel_Test() {
            // Arrange: add error to ModelState.
            var incident = Incidents.First();
            Controller.ModelState.AddModelError("error", "Invalid");

            var editModel = new IncidentEditViewModel
            {
                Mode = "Edit",
                Incident = incident,
                Customers = Customers,
                Products = Products,
                Technicians = Technicians
            };

            // Act
            var result = Controller.Edit(editModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IncidentEditViewModel>(viewResult.Model);
            // For an invalid model, Mode should be set to "Save".
            Assert.Equal("Save", model.Mode);
        }
        [Fact]
        public void Delete_Get_Action_Test() {
            // Act
            var result = Controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Incident>(viewResult.Model);
            Assert.Equal(1, model.IncidentID);
        }

        [Fact]
        public void Delete_Post_Action_Test() {
            // Arrange: select an incident to delete.
            var incidentToDelete = Incidents.First();

            // Act
            var result = Controller.Delete(incidentToDelete);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
            // Verify that Delete was called.
            MockIncidentRepo.Verify(repo => repo.Delete(incidentToDelete.IncidentID), Times.Once);
        }
        [Fact]
        public void SelectTech_Action_Test() {
            // Act
            var result = Controller.SelectTech();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("TechIncident", viewResult.ViewName);
            var model = Assert.IsAssignableFrom<TechIncidentViewModel>(viewResult.Model);
            Assert.NotEmpty(model.Technicians);
            Assert.NotEmpty(model.Incidents);
        }
        [Fact]
        public void ListByTech_Get_Action_Test() {
            // Arrange: simulate that all incidents are assigned to technician with ID 1 and are open.
            foreach (var inc in Incidents)
            {
                inc.TechnicianID = 1;
                inc.DateClosed = null;
            }

            // Act
            var result = Controller.ListByTech(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("TechIncidentList", viewResult.ViewName);
            var model = Assert.IsAssignableFrom<TechIncidentViewModel>(viewResult.Model);
            // In our dummy model, SelectedTech is newly created in the action, so check that Incidents are filtered.
            Assert.NotEmpty(model.Incidents);
        }
        [Fact]
        public void ListByTech_Post_Action_Test() {
            // Arrange: create a TechIncidentViewModel with a selected technician.
            var selectedTech = Technicians.First();
            var techModel = new TechIncidentViewModel
            {
                SelectedTech = selectedTech,
                Incidents = Incidents.Where(i => i.TechnicianID == selectedTech.TechnicianID && i.DateClosed == null).ToList(),
                Technicians = Technicians
            };

            // Simulate session contains the TechnicianID.
            var mockSession = new Mock<ISession>();
            byte[] techIdBytes = BitConverter.GetBytes(selectedTech.TechnicianID);
            mockSession.Setup(s => s.TryGetValue("TechnicianID", out techIdBytes)).Returns(true);
            mockSession.Setup(s => s.Remove(It.IsAny<string>()));
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(ctx => ctx.Session).Returns(mockSession.Object);

            var dummyRequestCookies = new Mock<IRequestCookieCollection>().Object;
            var dummyResponseCookies = new Mock<IResponseCookies>().Object;
            mockHttpContext.Setup(ctx => ctx.Request.Cookies).Returns(dummyRequestCookies);
            mockHttpContext.Setup(ctx => ctx.Response.Cookies).Returns(dummyResponseCookies);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            MockHttpContextAccessor.Setup(acc => acc.HttpContext).Returns(mockHttpContext.Object);

            // Reinitialize Controller to pick up new HttpContext.
            Controller = new IncidentController(
                MockIncidentRepo.Object,
                MockCustomerRepo.Object,
                MockProductRepo.Object,
                MockTechnicianRepo.Object,
                MockHttpContextAccessor.Object
            );

            // Act
            var result = Controller.ListByTech(techModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("TechIncidentList", viewResult.ViewName);
            var model = Assert.IsAssignableFrom<TechIncidentViewModel>(viewResult.Model);
            Assert.Equal(selectedTech.TechnicianID, model.SelectedTech.TechnicianID);
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
            MockCustomerRepo.Setup(repo => repo.GetById(1)).Returns(Customers.First());

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
        public List<Product> Products { get; set; }
        public Mock<IRepository<Product>> MockProductRepo { get; set; }
        public ProductController Controller { get; set; }

        // Constructor: common initialization.
        public Product_Controller_Tests() {
            // Create dummy product data.
            Products = new List<Product>
            {
                new Product { ProductID = 1, Name = "Product A", ReleaseDate = new DateTime(2020, 1, 1) },
                new Product { ProductID = 2, Name = "Product B", ReleaseDate = new DateTime(2021, 1, 1) }
            };

            // Set up repository mock.
            MockProductRepo = new Mock<IRepository<Product>>();
            MockProductRepo.Setup(repo => repo.GetAll()).Returns(Products);
            MockProductRepo.Setup(repo => repo.GetById(1)).Returns(Products.First());

            // Initialize the controller.
            Controller = new ProductController(MockProductRepo.Object);
            Controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        }

        [Fact]
        public void List_Action_Test() {
            // Act
            var result = Controller.List();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);
            // Ensure the product list is sorted by ReleaseDate.
            var expectedOrder = Products.OrderBy(p => p.ReleaseDate).ToList();
            Assert.Equal(expectedOrder.Select(p => p.ProductID), model.Select(p => p.ProductID));
        }

        [Fact]
        public void Add_Action_Test() {
            // Act
            var result = Controller.Add();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            // The Add action returns the "AddEdit" view.
            Assert.Equal("AddEdit", viewResult.ViewName);
            var model = Assert.IsAssignableFrom<Product>(viewResult.Model);
            // A new Product should have a default ID (0).
            Assert.Equal(0, model.ProductID);
            // Verify ViewBag.Action is set to "Add".
            Assert.Equal("Add", Controller.ViewBag.Action);
        }

        [Fact]
        public void Edit_Get_Action_Test() {
            // Act: Call GET Edit for ProductID 1.
            var result = Controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            // The GET Edit action returns the "AddEdit" view.
            Assert.Equal("AddEdit", viewResult.ViewName);
            var model = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(1, model.ProductID);
            // Verify ViewBag.Action is set to "Edit".
            Assert.Equal("Edit", Controller.ViewBag.Action);
        }

        [Fact]
        public void Edit_Post_Action_Valid_NewProduct_Test() {
            // Arrange: create a new product (ProductID == 0).
            var newProduct = new Product { ProductID = 0, Name = "New Product", ReleaseDate = new DateTime(2022, 1, 1) };
            Controller.ModelState.Clear(); // Ensure ModelState is valid.
            Controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            // Act: call the POST Edit action.
            var result = Controller.Edit(newProduct);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
            // Verify that Add was called.
            MockProductRepo.Verify(repo => repo.Add(newProduct), Times.Once);
        }

        [Fact]
        public void Edit_Post_Action_Valid_ExistingProduct_Test() {
            // Arrange: use an existing product (ProductID != 0).
            var existingProduct = Products.First();
            existingProduct.Name = "Updated Product";
            Controller.ModelState.Clear();
            Controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            // Act: call the POST Edit action.
            var result = Controller.Edit(existingProduct);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
            // Verify that Update was called.
            MockProductRepo.Verify(repo => repo.Update(existingProduct), Times.Once);
        }

        [Fact]
        public void Edit_Post_InvalidModelState_Test() {
            // Arrange: create a product with an invalid model state.
            var product = new Product { ProductID = 1, Name = "Invalid", ReleaseDate = new DateTime(2020, 1, 1) };
            Controller.ModelState.AddModelError("error", "Invalid model");

            // Act: call the POST Edit action.
            var result = Controller.Edit(product);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Delete_Get_Action_Test() {
            // Act: Call GET Delete for ProductID 1.
            var result = Controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(1, model.ProductID);
        }

        [Fact]
        public void Delete_Post_Action_Test() {
            // Arrange: select a product to delete.
            var productToDelete = Products.First();

            // Act: call the POST Delete action.
            var result = Controller.Delete(productToDelete);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
            // Verify that Delete was called.
            MockProductRepo.Verify(repo => repo.Delete(productToDelete.ProductID), Times.Once);
        }


    }
    public class Technician_Controller_Tests {
        public List<Technician> Technicians { get; set; }
        public Mock<IRepository<Technician>> MockTechnicianRepo { get; set; }
        public TechnicianController Controller { get; set; }
        public Technician_Controller_Tests() {
            // Initialize dummy technician data.
            Technicians = new List<Technician>
            {
                new Technician { TechnicianID = 1, Name = "Tech One", Email = "tech1@example.com", Phone = "123-456-7890" },
                new Technician { TechnicianID = 2, Name = "Tech Two", Email = "tech2@example.com", Phone = "234-567-8901" }
            };

            // Set up repository mock.
            MockTechnicianRepo = new Mock<IRepository<Technician>>();
            MockTechnicianRepo.Setup(repo => repo.GetAll()).Returns(Technicians);
            MockTechnicianRepo.Setup(repo => repo.GetById(1)).Returns(Technicians.First(t => t.TechnicianID == 1));

            // Initialize the controller.
            Controller = new TechnicianController(MockTechnicianRepo.Object);
        }

        [Fact]
        public void List_Action_Test() {
            // Act
            var result = Controller.List();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Technician>>(viewResult.Model);
            Assert.Equal(Technicians.Count, model.Count);
        }

        [Fact]
        public void Add_Get_Action_Test() {
            // Act
            var result = Controller.Add();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            // Expect the view name to be "Edit".
            Assert.Equal("Edit", viewResult.ViewName);
            var model = Assert.IsAssignableFrom<TechEditViewModel>(viewResult.Model);
            Assert.Equal("Add", model.Mode);
            Assert.NotNull(model.Technician);
        }

        [Fact]
        public void Add_Post_Action_Valid_Test() {

            var newTech = new Technician
            {
                TechnicianID = 0,
                Name = "New Tech",
                Email = "newtech@example.com",
                Phone = "345-678-9012"
            };
            var model = new TechEditViewModel
            {
                Mode = "Add",
                Technician = newTech
            };

            Controller.ModelState.Clear(); 

            // Act
            var result = Controller.Add(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
            // Verify repository Add is called.
            MockTechnicianRepo.Verify(repo => repo.Add(newTech), Times.Once);
        }
        [Fact]
        public void Add_Post_Action_Invalid_Test() {

            var newTech = new Technician
            {
                TechnicianID = 0,
                Name = "New Tech",
                Email = "bad-email", // invalid
                Phone = "345-678-9012"
            };
            var model = new TechEditViewModel
            {
                Mode = "Add",
                Technician = newTech
            };

            Controller.ModelState.Clear(); 

            // Act
            var result = Controller.Add(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Edit", viewResult.ViewName);
            var returnedModel = Assert.IsAssignableFrom<TechEditViewModel>(viewResult.Model);
            Assert.Equal("Add", returnedModel.Mode);
            Assert.False(Controller.ModelState.IsValid);
        }

    }

}
