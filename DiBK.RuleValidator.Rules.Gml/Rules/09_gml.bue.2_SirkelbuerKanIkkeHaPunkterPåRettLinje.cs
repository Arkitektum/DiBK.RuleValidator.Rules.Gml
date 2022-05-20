using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using static DiBK.RuleValidator.Extensions.Gml.Constants.Namespace;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class SirkelbuerKanIkkeHaPunkterPåRettLinje : Rule<IGmlValidationData>
    {
        private const double MIN_SANGITTA = 0.02;

        public override void Create()
        {
            Id = "gml.bue.2";
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
            var arcElements = document.GetFeatureElements()
                .Descendants(GmlNs + "Arc")
                .ToList();

            foreach (var element in arcElements)
            {
                var points = new List<double[]>();

                try
                {
                    points = GeometryHelper.GetCoordinates(element, dimensions);
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

                var circle = GeometryHelper.PointsToCircle(points[0], points[1], points[2]);

                if (circle == null)
                    continue;

                using var firstPoint = GeometryHelper.CreatePoint(points[0][0], points[0][1]);
                using var lastPoint = GeometryHelper.CreatePoint(points[2][0], points[2][1]);

                var chordHalfLength = firstPoint.Distance(lastPoint) / 2;
                var sangitta = circle.Radius - Math.Sqrt(Math.Pow(circle.Radius, 2) - Math.Pow(chordHalfLength, 2));

                if (sangitta >= MIN_SANGITTA)
                    continue;

                using var middlePoint = GeometryHelper.CreatePoint(points[1][0], points[1][1]);

                this.AddMessage(
                    Translate("Message"),
                    document.FileName, 
                    new[] { element.GetXPath() },
                    new[] { GmlHelper.GetFeatureGmlId(element) },
                    GeometryHelper.GetZoomToPoint(middlePoint)
                );
            }
        }
    }
}
