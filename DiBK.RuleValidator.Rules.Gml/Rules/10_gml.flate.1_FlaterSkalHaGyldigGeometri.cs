using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using DiBK.RuleValidator.Rules.Gml.Constants;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class FlaterSkalHaGyldigGeometri : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.flate.1";

            DependOn<AvgrensningenTilEnFlateKanIkkeKrysseSegSelv>().ToExecute();
            DependOn<HullMåLiggeInnenforFlatensYtreAvgrensning>().ToExecute();
            DependOn<HullKanIkkeOverlappeAndreHullISammeFlate>().ToExecute();
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any() && !data.Solids.Any())
                SkipRule();

            data.Surfaces.ForEach(Validate);
            data.Solids.ForEach(Validate);
        }

        private void Validate(GmlDocument document)
        {
            var indexedSurfaceGeometries = GetInvalidIndexedSurfaceGeometries(document);

            foreach (var indexed in indexedSurfaceGeometries)
            {
                if (indexed.Geometry == null)
                {
                    this.AddMessage(
                        indexed.ErrorMessage ?? "Flaten har ugyldig geometri.",
                        document.FileName,
                        new[] { indexed.Element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(indexed.Element) }
                    );
                }
                else
                {
                    this.AddMessage(
                        Translate("Message", GmlHelper.GetNameAndId(indexed.Element)),
                        document.FileName,
                        new[] { indexed.Element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(indexed.Element) }
                    );
                }
            }
        }

        private List<IndexedGeometry> GetInvalidIndexedSurfaceGeometries(GmlDocument document)
        {
            var indexedSurfaceGeometries = document.GetGeometriesByType(GmlGeometry.MultiSurface, GmlGeometry.Surface, GmlGeometry.Polygon)
                .Where(indexed => !indexed.IsValid)
                .ToList();

            var invalidSurfaceElements = GetInvalidSurfaceElements(document.Id);

            if (!invalidSurfaceElements.Any())
                return indexedSurfaceGeometries.ToList();
            
            return indexedSurfaceGeometries
                .ToList()
                .Where(indexed => !invalidSurfaceElements.Contains(indexed.Element))
                .ToList();
        }

        private HashSet<XElement> GetInvalidSurfaceElements(string documentId)
        {
            var selfIntersections = GetData<ConcurrentBag<XElement>>(DataKey.SelfIntersections + documentId);
            var overlappingHoles = GetData<ConcurrentBag<XElement>>(DataKey.OverlappingHoles + documentId);
            var holesOutsideBoundary = GetData<ConcurrentBag<XElement>>(DataKey.HolesOutsideBoundary + documentId);

            var elements = new HashSet<XElement>();
            elements.UnionWith(selfIntersections);
            elements.UnionWith(overlappingHoles);
            elements.UnionWith(holesOutsideBoundary);

            return elements;
        }
    }
}
