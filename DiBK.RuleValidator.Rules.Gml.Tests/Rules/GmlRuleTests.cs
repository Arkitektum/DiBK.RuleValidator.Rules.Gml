using DiBK.RuleValidator.Rules.Gml.Tests.Setup;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace DiBK.RuleValidator.Rules.Gml.Tests.Rules
{
    public class GmlRuleTests : IClassFixture<RuleConfigFixture>
    {
        private readonly IRuleValidator _validator;

        public GmlRuleTests(RuleConfigFixture fixture)
        {
            _validator = TestHelper.GetRuleValidator(fixture.RuleSettings);
            _validator.LoadRules<IGmlValidationData>();
        }

        [Fact(DisplayName = "gml.xlink.1: Fungerende referanser mellom objekter - FAILED")]
        public async Task FungerendeReferanserMellomObjekter_RuleWillFail()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.xlink.1-2D-fail.gml", "gml.xlink.1-3D-fail.gml");
            using var rule = _validator.GetRule<FungerendeReferanserMellomObjekter, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }
        
        [Fact(DisplayName = "gml.xlink.1: Fungerende referanser mellom objekter - PASSED")]
        public async Task FungerendeReferanserMellomObjekter_RuleWillPass()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.xlink.1-2D-pass.gml", "gml.xlink.1-3D-pass.gml");
            using var rule = _validator.GetRule<FungerendeReferanserMellomObjekter, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeTrue();
        }

        [Fact(DisplayName = "gml.gmlid.1: GML-ID for alle objekter i planen skal være unike - FAILED")]
        public async Task UnikGmlIdForAlleObjekterInnenforDatasettet_RuleWillFail()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.gmlid.1-2D-fail.gml", "gml.gmlid.1-3D-fail.gml");
            using var rule = _validator.GetRule<UnikGmlIdForAlleObjekterInnenforDatasettet, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.gmlid.1: GML-ID for alle objekter i planen skal være unike - PASSED")]
        public async Task UnikGmlIdForAlleObjekterInnenforDatasettet_RuleWillPass()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.gmlid.1-2D-pass.gml", "gml.gmlid.1-3D-pass.gml");
            using var rule = _validator.GetRule<UnikGmlIdForAlleObjekterInnenforDatasettet, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeTrue();
        }

        [Fact(DisplayName = "gml.uuid.1: LokalId er en gyldig UUID - FAILED")]
        public async Task LokalIdErEnGyldigUUID_RuleWillFail()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.uuid.1-fail.gml");
            using var rule = _validator.GetRule<LokalIdErEnGyldigUUID, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.uuid.1: LokalId er en gyldig UUID - PASSED")]
        public async Task LokalIdErEnGyldigUUID_RuleWillPass()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.uuid.1-pass.gml");
            using var rule = _validator.GetRule<LokalIdErEnGyldigUUID, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeTrue();
        }

        [Fact(DisplayName = "gml.epsg.1: Koordinatreferansesystem for kart i 2D - FAILED")]
        public async Task KoordinatreferansesystemForKart2D_RuleWillFail()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.epsg.1-fail.gml");
            using var rule = _validator.GetRule<KoordinatreferansesystemForKart2D, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.epsg.1: Koordinatreferansesystem for kart i 2D - PASSED")]
        public async Task KoordinatreferansesystemForKart2D_RuleWillPass()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.epsg.1-pass-1.gml");
            using var rule = _validator.GetRule<KoordinatreferansesystemForKart2D, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeTrue();
        }

        [Fact(DisplayName = "gml.epsg.1: Koordinatreferansesystem for kart i 2D - PASSED")]
        public async Task KoordinatreferansesystemForKart2D_RuleWillAlsoPass()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.epsg.1-pass-2.gml");
            using var rule = _validator.GetRule<KoordinatreferansesystemForKart2D, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeTrue();
        }

        [Fact(DisplayName = "gml.epsg.2: Koordinatreferansesystem for kart i 3D - FAILED")]
        public async Task KoordinatreferansesystemForKart3D_RuleWillFail()
        {
            var validationData = TestHelper.GetGmlValidationData(null, "gml.epsg.2-fail.gml");
            using var rule = _validator.GetRule<KoordinatreferansesystemForKart3D, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.epsg.2: Koordinatreferansesystem for kart i 3D - PASSED")]
        public async Task KoordinatreferansesystemForKart3D_RuleWillPass()
        {
            var validationData = TestHelper.GetGmlValidationData(null, "gml.epsg.2-pass.gml");
            using var rule = _validator.GetRule<KoordinatreferansesystemForKart3D, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeTrue();
        }

        [Fact(DisplayName = "gml.kurve.1: Kurver skal ha gyldig geometri - FAILED")]
        public async Task KurverSkalHaGyldigGeometri_RuleWillFail()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.kurve.1-fail.gml");
            using var rule = _validator.GetRule<KurverSkalHaGyldigGeometri, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.flate.1: Flater skal ha gyldig geometri - FAILED")]
        public async Task FlaterSkalHaGyldigGeometri_RuleWillFail()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.flate.1-fail.gml");
            using var rule = _validator.GetRule<FlaterSkalHaGyldigGeometri, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.flate.2: Avgrensningen til en flate kan ikke krysse seg selv - FAILED")]
        public async Task AvgrensningenTilEnFlateKanIkkeKrysseSegSelv_RuleWillFail()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.flate.2-fail.gml");
            using var rule = _validator.GetRule<AvgrensningenTilEnFlateKanIkkeKrysseSegSelv, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.flate.3: Avgrensningene til en flate skal nøstes i riktig retning - FAILED")]
        public async Task AvgrensningeneTilEnFlateSkalNøstesRiktig_RuleWillFail()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.flate.3-fail.gml");
            using var rule = _validator.GetRule<AvgrensningeneTilEnFlateSkalNøstesRiktig, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.flate.4: Hull i flate må ligge innenfor flatens ytre avgrensning - FAILED")]
        public async Task HullMåLiggeInnenforFlatensYtreAvgrensning_RuleWillFail()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.flate.4-fail.gml");
            using var rule = _validator.GetRule<HullMåLiggeInnenforFlatensYtreAvgrensning, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.flate.5: Hull i flate kan ikke overlappe andre hull i samme flate - FAILED")]
        public async Task HullKanIkkeOverlappeAndreHullISammeFlate_RuleWillFail()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.flate.5-fail.gml");
            using var rule = _validator.GetRule<HullKanIkkeOverlappeAndreHullISammeFlate, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.bue.1: Sirkelbuer kan ikke inneholde dobbeltpunkter - FAILED")]
        public async Task BueKanIkkeHaDobbeltpunkter_RuleWillFail()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.bue.1-fail.gml");
            using var rule = _validator.GetRule<BueKanIkkeHaDobbeltpunkter, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.bue.1: Sirkelbuer kan ikke inneholde dobbeltpunkter - PASSED")]
        public async Task BueKanIkkeHaDobbeltpunkter_RuleWillPass()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.bue.1-pass.gml");
            using var rule = _validator.GetRule<BueKanIkkeHaDobbeltpunkter, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeTrue();
        }

        [Fact(DisplayName = "gml.bue.2: Punktene kan ikke ligge på rett linje for sirkelbue - FAILED")]
        public async Task BueKanIkkeHaPunkterPåRettLinje_RuleWillFail()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.bue.2-fail.gml");
            using var rule = _validator.GetRule<BueKanIkkeHaPunkterPåRettLinje, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.bue.2: Punktene kan ikke ligge på rett linje for sirkelbue - PASSED")]
        public async Task BueKanIkkeHaPunkterPåRettLinje_RuleWillPass()
        {
            var validationData = TestHelper.GetGmlValidationData("gml.bue.2-pass.gml");
            using var rule = _validator.GetRule<BueKanIkkeHaPunkterPåRettLinje, IGmlValidationData>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeTrue();
        }
    }
}
