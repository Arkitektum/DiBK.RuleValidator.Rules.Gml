using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using DiBK.RuleValidator.Extensions.Gml.Models;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class SamsvarendeAvgrensingsgeometri : Rule<IGmlValidationInputV1>
    {
        public override void Create()
        {
            Id = "gml.avgr.1";
        }

        protected override void Validate(IGmlValidationInputV1 input)
        {
            if (!input.Surfaces.Any())
                SkipRule();

            var result = input.Surfaces.Select(document => Validate(document, input.Surfaces));

            if (!result.Contains(true))
                SkipRule();
        }

        private bool Validate(GmlDocument document, List<GmlDocument> documents)
        {
            var boundaryElements = document.GetFeatureElements()
                .Elements()
                .Where(element => element.Name.LocalName.StartsWith("avgrensesAv"))
                .ToList();

            if (!boundaryElements.Any())
                return false;

            var groupedBoundaryElements = boundaryElements
                .GroupBy(element => element.Parent)
                .ToList();

            foreach (var grouping in groupedBoundaryElements)
            {
                var featureElement = grouping.Key;

                var elementGroupings = grouping
                    .GroupBy(element => element.Name.LocalName)
                    .ToList();

                var surfaceGeoElement = GmlHelper.GetFeatureGeometryElements(featureElement).First();
                var multiSurface = document.GetOrCreateGeometry(surfaceGeoElement);

                if (!multiSurface.IsValid)
                    continue;

                foreach (var groupedBoundedByElements in elementGroupings)
                {
                    using var boundariesMultiSurface = GetBoundariesAsMultiSurface(groupedBoundedByElements, document, documents);
                    
                    if (!multiSurface.Geometry.EqualsTopologically(boundariesMultiSurface))
                    {
                        this.AddMessage(
                            Translate("Message", GmlHelper.GetNameAndId(featureElement)),
                            document.FileName,
                            new[] { surfaceGeoElement.GetXPath() },
                            new[] { featureElement.GetAttribute("gml:id") }
                        );
                    }
                }
            }

            return true;
        }

        private static Geometry GetBoundariesAsMultiSurface(IGrouping<string, XElement> groupedBoundedByElements, GmlDocument document, List<GmlDocument> documents)
        {
            var multiSurface = new Geometry(wkbGeometryType.wkbMultiSurface);
            var segments = new List<Segment>();

            foreach (var boundedByElement in groupedBoundedByElements)
            {
                var xLink = GmlHelper.GetXLink(boundedByElement);

                if (xLink?.GmlId == null)
                    continue;

                var boundaryElement = GmlHelper.GetElementByGmlId(documents, xLink.GmlId, xLink?.FileName ?? document.FileName);

                if (boundaryElement == null)
                    continue;

                var boundaryGeoElement = GmlHelper.GetFeatureGeometryElements(boundaryElement).First();
                var segmentElements = boundaryGeoElement.GetElements("*:segments/*");

                foreach (var segmentElement in segmentElements)
                {
                    var points = GeometryHelper.GetCoordinates(segmentElement)
                        .Select(coordinate => new Point(coordinate[0], coordinate[1]))
                        .ToList();

                    var segmentType = segmentElement.Name.LocalName == "Arc" ? SegmentType.Arc : SegmentType.LineStringSegment;

                    segments.Add(new Segment(points, segmentType));
                }
            }

            using var surfaces = GeometryHelper.ConvertSegmentsToSurfaces(segments);
            var surfaceWkts = surfaces.Select(surface => surface.ToWkt());

            foreach (var wkt in surfaceWkts)
            {
                try
                {
                    using var geometry = Geometry.CreateFromWkt(wkt);
                    multiSurface.AddGeometry(geometry);
                }
                catch
                {
                }
            }

            return multiSurface;
        }
    }
}
