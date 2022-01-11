using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System.Collections.Generic;
using System.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class KoordinatreferansesystemForKart3D : Rule<IGmlValidationData>
    {
        private static readonly string _messageTemplate = "Koordinatsystem '{0}' er ikke i henhold til godkjente koordinatsystem/EPSG-koder på https://register.geonorge.no/epsg-koder";

        public override void Create()
        {
            Id = "gml.epsg.2";
            Name = "Koordinatreferansesystem for kart i 3D";
            Description = "Koordinatsystemet for 3D-kart må være i henhold til godkjente koordinatsystem/EPSG-koder.";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Solids.Any())
                SkipRule();

            var validEpsgCodes = GetSetting<string[]>("ValidEpsgCodes3D");

            data.Solids.ForEach(document => Validate(document, validEpsgCodes));
        }

        public void Validate(GmlDocument document, string[] validEpsgCodes)
        {
            var envelopeElement = document.Document.Root.GetElement("*:boundedBy/*:Envelope");
            string envelopeEpsgCode = null;

            if (envelopeElement != null)
            {
                var srsName = envelopeElement.Attribute("srsName")?.Value;

                if (srsName == null)
                {
                    this.AddMessage(
                        $"{envelopeElement.GetName()} mangler gyldig koordinatsystem.",
                        document.FileName,
                        new[] { envelopeElement.GetXPath() }
                    );

                    return;
                }
                else
                {
                    var epsg = GmlHelper.GetEpsgCode(srsName);

                    if (epsg == null)
                    {
                        this.AddMessage(
                            string.Format(_messageTemplate, srsName),
                            document.FileName,
                            new[] { envelopeElement.GetXPath() }
                        );

                        return;
                    }
                    else if (!validEpsgCodes.Contains(epsg))
                    {
                        this.AddMessage(
                            string.Format(_messageTemplate, epsg),
                            document.FileName,
                            new[] { envelopeElement.GetXPath() }
                        );

                        return;
                    }
                    else
                    {
                        envelopeEpsgCode = epsg;
                    }
                }
            }

            var epgsDictionary = CreateEpsgDictionary(document, envelopeEpsgCode);

            if (epgsDictionary.Count > 1)
            {
                this.AddMessage(
                    $"Geometriene i datasettet har ulike koordinatreferansesystemkoder: {string.Join(", ", epgsDictionary.Select(grouping => grouping.Key))}.",
                    document.FileName
                );

                return;
            }

            if (envelopeEpsgCode == null)
            {
                var geometryElements = document.GetFeatures()
                    .SelectMany(featureElement => GmlHelper.GetFeatureGeometryElements(featureElement))
                    .ToList();

                foreach (var element in geometryElements)
                {
                    var srsName = element.Attribute("srsName")?.Value;

                    if (srsName == null)
                    {
                        this.AddMessage(
                            $"{element.GetName()} mangler gyldig koordinatsystem",
                            document.FileName,
                            new[] { element.GetXPath() },
                            new[] { GmlHelper.GetFeatureGmlId(element) }
                        );
                    }
                    else
                    {
                        var epsg = GmlHelper.GetEpsgCode(srsName);

                        if (epsg == null)
                        {
                            this.AddMessage(
                                string.Format(_messageTemplate, srsName),
                                document.FileName,
                                new[] { element.GetXPath() },
                                new[] { GmlHelper.GetFeatureGmlId(element) }
                            );
                        }
                        else if (!validEpsgCodes.Contains(epsg))
                        {
                            this.AddMessage(
                                string.Format(_messageTemplate, epsg),
                                document.FileName,
                                new[] { element.GetXPath() },
                                new[] { GmlHelper.GetFeatureGmlId(element) }
                            );
                        }
                    }
                }
            }
        }

        private static Dictionary<string, int> CreateEpsgDictionary(GmlDocument document, string envelopeEpsg)
        {
            var epgsDictionary = document.GetFeatures()
                .SelectMany(featureElement => featureElement.Descendants().Where(element => element.Attribute("srsName") != null))
                .Select(element =>
                {
                    var srsName = element.Attribute("srsName").Value;
                    return GmlHelper.GetEpsgCode(srsName) ?? srsName;
                })
                .GroupBy(epsg => epsg)
                .ToDictionary(grouping => grouping.Key, grouping => grouping.Count());

            if (envelopeEpsg != null)
            {
                if (epgsDictionary.ContainsKey(envelopeEpsg))
                    epgsDictionary[envelopeEpsg]++;
                else
                    epgsDictionary.Add(envelopeEpsg, 1);
            }

            return epgsDictionary;
        }
    }
}