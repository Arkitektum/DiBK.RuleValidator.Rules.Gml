using DiBK.RuleValidator.Extensions.Gml;
using System;
using System.Collections.Generic;

namespace DiBK.RuleValidator.Rules.Gml
{
    public interface IGmlValidationData : IDisposable
    {
        List<GmlDocument> Surfaces { get; }
        List<GmlDocument> Solids { get; }
    }
}
