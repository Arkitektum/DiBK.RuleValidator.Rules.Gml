using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using DiBK.RuleValidator.Rules.Gml.Constants;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            Parallel.ForEach(indexedSurfaceGeometries, indexed =>
            {
                if (indexed.Geometry == null)
                {
                    this.AddMessage(
                        indexed.ErrorMessage,
                        document.FileName,
                        new[] { indexed.XPath },
                        new[] { GmlHelper.GetFeatureGmlId(indexed.GeoElement) }
                    );
                }
                else
                {
                    this.AddMessage(
                        Translate("Message", GmlHelper.GetNameAndId(indexed.GeoElement)),
                        document.FileName,
                        new[] { indexed.XPath },
                        new[] { GmlHelper.GetFeatureGmlId(indexed.GeoElement) }
                    );
                }
            });
        }

        private List<IndexedGeometry> GetInvalidIndexedSurfaceGeometries(GmlDocument document)
        {
            var indexedSurfaceGeometries = document.GetIndexedGeometries()
                .Where(geometry => !geometry.IsValid &&
                    (geometry.Type == GmlGeometry.MultiSurface || geometry.Type == GmlGeometry.Surface || geometry.Type == GmlGeometry.Polygon));

            var invalidSurfaceXPaths = GetInvalidSurfaceXPaths(document.Id);

            if (!invalidSurfaceXPaths.Any())
                return indexedSurfaceGeometries.ToList();

            return indexedSurfaceGeometries
                .Where(indexed => !invalidSurfaceXPaths.Contains(indexed.XPath))
                .ToList();
        }

        private HashSet<string> GetInvalidSurfaceXPaths(string documentId)
        {
            var selfIntersections = GetData<ConcurrentBag<string>>(DataKey.SelfIntersections + documentId);
            var overlappingHoles = GetData<ConcurrentBag<string>>(DataKey.OverlappingHoles + documentId);
            var holesOutsideBoundary = GetData<ConcurrentBag<string>>(DataKey.HolesOutsideBoundary + documentId);

            var xPaths = new HashSet<string>();
            xPaths.UnionWith(selfIntersections);
            xPaths.UnionWith(overlappingHoles);
            xPaths.UnionWith(holesOutsideBoundary);

            return xPaths;
        }
    }
}
