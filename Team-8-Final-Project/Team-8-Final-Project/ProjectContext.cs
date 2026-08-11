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

        public DbSet<User> Users { get; set; }
        public DbSet<Author> Authors { get; set; }


        public DbSet<Book> Books { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }


        public DbSet<Loan> Loans { get; set; }
        public DbSet<Reservation> Reservations { get; set; }


        public DbSet<Fine> Fines { get; set; }
        public DbSet<Review> Reviews { get; set; }
     
        
        public DbSet<Shelf> Shelves { get; set; }
        public DbSet<Event> Events { get; set; }

        // 2 - connect to database

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(
            "Server=Laptopius\\SQLEXPRESS;Database=CompanyProjectDB;Trusted_Connection=True;TrustServerCertificate=True;"
            );
        }
    }
}
