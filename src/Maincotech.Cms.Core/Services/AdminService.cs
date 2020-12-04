using Maincotech.Caching;
using Maincotech.Cms.Dto;
using Maincotech.Cms.Models;
using Maincotech.Data;
using Maincotech.Domain.Repositories;
using Maincotech.Domain.Specifications;
using Maincotech.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maincotech.Cms.Services
{
    public class AdminService : IAdminService
    {
        private readonly DbContext _dbContext;
        private ICacheManager _cacheManager;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Article> _articleRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<ArticleTag> _articleTagRepository;
        private readonly IRepository<ArticleSEO> _articleSEORepository;

        public AdminService(DbContext cmsDbContext, ICacheManager cacheManager = null)
        {
            _dbContext = cmsDbContext;
            _cacheManager = cacheManager ?? new RuntimeCacheProvider();
            _categoryRepository = new EntityFrameworkRepository<Category>(new EntityFrameworkRepositoryContext(_dbContext));
            _articleRepository = new EntityFrameworkRepository<Article>(new EntityFrameworkRepositoryContext(_dbContext));
            _tagRepository = new EntityFrameworkRepository<Tag>(new EntityFrameworkRepositoryContext(_dbContext));
            _articleTagRepository = new EntityFrameworkRepository<ArticleTag>(new EntityFrameworkRepositoryContext(_dbContext));
            _articleSEORepository = new EntityFrameworkRepository<ArticleSEO>(new EntityFrameworkRepositoryContext(_dbContext));
        }

        public async Task CreateOrUpdateCategory(CategoryDto dto)
        {
            var entity = dto.To<Category>();
            _categoryRepository.AddOrUpdate(entity);
            _categoryRepository.Context.Commit();
        }

        public async Task DeleteCategories(List<Guid> categoriesToBeDeleted)
        {
            if (categoriesToBeDeleted.IsNotNullOrEmpty())
            {
                _categoryRepository.RemoveAll(DomainObjectSpecifications.IdIn<Category>(categoriesToBeDeleted));
            }
        }

        public async Task<CategoryDto> GetCategory(Guid id)
        {
            var entity = _categoryRepository.Find(DomainObjectSpecifications.Id<Category>(id), x => x.Children);
            return entity.To<CategoryDto>();
        }

        public async Task<IEnumerable<HierarchicalCategory>> GetHierarchicalCategory(List<Guid> categoriesToBeExcluded)
        {
            List<Category> entities;
            if (categoriesToBeExcluded.IsNotNullOrEmpty())
            {
                entities = _categoryRepository.FindAll(DomainObjectSpecifications.IdNotIn<Category>(categoriesToBeExcluded), x => x.Parent).ToList();
            }
            else
            {
                entities = _categoryRepository.FindAll(x => x.Parent).ToList();
            }
            TreeExtensions.ITree<Category> virtualRootNode = entities.ToTree((parent, child) => child.ParentId == parent.Id, x => x.Name);
            List<TreeExtensions.ITree<Category>> flattenedNodes = virtualRootNode.Children.Flatten(node => node.Children).ToList();

            return flattenedNodes.To<List<HierarchicalCategory>>();
        }

        public async Task<IEnumerable<CategoryDto>> GetCategories(SortGroup sortGroup, FilterCondition filters)
        {
            var entities =  _categoryRepository.GetAll(sortGroup, filters);
            return entities.To<List<CategoryDto>>();
        }

        public async Task CreateOrUpdateArticle(ArticleDto dto)
        {
            var entity = dto.To<Article>();
            _articleRepository.AddOrUpdate(entity);
            var tagEntities = new List<Tag>();
            if (dto.Tags.IsNotNullOrEmpty())
            {
                var tags = dto.Tags.Split(new char[] { ',' });
                foreach (var tag in tags)
                {
                    var tagEntity = _tagRepository.Find(CmsSpecifications.TagsWithNameAndType(tag, TagType.Public));
                    if (tagEntity == null)
                    {
                        tagEntity = new Tag() { Id = Guid.NewGuid(), TagType = TagType.Public, Name = tag };
                        _tagRepository.Add(tagEntity);
                        tagEntities.Add(tagEntity);
                    }
                }
            }

            var associatedTags = _articleTagRepository.FindAll(CmsSpecifications.TagsWithArticleId(dto.Id)).Select(x => x.TargetId).ToList();
            var tagsToBeDeleted = associatedTags.Where(x => tagEntities.Any(tag => tag.Id == x) == false);
            var tagsNeedToBeAdded = tagEntities.Where(x => associatedTags.Contains(x.Id) == false);

            if (tagsToBeDeleted.Any())
            {
                _articleTagRepository.RemoveAll(DomainObjectSpecifications.IdIn<ArticleTag>(tagsToBeDeleted.ToList()));
            }
            if (tagsNeedToBeAdded.Any())
            {
                foreach (var tagId in tagsNeedToBeAdded)
                {
                    var association = new ArticleTag { Id = Guid.NewGuid() };
                    association.SourceId = dto.Id;
                    association.TargetId = tagId.Id;
                    _articleTagRepository.Add(association);
                }
            }

            if (dto.IsPublished)
            {
                _articleSEORepository.AddOrUpdate(new ArticleSEO
                {
                    Id = dto.Id,
                    Description = dto.Summary,
                    Image = dto.Cover,
                    Keywords = dto.Tags,
                    Locale = dto.ContentLanguage,
                    PageName = dto.PageName,
                    Title = dto.Title,
                });
            }
            else
            {
                _articleSEORepository.Remove(new ArticleSEO { Id = entity.Id });
            }
            _articleRepository.Context.Commit();
        }

        public async Task<ArticleDto> GetArticle(Guid id)
        {
            var entity = _articleRepository.Find(DomainObjectSpecifications.Id<Article>(id));
            if (entity == null)
            {
                return null;
            }
            var dto = entity.To<ArticleDto>();
            var tags = _articleTagRepository.FindAll(CmsSpecifications.TagsWithArticleId(id), x => x.Target).Select(x => x.Target.Name);
            if (tags.Any())
            {
                dto.Tags = string.Join(",", tags);
            }

            return dto;
        }

        public async Task<PagedResult<ArticleDto>> GetPagedArticles(PagingQuery pagingQuery)
        {
            var entities = _articleRepository.GetPagedResult(pagingQuery);
            return entities.To<PagedResult<ArticleDto>>();
        }

        public async Task<IEnumerable<string>> GetPublicTags()
        {
            return _tagRepository.FindAll(CmsSpecifications.TagsWithType(TagType.Public)).Select(x => x.Name);
        }

        public async Task<IEnumerable<CategoryDto>> GetArticleCategories()
        {
            var ids = Enumerable.Select(_dbContext.Set<Article>(), x => x.CategoryId).Distinct().ToList();
            if (ids.Count == 0)
            {
                return Array.Empty<CategoryDto>();
            }
            var entities = _categoryRepository.FindAll(DomainObjectSpecifications.IdIn<Category>(ids)).ToList();
            return entities.To<List<CategoryDto>>();
        }
    }
}