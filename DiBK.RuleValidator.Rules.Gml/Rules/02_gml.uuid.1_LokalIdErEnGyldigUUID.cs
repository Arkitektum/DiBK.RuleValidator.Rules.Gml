using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class LokalIdErEnGyldigUUID : Rule<IGmlValidationInputV1>
    {
        private static readonly Regex _uuidRegex = 
            new("^[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public override void Create()
        {
            Id = "gml.uuid.1";
        }

        protected override void Validate(IGmlValidationInputV1 input)
        {
            if (!input.Documents.Any())
                SkipRule();

            input.Documents.ForEach(Validate);
        }

        private void Validate(GmlDocument document)
        {
            if (document == null)
                return;

            var groupedElements = document.GetFeatureElements()
                .Descendants()
                .Where(element => element.Name.LocalName == "lokalId")
                .GroupBy(element => element.Value)
                .ToList();

            foreach (var grouping in groupedElements)
            {
                var lokalId = grouping.Key;

                if (!_uuidRegex.IsMatch(lokalId))
                {
                    foreach (var element in grouping)
                    {
                        var (LineNumber, LinePosition) = element.GetLineInfo();

                        this.AddMessage(
                            Translate("Message1", GmlHelper.GetNameAndId(GmlHelper.GetFeatureElement(element)), lokalId),
                            document.FileName,
                            new[] { element.GetXPath() },
                            new[] { GmlHelper.GetFeatureGmlId(element) },
                            LineNumber,
                            LinePosition
                        );
                    }
                }
                else if (grouping.Count() > 1)
                {
                    var firstElement = grouping.First();

                    this.AddMessage(
                        Translate("Message2", GmlHelper.GetNameAndId(GmlHelper.GetFeatureElement(firstElement)), lokalId),
                        document.FileName,
                        grouping.Select(element => element.GetXPath()),
                        grouping.Select(GmlHelper.GetFeatureGmlId)
                    );
                }
            }
        }
    }
}
