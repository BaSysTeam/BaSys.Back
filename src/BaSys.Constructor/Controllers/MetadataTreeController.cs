using BaSys.Common.Infrastructure;
using BaSys.Metadata.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Constructor.Controllers
{
    [Route("api/constructor/v1/[controller]")]
    [ApiController]
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
            var result = GetChildrenNodes(null, metadataGroups);
            
            return result;
        }

        private List<MetadataTreeNode> GetChildrenNodes(Guid? parentUid, IList<MetadataGroup> source)
        {
            var nodes = new List<MetadataTreeNode>();
            var groups = source.Where(x => x.ParentUid == parentUid);
            foreach (var group in groups)
            {
                var node = new MetadataTreeNode
                {
                    Key = group.Uid.ToString(),
                    Label = group.Title,
                    Icon = group.IconClass,
                    Children = GetChildrenNodes(group.Uid, source)
                };

                nodes.Add(node);
            }

            return nodes;
        }
    }
}
