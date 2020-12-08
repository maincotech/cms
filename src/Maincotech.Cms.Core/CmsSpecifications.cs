using Maincotech.Cms.Models;
using Maincotech.Domain.Specifications;
using System;

namespace Maincotech.Cms
{
    public static class CmsSpecifications
    {
        public static ISpecification<ArticleTag> TagsWithArticleId(Guid id)
        {
            return Specification<ArticleTag>.Eval(entity => entity.SourceId == id);
        }

        public static ISpecification<Tag> TagsWithNameAndType(string tag, TagType tagType)
        {
            return Specification<Tag>.Eval(entity => entity.Name == tag && entity.TagType == tagType);
        }

        public static ISpecification<Tag> TagsWithType(TagType tagType)
        {
            return Specification<Tag>.Eval(entity => entity.TagType == tagType);
        }

        public static ISpecification<Article> ArticleWithPageName(string pageName)
        {
            return Specification<Article>.Eval(entity => entity.PageName == pageName);
        }

        public static ISpecification<ArticleSEO> ArticleSEOWithId(Guid id)
        {
            return Specification<ArticleSEO>.Eval(entity => entity.Id == id);
        }
    }
}