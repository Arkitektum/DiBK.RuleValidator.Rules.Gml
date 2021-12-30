using DiBK.RuleValidator.Config;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class Setup : IRuleSetup
    {
        public RuleConfig CreateConfig()
        {
            return RuleConfig
                .Create<IGmlValidationData>("Generell geometri")
                .AddGroup("GenerellGeometri", "Generell geometri", group => group
                    .AddRule<FungerendeReferanserMellomObjekter>()
                    .AddRule<UnikGmlIdForAlleObjekterInnenforDatasettet>()
                    .AddRule<LokalIdErEnGyldigUUID>()
                    .AddRule<KoordinatreferansesystemForKart2D>()
                    .AddRule<KoordinatreferansesystemForKart3D>()
                    .AddRule<Datasettoppløsning>()
                    .AddRule<KurverSkalHaGyldigGeometri>()
                    .AddRule<FlaterSkalHaGyldigGeometri>()
                    .AddRule<AvgrensningenTilEnFlateKanIkkeKrysseSegSelv>()
                    .AddRule<AvgrensningeneTilEnFlateSkalNøstesRiktig>()
                    .AddRule<HullMåLiggeInnenforFlatensYtreAvgrensning>()
                    .AddRule<HullKanIkkeOverlappeAndreHullISammeFlate>()
                    .AddRule<BueKanIkkeHaDobbeltpunkter>()
                    .AddRule<BueKanIkkeHaPunkterPåRettLinje>()
                )
                .WithGlobalSettings(new()
                {
                    { "ValidEpsgCodes2D", new[] { "25832", "25833", "25835" } },
                    { "ValidEpsgCodes3D", new[] { "5972", "5973", "5975" } },
                })
                .Build();
        }
    }
}
