using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using OSGeo.OGR;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class SamsvarendeAvgrensingsgeometri : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.avgr.1";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any() || data.Surfaces.All(document => !Validate(document, data.Surfaces)))
                SkipRule();
        }

        private bool Validate(GmlDocument document, List<GmlDocument> documents)
        {
            var hasBoundedBy = false;
            var featureElements = document.GetFeatureElements();

            foreach (var featureElement in featureElements)
            {
                var boundedByElements = featureElement.Elements()
                    .Where(element => element.Name.LocalName.StartsWith("avgrensesAv"));

                if (!boundedByElements.Any())
                    continue;

                hasBoundedBy = true;

                var elementGroupings = boundedByElements
                    .GroupBy(element => element.Name.LocalName);

                foreach (var groupedBoundedByElements in elementGroupings)
                {
                    using var boundaryGeometries = GetBoundaryGeometries(groupedBoundedByElements, document, documents);
                    var surfaceGeoElement = GmlHelper.GetFeatureGeometryElement(featureElement);
                    using var surfaceGeometry = document.GetOrCreateGeometry(surfaceGeoElement, out var errorMessage2);

                    if (surfaceGeometry == null ||
                        !boundaryGeometries.TryConvertToNtsGeometry(out var ntsMultiLineString) ||
                        !surfaceGeometry.TryConvertToNtsGeometry(out var ntsMultiPolygon) ||
                        !ntsMultiPolygon.Boundary.EqualsTopologically(ntsMultiLineString))
                    {
                        this.AddMessage(
                            Translate("Message", GmlHelper.GetNameAndId(featureElement)),
                            document.FileName,
                            new[] { surfaceGeoElement.GetXPath() },
                            new[] { featureElement.GetAttribute("gml:id") }
                        );
                    }
                }
            }

            return hasBoundedBy;
        }

        private static Geometry GetBoundaryGeometries(IGrouping<string, XElement> groupedBoundedByElements, GmlDocument document, List<GmlDocument> documents)
        {
            var boundaryGeometries = new Geometry(wkbGeometryType.wkbMultiCurve);

            foreach (var boundedByElement in groupedBoundedByElements)
            {
                var xLink = GmlHelper.GetXLink(boundedByElement);

                if (xLink?.GmlId == null)
                    continue;

                var boundaryElement = GmlHelper.GetElementByGmlId(documents, xLink.GmlId, xLink?.FileName ?? document.FileName);

                if (boundaryElement == null)
                    continue;

                var boundaryGeoElement = GmlHelper.GetFeatureGeometryElement(boundaryElement);
                using var boundaryGeometry = document.GetOrCreateGeometry(boundaryGeoElement, out var errorMessage1);

                if (boundaryGeometry == null)
                    continue;

                boundaryGeometries.AddGeometry(boundaryGeometry);
            }

            return boundaryGeometries;
        }
    }
}
