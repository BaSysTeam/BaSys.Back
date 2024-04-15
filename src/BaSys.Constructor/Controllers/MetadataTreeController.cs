using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.Metadata.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class MetadataTreeController : ControllerBase
    {
        /// <summary>
        /// Retrieve metadata tree.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetMetadataTree()
        {
            var metadataTree = GetMetadataTreeNodes();
            var result = new ResultWrapper<List<MetadataTreeNode>>();
            result.Success(metadataTree);

            return Ok(result);
        }

        private List<MetadataTreeNode> GetMetadataTreeNodes()
        {
            var metadataGroups = MetadataGroupDefaults.AllGroups();
            var nodes = GetNodes(metadataGroups.ToList());
            var systemNode = nodes.FirstOrDefault(x => x.Label.ToLower() == "system");
            if (systemNode != null)
            {
                systemNode.Children.Add(new MetadataTreeNode
                {
                    Key = Guid.NewGuid().ToString(),
                    Label = "DataTypes",
                    NodeType = MetadataTreeNodeTypes.Element
                });
                systemNode.Children.Add(new MetadataTreeNode
                {
                    Key = Guid.NewGuid().ToString(),
                    Label = "MetadataKinds",
                    NodeType = MetadataTreeNodeTypes.Element
                });
            }

            return nodes;
        }

        private List<MetadataTreeNode> GetNodes(List<MetadataGroup> metadataGroups)
        {
            var result = new List<MetadataTreeNode>();

            foreach (var group in metadataGroups)
            {
                var node = new MetadataTreeNode
                {
                    Key = group.Uid.ToString(),
                    Label = group.Title,
                    Icon = group.IconClass,
                    NodeType = MetadataTreeNodeTypes.Group
                };

                result.Add(node);
            }

            return result;
        }
    }
}
