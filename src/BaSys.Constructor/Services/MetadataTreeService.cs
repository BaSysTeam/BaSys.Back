using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Metadata.DTOs;
using BaSys.Metadata.Models;
using Humanizer;

namespace BaSys.Constructor.Services
{
    public class MetadataTreeService : IMetadataTreeService
    {
        private readonly IMetadataGroupsService _metadataGroupsService;

        public MetadataTreeService(IMetadataGroupsService metadataGroupsService)
        {
            _metadataGroupsService = metadataGroupsService;
        }

        public async Task<ResultWrapper<int>> AddAsync(MetadataGroupDto dto, string dbName)
        {
            var result = new ResultWrapper<int>();

            try
            {
                var insertResult = await _metadataGroupsService.InsertAsync(dto, dbName);
                result.Success(insertResult);
            }
            catch (Exception ex)
            {
                result.Error(-1, "Add metadata group error", ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> DeleteAsync(string uid, string dbName)
        {
            var result = new ResultWrapper<int>();

            if (!Guid.TryParse(uid, out var groupUid))
            {
                result.Error(-1, "Incorrect uid");
                return result;
            }

            try
            {
                var deleteResult = await _metadataGroupsService.DeleteAsync(groupUid, dbName);
                result.Success(deleteResult);
            }
            catch (Exception ex)
            {
                result.Error(-1, "Delete error", ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<List<MetadataTreeNode>>> GetChildrenAsync(string uid, string dbName)
        {
            var result = new ResultWrapper<List<MetadataTreeNode>>();
            var nodes = new List<MetadataTreeNode>();

            if (!Guid.TryParse(uid, out var parentUid))
            {
                result.Error(-1, "Incorrect parent uid");
                return result;
            }

            try
            {
                var children = await _metadataGroupsService.GetChildrenAsync(parentUid, dbName);
                foreach (var child in children)
                {
                    var node = new MetadataTreeNode
                    {
                        Key = child.Uid,
                        ParentKey = uid,
                        Label = child.Title,
                        Icon = child.IconClass,
                        NodeType = MetadataTreeNodeTypes.Group
                    };

                    var hasChildren = await _metadataGroupsService.HasChildrenAsync(new Guid(child.Uid), dbName);
                    node.Leaf = !hasChildren;
                    nodes.Add(node);
                }

                result.Success(nodes);
            }
            catch (Exception ex)
            {
                result.Error(-1, "Get children error", ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<List<MetadataTreeNode>>> GetDefaultNodesAsync(string dbName)
        {
            var result = new ResultWrapper<List<MetadataTreeNode>>();
            var defaultNodes = new List<MetadataTreeNode>();
            var defaultGroups = MetadataGroupDefaults.AllGroups();

            try
            {
                foreach (var group in defaultGroups)
                {
                    var node = new MetadataTreeNode
                    {
                        Key = group.Uid.ToString(),
                        Label = group.Title,
                        Icon = group.IconClass,
                        Leaf = true,
                        IsStandard = group.IsStandard,
                        NodeType = MetadataTreeNodeTypes.Group
                    };

                    if (group.Title.ToLower() == "metadata")
                    {
                        var hasChildren = await _metadataGroupsService.HasChildrenAsync(group.Uid, dbName);
                        node.Leaf = !hasChildren;
                    }
                    if (group.Title.ToLower() == "system")
                    {
                        node.Leaf = false;
                        node.Children.Add(new MetadataTreeNode
                        {
                            Key = Guid.NewGuid().ToString(),
                            ParentKey = group.Uid.ToString(),
                            Label = "DataTypes",
                            Leaf = true,
                            IsStandard = true,
                            NodeType = MetadataTreeNodeTypes.Element
                        });
                        node.Children.Add(new MetadataTreeNode
                        {
                            Key = Guid.NewGuid().ToString(),
                            ParentKey = group.Uid.ToString(),
                            Label = "MetadataKinds",
                            Leaf = true,
                            IsStandard = true,
                            NodeType = MetadataTreeNodeTypes.Element
                        });
                    }

                    defaultNodes.Add(node);
                }

                result.Success(defaultNodes);
            }
            catch (Exception ex)
            {
                result.Error(-1, "Get default nodes error", ex.Message);
            }
            
            return result;
        }
    }
}
