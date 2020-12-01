using Maincotech.Localization;
using System.Globalization;
using System.Reflection;

namespace Maincotech.Cms
{
    public class CmsLocalizer : AssemblyBasedLocalizer
    {
        public CmsLocalizer(CultureInfo cultureInfo) : base(Assembly.GetExecutingAssembly(), cultureInfo)
        {
        }
    }
}