using AntDesign;
using AutoMapper;
using Maincotech.Adapter;
using Maincotech.Cms.Dto;
using Maincotech.Cms.Models;
using Maincotech.Cms.Pages.Admin.Blog;
using Maincotech.Cms.Pages.Admin.Category;
using Maincotech.Data;

using System;
using System.Collections.Generic;

namespace Maincotech.Cms.Blazor.MapperProfiles
{
    public class ViewModelToDtoMapperProfile : Profile, IOrderedMapperProfile
    {
        public int Order => 3;

        public ViewModelToDtoMapperProfile()
        {
            CreateMap<CategoryViewModel, CategoryDto>()
                .ForMember(dest => dest.ParentId, opt => opt.MapFrom((src, dest, destMember, context) => src.ParentId?.ToString()));

            CreateMap<CategoryViewModel, CategoryDto>()
             //  .ForMember(x => x.Children, opt => opt.Ignore())
             .ForMember(x => x.ParentId, opt => opt.MapFrom(
                 (src, dest, destMember, context) =>
                 string.IsNullOrEmpty(src.ParentId) ?
                 new Nullable<Guid>() :
                 new Guid?(Guid.Parse(src.ParentId))));

            CreateMap<BlogViewModel, ArticleDto>()
               .ForMember(dest => dest.PageName, opt => opt.MapFrom((src, dest, destMember, context) => $"{src.PageName}.html"))
               .ForMember(dest => dest.Tags, opt=> opt.MapFrom((src, dest, destMember, context) => string.Join(',',src.SelectedTags)));
        }

        private static CascaderNode Convert(TreeExtensions.ITree<Category> src)
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