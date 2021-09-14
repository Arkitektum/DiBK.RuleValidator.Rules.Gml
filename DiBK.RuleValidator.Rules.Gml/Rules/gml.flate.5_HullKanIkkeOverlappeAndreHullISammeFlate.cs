using DiBK.RuleValidator.Models;
using DiBK.RuleValidator.Extensions;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class HullKanIkkeOverlappeAndreHullISammeFlate : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.flate.5";
            Name = "Hull i flate kan ikke overlappe andre hull i samme flate";
        }

        protected override Status Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any())
                return Status.NOT_EXECUTED;

            data.Surfaces.ForEach(Validate);

            return HasMessages ? Status.FAILED : Status.PASSED;
        }

        private void Validate(GmlDocument document)
        {
            var polygonElements = document.GetFeatures().GetElements("//gml:Polygon | //gml:PolygonPatch");

            foreach (var element in polygonElements)
            {
                var interiorElements = element.GetElements("*:interior");

                if (interiorElements.Count() < 2)
                    continue;

                var interiors = new List<(XElement, Geometry)>();

                foreach (var interiorElement in interiorElements)
                {
                    try
                    {
                        interiors.Add((interiorElement, CreatePolygon(interiorElement)));
                    }
                    catch (Exception exception)
                    {
                        this.AddMessage(exception.Message, document.FileName, new[] { interiorElement.GetXPath() });
                    }
                }

                if (interiors.Count < 2)
                    continue;

                for (var i = 0; i < interiors.Count; i++)
                {
                    var (geoElement, geometry) = interiors[i];
                    var otherInteriors = interiors.Skip(i + 1).ToList();

                    for (int j = 0; j < otherInteriors.Count; j++)
                    {
                        var (otherGeoElement, otherGeometry) = otherInteriors[j];

                        if (geometry.Overlaps(otherGeometry))
                        {
                            this.AddMessage(
                                $"{element.GetName()} '{element.GetAttribute("gml:id")}': Et hull i flaten overlapper et annet hull i samme flate.",
                                document.FileName,
                                new[] { geoElement.GetXPath(), otherGeoElement.GetXPath() }
                            );
                        }
                    }
                }
            }
        }

        private static Geometry CreatePolygon(XElement element)
        {
            var points = GeometryHelper.GetCoordinates(element);
            var polygon = new Geometry(wkbGeometryType.wkbPolygon);
            var linearRing = new Geometry(wkbGeometryType.wkbLinearRing);

            foreach (var point in points)
                linearRing.AddPoint(point[0], point[1], 0);

            polygon.AddGeometry(linearRing);

            return polygon;
        }
    }
}
