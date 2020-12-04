using AntDesign;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Maincotech.Cms.Pages.Category
{
    [Authorize(Policy = "Admin")]
    public partial class Index
    {
        [Inject]
        public IndexViewModel IndexViewModel
        {
            get => ViewModel;
            set => ViewModel = value;
        }

        private readonly ListGridType _listGridType = new ListGridType
        {
            Gutter = 24,
            Column = 4
        };

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _IsLoading = true;
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

        public void Create()
        {
            NavigationManager.NavigateTo("/admin/categories/edit");
        }

        public void Edit(Guid id)
        {
            NavigationManager.NavigateTo($"/admin/categories/edit/{id}", forceLoad: true);
        }
    }
}