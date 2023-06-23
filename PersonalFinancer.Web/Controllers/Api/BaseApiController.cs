namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using PersonalFinancer.Data.Models.Contracts;
	using PersonalFinancer.Services.Api;
	using PersonalFinancer.Services.Api.Models;
	using PersonalFinancer.Web.Extensions;
	using PersonalFinancer.Web.Models.Api;
	using System.ComponentModel.DataAnnotations;
	using System.Text;

	[Authorize]
	public abstract class BaseApiController<T> : ControllerBase where T : BaseCacheableApiEntity, new()
	{
		private readonly IApiService<T> apiService;

		protected BaseApiController(IApiService<T> apiService)
			=> this.apiService = apiService;

		/// <summary>
		/// Throws ArgumentException if you try to create Entity with existing name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		protected async Task<IActionResult> CreateEntityAsync(IApiInputModel inputModel)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest(this.GetErrors(this.ModelState.Values));

			ApiOutputServiceModel model = await this.apiService.CreateEntityAsync(
				inputModel.Name,
				inputModel.OwnerId ?? throw new InvalidOperationException("Owner ID cannot be null."));

			return this.Created(string.Empty, model);
		}

		protected async Task<IActionResult> DeleteEntityAsync([Required] Guid id)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest(this.GetErrors(this.ModelState.Values));

			try
			{
				await this.apiService.DeleteEntityAsync(id, this.User.IdToGuid(), this.User.IsAdmin());
			}
			catch (ArgumentException)
			{
				return this.Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			return this.NoContent();
		}

		private string GetErrors(ModelStateDictionary.ValueEnumerable modelStateValues)
		{
			var errors = new StringBuilder();

			foreach (ModelStateEntry modelStateVal in modelStateValues)
			{
				foreach (ModelError error in modelStateVal.Errors)
					errors.AppendLine(error.ErrorMessage);
			}

			return errors.ToString().TrimEnd();
		}
	}
}
