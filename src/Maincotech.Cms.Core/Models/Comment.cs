using Maincotech.Domain;
using Maincotech.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Maincotech.Cms.Models
{
    public class Comment : DomainObject
    {
        public Guid ArticleId { get; set; }
        public string Content { get; set; }

        // [ForeignKey("ReplyTo")]
        public virtual List<Comment> Replies { get; set; }

        public Guid? ReplyTo { get; set; }
    }

    public class CommentMappingConfiguration : EntityTypeConfiguration<Comment>
    {
        public override void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable(nameof(Comment));
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.Replies).WithOne().HasForeignKey(x => x.ReplyTo);
            base.Configure(builder);
        }
    }
}