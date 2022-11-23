using DiBK.RuleValidator.Config;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class Setup : IRuleSetup
    {
        public RuleConfig CreateConfig()
        {
            return RuleConfig
                .Create<IGmlValidationInputV1>("Generell GML v1")
                .AddGroup("GenerellGmlV1", "Generell GML v1", group => group
                    .AddRule<UnikGmlIdForAlleObjekterInnenforDatasettet>()
                    .AddRule<LokalIdErEnGyldigUUID>()
                    .AddRule<KurverSkalHaGyldigGeometri>()
                    .AddRule<KurverKanIkkeHaDobbeltpunkter>()
                    .AddRule<SirkelbuerKanKunInneholdeTrePunkter>()
                    .AddRule<SirkelbuerKanIkkeHaPunkterPåRettLinje>()
                    .AddRule<FlaterSkalHaGyldigGeometri>()
                    .AddRule<AvgrensningenTilEnFlateKanIkkeKrysseSegSelv>()
                    .AddRule<AvgrensningeneTilEnFlateSkalNøstesRiktig>()
                    .AddRule<HullMåLiggeInnenforFlatensYtreAvgrensning>()
                    .AddRule<HullKanIkkeOverlappeAndreHullISammeFlate>()
                    .AddRule<SamsvarendeAvgrensingsgeometri>()
                )
                .Build();
        }
    }
}
