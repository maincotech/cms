using Maincotech.Localization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Maincotech.Cms
{
    public class CmsOptionsBuilder
    {
        public CmsOptionsBuilder()
        {
            Options = new CmsOptions
            {
                SupportLanguages = new List<string> { "en-US", "ja-JP", "zh-CN" },
                JqueryUpload = "/api/files/jqueryUpload",
                VditorUpload = "/api/files/vditorUpload",
            };
        }

        public CmsOptions Options { get; }

        public CmsOptionsBuilder UseLayout(Type layoutType)
        {
            Options.Layout = layoutType;

            return this;
        }

        public CmsOptionsBuilder UseUploadAPI(string jqueryUpload, string vditorUpload)
        {
            Options.JqueryUpload = jqueryUpload;
            Options.VditorUpload = vditorUpload;
            return this;
        }

        public CmsOptionsBuilder RegisterCmsLocalizer(IServiceProvider serviceProvider)
        {
            var localizer = serviceProvider.GetRequiredService<ILocalizer>();
            localizer.AddAdditionalLocalizer(new CmsLocalizer(localizer.CurrentCulture));
            return this;
        }

        public CmsOptionsBuilder SetSupportLanguages(List<string> SupportLanguages)
        {
            Options.SupportLanguages = SupportLanguages;
            return this;
        }

        public CmsOptionsBuilder UseAreaName(IServiceProvider serviceProvider, string areaName)
        {
            Options.AreaName = areaName;
            return this;
        }
    }
}