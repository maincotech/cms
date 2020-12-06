using Maincotech.Caching;
using Maincotech.Cms.Dto;
using Maincotech.Cms.Models;
using Maincotech.Data;
using Maincotech.Domain.Repositories;
using Maincotech.Domain.Specifications;
using Maincotech.EF;
using Maincotech.Localization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Maincotech.Cms.Services
{
    public class UserService : IUserService
    {
        private readonly DbContext _dbContext;
        private ICacheManager _cacheManager;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Article> _articleRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<ArticleTag> _articleTagRepository;
        private readonly IRepository<ArticleSEO> _articleSEORepository;
        private ILocalizationService _localizationService;

        public UserService(DbContext cmsDbContext, ILocalizationService localizationService, ICacheManager cacheManager = null)
        {
            _dbContext = cmsDbContext;
            _cacheManager = cacheManager ?? new RuntimeCacheProvider();
            _localizationService = localizationService;
            _categoryRepository = new EntityFrameworkRepository<Category>(new EntityFrameworkRepositoryContext(_dbContext));
            _articleRepository = new EntityFrameworkRepository<Article>(new EntityFrameworkRepositoryContext(_dbContext));
            _tagRepository = new EntityFrameworkRepository<Tag>(new EntityFrameworkRepositoryContext(_dbContext));
            _articleTagRepository = new EntityFrameworkRepository<ArticleTag>(new EntityFrameworkRepositoryContext(_dbContext));
            _articleSEORepository = new EntityFrameworkRepository<ArticleSEO>(new EntityFrameworkRepositoryContext(_dbContext));
        }

        public async Task<PagedResult<LocalizedArticleDto>> GetArticles(string cultureCode, PagingQuery pagingQuery)
        {
            var entities = _articleRepository.GetPagedResult(pagingQuery, x => x.Category);
            foreach (var entity in entities)
            {
                entity.Category = await _localizationService.GetLocalizedEntity<Category>(entity.Category, cultureCode);
            }
            var dtos = entities.To<PagedResult<LocalizedArticleDto>>();
            foreach (var dto in dtos)
            {
                var tags = _articleTagRepository.FindAll(CmsSpecifications.TagsWithArticleId(dto.Id), x => x.Target).Select(x => x.Target.Name);
                if (tags.Any())
                {
                    dto.Tags = string.Join(",", tags);
                }
            }
            return dtos;
        }

        public async Task<IEnumerable<CategoryArticlesDto>> GetCategoryArticles(string cultureCode)
        {
            var categories = Enumerable.GroupBy(_dbContext.Set<Article>().Include(x => x.Category), x => x.Category).Select(x =>
               {
                   return new
                   {
                       Category = x.Key,
                       Count = x.Count()
                   };
               });
            var dtos = new List<CategoryArticlesDto>();
            foreach (var category in categories)
            {
                dtos.Add(new CategoryArticlesDto
                {
                    CategoryId = category.Category.Id,
                    Count = category.Count,
                    CategoryName = (await _localizationService.GetLocalizedEntity<Category>(category.Category, cultureCode)).Name
                });
            }

            return dtos;
        }

        public async Task<LocalizedArticleDto> GetLocalizedArticle(Guid id, string cultureCode)
        {
            var entity = _articleRepository.Find(DomainObjectSpecifications.Id<Article>(id), x => x.Category);
            entity.Category = await _localizationService.GetLocalizedEntity<Category>(entity.Category, cultureCode);
            var dto = entity.To<LocalizedArticleDto>();
            var tags = _articleTagRepository.FindAll(CmsSpecifications.TagsWithArticleId(id), x => x.Target).Select(x => x.Target.Name);
            if (tags.Any())
            {
                dto.Tags = string.Join(",", tags);
            }

            return dto;
        }

        public async Task<LocalizedArticleDto> GetLocalizedArticle(string pageName, string cultureCode)
        {
            var entity = _articleRepository.Find(CmsSpecifications.ArticleWithPageName(pageName), x => x.Category);
            if (entity == null)
            {
                throw new FileNotFoundException(pageName);
            }
            entity.Views++;
            _articleRepository.Update(entity);
            _articleRepository.Context.Commit();

            entity.Category = await _localizationService.GetLocalizedEntity<Category>(entity.Category, cultureCode);
            var dto = entity.To<LocalizedArticleDto>();
            var tags = _articleTagRepository.FindAll(CmsSpecifications.TagsWithArticleId(entity.Id), x => x.Target).Select(x => x.Target.Name);
            if (tags.Any())
            {
                dto.Tags = string.Join(",", tags);
            }

            return dto;
        }

        public async Task Like(Guid id, bool isLike = false)
        {
            var entity = _articleRepository.GetByKey(id);
            if (isLike)
            {
                entity.Likes++;
            }
            else
            {
                if (entity.Likes > 0)
                {
                    entity.Likes--;
                }
            }
            _articleRepository.Update(entity);
            _articleRepository.Context.Commit();
        }

        public async Task<IEnumerable<ArticleSEODto>> GetArticleSEOs()
        {
            var enities = _articleSEORepository.GetAll().ToList();
            return enities.To<List<ArticleSEODto>>();
        }
    }
}