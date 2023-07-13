﻿namespace PersonalFinancer.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Data.Models.Contracts;
    using PersonalFinancer.Services.Api;
    using PersonalFinancer.Services.Api.Models;
    using PersonalFinancer.Web.Extensions;
	using PersonalFinancer.Web.Models.Api;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    [Authorize]
	public abstract class BaseApiController<T> : ControllerBase where T : BaseApiEntity, new()
	{
		private readonly IApiService<T> apiService;

		protected BaseApiController(IApiService<T> apiService)
			=> this.apiService = apiService;

		/// <exception cref="ArgumentException">When try to create entity with existing name.</exception>
		[Produces("application/json")]
		[ProducesResponseType(typeof(ApiEntityDTO), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		protected async Task<IActionResult> CreateEntityAsync(IApiEntityInputModel inputModel)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest(GetErrors(this.ModelState.Values));

			ApiEntityDTO model = await this.apiService.CreateEntityAsync(
				inputModel.Name,
				inputModel.OwnerId ?? throw new InvalidOperationException(
					string.Format(ExceptionMessages.NotNullableProperty, inputModel.OwnerId)));

			return this.Created(string.Empty, model);
		}

		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		protected async Task<IActionResult> DeleteEntityAsync([Required] Guid id)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest(GetErrors(this.ModelState.Values));

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
