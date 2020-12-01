using Maincotech.Domain;
using Maincotech.EF;

namespace Maincotech.Cms.Models
{
    public class ArticleAttachment : AssociationBase<Article, Attachment>
    {
        public ArticleAttachment()
        {
            Relation = "HasZeroToMany";
        }
    }

    public class ArticleAttachmentMappingConfiguration : AssociationMappingConfiguration<ArticleAttachment, Article, Attachment>
    {
    }
}