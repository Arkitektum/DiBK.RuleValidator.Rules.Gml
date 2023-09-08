using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using DiBK.RuleValidator.Rules.Gml.Constants;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class SirkelbuerKanIkkeHaPunkterPåRettLinje : Rule<IGmlValidationInputV1>
    {
        private const double MIN_SANGITTA = 0.02;

        public override void Create()
        {
            Id = "gml.bue.2";

            DependOn<SirkelbuerKanKunInneholdeTrePunkter>().ToExecute();
        }

        protected override void Validate(IGmlValidationInputV1 input)
        {
            if (!input.Documents.Any())
                SkipRule();

            input.Documents.ForEach(Validate);
        }

        private void Validate(GmlDocument document)
        {
            var indexedGeometries = GetIndexedGeometries(document);

            foreach (var indexed in indexedGeometries)
            {
                using var points = GeometryHelper.GetPointGeometries(indexed.Geometry);
                var circle = GeometryHelper.PointsToCircle(points[0], points[1], points[2]);

                if (circle == null)
                    continue;

                var chordHalfLength = points[0].Distance(points[2]) / 2;
                var sangitta = circle.Radius - Math.Sqrt(Math.Pow(circle.Radius, 2) - Math.Pow(chordHalfLength, 2));

                if (sangitta >= MIN_SANGITTA)
                    continue;

                var (LineNumber, LinePosition) = indexed.Element.GetLineInfo();

                this.AddMessage(
                    Translate("Message"),
                    document.FileName, 
                    new[] { indexed.Element.GetXPath() },
                    new[] { GmlHelper.GetFeatureGmlId(indexed.Element) },
                    LineNumber,
                    LinePosition,
                    GeometryHelper.GetZoomToPoint(points[1])
                );
            }
        }

        private List<IndexedGeometry> GetIndexedGeometries(GmlDocument document)
        {
            var invalidElements = GetData<ConcurrentBag<XElement>>(DataKey.InvalidArcs + document.Id);

            return document.GetGeometriesByType(GmlGeometry.Arc)
                .Where(indexedGeometry => !invalidElements.Any(element => element.Equals(indexedGeometry.Element)))
                .ToList();
        }
    }
}
