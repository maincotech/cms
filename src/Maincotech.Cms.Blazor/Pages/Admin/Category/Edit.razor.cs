using AntDesign;
using Maincotech.Web.Components.JQuery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Maincotech.Cms.Pages.Admin.Category
{
    public partial class Edit
    {
        [Parameter] public string Id { get; set; }

        [Inject]
        public MessageService MessageService { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (Id.IsNotNullOrEmpty())
            {
                ViewModel = new CategoryViewModel()
                {
                    Id = Guid.Parse(Id)
                };
                await ViewModel.LoadPossibleParents();
                await ViewModel.Load();
            }
            else
            {
                ViewModel = new CategoryViewModel()
                {
                    Id = Guid.NewGuid()
                };
                await ViewModel.LoadPossibleParents();
            }
        }

        private bool BeforeUpload(UploadFileItem file)
        {
            var isJpgOrPng = file.Type == "image/jpeg" || file.Type == "image/png";
            if (!isJpgOrPng)
            {
                MessageService.Error("You can only upload JPG/PNG file!");
            }
            var isLt2M = file.Size / 1024 / 1024 < 2;
            if (!isLt2M)
            {
                MessageService.Error("Image must smaller than 2MB!");
            }
            return isJpgOrPng && isLt2M;
        }

        private bool _IsUploadingIcon;

        public bool IsUploadingIcon
        {
            get => _IsUploadingIcon;
            set
            {
                if (_IsUploadingIcon != value)
                {
                    _IsUploadingIcon = value;
                    InvokeAsync(StateHasChanged);
                }
            }
        }

        private void HandleChange(UploadInfo fileinfo)
        {
            IsUploadingIcon = fileinfo.File.State == UploadState.Uploading;

            if (fileinfo.File.State == UploadState.Success)
            {
                var uploadResult = fileinfo.File.GetResponse<UploadResult>(new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                ViewModel.Icon = uploadResult.Url;
            }
            InvokeAsync(StateHasChanged);
        }

        private void Cancel()
        {
            var targetPath = Options.AdminAreaName.IsNullOrEmpty() ? "/categories" : $"/{Options.AdminAreaName}/categories";
            NavigationManager.NavigateTo(targetPath);
        }

        private async Task Save()
        {
            await ViewModel.Save();
            var targetPath = Options.AdminAreaName.IsNullOrEmpty() ? "/categories" : $"/{Options.AdminAreaName}/categories";
            NavigationManager.NavigateTo(targetPath, forceLoad: true);
        }
    }
}