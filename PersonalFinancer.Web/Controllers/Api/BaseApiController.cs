namespace PersonalFinancer.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using System.Text;

    [Authorize]
    public abstract class BaseApiController : ControllerBase
    {
        protected string GetErrors(ModelStateDictionary.ValueEnumerable modelStateValues)
        {
            var errors = new StringBuilder();

            foreach (ModelStateEntry modelStateVal in modelStateValues)
            {
                foreach (ModelError error in modelStateVal.Errors)
                    _ = errors.AppendLine(error.ErrorMessage);
            }

            return errors.ToString().TrimEnd();
        }
    }
}
