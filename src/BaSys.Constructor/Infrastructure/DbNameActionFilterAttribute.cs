using BaSys.Common.Infrastructure;
using BaSys.Constructor.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BaSys.Constructor.Infrastructure
{
    public class DbNameActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as ApiControllerBase;
            if (controller != null)
            {
                controller.CreateConnection();
            }
        }

    }
}
