using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;

namespace PersonalFinancer.Services.ModelsState
{
	public class ModelStateService : IModelStateService
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
