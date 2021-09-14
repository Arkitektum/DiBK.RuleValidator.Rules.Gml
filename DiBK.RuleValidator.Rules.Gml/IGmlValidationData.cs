using DiBK.RuleValidator.Extensions;
using System.Collections.Generic;

namespace DiBK.RuleValidator.Rules.Gml
{
    public interface IGmlValidationData
    {
        List<GmlDocument> Surfaces { get; }
        List<GmlDocument> Solids { get; }
    }
}
