using Maincotech.Domain;
using Maincotech.EF;

namespace Maincotech.Cms.Models
{
    public class ArticleTag : AssociationBase<Article, Tag>
    {
        public ArticleTag()
        {
            Relation = "HasZeroToMany";
        }
    }

    public class ArticleTagMappingConfiguration : AssociationMappingConfiguration<ArticleTag, Article, Tag>
    {
    }
}