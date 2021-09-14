using DiBK.RuleValidator.Models;
using DiBK.RuleValidator.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class UnikGmlIdForAlleObjekterInnenforDatasettet : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.gmlid.1";
            Name = "GML-ID for alle objekter i planen skal være unike";
        }

        protected override Status Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any() && !data.Solids.Any())
                return Status.NOT_EXECUTED;

            var duplicates = GetGmlIds(data)
                .GroupBy(tuple => new { tuple.fileName, tuple.gmlId })
                .Where(grouping => grouping.Count() > 1);

            foreach (var grouping in duplicates)
            {
                foreach (var (fileName, gmlId, element) in grouping)
                {
                    this.AddMessage(
                        $"GML-ID '{gmlId}' til geometritypen '{element.GetName()}' finnes to eller flere ganger.",
                        fileName,
                        new[] { element.GetXPath() }
                    );
                }
            }

            return HasMessages ? Status.FAILED : Status.PASSED;
        }

        private static List<(string fileName, string gmlId, XElement element)> GetGmlIds(IGmlValidationData data)
        {
            var gmlIds = new List<(string fileName, string gmlId, XElement element)>();

            void AddGmlIds(GmlDocument document)
            {
                if (document == null)
                    return;

                var gmlElements = document.GetElements("//*[@gml:id]");

                foreach (var element in gmlElements)
                    gmlIds.Add((document.FileName, element.GetAttribute("gml:id"), element));
            }

            var documents = (data.Surfaces ?? new()).Concat(data.Solids ?? new());

            foreach (var document in documents)
                AddGmlIds(document);

            return gmlIds;
        }
    }
}


