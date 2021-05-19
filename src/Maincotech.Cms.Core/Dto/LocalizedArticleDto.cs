﻿using System;

namespace Maincotech.Cms.Dto
{
    public class LocalizedArticleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string HtmlContent { get; set; }
        public string MarkdownContent { get; set; }
        public string CategoryName { get; set; }

        public string Author { get; set; }
        public bool IsPublished { get; set; }
        public string Tags { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public string ContentLanguage { get; set; }
        public string PageName { get; set; }

        public int Views { get; set; }
        public int Likes { get; set; }
        public int CommentsCount { get; set; }
        public string Cover { get; set; }
    }
}