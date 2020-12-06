using AntDesign;
using Maincotech.Cms.Dto;
using Maincotech.Data;
using Maincotech.Localization.Models;
using Maincotech.Logging;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maincotech.Cms.Pages.Admin.Category
{
    public class CategoryViewModel : ReactiveObject
    {
        private static ILogger _Logger = AppRuntimeContext.Current.GetLogger<CategoryViewModel>();

        private readonly Maincotech.Cms.Services.IAdminService _dataAdminService;
        private readonly Maincotech.Localization.ILocalizationService _localizationService;

        private bool _IsLoadingParents;

        public bool IsLoadingParents
        {
            get => _IsLoadingParents;
            set => this.RaiseAndSetIfChanged(ref _IsLoadingParents, value);
        }

        private bool _IsLoading;

        public bool IsLoading
        {
            get => _IsLoading;
            set => this.RaiseAndSetIfChanged(ref _IsLoading, value);
        }

        public CategoryViewModel()
        {
            _dataAdminService = AppRuntimeContext.Current.Resolve<Maincotech.Cms.Services.IAdminService>();
            _localizationService = AppRuntimeContext.Current.Resolve<Maincotech.Localization.ILocalizationService>();
        }

        private string _Icon;

        public string Icon
        {
            get => _Icon;
            set => this.RaiseAndSetIfChanged(ref _Icon, value);
        }

        private string _Name;

        public string Name
        {
            get => _Name;
            set => this.RaiseAndSetIfChanged(ref _Name, value);
        }

        private string _Description;

        public string Description
        {
            get => _Description;
            set => this.RaiseAndSetIfChanged(ref _Description, value);
        }

        private Guid _Id;

        public Guid Id
        {
            get => _Id;
            set => this.RaiseAndSetIfChanged(ref _Id, value);
        }

        private string _ParentId;

        public string ParentId
        {
            get => _ParentId;
            set => this.RaiseAndSetIfChanged(ref _ParentId, value);
        }

        public async Task Load()
        {
            try
            {
                IsLoading = true;
                var entity = await _dataAdminService.GetCategory(Id);
                this.MergeDataFrom(entity.To<CategoryViewModel>());
            }
            finally
            {
                IsLoading = false;
            }
        }

        public List<CascaderNode> PossibleParents { get; set; }

        public async Task Save()
        {
            var entity = this.To<CategoryDto>();
            await _dataAdminService.CreateOrUpdateCategory(entity);
        }

        public async Task LoadPossibleParents()
        {
            if (IsLoadingParents)
            {
                return;
            }
            try
            {
                IsLoadingParents = true;

                SortGroup sortGroup = new SortGroup
                {
                    SortRules = new List<SortRule>
                    {
                        new SortRule { Field = "Name", SortOrder = SortOrder.Ascending}
                    }
                };
                var filters = new FilterCondition();
                var allCategories = (await _dataAdminService.GetCategories(sortGroup, filters)).ToList();

                PossibleParents = TypeConverters.Convert(allCategories, Id);
                //var categories = await _dataAdminService.GetCategories(idsShouldBeExcluded);
                // PossibleParents.AddRange(categories);
            }
            finally
            {
                IsLoadingParents = false;
            }
        }

        public string GetTermsOfName(string language)
        {
            var key = $"Category.{Id}.Name"; //TermsHelper.GenerateTermsKey(category, "Name");
            var terms = _localizationService.GetTerms(key, language).GetAwaiter().GetResult();
            return terms?.Value;
        }

        public void AddTermsOfName(string language, string terms)
        {
            var key = $"Category.{Id}.Name";
            var entity = new Terms() { Id = Guid.NewGuid(), CultureCode = language, Value = terms, Key = key };
            _localizationService.AddOrUpdateTerms(entity);
        }

        public void AddTermsOfDescription(string language, string terms)
        {
            var key = $"Category.{Id}.Description";
            var entity = new Terms() { Id = Guid.NewGuid(), CultureCode = language, Value = terms, Key = key };
            _localizationService.AddOrUpdateTerms(entity);
        }

        public string GetTermsOfDescription(string language)
        {
            var key = $"Category.{Id}.Description";
            var terms = _localizationService.GetTerms(key, language).GetAwaiter().GetResult();
            return terms?.Value;
        }
    }
}