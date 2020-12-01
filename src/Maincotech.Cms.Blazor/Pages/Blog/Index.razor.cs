using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Maincotech.Cms.Pages.Blog
{
    public partial class Index
    {
        [Inject] public IndexViewModel IndexViewModel { get => ViewModel; set => ViewModel = value; }

        private bool _IsLoading;

        public bool IsLoading
        {
            get => _IsLoading;
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    InvokeAsync(StateHasChanged);
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await ViewModel.LoadCategories();
            Search();
        }

        private void Search()
        {
            IsLoading = true;
            ViewModel.Load.Execute().Subscribe(items => { },
                ex =>
                {
                    Console.WriteLine(ex);
                    IsLoading = false;
                },
                () =>
                {
                    IsLoading = false;
                });
        }

        private void Create()
        {
            var targetPath = Options.AreaName.IsNullOrEmpty() ? "/Blogs/Edit" : $"/{Options.AreaName}/Blogs/Edit";
            NavigationManager.NavigateTo(targetPath);
        }

        private string GetEditLink(string id)
        {
            return Options.AreaName.IsNullOrEmpty() ? $"/Blogs/Edit/{id}" : $"/{Options.AreaName}/Blogs/Edit/{id}";
        }
    }
}