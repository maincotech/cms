using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive;
using System.Threading.Tasks;

namespace Maincotech.Cms.Pages.Blog
{
    public class BlogViewModel : ReactiveObject
    {
        private static Maincotech.Logging.ILogger _Logger = AppRuntimeContext.Current.GetLogger<BlogViewModel>();
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string HtmlContent { get; set; }
        public string MarkdownContent { get; set; }
        public string CategoryName { get; set; }
        public string Author { get; set; }
        public bool IsPublished { get; set; }
        public string Tags { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public string ContentLanguage { get; set; }
        public string PageName { get; set; }
        public string Views { get; set; }
        public string Likes { get; set; }
        public string CommentsCount { get; set; }
        public string Cover { get; set; }

        private Maincotech.Cms.Services.IUserService _service;

        public BlogViewModel()
        {
            _service = AppRuntimeContext.Current.Resolve<Maincotech.Cms.Services.IUserService>();
            Load = ReactiveCommand.CreateFromTask(LoadAll);
            Load.ThrownExceptions.Subscribe(exception =>
            {
                _Logger.Error("Unexpected error occurred.", exception);
            });
        }

        public ReactiveCommand<Unit, Unit> Load { get; }

        public async Task LoadAll()
        {
            var entity = await _service.GetLocalizedArticle(PageName, CultureInfo.CurrentCulture.Name);
            this.MergeDataFrom(entity.To<BlogViewModel>(), new List<string> { "Categories", "AllTags", "IsLoaded" });
        }

        public async Task Like(bool isLike)
        {
            await _service.Like(Id, isLike);
        }
    }
}