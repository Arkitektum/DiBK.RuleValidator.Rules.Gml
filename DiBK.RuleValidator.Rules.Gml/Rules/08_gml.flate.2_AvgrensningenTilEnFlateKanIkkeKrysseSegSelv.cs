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

            SetData(string.Format(DataKey.Selvkryss, document.Id), _xPaths);
        }

        private void DetectSelfIntersection(GmlDocument document, XElement element, Geometry polygon)
        {
            var point = GeometryHelper.DetectSelfIntersection(polygon);

            if (point == default)
                return;

            var pointX = point.X.ToString(CultureInfo.InvariantCulture);
            var pointY = point.Y.ToString(CultureInfo.InvariantCulture);
            var xPath = element.GetXPath();

            this.AddMessage(
                Translate("Message", GmlHelper.GetNameAndId(element), pointX, pointY),
                document.FileName,
                new[] { xPath },
                new[] { GmlHelper.GetFeatureGmlId(element) },
                $"POINT ({pointX} {pointY})"
            );

            _xPaths.Add(xPath);
        }
    }
}
