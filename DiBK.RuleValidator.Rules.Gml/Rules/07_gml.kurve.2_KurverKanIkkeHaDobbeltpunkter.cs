using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class KurverKanIkkeHaDobbeltpunkter : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.kurve.2";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any() && !data.Solids.Any())
                SkipRule();

            data.Surfaces.ForEach(document => Validate(document, 2));
            data.Solids.ForEach(document => Validate(document, 3));
        }

        private void Validate(GmlDocument document, int dimensions)
        {
            var curveElements = GetCurveElements(document);

            foreach (var element in curveElements)
            {
                var pointTuples = new List<(double[] PointA, double[] PointB)>();

                try
                {
                    var coordinatePairs = GeometryHelper.GetCoordinates(element, dimensions);

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
                    .FirstOrDefault(tuple => tuple.PointA[0] == tuple.PointB[0] && tuple.PointA[1] == tuple.PointB[1] && (dimensions != 3 || tuple.PointA[2] == tuple.PointB[2]));

                if (doublePoint != default)
                {
                    var x = doublePoint.PointA[0];
                    var y = doublePoint.PointA[1];
                    var z = dimensions == 3 ? doublePoint.PointA[2] : 0;

                    using var point = GeometryHelper.CreatePoint(x, y, z);
                    FormattableString pointString;

                    if (dimensions == 2)
                        pointString = $"{x}, {y}";
                    else
                        pointString = $"{x}, {y}, {z}";

                    this.AddMessage(
                        Translate("Message", element.Name.LocalName, FormattableString.Invariant(pointString)),
                        document.FileName,
                        new[] { element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(element) },
                        GeometryHelper.GetZoomToPoint(point, dimensions)
                    );
                }
            }
        }

        private static IEnumerable<XElement> GetCurveElements(GmlDocument document)
        {
            var lineStringElements = document.GetFeatureGeometryElements(GmlGeometry.LineString);

            var curveSegmentElements = document.GetGeometryElements(GmlGeometry.Curve)
                .GetElements("*:segments/*");

            var linearRingElements = document.GetGeometryElements(GmlGeometry.Polygon)
                .GetElements("*:LinearRing");

            return lineStringElements
                .Concat(curveSegmentElements)
                .Concat(linearRingElements);
        }
    }
}
