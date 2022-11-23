using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class KurverSkalHaGyldigGeometri : Rule<IGmlValidationInputV1>
    {
        public override void Create()
        {
            Id = "gml.kurve.1";
        }

        protected override void Validate(IGmlValidationInputV1 input)
        {
            if (!input.Surfaces.Any() && !input.Solids.Any())
                SkipRule();

            input.Surfaces.ForEach(Validate);
            input.Solids.ForEach(Validate);
        }

        private void Validate(GmlDocument document)
        {
            var indexedCurveGeometries = document.GetGeometriesByType(GmlGeometry.Curve, GmlGeometry.LineString)
                .Where(indexed => !indexed.IsValid)
                .ToList();

            foreach (var indexed in indexedCurveGeometries)
            {
                var errorMessage = indexed.ErrorMessage ?? "Kurven har ugyldig geometri.";

                this.AddMessage(
                    $"{GmlHelper.GetNameAndId(indexed.Element)}: {errorMessage}",
                    document.FileName,
                    new[] { indexed.Element.GetXPath() },
                    new[] { GmlHelper.GetFeatureGmlId(indexed.Element) }
                );
            }
        }
    }
}
