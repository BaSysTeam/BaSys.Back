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
                var dbName = controller.User.Claims.FirstOrDefault(x => x.Type == GlobalConstants.DbNameClaim)?.Value;
                controller.DbName = dbName;  // Assuming you add a public string DbName property on the controller
            }
        }
    }

}
