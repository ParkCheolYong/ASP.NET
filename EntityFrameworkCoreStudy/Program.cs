using EntityFrameworkCoreStudy.Data;
using EntityFrameworkCoreStudy.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityFrameworkCoreStudy
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var db = new EfStudyDbContext())
			{
				// 1. SELECT
				//   1) DbSet<User> selectList = db.Users;
				//   2) List<User> selectList = db.Users.ToList();
				//   3) IEnumerable<User> selectList = db.Users.AsEnumerable();
				//   4) IQueryable<User> selectList = from user in db.Users
				//      		 					  select user; //Linq to Sql

				// # IEnumerable vs IQueryable
				// Extension Query => 작성이 가능
				// 1. IEnumerable => 쿼리를 실행한 데이터를 Client 컴퓨터 메모리에 저장함 => Client 컴퓨터기 때문에 서버에 비해 느림
				// 2. IQueryable => 쿼리를 실행한 데이터를 Server에 저장함 => 빠르지만 서버에 부담이 생김

				// 2. INSERT
				// db.Users.Add(User);
				// db.SaveChanges();	// commit

				// 3. UPDATE
				//var user = new User { UserId = 1, UserName = "김길동" };
				//db.Entry(user).State = EntityState.Modified;
				//db.SaveChanges();

				// 4. DELETE
				//var user = new User { UserId = 1};
				//db.Users.Remove(user);
				//db.SaveChanges();

				var selectList = db.Users
					.Include(u => u.Position)
					.ToList();

				foreach (var user in selectList)
				{
					Console.WriteLine($"{user.UserId}.{user.UserName}({user.Birth}, {user.Position.PositionName})");
				}
			}
		}
	}



	
}
