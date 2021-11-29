using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Rules.Gml.Constants;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class HullKanIkkeOverlappeAndreHullISammeFlate : Rule<IGmlValidationData>
    {
        private readonly HashSet<string> _xPaths = new();

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
                        interiors.Add((interiorElement, GeometryHelper.CreatePolygon(interiorElement)));
                    }
                    catch
                    {
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
                                $"{GmlHelper.GetNameAndId(element)}': Et hull i flaten overlapper et annet hull i samme flate.",
                                document.FileName,
                                new[] { geoElement.GetXPath(), otherGeoElement.GetXPath() },
                                new[] { GmlHelper.GetFeatureGmlId(element) }
                            );

                            _xPaths.Add(GmlHelper.GetBaseGmlElement(element).GetXPath());
                        }
                    }
                }
            }

            SetData(string.Format(DataKey.OverlappendeHull, document.Id), _xPaths);
        }
    }
}
