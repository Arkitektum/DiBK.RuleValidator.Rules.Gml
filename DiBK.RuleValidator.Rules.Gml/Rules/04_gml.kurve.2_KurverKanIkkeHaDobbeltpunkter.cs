﻿using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using static DiBK.RuleValidator.Extensions.Gml.Constants.Namespace;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class KurverKanIkkeHaDobbeltpunkter : Rule<IGmlValidationInputV1>
    {
        public override void Create()
        {
            Id = "gml.kurve.2";
        }

        protected override void Validate(IGmlValidationInputV1 input)
        {
            if (!input.Documents.Any())
                SkipRule();

            input.Documents.ForEach(Validate);
        }

        private void Validate(GmlDocument document)
        {
            var curveElements = GetCurveElements(document);
            
            Parallel.ForEach(curveElements, element =>
            {
                var geometry = document.GetOrCreateGeometry(element);

                if (geometry.ErrorMessage != null)
                {
                    var (LineNumber, LinePosition) = element.GetLineInfo();

                    this.AddMessage(
                        geometry.ErrorMessage,
                        document.FileName,
                        new[] { element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(element) },
                        LineNumber,
                        LinePosition
                    );

                    return;
                }

                var dimensions = geometry.Geometry.GetCoordinateDimension();
                var pointTuples = new List<(double[] PointA, double[] PointB)>();

                try
                {
                    var coordinatePairs = GeometryHelper.GetCoordinates(element, dimensions);

                    for (var i = 1; i < coordinatePairs.Count; i++)
                        pointTuples.Add((coordinatePairs[i - 1], coordinatePairs[i]));
                }
                catch (Exception exception)
                {
                    var (LineNumber, LinePosition) = element.GetLineInfo();

                    this.AddMessage(
                        exception.Message,
                        document.FileName,
                        new[] { element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(element) },
                        LineNumber,
                        LinePosition
                    );

                    return;
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

                    var (LineNumber, LinePosition) = element.GetLineInfo();

                    this.AddMessage(
                        Translate("Message", element.Name.LocalName, FormattableString.Invariant(pointString)),
                        document.FileName,
                        new[] { element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(element) },
                        LineNumber,
                        LinePosition,
                        GeometryHelper.GetZoomToPoint(point, dimensions)
                    );
                }
            });
        }

        private static List<XElement> GetCurveElements(GmlDocument document)
        {
            var lineStringElements = document.GetFeatureGeometryElements(GmlGeometry.LineString);

            var curveSegmentElements = document.GetGeometryElements(GmlGeometry.Curve)
                .Elements()
                .Elements();

            var linearRingElements = document.GetGeometryElements(GmlGeometry.Polygon)
                .Descendants(GmlNs + GmlGeometry.LinearRing);

            return lineStringElements
                .Concat(curveSegmentElements)
                .Concat(linearRingElements)
                .ToList();
        }
    }
}
