using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts.Models;

namespace PersonalFinancer.Web.ViewComponents
{
	public class AccountCardListViewComponent : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync(
													IEnumerable<AccountCardViewModel> accounts,
													string area,
													string controller,
													string action)
		{
			var viewComponent = await Task.Run(() =>
			{
				ViewBag.Area = area;
				ViewBag.Controller = controller;
				ViewBag.Action = action;
				return View(accounts);
			});

			return viewComponent;
		}
	}
}
