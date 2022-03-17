using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class LinjeKanIkkeHaDobbeltpunkter : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.linje.1";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any())
                SkipRule();

            data.Surfaces.ForEach(Validate);
        }

        private void Validate(GmlDocument document)
        {
            var lineElements = GetLineElements(document);

            foreach (var element in lineElements)
            {
                var pointTuples = new List<(double[] PointA, double[] PointB)>();

                try
                {
                    var coordinatePairs = GeometryHelper.GetCoordinates(element);

                    for (var i = 1; i < coordinatePairs.Count; i++)
                        pointTuples.Add((coordinatePairs[i - 1], coordinatePairs[i]));
                }
                catch (Exception exception)
                {
                    this.AddMessage(
                        exception.Message,
                        document.FileName,
                        new[] { element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(element) }
                    );

                    continue;
                }

                var doublePoint = pointTuples
                    .FirstOrDefault(tuple => tuple.PointA[0] == tuple.PointB[0] && tuple.PointA[1] == tuple.PointB[1]);

                if (doublePoint != default)
                {
                    var x = doublePoint.PointA[0];
                    var y = doublePoint.PointA[1];
                    using var point = GeometryHelper.CreatePoint(x, y);

                    this.AddMessage(
                        Translate("Message", element.Name.LocalName, x.ToString(CultureInfo.InvariantCulture), y.ToString(CultureInfo.InvariantCulture)),
                        document.FileName,
                        new[] { element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(element) },
                        GeometryHelper.GetZoomToPoint(point)
                    );
                }
            }
        }

        private static IEnumerable<XElement> GetLineElements(GmlDocument document)
        {
            var lineStringElements = document.GetFeatureGeometryElements(GmlGeometry.LineString);

            var lineStringSegmentElements = document.GetGeometryElements(GmlGeometry.Curve)
                .Descendants(GmlHelper.GmlNs + "LineStringSegment");

            var linearRingElements = document.GetGeometryElements(GmlGeometry.Polygon)
                .Descendants(GmlHelper.GmlNs + "LinearRing");

            return lineStringElements
                .Concat(lineStringSegmentElements)
                .Concat(linearRingElements);
        }
    }
}
