using AspnetCoreStudy.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreStudy.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			var hongUser = new User
			{
				UserNo = 1,
				UserName = "홍길동"
			};

			// # 1번째 방식 View(model);
			//return View(hongUser);

			// 2번째 방식 ViewBag
			//ViewBag.User = hongUser;
			//return View();

			// 3번째 방식 ViewData
			ViewData["UserNo"] = hongUser.UserNo;
			ViewData["UserName"] = hongUser.UserName;
			return View();
		}

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
