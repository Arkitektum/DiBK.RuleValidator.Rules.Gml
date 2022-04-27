using DiBK.RuleValidator.Config;
using Microsoft.Extensions.DependencyInjection;
using OSGeo.OGR;
using System.Globalization;

namespace DiBK.RuleValidator.Rules.Gml.Tests.Setup
{
    public class RuleConfigFixture
    {
        public RuleConfigFixture()
        {
            var cultureInfo = new CultureInfo("nb-NO");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            Ogr.RegisterAll();
            Ogr.UseExceptions();

            var services = new ServiceCollection();

            services.AddRuleValidator(settings =>
            {
                settings.AddRules("DiBK.RuleValidator.Rules.Gml");
                settings.MaxMessageCount = 500;
            });

            var serviceProvider = services.BuildServiceProvider();

            RuleSettings = serviceProvider.GetRequiredService<IRuleSettings>();
        }

        public IRuleSettings RuleSettings { get; private set; }
    }
}
