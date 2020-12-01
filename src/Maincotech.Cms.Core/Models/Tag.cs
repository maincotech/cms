using Maincotech.Domain;
using Maincotech.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maincotech.Cms.Models
{
    public class Tag : EntityBase
    {
        public string Name { get; set; }

        public TagType TagType { get; set; }
    }

    public enum TagType
    {
        Public,
        Shared,
        Private
    }

    public class TagMappingConfiguration : EntityTypeConfiguration<Tag>
    {
        public override void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable(nameof(Tag));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(50).IsRequired();

            base.Configure(builder);
        }
    }
}