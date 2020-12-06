using Maincotech.Localization;
using Microsoft.AspNetCore.Components;

namespace Maincotech.Cms.Pages.Admin.Blog
{
    public partial class ArticleBriefView
    {
        [Inject] public ILocalizer L { get; set; }
        [Parameter] public BlogViewModel Data { get; set; }

        private string GetBriefData()
        {
            if (string.IsNullOrEmpty(Data.MarkdownContent))
            {
                return Data.Summary;
            }
            return Data.MarkdownContent.Length > 150 ? $"{Data.MarkdownContent.Substring(0, 147)}..." : Data.MarkdownContent;
        }
    }
}