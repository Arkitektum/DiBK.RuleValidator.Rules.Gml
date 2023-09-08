using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using DiBK.RuleValidator.Rules.Gml.Constants;
using System.Collections.Concurrent;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class SirkelbuerKanKunInneholdeTrePunkter : Rule<IGmlValidationInputV1>
    {
        private readonly ConcurrentBag<XElement> _invalidElements = new();

        public override void Create()
        {
            Id = "gml.bue.1";
        }

        protected override void Validate(IGmlValidationInputV1 input)
        {
            if (!input.Documents.Any())
                SkipRule();

            input.Documents.ForEach(Validate);
        }

        private void Validate(GmlDocument document)
        {
            SetData(DataKey.InvalidArcs + document.Id, _invalidElements);

            var indexedGeometries = document.GetGeometriesByType(GmlGeometry.Arc);

            foreach (var indexed in indexedGeometries)
            {  
                if (indexed.ErrorMessage != null)
                {
                    var (LineNumber, LinePosition) = indexed.Element.GetLineInfo();

                    this.AddMessage(
                        indexed.ErrorMessage,
                        document.FileName,
                        new[] { indexed.Element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(indexed.Element) },
                        LineNumber,
                        LinePosition
                    );

                    _invalidElements.Add(indexed.Element);
                }
                else if (indexed.Geometry.GetPointCount() != 3)
                {
                    var (LineNumber, LinePosition) = indexed.Element.GetLineInfo();

                    this.AddMessage(
                        Translate("Message"),
                        document.FileName,
                        new[] { indexed.Element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(indexed.Element) },
                        LineNumber,
                        LinePosition
                    );

                    _invalidElements.Add(indexed.Element);
                }
            }
        }
    }
}
