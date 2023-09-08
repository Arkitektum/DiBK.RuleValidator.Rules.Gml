using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using DiBK.RuleValidator.Rules.Gml.Constants;
using OSGeo.OGR;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class HullMåLiggeInnenforFlatensYtreAvgrensning : Rule<IGmlValidationInputV1>
    {
        private readonly ConcurrentBag<XElement> _invalidElements = new();

        public override void Create()
        {
            Id = "gml.flate.4";
        }

        protected override void Validate(IGmlValidationInputV1 input)
        {
            if (!input.Documents.Any())
                SkipRule();

            input.Documents.ForEach(Validate);
        }

        private void Validate(GmlDocument document)
        {
            SetData(DataKey.HolesOutsideBoundary + document.Id, _invalidElements);

            var polygonElements = document.GetFeatureGeometryElements(GmlGeometry.MultiSurface, GmlGeometry.Surface, GmlGeometry.Polygon)
                .SelectMany(element =>
                {
                    return element.DescendantsAndSelf()
                        .Where(element => element.Name.LocalName == GmlGeometry.Polygon || element.Name.LocalName == GmlGeometry.PolygonPatch);
                })
                .ToList();

            foreach (var element in polygonElements)
            {
                var dimension = GmlHelper.GetDimension(element);
                var exteriorElement = element.GetElement("*:exterior/*");
                Geometry exterior = null;

                try
                {
                    if (dimension == 3)
                        AddSrsDimensionAttribute(exteriorElement);

                    using var exteriorRing = Geometry.CreateFromGML(exteriorElement.ToString());
                    exterior = GeometryHelper.CreatePolygonFromRing(exteriorRing);
                }
                catch
                {
                    continue;
                }

                var interiorElements = element.GetElements("*:interior/*");

                Parallel.ForEach(interiorElements, interiorElement =>
                {
                    try
                    {
                        if (dimension == 3)
                            AddSrsDimensionAttribute(interiorElement);

                        using var interiorRing = Geometry.CreateFromGML(interiorElement.ToString());
                        using var interior = GeometryHelper.CreatePolygonFromRing(interiorRing);

                        if (!exterior.Contains(interior))
                        {
                            var (LineNumber, LinePosition) = interiorElement.GetLineInfo();

                            this.AddMessage(
                                Translate("Message", GmlHelper.GetNameAndId(element)),
                                document.FileName,
                                new[] { interiorElement.GetXPath() },
                                new[] { GmlHelper.GetFeatureGmlId(element) },
                                LineNumber,
                                LinePosition
                            );

                            _invalidElements.Add(GmlHelper.GetBaseGmlElement(element));
                        }
                    }
                    catch
                    {
                    }
                });

                exterior.Dispose();
            }
        }

        private static void AddSrsDimensionAttribute(XElement geoElement)
        {
            if (geoElement.Attribute("srsDimension") == null)
                geoElement.Add(new XAttribute("srsDimension", 3));
        }
    }
}
