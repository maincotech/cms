using AntDesign;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Maincotech.Cms.Pages.Blog
{
    public partial class View
    {
        [Parameter] public string PageName { get; set; }
        [Inject] public IndexViewModel IndexViewModel { get; set; }
        [Inject] AntDesign.JsInterop.InteropService InteropService { get; set; }
        [Inject] private MessageService MessageService { get; set; }

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
            if (IndexViewModel.Categories.IsNullOrEmpty())
            {
                IsLoadingCategories = true;
                IndexViewModel.LoadCategories.Execute().Subscribe(items => { },
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


        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            IsLoading = true;
            if (PageName.IsNullOrEmpty())
            {
                NavigationManager.NavigateTo("/exception/404");
                return;
            }
            else
            {
                var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                ViewModel = new BlogViewModel()
                {
                    PageName = PageName
                };
                //
            }
            IsLoading = true;
            ViewModel.Load.Execute().Subscribe(
                (unit) => { },
                (ex) =>
                {
                    Console.WriteLine(ex);

                    if (ex is FileNotFoundException)
                    {
                        IsLoading = false;
                        NavigationManager.NavigateTo("/exception/404");
                    }
                    else
                    {
                        NavigationManager.NavigateTo("/exception/500");
                    }
                },
                () =>
                {
                    IsLoading = false;
                });

            LoadCategories();
            IndexViewModel.SearchText = "";
            IndexViewModel.SelectedCategories.Clear();
        }

        private async Task Copy()
        {
            await InteropService.Copy(NavigationManager.Uri.ToString());
            await MessageService.Success("The link has been copied.");
        }

        private bool isLike;

        private async Task Like()
        {
            isLike = !isLike;
            await ViewModel.Like(isLike);
        }

        private void Search()
        {
            if (Options.UserAreaName.IsNullOrEmpty())
            {
                NavigationManager.NavigateTo($"/{L.CurrentCulture.Name}/blogs");
                return;
            }
            NavigationManager.NavigateTo($"{Options.UserAreaName}/{L.CurrentCulture.Name}/blogs");
        }
    }
}