using Microsoft.EntityFrameworkCore;
using System;

namespace EntityFrameworkCoreStudy
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var db = new EfStudyDbContext())
			{
				var userList = db.Users;

				foreach(var user in userList)
				{
					Console.WriteLine(user.UserName);
				}
			}
		}
	}

	public class EfStudyDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AspnetCoreDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
		}
	}

	public class User
	{
		public int UserId { get; set; }

		public string UserName { get; set; }

		public string Birth { get; set; }
	}
}
