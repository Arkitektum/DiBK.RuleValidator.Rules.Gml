using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using DiBK.RuleValidator.Rules.Gml.Constants;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class AvgrensningenTilEnFlateKanIkkeKrysseSegSelv : Rule<IGmlValidationData>
    {
        private readonly HashSet<string> _xPaths = new();

        public override void Create()
        {
            Id = "gml.flate.2";
            Name = "Avgrensningen til en flate kan ikke krysse seg selv";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any())
                SkipRule();

            data.Surfaces.ForEach(Validate);
        }

        private void Validate(GmlDocument document)
        {
            var surfaceElements = document.GetFeatures().GetElements("*/gml:MultiSurface | */gml:Surface | */gml:Polygon").ToList();

            foreach (var element in surfaceElements)
            {
                using var geometry = document.GetOrCreateGeometry(element, out var errorMessage);

                if (geometry == null || geometry.IsSimple())
                    continue;

                DetectSelfIntersection(document, element, geometry);
            }

            SetData(string.Format(DataKey.Selvkryss, document.Id), _xPaths);
        }

        private void DetectSelfIntersection(GmlDocument document, XElement element, Geometry polygon)
        {
            var point = GeometryHelper.DetectSelfIntersection(polygon);

            if (point == default)
                return;

            var pointWkt = FormattableString.Invariant($"POINT ({point.X} {point.Y})");
            var xPath = element.GetXPath();

            this.AddMessage(
                FormattableString.Invariant($"{GmlHelper.GetNameAndId(element)}: Avgrensningen krysser seg selv ved punktet ({point.X}, {point.Y})."),
                document.FileName,
                new[] { xPath },
                new[] { GmlHelper.GetFeatureGmlId(element) },
                pointWkt
            );

            _xPaths.Add(xPath);
        }
    }
}
