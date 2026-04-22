using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebApplication2.Controllers;
using WebApplication2.model;
using Xunit;

namespace AppTests
{
    public  class IncidentsTests
    {
        private IncidentsDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<IncidentsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
            return new IncidentsDbContext(options);
        }

            [Fact]
        public async Task GetIncidents_WhenDataExists_ReturnsAllIncidents()
        {
            var context = GetDbContext();
            context.Incidents.AddRange(
            new Incident { title = "Incident1", status = "OPEN", severity = "HIGH" },
            new Incident { title = "Incident2", status = "CLOSED", severity = "LOW" }
            );
            context.SaveChanges();
            var controller = new IncidentsDbController(context);
            var result = await controller.GetIncidents();
            var incidents = Assert.IsType<List<Incident>>(result.Value);
            Assert.Equal(2, incidents.Count);
        }


        [Fact]
        public async Task GetIncident_ExistingId_ReturnsIncident()
        {
            var context = GetDbContext();
            var incident = new Incident { id = 1, title = "Test", status = "OPEN" };
            context.Incidents.Add(incident);
            context.SaveChanges();
            var controller = new IncidentsDbController(context);
            var result = await controller.GetIncident(1);
            Assert.NotNull(result.Value);
            Assert.Equal("Test", result.Value.title);
        }



        [Fact]
        public async Task PostIncident_ValidData_CreatesIncident()
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);
            var incident = new Incident { title = "NewIncident", status = "OPEN" };
            var result = await controller.PostIncident(incident);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdIncident = Assert.IsType<Incident>(createdResult.Value);
            Assert.Equal("NewIncident", createdIncident.title);
            Assert.Equal(1, context.Incidents.Count());
        }



        [Fact]
        public async Task PutIncident_ValidUpdate_ReturnsNoContent()
        {
            var context = GetDbContext();
            var incident = new Incident { id = 1, title = "Old", status = "OPEN" };
            context.Incidents.Add(incident);
            context.SaveChanges();
            var controller = new IncidentsDbController(context);
            incident.title = "Updated";
            var result = await controller.PutIncident(1, incident);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Updated", context.Incidents.First().title);
        }
        [Fact]
        public async Task PutIncident_IdMismatch_ReturnsBadRequest()
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);
            var incident = new Incident { id = 2, title = "Test" };
            var result = await controller.PutIncident(1, incident);
            Assert.IsType<BadRequestResult>(result);
        }


        [Fact]
        public async Task DeleteIncident_ExistingId_RemovesIncident()
        {
            var context = GetDbContext();
            var incident = new Incident { id = 1, title = "DeleteMe" };
            context.Incidents.Add(incident);
            context.SaveChanges();
            var controller = new IncidentsDbController(context);
            var result = await controller.DeleteIncident(1);
            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Incidents);
        }
        [Fact]
        public async Task DeleteIncident_NotFound_ReturnsNotFound()
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);
            var result = await controller.DeleteIncident(99);
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task FilterBySeverityAsync_ValidSeverity_ReturnsFilteredData()
        {
            var context = GetDbContext();
            context.Incidents.AddRange(
            new Incident { title = "A", severity = "HIGH" },
            new Incident { title = "B", severity = "LOW" }
            );
            context.SaveChanges();
            var controller = new IncidentsDbController(context);
            var result = await controller.FilterBySeverityAsync("HIGH");
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<List<Incident>>(okResult.Value);
            Assert.Single(data);
        }



        [Fact]
        public async Task PostIncident_MissingTitle_ReturnsBadRequest()
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);


            var incident = new Incident
            {
                status = "OPEN",
                severity = "HIGH"
            };
            controller.ModelState.AddModelError("Title", "Required");
            var result = await controller.PostIncident(incident);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }


        [Theory]
        [InlineData("Low")]
        [InlineData("Medium")]
        [InlineData("High")]
        [InlineData("Critical")]
        public async Task PostIncident_ValidSeverity_ReturnsCreated(string severity)
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);
            var incident = new Incident
            {
                title = "Test",
                status = "OPEN",
                severity = severity
            };
            var result = await controller.PostIncident(incident);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdIncident = Assert.IsType<Incident>(createdResult.Value);
            Assert.Equal(severity, createdIncident.severity);
        }

        [Theory]
        [InlineData("Abc")]
        [InlineData("Azerty")]
        [InlineData("123")]
        [InlineData("")]
        public async Task PostIncident_InvalidSeverity_ReturnsBadRequest(string severity)
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);
            var incident = new Incident
            {
                title = "Test",
                status = "OPEN",
                severity = severity
            };
            controller.ModelState.AddModelError("Severity", "Invalid severity");
            var result = await controller.PostIncident(incident);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }


    }
}
