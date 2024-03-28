using BaSys.Common.Infrastructure;
using BaSys.Metadata.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    public class DataTypesController : ControllerBase
    {
        /// <summary>
        /// Retrieve all data types.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllDataTypes()
        {
            var dataTypes = DataTypeDefaults.AllTypes();
            var result = new ResultWrapper<IList<DataType>>();
            result.Success(dataTypes);

            return Ok(result);
        }
    }
}
