using DiBK.RuleValidator.Models;
using DiBK.RuleValidator.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class KoordinatreferansesystemForKart2D : Rule<IGmlValidationData>
    {
        private static readonly Regex _srsNameRegex = new(@"^(http:\/\/www\.opengis\.net\/def\/crs\/EPSG\/0\/|^urn:ogc:def:crs:EPSG::)(?<epsg>\d+)$", RegexOptions.Compiled);
        private static readonly string _messageTemplate = "Koordinatsystem '{0}' er ikke i henhold til godkjente koordinatsystem/EPSG-koder på https://register.geonorge.no/epsg-koder";

        public override void Create()
        {
            Id = "gml.epsg.1";
            Name = "Koordinatreferansesystem for kart i 2D";
            Description = "Koordinatsystemet for 2D-kart må være i UTM 32, 33 eller 35 (EPSG-kode 25832, 25833, 25835).";
        }

        protected override Status Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any())
                return Status.NOT_EXECUTED;

            var espgCodes = (int[])GetSetting("EspgCodes2D");

            data.Surfaces.ForEach(document => Messages.AddRange(Validate(document, espgCodes)));

            return HasMessages ? Status.FAILED : Status.PASSED;
        }

        public static List<RuleMessage> Validate(GmlDocument document, int[] espgCodes)
        {
            var messages = new List<RuleMessage>();
            var codes = new List<int>();
            var envElement = document.GetElement("*/*:boundedBy/*:Envelope");
            int? envEpsg = null;

            if (envElement != null)
            {
                var srsName = envElement.GetAttribute("srsName");

                if (srsName == null)
                {
                    messages.Add(new GmlRuleMessage
                    {
                        Message = $"{envElement.GetName()} mangler gyldig koordinatsystem",
                        FileName = document.FileName,
                        XPath = new[] { envElement.GetXPath() }
                    });
                }
                else
                {
                    var epsg = GetEpsgCode(srsName);

                    if (!epsg.HasValue)
                    {
                        messages.Add(new GmlRuleMessage
                        {
                            Message = string.Format(_messageTemplate, srsName),
                            FileName = document.FileName,
                            XPath = new[] { envElement.GetXPath() }
                        });
                    }
                    else if (!espgCodes.Contains(epsg.Value))
                    {
                        messages.Add(new GmlRuleMessage
                        {
                            Message = string.Format(_messageTemplate, epsg.Value),
                            FileName = document.FileName,
                            XPath = new[] { envElement.GetXPath() }
                        });
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
                    var epsg = GetEpsgCode(srsName);

                    if (!epsg.HasValue)
                    {
                        messages.Add(new GmlRuleMessage
                        {
                            Message = string.Format(_messageTemplate, srsName),
                            FileName = document.FileName,
                            XPath = new[] { element.GetXPath() }
                        });
                    }
                    else if (!espgCodes.Contains(epsg.Value))
                    {
                        messages.Add(new GmlRuleMessage
                        {
                            Message = string.Format(_messageTemplate, epsg.Value),
                            FileName = document.FileName,
                            XPath = new[] { element.GetXPath() }
                        });
                    }
                    else
                    {
                        codes.Add(epsg.Value);
                    }
                }
                else if (!envEpsg.HasValue)
                {                    
                    messages.Add(new GmlRuleMessage
                    {
                        Message = $"{element.GetName()} '{element.GetAttribute("gml:id")}' mangler gyldig koordinatsystem.",
                        FileName = document.FileName,
                        XPath = new[] { element.GetXPath() }
                    });
                }
            }

            var codeGroupings = codes.GroupBy(code => code);

            if (codeGroupings.Count() > 1)
            {
                messages.Add(new GmlRuleMessage
                {
                    Message = $"Geometriene i datasettet har ulike koordinatreferansesystemkoder: {string.Join(", ", codeGroupings.Select(grouping => grouping.Key))}.",
                    FileName = document.FileName,
                    XPath = Array.Empty<string>()
                });
            }

            return messages;
        }

        private static int? GetEpsgCode(string srsName)
        {
            var match = _srsNameRegex.Match(srsName);

            if (!match.Success)
                return null;

            return int.Parse(match.Groups["epsg"].Value);
        }
    }
}
