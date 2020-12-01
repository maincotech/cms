using Maincotech.Cms.Dto;
using Maincotech.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maincotech.Cms.Services
{
    public interface IAdminService
    {
        #region Category Operations

        Task CreateOrUpdateCategory(CategoryDto dto);

        Task DeleteCategories(List<Guid> categoriesToBeDeleted);

        Task<CategoryDto> GetCategory(Guid id);

        Task<IEnumerable<CategoryDto>> GetCategories(SortGroup sortGroup=null, FilterCondition filters=null);

        #endregion Category Operations

        #region Article Operations

        Task CreateOrUpdateArticle(ArticleDto dto);

        Task<ArticleDto> GetArticle(Guid id);

        Task<PagedResult<ArticleDto>> GetPagedArticles(PagingQuery pagingQuery);

        Task<IEnumerable<CategoryDto>> GetArticleCategories();

        #endregion Article Operations

        #region Tags Operations

        Task<IEnumerable<string>> GetPublicTags();

        #endregion Tags Operations
    }
}