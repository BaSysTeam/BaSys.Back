using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.DTO.Core;
using BaSys.Host.DAL.Abstractions;
using BaSys.Metadata.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class DataTypesController : ControllerBase
    {
        private readonly IDataTypesService _dataTypesService;
        private readonly IMainConnectionFactory _connectionFactory;
        public DataTypesController(IMainConnectionFactory connectionFactory, IDataTypesService dataTypesService)
        {
            _dataTypesService = dataTypesService;
            _connectionFactory = connectionFactory;
        }
        
        /// <summary>
        /// Retrieve all data types.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllDataTypes()
        {
            var result = new ResultWrapper<IList<DataTypeDto>>();
            
            using(IDbConnection connection = _connectionFactory.CreateConnection())
            {
                _dataTypesService.SetUp(connection);
                var dataTypes = await _dataTypesService.GetAllDataTypes();
                result.Success(dataTypes.Select(x => new DataTypeDto(x)).ToList());
            }

            return Ok(result);
        }
    }
}
