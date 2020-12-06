using Maincotech.Cms.Services;
using Maincotech.Data;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Maincotech.Cms.Pages.Admin.Category
{
    public class IndexViewModel : ReactiveObject
    {
        private static Maincotech.Logging.ILogger _Logger = AppRuntimeContext.Current.GetLogger<IndexViewModel>();

        private readonly IAdminService _dataAdminService;

        private readonly ObservableAsPropertyHelper<IEnumerable<TreeExtensions.ITree<CategoryViewModel>>> _categories;
        public IEnumerable<TreeExtensions.ITree<CategoryViewModel>> Categories => _categories.Value;

        private readonly ObservableAsPropertyHelper<bool> _isLoading;
        public bool IsLoading => _isLoading.Value;

        public ReactiveCommand<Unit, IEnumerable<TreeExtensions.ITree<CategoryViewModel>>> Load { get; }

        public IndexViewModel()
        {
            _dataAdminService = AppRuntimeContext.Current.Resolve<IAdminService>();
            Load = ReactiveCommand.CreateFromTask(LoadAll);
            _categories = Load.ToProperty(this, x => x.Categories, scheduler: RxApp.MainThreadScheduler);
            Load.ThrownExceptions.Subscribe(exception =>
            {
                _Logger.Error("Unexpected error occurred.", exception);
            });

            this.WhenAnyObservable(x => x.Load.IsExecuting).ToProperty(this, x => x.IsLoading, out _isLoading);
            // Load.Execute(Unit.Default).Subscribe();

            //Create = ReactiveCommand.Create(() => _navigationManager.NavigateTo("/categories/edit"));
        }

   

        private async Task<IEnumerable<TreeExtensions.ITree<CategoryViewModel>>> LoadAll()
        {
            var result = new List<TreeExtensions.ITree<CategoryViewModel>>() { null };
            var entities = await _dataAdminService.GetCategories(null, null).ConfigureAwait(false);
            var viewModels = AppRuntimeContext.Current.Adapt<List<CategoryViewModel>>(entities);
            TreeExtensions.ITree<CategoryViewModel> virtualRootNode = viewModels.ToTree((parent, child) => child.ParentId == parent.Id.ToString(), x => x.Name);
            result.AddRange(virtualRootNode.Children);
            return result;
        }
    }
}