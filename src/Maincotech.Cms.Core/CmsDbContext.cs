using Maincotech.Cms.Models;
using Maincotech.EF;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Maincotech.Cms
{
    public class CmsDbContext : BoundedDbContext
    {
        public CmsDbContext(DbContextOptions<CmsDbContext> options) : base(options)
        {
        }

        protected override IEnumerable<IMappingConfiguration> GetConfigurations()
        {
            var configurations = new List<IMappingConfiguration>
            {
                new TagMappingConfiguration(),
                new CommentMappingConfiguration(),
                new CategoryMappingConfiguration(),
                new AttachmentMappingConfiguration(),
                new ArticleTagMappingConfiguration(),
                new ArticleAttachmentMappingConfiguration(),
                new ArticleMappingConfiguration(),
                new ArticleSEOMappingConfiguration()
            };
            return configurations;
        }

        public override void EnsureCreated()
        {
            base.EnsureCreated(); //Make sure database is created.

            //Make sure table is created.
            this.EnsureTables();
        }

        public override void EnsureDeleted()
        {
            //delete tables
            this.DropTables();
            base.EnsureDeleted();
        }
    }
}