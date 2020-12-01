using AutoMapper;
using Maincotech.Adapter;
using Maincotech.Cms.Dto;
using Maincotech.Cms.Models;
using System;

namespace Maincotech.Cms.MapperProfiles
{
    internal class EntityToDtoMapperProfile : Profile, IOrderedMapperProfile
    {
        public int Order => 2;

        public EntityToDtoMapperProfile()
        {
            CreateMap<Category, CategoryDto>();

            CreateMap<Article, ArticleDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom((src, dest, destMember, context) => src.CreatedBy));

            CreateMap<Article, LocalizedArticleDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom((src, dest, destMember, context) => src.Category.Name))
                .ForMember(dest => dest.Author, opt => opt.MapFrom((src, dest, destMember, context) => src.CreatedBy))
                .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom((src, dest, destMember, context) => src.Comments.IsNotNullOrEmpty() ? src.Comments.Count : 0));

            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom((src, dest, destMember, context) => src.CreatedBy));

            CreateMap<ArticleSEO, ArticleSEODto>();

            //CreateMap<CategoryViewModel, Category>()
            //    .ForMember(x => x.Children, opt => opt.Ignore())
            //    .ForMember(x => x.ParentId, opt => opt.MapFrom(
            //        (src, dest, destMember, context) =>
            //        string.IsNullOrEmpty(src.ParentId) ?
            //        new Nullable<Guid>() :
            //        new Guid?(Guid.Parse(src.ParentId))));

            //CreateMap<BlogViewModel, Article>()
            //    .ForMember(x => x.LastModifiedTime, opt => opt.MapFrom((src, dest, destMember, context) => DateTime.UtcNow));
        }
    }
}