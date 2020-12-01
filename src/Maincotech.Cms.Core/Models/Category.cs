using Maincotech.Domain;
using Maincotech.EF;
using Maincotech.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Maincotech.Cms.Models
{
    public class Category : EntityBase
    {
        /// <summary>
        /// Sub categories of the current category.
        /// </summary>
        //[ForeignKey("ParentId")]
        public virtual List<Category> Children { get; set; }

        /// <summary>
        /// The category icon file
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The name of the category
        /// </summary>
        // [Required(ErrorMessageResourceName = "NameIsRequired", ErrorMessageResourceType = typeof(CMSCoreResources))]
        // [Index(IsUnique = true)]
        // [MaxLength(450)]
        [Localizable]
        public string Name { get; set; }

        [Localizable]
        public string Description { get; set; }

        /// <summary>
        /// Parent category's id. If is null , means the category is root category.
        /// </summary>
        // [Index]
        public Guid? ParentId { get; set; }

        public virtual Category Parent { get; set; }
    }

    public class CategoryMappingConfiguration : EntityTypeConfiguration<Category>
    {
        public override void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable(nameof(Category));
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name);
            builder.Property(x => x.Description);

            builder.HasMany(x => x.Children)
                .WithOne(x => x.Parent)
                .HasForeignKey(x => x.ParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            base.Configure(builder);
        }
    }
}