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

        private bool _IsLoadingCategories;

        public bool IsLoadingCategories
        {
            get => _IsLoadingCategories;
            set
            {
                if (_IsLoadingCategories != value)
                {
                    _IsLoadingCategories = value;
                    InvokeAsync(StateHasChanged);
                }
            }
        }

        private void LoadCategories()
        {
            if(ViewModel.Categories.IsNullOrEmpty())
            {
                IsLoadingCategories = true;
                ViewModel.LoadCategories.Execute().Subscribe(items => { },
                    ex =>
                    {
                        Console.WriteLine(ex);
                        IsLoadingCategories = false;
                    },
                    () =>
                    {
                        IsLoadingCategories = false;
                    });
            }            
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            LoadCategories();
            Search();
        }
    }
}