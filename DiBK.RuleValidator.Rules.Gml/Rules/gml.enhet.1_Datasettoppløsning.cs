using DiBK.RuleValidator.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class Datasettoppløsning : Rule<IGmlValidationData>
    {
        private static readonly Regex _twoDecimalRegex = new(@"^-?\d+(.\d{1,2})?$", RegexOptions.Compiled);

        public override void Create()
        {
            Id = "gml.enhet.1";
            Name = "Datasettoppløsning";
            Description = "Enhet i datasettet settes til 0,01, dvs. centimeternivå";
            MessageType = MessageType.WARNING;
        }

        protected override Status Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any() && !data.Solids.Any())
                return Status.NOT_EXECUTED;

            data.Surfaces.Concat(data.Solids).ToList().ForEach(Validate);

            return HasMessages ? Status.WARNING : Status.PASSED;
        }

        private void Validate(GmlDocument document)
        {
            if (document == null)
                return;

            var gmlIds = new List<string>();
            var elements = document.GetFeatures().GetElements("//*:pos | //*:posList");

            foreach (var posElement in elements)
            {
                var posList = posElement.Value.Split(" ");

                if (posList.All(pos => _twoDecimalRegex.IsMatch(pos)))
                    continue;

                var gmlElement = GmlHelper.GetClosestGmlElement(posElement);
                var gmlId = gmlElement.GetAttribute("gml:id");

                if (gmlIds.Contains(gmlId))
                    continue;

                gmlIds.Add(gmlId);

                this.AddMessage(
                    $"{gmlElement.GetName()} '{gmlId}' er ikke angitt med koordinater på centimenternivå (maks 2 desimaler).",                    
                    document.FileName,
                    new[] { posElement.GetXPath() }
                );
            }
        }
    }
}
