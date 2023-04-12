namespace PersonalFinancer.Services.ModelsState
{
	using static Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary;

	public interface IModelStateService
	{
		string GetErrors(ValueEnumerable modelStateValues);
	}
}
