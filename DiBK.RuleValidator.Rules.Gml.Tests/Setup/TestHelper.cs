using DiBK.RuleValidator.Config;
using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Rules.Gml.Tests.Model;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
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
            return new RuleValidator(new RuleService(), ruleSettings, Mock.Of<ILogger<RuleValidator>>(), Mock.Of<ILogger<Rule>>());
        }

        public static IGmlValidationData GetGmlValidationData(
            string map2DFileName = null,
            string map3DFileName = null)
        {
            return GmlValidationData.Create(
                GetGmlValidationData(map2DFileName),
                GetGmlValidationData(map3DFileName)
            );
        }

        private static List<GmlDocument> GetGmlValidationData(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return new();

            using var stream = GetResourceStream(fileName);

            return new List<GmlDocument> { GmlDocument.Create(new InputData(stream, fileName, null)) };
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
