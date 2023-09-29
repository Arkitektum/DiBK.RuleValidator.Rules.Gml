using DiBK.RuleValidator.Extensions;
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
                var indexed = document.GetOrCreateGeometry(element);

                if (indexed.ErrorMessage != null)
                {
                    var (LineNumber, LinePosition) = element.GetLineInfo();

                    this.AddMessage(
                        indexed.ErrorMessage,
                        document.FileName,
                        new[] { element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(element) },
                        LineNumber,
                        LinePosition
                    );

                    return;
                }

                var points = indexed.Geometry.GetPoints();

                for (var i = 1; i < points.Length; i++)
                {
                    if (!points[i].SequenceEqual(points[i - 1]))
                        continue;

                    var dimensions = indexed.Geometry.GetCoordinateDimension();
                    var x = points[i][0];
                    var y = points[i][1];
                    var z = dimensions == 3 ? points[i][2] : 0;

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
