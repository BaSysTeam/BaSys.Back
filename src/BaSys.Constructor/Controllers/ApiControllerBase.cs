using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Constructor.Controllers
{
   
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        public string? DbName { get; set; } = string.Empty;
    }
}
