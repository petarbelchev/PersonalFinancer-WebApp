﻿namespace PersonalFinancer.Web.Controllers
{
	using Microsoft.AspNetCore.Mvc;

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