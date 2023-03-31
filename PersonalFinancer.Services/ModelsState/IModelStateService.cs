using PersonalFinancer.Services.Accounts.Models;

using static Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary;

namespace PersonalFinancer.Services.ModelsState
{
	public interface IModelStateService
	{
		string GetErrors(ValueEnumerable modelStateValues);
	}
}
