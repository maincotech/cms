using Microsoft.AspNetCore.Components;

namespace Maincotech.Cms.Pages.Blog
{
    public partial class ArticleBriefView
    {
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