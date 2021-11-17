using DiBK.RuleValidator.Config;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DiBK.RuleValidator.Rules.Gml.Tests.Setup
{
    public class RuleConfigFixture
    {
        public RuleConfigFixture()
        {
            var services = new ServiceCollection();

            services.AddRuleValidator(new[] {
                Assembly.Load("DiBK.RuleValidator.Rules.Gml")
            });
            
            var serviceProvider = services.BuildServiceProvider();

            RuleConfigs = serviceProvider.GetRequiredService<IRuleConfigs>();
        }

        public IRuleConfigs RuleConfigs { get; private set; }
    }
}
