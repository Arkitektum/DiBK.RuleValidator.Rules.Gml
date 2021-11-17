using Microsoft.Extensions.DependencyInjection;
using OSGeo.OGR;
using System.Globalization;

namespace DiBK.RuleValidator.Rules.Gml.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure()
        {
            var cultureInfo = new CultureInfo("nb-NO");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            Ogr.RegisterAll();
            Ogr.UseExceptions();
        }
    }
}
