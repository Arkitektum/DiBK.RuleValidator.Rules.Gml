using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class KurverSkalHaGyldigGeometri : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.kurve.1";
            Name = "Kurver skal ha gyldig geometri";
            Documentation = "https://dibk.atlassian.net/wiki/spaces/FP/pages/1918763029/gml.kurve.1";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any())
                SkipRule();

            data.Surfaces.ForEach(Validate);
        }

        private void Validate(GmlDocument document)
        {
            var curveElements = document.GetFeatures().GetElements("//gml:Curve");

            foreach (var element in curveElements)
            {
                using var geometry = document.GetOrCreateGeometry(element, out var errorMessage);

                if (geometry == null)
                {
                    this.AddMessage(errorMessage, document.FileName, new[] { element.GetXPath() }, new[] { GmlHelper.GetFeatureGmlId(element) });
                }
            }
        }
    }
}
