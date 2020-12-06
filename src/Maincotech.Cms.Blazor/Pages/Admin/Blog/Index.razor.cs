using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Maincotech.Cms.Pages.Admin.Blog
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
            var targetPath = Options.AdminAreaName.IsNullOrEmpty() ? "/Blogs/Edit" : $"/{Options.AdminAreaName}/Blogs/Edit";
            NavigationManager.NavigateTo(targetPath);
        }

        private string GetEditLink(string id)
        {
            return Options.AdminAreaName.IsNullOrEmpty() ? $"/Blogs/Edit/{id}" : $"/{Options.AdminAreaName}/Blogs/Edit/{id}";
        }
    }
}