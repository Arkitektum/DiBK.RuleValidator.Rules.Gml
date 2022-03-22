﻿using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using DiBK.RuleValidator.Extensions.Gml.Models;
using OSGeo.OGR;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class SamsvarendeAvgrensingsgeometri : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.avgr.1";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any())
                SkipRule();

            var result = data.Surfaces.Select(document => Validate(document, data.Surfaces));

            if (!result.Contains(true))
                SkipRule();
        }

        private bool Validate(GmlDocument document, List<GmlDocument> documents)
        {
            var hasBoundedBy = false;
            var featureElements = document.GetFeatureElements();

            foreach (var featureElement in featureElements)
            {
                var boundedByElements = featureElement.Elements()
                    .Where(element => element.Name.LocalName.StartsWith("avgrensesAv"));

                if (!boundedByElements.Any())
                    continue;

                hasBoundedBy = true;

                var elementGroupings = boundedByElements
                    .GroupBy(element => element.Name.LocalName);

                var surfaceGeoElement = GmlHelper.GetFeatureGeometryElements(featureElement).First();
                using var multiSurface = document.GetOrCreateGeometry(surfaceGeoElement, out var errorMessage);

                foreach (var groupedBoundedByElements in elementGroupings)
                {
                    using var boundariesMultiSurface = GetBoundariesAsMultiSurface(groupedBoundedByElements, document, documents);

                    if (!multiSurface.EqualsTopologically(boundariesMultiSurface))
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

            return hasBoundedBy;
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

            var surfaces = GeometryHelper.ConvertSegmentsToSurfaces(segments);
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