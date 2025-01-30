using BaSys.Workflows.DTO;
using BaSys.Workflows.Infrastructure;
using SharpYaml.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Workflows.UnitTests
{
    [TestFixture]
    public class WorkflowParametersParserTests
    {
        private Dictionary<string, object?> _check;

        [SetUp]
        public void SetUp()
        {
            _check = new Dictionary<string, object?>();
            _check.Add("delay", 5);
            _check.Add("message", "Some message");
            _check.Add("bool_false", false);
            _check.Add("bool_true", true);
            _check.Add("price", 12.34M);
            _check.Add("dead_line", new DateTime(2025, 4, 1, 23, 59, 59));
        }


        [TestCase("delay","integer", "5")]
        [TestCase("message", "string", "Some message")]
        [TestCase("bool_false", "boolean", "false")]
        [TestCase("bool_true", "boolean", "true")]
        [TestCase("price", "number", "12.34")]
        [TestCase("dead_line", "date", "2025-04-01T23:59:59")]
        public void Parse_Parameter_ReturnParsedValue(string name, string dataType, string value)
        {
            var parameters = new List<WorkflowParameterDto>();
            parameters.Add(new WorkflowParameterDto { Name = name, DataType = dataType, Value = value });

            var result = WorkflowParametersParser.Parse(parameters);

            Assert.That(result[name], Is.EqualTo(_check[name]));
        }

        [Test]
        public void Parse_EmptyParameterName_ThrowException()
        {
            var parameters = new List<WorkflowParameterDto>();
            parameters.Add(new WorkflowParameterDto { Name = string.Empty, DataType = "string", Value = "Some message" });

            Assert.Throws<ArgumentException>(() => WorkflowParametersParser.Parse(parameters));
        }

        [TestCase("unknown")]
        [TestCase("")]
        public void Parse_WrongDataType_ThrowException(string dataType)
        {
            var parameters = new List<WorkflowParameterDto>();
            parameters.Add(new WorkflowParameterDto { Name = "message", DataType = dataType, Value = "Some message" });

            Assert.Throws<ArgumentException>(() => WorkflowParametersParser.Parse(parameters));
        }

        [Test]
        public void Parse_Object_ReturnParametersDictionary()
        {
            Console.WriteLine(Texts.WorkflowObjectParameter);
            var parameters = new List<WorkflowParameterDto>();
            parameters.Add(new WorkflowParameterDto { Name = "section", DataType = "object", Value = Texts.WorkflowObjectParameter });

            var result = WorkflowParametersParser.Parse(parameters);

            var section = result["section"] as Dictionary<string, object?>;

            Assert.That(section["price"], Is.EqualTo(12.34M));

        }
    }
}
