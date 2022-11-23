using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using DiBK.RuleValidator.Rules.Gml.Constants;
using OSGeo.OGR;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class AvgrensningenTilEnFlateKanIkkeKrysseSegSelv : Rule<IGmlValidationInputV1>
    {
        private readonly ConcurrentBag<XElement> _invalidElements = new();

        public override void Create()
        {
            Id = "gml.flate.2";
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
            SetData(DataKey.SelfIntersections + document.Id, _invalidElements);

            var indexedSurfaceGeometries = document.GetGeometriesByType(GmlGeometry.MultiSurface, GmlGeometry.Surface, GmlGeometry.Polygon)
                .Where(indexed => !indexed.IsValid)
                .ToList();

            foreach (var indexed in indexedSurfaceGeometries)
            {
                if (indexed.Geometry == null || indexed.Geometry.IsSimple())
                    continue;

                DetectSelfIntersection(document, indexed.Element, indexed.Geometry);
            }
        }

        private void DetectSelfIntersection(GmlDocument document, XElement element, Geometry surface)
        {
            using var point = GeometryHelper.DetectSelfIntersection(surface);

            if (point == null)
                return;

            var pointX = point.GetX(0).ToString(CultureInfo.InvariantCulture);
            var pointY = point.GetY(0).ToString(CultureInfo.InvariantCulture);

            this.AddMessage(
                Translate("Message", GmlHelper.GetNameAndId(element), pointX, pointY),
                document.FileName,
                new[] { element.GetXPath() },
                new[] { GmlHelper.GetFeatureGmlId(element) },
                GeometryHelper.GetZoomToPoint(point)
            );

            _invalidElements.Add(element);
        }
    }
}
