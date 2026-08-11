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

        public DbSet<User> users { get; set; }
        public DbSet<Author> authors { get; set; }


        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<Book> books { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<BookCopy> bookCopies { get; set; }

        public DbSet<Fine> Fines { get; set; }

        public DbSet<Review> Reviews { get; set; }


        public DbSet<Loan> loans { get; set; }
        public DbSet<Reservation> reservations { get; set; }
        
        
        public DbSet<Shelf> shelves { get; set; }
        public DbSet<Event> events { get; set; }

        // 2 - connect to database

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(
            "Server=Server=DESKTOP-8CRFLQK\\\\SQLEXPRESS;"
            );
        }
    }
}
