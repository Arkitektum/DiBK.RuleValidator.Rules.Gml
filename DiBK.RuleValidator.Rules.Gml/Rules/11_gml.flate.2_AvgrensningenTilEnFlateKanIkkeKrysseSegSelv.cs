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
    public class AvgrensningenTilEnFlateKanIkkeKrysseSegSelv : Rule<IGmlValidationData>
    {
        private readonly ConcurrentBag<string> _xPaths = new();

        public override void Create()
        {
            Id = "gml.flate.2";
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
            SetData(DataKey.SelfIntersections + document.Id, _xPaths);

            var indexedSurfaceGeometries = document.GetIndexedGeometries()
                .Where(geometry => !geometry.IsValid && 
                    (geometry.Type == GmlGeometry.MultiSurface || geometry.Type == GmlGeometry.Surface || geometry.Type == GmlGeometry.Polygon))                
                .ToList();

            foreach (var indexed in indexedSurfaceGeometries)
            {
                if (indexed.Geometry == null || indexed.Geometry.IsSimple())
                    continue;

                DetectSelfIntersection(document, indexed.GeoElement, indexed.Geometry);
            }
        }

        private void DetectSelfIntersection(GmlDocument document, XElement element, Geometry surface)
        {
            using var point = GeometryHelper.DetectSelfIntersection(surface);

            if (point == null)
                return;

            var pointX = point.GetX(0).ToString(CultureInfo.InvariantCulture);
            var pointY = point.GetY(0).ToString(CultureInfo.InvariantCulture);
            var xPath = element.GetXPath();

            this.AddMessage(
                Translate("Message", GmlHelper.GetNameAndId(element), pointX, pointY),
                document.FileName,
                new[] { xPath },
                new[] { GmlHelper.GetFeatureGmlId(element) },
                GeometryHelper.GetZoomToPoint(point)
            );

            _xPaths.Add(xPath);
        }
    }
}
