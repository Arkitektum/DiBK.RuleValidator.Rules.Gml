using DiBK.RuleValidator.Extensions;
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
            Description = "Koordinatsystemet for 3D-kart må være i UTM 32, 33 eller 35 (EPSG-kode 5972, 5973, 5975).";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Solids.Any())
                SkipRule();

            var espgCodes = GetSetting<int[]>("EspgCodes3D");

            data.Solids.ForEach(document => Validate(document, espgCodes));
        }

        public void Validate(GmlDocument document, int[] espgCodes)
        {
            var codes = new List<int>();
            var envElement = document.GetElement("*/*:boundedBy/*:Envelope");
            int? envEpsg = null;

            if (envElement != null)
            {
                var srsName = envElement.GetAttribute("srsName");

                if (srsName == null)
                {
                    this.AddMessage(
                        $"{envElement.GetName()} mangler gyldig koordinatsystem",
                        document.FileName,
                        new[] { envElement.GetXPath() }
                    );
                }
                else
                {
                    var epsg = GmlHelper.GetEpsgCode(srsName);

                    if (!epsg.HasValue)
                    {
                        this.AddMessage(
                            string.Format(_messageTemplate, srsName),
                            document.FileName,
                            new[] { envElement.GetXPath() }
                        );
                    }
                    else if (!espgCodes.Contains(epsg.Value))
                    {
                        this.AddMessage(
                            string.Format(_messageTemplate, epsg.Value),
                            document.FileName,
                            new[] { envElement.GetXPath() }
                        );
                    }
                    else
                    {
                        envEpsg = epsg;
                        codes.Add(epsg.Value);
                    }
                }
            }

            var geoElements = document.GetFeatures().GetElements("//gml:*[@gml:id]");

            foreach (var element in geoElements)
            {
                var srsName = element.GetAttribute("srsName");

                if (srsName != null)
                {
                    var epsg = GmlHelper.GetEpsgCode(srsName);

                    if (!epsg.HasValue)
                    {
                        this.AddMessage(
                            string.Format(_messageTemplate, srsName),
                            document.FileName,
                            new[] { element.GetXPath() },
                            new[] { GmlHelper.GetFeatureGmlId(element) }
                        );
                    }
                    else if (!espgCodes.Contains(epsg.Value))
                    {
                        this.AddMessage(
                            string.Format(_messageTemplate, epsg.Value),
                            document.FileName,
                            new[] { element.GetXPath() },
                            new[] { GmlHelper.GetFeatureGmlId(element) }
                        );
                    }
                    else
                    {
                        codes.Add(epsg.Value);
                    }
                }
                else if (!envEpsg.HasValue)
                {
                    this.AddMessage(
                        $"{GmlHelper.GetNameAndId(element)} mangler gyldig koordinatsystem.",
                        document.FileName,
                        new[] { element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(element) }
                    );
                }
            }

            var codeGroupings = codes.GroupBy(code => code);

            if (codeGroupings.Count() > 1)
            {
                this.AddMessage(
                    $"Geometriene i datasettet har ulike koordinatreferansesystemkoder: {string.Join(", ", codeGroupings.Select(grouping => grouping.Key))}.",
                    document.FileName
                );
            }
        }
    }
}
