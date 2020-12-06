using AntDesign;
using AutoMapper;
using Maincotech.Adapter;
using Maincotech.Cms.Dto;
using Maincotech.Cms.Pages.Admin.Blog;
using Maincotech.Cms.Pages.Admin.Category;
using Maincotech.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maincotech.Cms.Blazor.MapperProfiles
{
    public class DtoToViewModelMapperProfile : Profile, IOrderedMapperProfile
    {
        public int Order => 2;

        public DtoToViewModelMapperProfile()
        {
            CreateMap<CategoryDto, CategoryViewModel>()
                .ForMember(dest => dest.ParentId, opt => opt.MapFrom((src, dest, destMember, context) => src.ParentId?.ToString()));

            CreateMap<CategoryDto, CategoryViewModel>()
                         .ForMember(dest => dest.ParentId, opt => opt.MapFrom((src, dest, destMember, context) => src.ParentId?.ToString()));

            CreateMap<List<CategoryDto>, List<CascaderNode>>()
                .ConstructUsing(src => Convert(src));

            CreateMap<ArticleDto, BlogViewModel>()
              .ForMember(dest => dest.CategoryId, opt => opt.MapFrom((src, dest, destMember, context) => src.CategoryId.ToString()))
              .ForMember(dest => dest.CurrentTags, opt => opt.MapFrom((src, dest, destMember, context) => src.Tags.IsNotNullOrEmpty() ? src.Tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>()))
              .ForMember(dest => dest.PageName, opt => opt.MapFrom((src, dest, destMember, context) => src.PageName.Replace(".html", "")));


            CreateMap<LocalizedArticleDto, Maincotech.Cms.Pages.Blog.BlogViewModel>();

        }

        private static List<CascaderNode> Convert(List<CategoryDto> src)
        {
            TreeExtensions.ITree<CategoryDto> virtualRootNode = src.ToTree((parent, child) => child.ParentId == parent.Id, x => x.Name);
            return Enumerable.Select(virtualRootNode.Children, x => Convert(x)).ToList();
        }

        private static CascaderNode Convert(TreeExtensions.ITree<CategoryDto> src)
        {
            var node = new CascaderNode()
            {
                Value = src.Data.Id.ToString(),
                Label = src.Data.Name
            };
            if (src.Children?.Count > 0)
            {
                var children = new List<CascaderNode>();
                foreach (var child in src.Children)
                {
                    children.Add(Convert(child));
                }
                node.Children = children;
            }

            return node;
        }
    }
}