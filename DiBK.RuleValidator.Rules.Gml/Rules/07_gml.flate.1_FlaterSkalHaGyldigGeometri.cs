using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using DiBK.RuleValidator.Rules.Gml.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    [Translation("gml.flate.1")]
    public class FlaterSkalHaGyldigGeometri : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.flate.1";

            DependOn<AvgrensningenTilEnFlateKanIkkeKrysseSegSelv>().ToExecute();
            DependOn<HullMåLiggeInnenforFlatensYtreAvgrensning>().ToExecute();
            DependOn<HullKanIkkeOverlappeAndreHullISammeFlate>().ToExecute();
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any())
                SkipRule();

            data.Surfaces.ForEach(Validate);
        }

        private void Validate(GmlDocument document)
        {
            var surfaceElements = GetSurfaceElements(document);

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
                        Translate("Message", GmlHelper.GetNameAndId(element)),
                        document.FileName,
                        new[] { element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(element) }
                    );
                }
            }
        }

        private List<XElement> GetSurfaceElements(GmlDocument document)
        {
            var surfaceElements = document.GetFeatureGeometryElements(GmlGeometry.MultiSurface, GmlGeometry.Surface, GmlGeometry.Polygon);

            var invalidSurfaceXPaths = GetInvalidSurfaceXPaths(document.Id);

            if (!invalidSurfaceXPaths.Any())
                return surfaceElements.ToList();

            return surfaceElements
                .Where(element => !invalidSurfaceXPaths.Contains(element.GetXPath()))
                .ToList();
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
