﻿using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CliientApp
{
    public class ChatRoomDbContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Client> Clients { get; set; }

        public ChatRoomDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>().ToTable("Clients");

            modelBuilder.Entity<Client>().Property(c => c.Login).IsRequired().HasMaxLength(24);
            modelBuilder.Entity<Client>().HasIndex(c => c.Login).IsUnique();
            modelBuilder.Entity<Client>().HasCheckConstraint("Login", "Login != ''");


            modelBuilder.Entity<Client>().Property(c => c.Password).IsRequired().HasMaxLength(24);
            modelBuilder.Entity<Client>().HasCheckConstraint("Password", "Password != '' AND Password LIKE N'[^./%$#@,]%'");

            modelBuilder.Entity<Client>().Property(c => c.IPEndPoint).IsRequired(false);

            modelBuilder.Entity<Client>().HasData(new Client[]
            {
                    new Client()
                    {
                        Id = 1,
                        Login = "User1",
                        Password = "1111"
                    },
                     new Client()
                    {
                        Id = 2,
                        Login = "User2",
                        Password = "2222"
                    },
                       new Client()
                    {
                        Id = 3,
                        Login = "User3",
                        Password = "3333"
                    },
                     new Client()
                    {
                        Id = 4,
                        Login = "User4",
                        Password = "4444"
                    }
                });

        }
    }
}
