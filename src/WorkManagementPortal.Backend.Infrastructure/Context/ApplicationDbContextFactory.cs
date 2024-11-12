using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WorkManagementPortal.Backend.Infrastructure.Context
{
    

    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../WorkManagementPortal.Backend.API");

            // Build the configuration manually, pointing to the Web API project folder
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)  // Set the base path to the Web API project
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load the appsettings.json file
                .Build();

            // Build the DbContext options using the connection string from appsettings.json
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            // Return a new instance of ApplicationDbContext
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }


}
