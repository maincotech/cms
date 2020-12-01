using Maincotech.Domain;
using Maincotech.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maincotech.Cms.Models
{
    public class Attachment : EntityBase
    {
        public string Name { get; set; }
        public long? Size { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public string RbsInfo { get; set; }
    }

    public class AttachmentMappingConfiguration : EntityTypeConfiguration<Attachment>
    {
        public override void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.ToTable(nameof(Attachment));
            builder.HasKey(x => x.Id);
            base.Configure(builder);
        }
    }
}