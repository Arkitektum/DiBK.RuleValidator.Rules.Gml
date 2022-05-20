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
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any() && !data.Solids.Any())
                SkipRule();

            data.Surfaces.ForEach(Validate);
            data.Solids.ForEach(Validate);
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
