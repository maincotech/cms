using Maincotech.Domain;
using Maincotech.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Maincotech.Cms.Models
{
    public class Article : DomainObject
    {
        public string Title { get; set; }

        //[MaxLength(500)]
        public string Summary { get; set; }

        public string HtmlContent { get; set; }

        public string MarkdownContent { get; set; }

        //  [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public Guid CategoryId { get; set; }

        // [ForeignKey("ArticleId")]
        //public virtual List<Attachment> Attachments { get; set; }

        // [ForeignKey("ArticleId")]
        public virtual List<Comment> Comments { get; set; }

        public bool IsPublished { get; set; }

        public int Views { get; set; }
        public int Likes { get; set; }

        //For SEO
        public string Cover { get; set; }
        public string PageName { get; set; }
        public string ContentLanguage { get; set; }
    }

    public class ArticleMappingConfiguration : EntityTypeConfiguration<Article>
    {
        public override void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.ToTable(nameof(Article));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(50);
            builder.Property(x => x.ContentLanguage).HasMaxLength(10);
            builder.Property(x => x.Summary).HasMaxLength(500);
            builder.Property(x => x.HtmlContent);
            builder.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId);
            //builder.HasMany(x => x.Attachments).WithOne().HasForeignKey(x => x.ArticleId);
            builder.HasMany(x => x.Comments).WithOne().HasForeignKey(x => x.ArticleId);
            builder.Property(x => x.IsPublished);
            base.Configure(builder);
        }
    }
}