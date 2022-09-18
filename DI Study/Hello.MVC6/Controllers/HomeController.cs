using Hello.BLL;
using Hello.MVC6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Hello.MVC6.Controllers
{
	public class HomeController : Controller
	{
		private readonly UserBll _userBll;

		public HomeController(UserBll userBll)
		{
			_userBll = userBll;
		}

		public IActionResult Index()
		{
			var userList = _userBll.GetUserList();

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
