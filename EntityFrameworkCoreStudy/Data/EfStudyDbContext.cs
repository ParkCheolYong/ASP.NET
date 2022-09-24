using EntityFrameworkCoreStudy.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCoreStudy.Data
{
	public class EfStudyDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Position> Positions { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AspnetCoreDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// EF Fluent API
			modelBuilder.Entity<User>().ToTable("s_Users");

			//modelBuilder.Entity<User>().Property(u => u.UserName).HasColumnName("s_userName");
		}
	}
}
