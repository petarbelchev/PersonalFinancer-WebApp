namespace PersonalFinancer.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Services.Api;
    using PersonalFinancer.Web.Models.Api;
    using System.ComponentModel.DataAnnotations;

    [Route("api/categories")]
	[ApiController]
	public class CategoriesApiController : BaseApiController<Category>
	{
		public CategoriesApiController(IApiService<Category> apiService)
			: base(apiService)
		{ }

		[HttpPost]
		public async Task<IActionResult> CreateCategory(CategoryInputModel inputModel)
		{
			try
			{
				return await this.CreateEntityAsync(inputModel);
			}
			catch (ArgumentException)
			{
				return this.BadRequest(string.Format(ExceptionMessages.ExistingUserEntityName, "category", inputModel.Name));
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCategory([Required] Guid id)
			=> await this.DeleteEntityAsync(id);
	}
}
