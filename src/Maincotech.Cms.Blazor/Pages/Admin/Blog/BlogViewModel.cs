using AntDesign;
using Maincotech.Cms.Dto;
using Maincotech.Logging;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Maincotech.Cms.Pages.Admin.Blog
{
    public class BlogViewModel : ReactiveObject
    {
        private string _Title;

        [Required]
        public string Title
        {
            get => _Title;
            set => this.RaiseAndSetIfChanged(ref _Title, value);
        }

        private string _Id;

        public string Id
        {
            get => _Id;
            set => this.RaiseAndSetIfChanged(ref _Id, value);
        }

        private bool _IsPublished;

        public bool IsPublished
        {
            get => _IsPublished;
            set => this.RaiseAndSetIfChanged(ref _IsPublished, value);
        }

        public bool IsLoaded { get; private set; }

        private static ILogger _Logger = AppRuntimeContext.Current.GetLogger<BlogViewModel>();
        private readonly Maincotech.Cms.Services.IAdminService _dataAdminService;

        public BlogViewModel()
        {
            _dataAdminService = AppRuntimeContext.Current.Resolve<Maincotech.Cms.Services.IAdminService>();
            Load = ReactiveCommand.CreateFromTask(LoadAll);
        }

        public ReactiveCommand<Unit, Unit> Load { get; }

        public string ContentLanguage { get; set; }
        public string PageName { get; set; }

        public async Task LoadAll()
        {
            //Load Categories
            var allCategories = (await _dataAdminService.GetCategories()).ToList();
            Categories = TypeConverters.Convert(allCategories, Guid.Empty);
            //Load Tags
            AllTags = (await _dataAdminService.GetPublicTags()).ToList();

            //Load Self
            if (Id.IsNotNullOrEmpty())
            {
                var entity = await _dataAdminService.GetArticle(Guid.Parse(Id));
                this.MergeDataFrom(entity.To<BlogViewModel>(), new List<string> { "Categories", "AllTags", "IsLoaded" });
            }
            else
            {
                Id = Guid.NewGuid().ToString();
            }

            IsLoaded = true;
        }

        private bool _IsLoadingCategories;

        public bool IsLoadingCategories
        {
            get => _IsLoadingCategories;
            set => this.RaiseAndSetIfChanged(ref _IsLoadingCategories, value);
        }
        public string Cover { get; set; }

        //public async Task Load()
        //{
        //    try
        //    {
        //        IsLoading = true;
        //        var entity = await _dataAdminService.GetArticle(Id);
        //        this.MergeDataFrom(entity.To<BlogViewModel>(), new List<string> { "Categories", "AllTags" });
        //    }
        //    finally
        //    {
        //        IsLoading = false;
        //    }
        //}

        public List<CascaderNode> Categories { get; set; } = new List<CascaderNode>();

        private bool _IsLoadingTags;

        public bool IsLoadingTags
        {
            get => _IsLoadingTags;
            set => this.RaiseAndSetIfChanged(ref _IsLoadingTags, value);
        }

        private DateTime _LastModifiedTime;

        public DateTime LastModifiedTime
        {
            get => _LastModifiedTime;
            set => this.RaiseAndSetIfChanged(ref _LastModifiedTime, value);
        }

        public DateTime CreationTime { get; set; }

        public List<string> AllTags { get; set; } = new List<string>();
        public IEnumerable<string> SelectedTags { get; set; } = new List<string>();

        private string _Author;

        public string Author
        {
            get => _Author;
            set => this.RaiseAndSetIfChanged(ref _Author, value);
        }

        public async Task Save()
        {
            var dto = this.To<ArticleDto>();
            await _dataAdminService.CreateOrUpdateArticle(dto);
        }

        public async Task LoadCategories()
        {
            if (IsLoadingCategories)
            {
                return;
            }
            try
            {
                IsLoadingCategories = true;
                var allCategories = (await _dataAdminService.GetCategories()).ToList();
                Categories = TypeConverters.Convert(allCategories, Guid.Empty);
            }
            finally
            {
                IsLoadingCategories = false;
            }
        }

        public async Task LoadTags()
        {
            if (IsLoadingTags)
            {
                return;
            }
            try
            {
                IsLoadingTags = true;

                AllTags = (await _dataAdminService.GetPublicTags()).ToList();
            }
            finally
            {
                IsLoadingTags = false;
            }
        }

        private string _Summary;

        public string Summary
        {
            get => _Summary;
            set => this.RaiseAndSetIfChanged(ref _Summary, value);
        }

        private string _Tags = string.Empty;

        public string Tags
        {
            get => _Tags;
            set => this.RaiseAndSetIfChanged(ref _Tags, value);
        }

        private string _CategoryId;

        [Required]
        public string CategoryId
        {
            get => _CategoryId;
            set => this.RaiseAndSetIfChanged(ref _CategoryId, value);
        }

        private string _HtmlContent;

        public string HtmlContent
        {
            get => _HtmlContent;
            set => this.RaiseAndSetIfChanged(ref _HtmlContent, value);
        }

        private string _MarkdownContent;

        public string MarkdownContent
        {
            get => _MarkdownContent;
            set => this.RaiseAndSetIfChanged(ref _MarkdownContent, value);
        }
    }
}