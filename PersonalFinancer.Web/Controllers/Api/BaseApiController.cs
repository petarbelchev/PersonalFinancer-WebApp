namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ModelBinding;

	using System.Text;

	public class BaseApiController : ControllerBase
	{
		public string GetErrors(ModelStateDictionary.ValueEnumerable modelStateValues)
		{
			var errors = new StringBuilder();

			foreach (var modelStateVal in modelStateValues)
			{
				foreach (var error in modelStateVal.Errors)
				{
					errors.AppendLine(error.ErrorMessage);
				}
			}

			return errors.ToString().TrimEnd();
		}
	}
}
