using DiBK.RuleValidator.Extensions.Gml;
using System;
using System.Collections.Generic;

namespace DiBK.RuleValidator.Rules.Gml
{
    public interface IGmlValidationInputV1 : IDisposable
    {
        List<GmlDocument> Surfaces { get; }
        List<GmlDocument> Solids { get; }
    }
}
