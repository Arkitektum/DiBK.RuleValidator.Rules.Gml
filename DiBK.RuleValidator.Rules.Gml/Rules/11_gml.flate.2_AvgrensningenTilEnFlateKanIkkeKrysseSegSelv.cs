using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using DiBK.RuleValidator.Rules.Gml.Constants;
using OSGeo.OGR;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class AvgrensningenTilEnFlateKanIkkeKrysseSegSelv : Rule<IGmlValidationData>
    {
        private readonly HashSet<string> _xPaths = new();

        public override void Create()
        {
            Id = "gml.flate.2";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any())
                SkipRule();

            data.Surfaces.ForEach(Validate);
        }

        private void Validate(GmlDocument document)
        {
            var surfaceElements = document.GetFeatureGeometryElements(GmlGeometry.MultiSurface, GmlGeometry.Surface, GmlGeometry.Polygon);

            foreach (var element in surfaceElements)
            {
                using var geometry = document.GetOrCreateGeometry(element, out var errorMessage);

                if (geometry == null || geometry.IsSimple())
                    continue;

                DetectSelfIntersection(document, element, geometry);
            }

            SetData(string.Format(DataKey.SelfIntersections, document.Id), _xPaths);
        }

        private void DetectSelfIntersection(GmlDocument document, XElement element, Geometry polygon)
        {
            using var point = GeometryHelper.DetectSelfIntersection(polygon);

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
