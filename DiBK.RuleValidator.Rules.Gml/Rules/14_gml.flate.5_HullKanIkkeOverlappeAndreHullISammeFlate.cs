using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using DiBK.RuleValidator.Rules.Gml.Constants;
using OSGeo.OGR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class HullKanIkkeOverlappeAndreHullISammeFlate : Rule<IGmlValidationData>
    {
        private readonly HashSet<string> _xPaths = new();

        public override void Create()
        {
            Id = "gml.flate.5";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any())
                SkipRule();

            data.Surfaces.ForEach(Validate);
        }

        private void Validate(GmlDocument document)
        {
            var polygonElements = document.GetFeatureElements().GetElements("//gml:Polygon | //gml:PolygonPatch");

            foreach (var element in polygonElements)
            {
                var interiorRingElements = element.GetElements("*:interior/*");

                if (interiorRingElements.Count() < 2)
                    continue;

                var interiors = new List<(XElement, Geometry)>();

                foreach (var interiorRingElement in interiorRingElements)
                {
                    try
                    {
                        using var interiorRing = document.GetOrCreateGeometry(interiorRingElement, out var errorMessage);

                        if (interiorRing != null)
                        {
                            interiors.Add((interiorRingElement, GeometryHelper.CreatePolygonFromRing(interiorRing)));
                        }
                    }
                    catch
                    {
                    }
                }

                if (interiors.Count < 2)
                    continue;

                Parallel.ForEach(interiors, interior =>
                {
                    var (geoElement, geometry) = interior;

                    for (int i = 0; i < interiors.Count; i++)
                    {
                        var (otherGeoElement, otherGeometry) = interiors[i];

                        if (geoElement == otherGeoElement || !GeometryHelper.Overlaps(geometry, otherGeometry))
                            continue;

                        this.AddMessage(
                            Translate("Message", GmlHelper.GetNameAndId(element)),
                            document.FileName,
                            new[] { geoElement.GetXPath(), otherGeoElement.GetXPath() },
                            new[] { GmlHelper.GetFeatureGmlId(element) }
                        );

                        _xPaths.Add(GmlHelper.GetBaseGmlElement(element).GetXPath());
                    }
                });
            }

            SetData(string.Format(DataKey.OverlappingHoles, document.Id), _xPaths);
        }
    }
}
