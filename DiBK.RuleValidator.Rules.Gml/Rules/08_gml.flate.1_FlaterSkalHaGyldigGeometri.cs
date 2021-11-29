using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Rules.Gml.Constants;
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
            DependOn<HullMåLiggeInnenforFlatensYtreAvgrensning>().ToExecute();
            DependOn<HullKanIkkeOverlappeAndreHullISammeFlate>().ToExecute();
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
            var invalidSurfaceXPaths = GetInvalidSurfaceXPaths(document.Id);

            var surfaceElements = document.GetFeatures()
                .GetElements("*/gml:MultiSurface | */gml:Surface | */gml:Polygon")
                .Where(element => !invalidSurfaceXPaths.Contains(element.GetXPath()));

            foreach (var element in surfaceElements)
            {
                using var geometry = document.GetOrCreateGeometry(element, out var errorMessage);

                if (geometry == null)
                {
                    this.AddMessage(
                        errorMessage, 
                        document.FileName, 
                        new[] { element.GetXPath() }, 
                        new[] { GmlHelper.GetFeatureGmlId(element) }
                    );
                }
                else if (!geometry.IsValid())
                {
                    this.AddMessage(
                        $"{GmlHelper.GetNameAndId(element)}: Geometrien er ugyldig.",
                        document.FileName,
                        new[] { element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(element) }
                    );
                }
            }
        }

        private List<string> GetInvalidSurfaceXPaths(string documentId)
        {
            var selfIntersections = GetData<HashSet<string>>(string.Format(DataKey.Selvkryss, documentId));
            var overlappingHoles = GetData<HashSet<string>>(string.Format(DataKey.OverlappendeHull, documentId));
            var holesOutsideBoundary = GetData<HashSet<string>>(string.Format(DataKey.HullUtenforYtreAvgrensning, documentId));

            var xPaths = new HashSet<string>();
            xPaths.UnionWith(selfIntersections);
            xPaths.UnionWith(overlappingHoles);
            xPaths.UnionWith(holesOutsideBoundary);

            return xPaths.ToList();
        }
    }
}
