namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models.Contracts;
	using PersonalFinancer.Services.Api;
	using PersonalFinancer.Services.Api.Models;
	using PersonalFinancer.Web.Models.Api;
	using System.ComponentModel.DataAnnotations;
	using System.Text;

	[Authorize]
	public abstract class BaseApiController<T> : ControllerBase where T : BaseApiEntity, new()
	{
		private readonly string entityName;
		private readonly IApiService<T> apiService;
		private readonly ILogger<BaseApiController<T>> logger;

		protected BaseApiController(
			IApiService<T> apiService,
			ILogger<BaseApiController<T>> logger)
		{
			this.entityName = typeof(T).Name.ToLower();

			if (this.entityName == "accounttype")
				this.entityName = "account type";

			this.apiService = apiService;
			this.logger = logger;
		}

		/// <exception cref="ArgumentException">When try to create entity with existing name.</exception>
		[Produces("application/json")]
		[ProducesResponseType(typeof(ApiEntityDTO), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		protected async Task<IActionResult> CreateEntityAsync(IApiEntityInputModel inputModel)
		{
			if (!this.ModelState.IsValid)
			{
				this.logger.LogWarning(
					LoggerMessages.CreateEntityWithInvalidInputData,
					this.User.Id(),
					this.entityName);

				return this.BadRequest(GetErrors(this.ModelState.Values));
			}

			ApiEntityDTO model;

			try
			{
				model = await this.apiService.CreateEntityAsync(
					inputModel.Name,
					inputModel.OwnerId ?? throw new InvalidOperationException(
						string.Format(ExceptionMessages.NotNullableProperty, inputModel.OwnerId)));
			}
			catch (ArgumentException)
			{
				return this.BadRequest(string.Format(
					ExceptionMessages.ExistingUserEntityName,
					this.entityName,
					inputModel.Name));
			}

			return this.Created(string.Empty, model);
		}

		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		protected async Task<IActionResult> DeleteEntityAsync([Required] Guid id)
		{
			if (!this.ModelState.IsValid)
			{
				this.logger.LogWarning(
					LoggerMessages.DeleteEntityWithInvalidInputData,
					this.User.Id(),
					this.entityName);

				return this.BadRequest(GetErrors(this.ModelState.Values));
			}

			try
			{
				await this.apiService.DeleteEntityAsync(id, this.User.IdToGuid(), this.User.IsAdmin());
			}
			catch (UnauthorizedAccessException)
			{
				this.logger.LogWarning(
					LoggerMessages.UnauthorizedEntityDeletion,
					this.User.Id(),
					this.entityName,
					id);

				return this.Unauthorized();
			}
			catch (InvalidOperationException)
			{
				this.logger.LogWarning(
					LoggerMessages.DeleteEntityWithInvalidInputData,
					this.User.Id(),
					this.entityName);

				return this.BadRequest();
			}

			return this.NoContent();
		}

		private static string GetErrors(ModelStateDictionary.ValueEnumerable modelStateValues)
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
