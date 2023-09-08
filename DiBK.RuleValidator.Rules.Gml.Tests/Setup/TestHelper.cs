using DiBK.RuleValidator.Config;
using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using DiBK.RuleValidator.Rules.Gml.Tests.Model;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DiBK.RuleValidator.Rules.Gml.Tests.Setup
{
    public class TestHelper
    {
        private static readonly string[] _manifestResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

        public static IRuleValidator GetRuleValidator(IRuleSettings ruleSettings)
        {
            return new RuleValidator(new RuleService(), new TranslationService(ruleSettings), ruleSettings, Mock.Of<ILogger<RuleValidator>>(), Mock.Of<ILogger<Rule>>());
        }

        public static IGmlValidationInputV1 GetGmlValidationInputV1(params string[] fileNames)            
        {
            var documents = fileNames
                .Select(fileName => GetGmlValidationInputV1(fileName))
                .ToList();

            return GmlValidationInputV1.Create(documents);
        }

        private static GmlDocument GetGmlValidationInputV1(string fileName)
        {
            using var stream = GetResourceStream(fileName);

            return GmlDocument.Create(new InputData(stream, fileName, null));
        }

        private static Stream GetResourceStream(string fileName)
        {
            var name = _manifestResourceNames.SingleOrDefault(name => name.EndsWith(fileName));

            if (name == null)
                return null;

            return Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
        }
    }
}
