using Maincotech.Domain;
using Maincotech.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maincotech.Cms.Models
{
    public class ArticleSEO : EntityBase
    {
        public string PageName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Video { get; set; }
        public string Locale { get; set; }
        public string Keywords { get; set; }
    }

    public class ArticleSEOMappingConfiguration : EntityTypeConfiguration<ArticleSEO>
    {
        public override void Configure(EntityTypeBuilder<ArticleSEO> builder)
        {
            builder.ToTable(nameof(ArticleSEO));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(50);
            base.Configure(builder);
        }
    }
}