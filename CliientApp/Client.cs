using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliientApp
{
    class ChatRoomDB : DbContext
    {
        public DbSet<Client> Clients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(@"Data Source=DEVBOX-PC;
                                Initial Catalog = ChatRoom;
                                Integrated Security=True;Connect Timeout=2;Encrypt=False;TrustServerCertificate=False;
                                ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
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
    internal class Client
    {
        public int Id { get; set; }
        public string Login { get; set; }    
        public string Password { get; set; }    
    }
}
