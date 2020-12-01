using Maincotech.Localization;
using System;
using System.Collections.Generic;

namespace Maincotech.Cms
{
    public class CmsOptions
    {
        public string JqueryUpload { get; set; }
        public string VditorUpload { get; set; }
        public List<string> SupportLanguages { get; set; }
        public string AreaName { get; set; }
        public Type Layout { get; set; }
    }
}