﻿using Microsoft.AspNetCore.Mvc;

namespace PersonalFinancer.Web.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		public IActionResult Error()
		{
			throw new NotImplementedException();
		}
	}
}