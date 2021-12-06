using DiBK.RuleValidator.Config;
using Microsoft.Extensions.DependencyInjection;

namespace DiBK.RuleValidator.Rules.Gml.Tests.Setup
{
    public class RuleConfigFixture
    {
        public RuleConfigFixture()
        {
            var services = new ServiceCollection();

            services.AddRuleValidator(settings =>
            {
                settings.AddAssemblies("DiBK.RuleValidator.Rules.Gml");
                settings.MaxMessageCount = 500;
            });

            var serviceProvider = services.BuildServiceProvider();

            RuleSettings = serviceProvider.GetRequiredService<IRuleSettings>();
        }

        public IRuleSettings RuleSettings { get; private set; }
    }
}
