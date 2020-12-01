using Maincotech.Cms.Dto;
using Maincotech.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maincotech.Cms.Services
{
    public interface IUserService
    {
        Task<LocalizedArticleDto> GetLocalizedArticle(Guid id, string cultureCode);

        Task<LocalizedArticleDto> GetLocalizedArticle(string pageName, string cultureCode);

        Task Like(Guid id);

        Task<IEnumerable<CategoryArticlesDto>> GetCategoryArticles(string cultureCode);

        Task<PagedResult<LocalizedArticleDto>> GetArticles(string cultureCode, PagingQuery pagingQuery);

        Task<IEnumerable<ArticleSEODto>> GetArticleSEOs();
    }
}