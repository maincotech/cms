using AntDesign;
using Maincotech.Cms.Dto;
using Maincotech.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maincotech.Cms
{
    public static class TypeConverters
    {
        public static List<CascaderNode> Convert(List<CategoryDto> src, Guid current)
        {
            TreeExtensions.ITree<CategoryDto> virtualRootNode = src.ToTree((parent, child) => child.ParentId == parent.Id, x => x.Name);
            return Enumerable.Select(virtualRootNode.Children, x => Convert(x, current)).ToList();
        }

        private static CascaderNode Convert(TreeExtensions.ITree<CategoryDto> src, Guid current)
        {
            var node = new CascaderNode()
            {
                Value = src.Data.Id.ToString(),
                Label = src.Data.Name,
                Disabled = src.Data.Id == current || src.GetParents<CategoryDto>().Any(x => x.Id == current)
            };
            if (src.Children?.Count > 0)
            {
                var children = new List<CascaderNode>();
                foreach (var child in src.Children)
                {
                    children.Add(Convert(child, current));
                }
                node.Children = children;
            }

            return node;
        }
    }
}