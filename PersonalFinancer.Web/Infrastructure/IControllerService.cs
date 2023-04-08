namespace PersonalFinancer.Web.Infrastructure
{
    using Web.Models.Accounts;

    using static Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary;

    public interface IControllerService
    {
        string GetModelErrors(ValueEnumerable modelStateValues);

        Task PrepareAccountFormModelForReturn<T>(T inputModel)
            where T : IAccountFormModel;

        Task PrepareTransactionFormModelForReturn(TransactionFormModel inputModel);
    }
}
