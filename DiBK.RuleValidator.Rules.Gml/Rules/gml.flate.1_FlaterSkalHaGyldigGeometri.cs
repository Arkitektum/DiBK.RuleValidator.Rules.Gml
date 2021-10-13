using DiBK.RuleValidator.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class FlaterSkalHaGyldigGeometri : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.flate.1";
            Name = "Flater skal ha gyldig geometri";
            Documentation = "https://dibk.atlassian.net/wiki/spaces/FP/pages/1933574255/gml.flate.1";

            DependOn<AvgrensningenTilEnFlateKanIkkeKrysseSegSelv>().ToExecute();
        }

        protected override Status Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any())
                return Status.NOT_EXECUTED;

            data.Surfaces.ForEach(Validate);

            return HasMessages ? Status.FAILED : Status.PASSED;
        }

        private void Validate(GmlDocument document)
        {
            var selfIntersections = GetData<List<string>>($"SelfIntersections_{document.Id}");

            var surfaceElements = document.GetFeatures()
                .GetElements("*/gml:MultiSurface | */gml:Surface | */gml:Polygon | */gml:PolygonPatch")
                .Where(element => !selfIntersections.Contains(element.GetAttribute("gml:id")));

            foreach (var element in surfaceElements)
            {
                using var geometry = document.GetOrCreateGeometry(element, out var errorMessage);

                if (geometry == null)
                {
                    this.AddMessage(errorMessage, document.FileName, new[] { element.GetXPath() }, new[] { GmlHelper.GetFeatureGmlId(element) });
                }
                else if (!geometry.IsValid())
                {
                    this.AddMessage(
                        $"{element.GetName()} '{element.GetAttribute("gml:id")}': Geometrien er ugyldig.", 
                        document.FileName, 
                        new[] { element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(element) }
                    );
                }
            }
        }
    }
}
