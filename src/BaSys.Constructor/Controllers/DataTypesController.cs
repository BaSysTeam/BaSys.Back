using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Metadata.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class DataTypesController : ControllerBase
    {
        private readonly IDataTypesService _dataTypesService; 
        public DataTypesController(IDataTypesService dataTypesService)
        {
            _dataTypesService = dataTypesService;
        }
        
        /// <summary>
        /// Retrieve all data types.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllDataTypes()
        {
            var dataTypes = await _dataTypesService.GetAllDataTypes();
            var result = new ResultWrapper<IList<DataType>>();
            result.Success(dataTypes);

            return Ok(result);
        }
    }
}
