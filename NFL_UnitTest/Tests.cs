using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsPro.Controllers;
using SportsPro.Models;
using Xunit;

namespace NFL_UnitTest {
    public class IncidentControllerTests {
        [Fact]
        public void Incident_Controller_Test() {
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

            // Set up a mock ISession.
            var sessionMock = new Mock<ISession>();
            // Instead of setting up GetInt32 (an extension method), we set up TryGetValue.
            // Using It.Ref<byte[]>.IsAny tells Moq to ignore the out parameter value.
            sessionMock.Setup(s => s.TryGetValue("TechnicianID", out It.Ref<byte[]>.IsAny))
                       .Returns(false);
            // Setup Remove as a no-op.
            sessionMock.Setup(s => s.Remove(It.IsAny<string>()));

            // Set up a mock HttpContext that returns our mock session.
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(ctx => ctx.Session).Returns(sessionMock.Object);
            // Setup dummy cookie collections.
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
}
