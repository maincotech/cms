using Maincotech.Cms.Dto;
using Maincotech.Data;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive;
using System.Threading.Tasks;

namespace Maincotech.Cms.Pages.Blog
{
    public class IndexViewModel : ReactiveObject
    {
        private static Maincotech.Logging.ILogger _Logger = AppRuntimeContext.Current.GetLogger<IndexViewModel>();

        private Maincotech.Cms.Services.IUserService _service;
        private readonly ObservableAsPropertyHelper<IEnumerable<BlogViewModel>> _items;
        public IEnumerable<BlogViewModel> Items => _items.Value;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;
        public bool IsLoading => _isLoading.Value;

        private readonly ObservableAsPropertyHelper<IEnumerable<CategoryArticlesDto>> _categories;
        public IEnumerable<CategoryArticlesDto> Categories => _categories.Value;
        private readonly ObservableAsPropertyHelper<bool> _IsLoadingCategories;
        public bool IsLoadingCategories => _IsLoadingCategories.Value;

        public ReactiveCommand<Unit, IEnumerable<BlogViewModel>> Load { get; }

        public ReactiveCommand<Unit, IEnumerable<CategoryArticlesDto>> LoadCategories { get; }

        //    public ReactiveCommand<Unit, Unit> Create { get; }

        public string SearchText { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int Total { get; set; }
        public List<string> SelectedCategories { get; set; } = new List<string>();

        public IndexViewModel()
        {
            _service = AppRuntimeContext.Current.Resolve<Maincotech.Cms.Services.IUserService>();
            Load = ReactiveCommand.CreateFromTask(LoadAll);
            _items = Load.ToProperty(this, x => x.Items, scheduler: RxApp.MainThreadScheduler);
            Load.ThrownExceptions.Subscribe(exception =>
            {
                _Logger.Error("Unexpected error occurred.", exception);
            });

            this.WhenAnyObservable(x => x.Load.IsExecuting).ToProperty(this, x => x.IsLoading, out _isLoading);

            LoadCategories = ReactiveCommand.CreateFromTask(LoadBlogCategories);
            _categories = LoadCategories.ToProperty(this, x => x.Categories, scheduler: RxApp.MainThreadScheduler);
            LoadCategories.ThrownExceptions.Subscribe(exception =>
            {
                _Logger.Error("Unexpected error occurred.", exception);
            });

            this.WhenAnyObservable(x => x.LoadCategories.IsExecuting).ToProperty(this, x => x.IsLoadingCategories, out _IsLoadingCategories);
        }

        private async Task<IEnumerable<BlogViewModel>> LoadAll()
        {
            var result = new List<BlogViewModel>();

            var filterRules = new FilterCondition();

            if (SelectedCategories.IsNotNullOrEmpty())
            {
                var filterCategories = new FilterGroup();
                foreach (var category in SelectedCategories)
                {
                    filterCategories.Add(new FilterRule
                    {
                        Field = "CategoryId",
                        FilterOperator = FilterOperator.Equal,
                        PropertyValues = new List<object> { Guid.Parse(category) },
                        LogicalOperator = LogicalOperator.Or
                    });
                }

                filterRules.Add(filterCategories);
            }

            if (SearchText.IsNotNullOrEmpty())
            {
                var filterGroup = new FilterGroup()
                {
                    LogicalOperator = LogicalOperator.And
                };
                filterGroup.Add(new FilterRule("Title", FilterOperator.Contains, LogicalOperator.Or, SearchText));
                filterGroup.Add(new FilterRule("Summary", FilterOperator.Contains, LogicalOperator.Or, SearchText));
                filterGroup.Add(new FilterRule("MarkdownContent", FilterOperator.Contains, LogicalOperator.Or, SearchText));
                filterRules.Add(filterGroup);
            }

            var pageQuery = new PagingQuery
            {
                Pagination = new Pagination
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                },
                FilterCondition = filterRules
            };
            var entities = await _service.GetArticles(CultureInfo.CurrentCulture.Name, pageQuery);
            Total = entities.TotalRecords;
            if (entities.Count > 0)
            {
                var viewModels = AppRuntimeContext.Current.Adapt<List<BlogViewModel>>(entities);
                result.AddRange(viewModels);
            }
            //  await Task.Delay(1000 * 3);
            return result;
        }

        public async Task<IEnumerable<CategoryArticlesDto>> LoadBlogCategories()
        {
            return await _service.GetCategoryArticles(CultureInfo.CurrentCulture.Name);
        }
    }
}