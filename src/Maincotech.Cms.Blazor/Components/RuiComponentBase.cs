using Maincotech.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ReactiveUI;
using ReactiveUI.Blazor;

namespace Maincotech.Cms.Blazor.Components
{
    public partial class RuiComponentBase<TViewModel> : ReactiveComponentBase<TViewModel> where TViewModel : ReactiveObject
    {
        [Inject] protected ILocalizer L { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] protected CmsOptions Options { get; set; }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            //Check if is in the translation mode
            var isInTranslationMode = false;
            if (NavigationManager.TryGetQueryString<bool>("translation", out isInTranslationMode))
            {
                L.IsInTranslationMode = isInTranslationMode;
            }
        }
    }
}