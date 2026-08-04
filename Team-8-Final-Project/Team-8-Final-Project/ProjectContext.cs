using Team_8_Final_Project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Team_8_Final_Project
{
    public class ProjectContext : DbContext
    {
        // 1 - register models

        // public DbSet<Employee> employees { get; set; } // change name to model name
        // public DbSet<Department> departments { get; set; } // change name to model name

        public DbSet<Loan> Loans { get; set; }

        // 2 - connect to database

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(
            "Server=;Database=CompanyProjectDB;Trusted_Connection=True;TrustServerCertificate=True;" // change server name to your server name when testing 
            );
        }
    }
}
