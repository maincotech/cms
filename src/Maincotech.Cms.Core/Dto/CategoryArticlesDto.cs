using System;

namespace Maincotech.Cms.Dto
{
    public class CategoryArticlesDto
    {
        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int Count { get; set; }
    }
}