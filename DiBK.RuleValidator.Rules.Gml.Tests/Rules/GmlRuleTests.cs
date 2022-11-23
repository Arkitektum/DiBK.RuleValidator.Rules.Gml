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
            _validator.LoadRules<IGmlValidationInputV1>();
        }

        [Fact(DisplayName = "gml.gmlid.1: GML-ID for alle objekter i planen skal være unike - FAILED")]
        public async Task UnikGmlIdForAlleObjekterInnenforDatasettet_RuleWillFail()
        {
            using var validationData = TestHelper.GetGmlValidationInputV1("gml.gmlid.1-2D-fail.gml", "gml.gmlid.1-3D-fail.gml");
            using var rule = _validator.GetRule<UnikGmlIdForAlleObjekterInnenforDatasettet, IGmlValidationInputV1>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.gmlid.1: GML-ID for alle objekter i planen skal være unike - PASSED")]
        public async Task UnikGmlIdForAlleObjekterInnenforDatasettet_RuleWillPass()
        {
            using var validationData = TestHelper.GetGmlValidationInputV1("gml.gmlid.1-2D-pass.gml", "gml.gmlid.1-3D-pass.gml");
            using var rule = _validator.GetRule<UnikGmlIdForAlleObjekterInnenforDatasettet, IGmlValidationInputV1>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeTrue();
        }

        [Fact(DisplayName = "gml.uuid.1: LokalId er en gyldig UUID - FAILED")]
        public async Task LokalIdErEnGyldigUUID_RuleWillFail()
        {
            using var validationData = TestHelper.GetGmlValidationInputV1("gml.uuid.1-fail.gml");
            using var rule = _validator.GetRule<LokalIdErEnGyldigUUID, IGmlValidationInputV1>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.uuid.1: LokalId er en gyldig UUID - PASSED")]
        public async Task LokalIdErEnGyldigUUID_RuleWillPass()
        {
            using var validationData = TestHelper.GetGmlValidationInputV1("gml.uuid.1-pass.gml");
            using var rule = _validator.GetRule<LokalIdErEnGyldigUUID, IGmlValidationInputV1>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeTrue();
        }

        [Fact(DisplayName = "gml.kurve.1: Kurver skal ha gyldig geometri - FAILED")]
        public async Task KurverSkalHaGyldigGeometri_RuleWillFail()
        {
            using var validationData = TestHelper.GetGmlValidationInputV1("gml.kurve.1-fail.gml");
            using var rule = _validator.GetRule<KurverSkalHaGyldigGeometri, IGmlValidationInputV1>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.kurve.2: Kurver kan ikke inneholde dobbeltpunkter - FAILED")]
        public async Task LinjeKanIkkeHaDobbeltpunkter_RuleWillFail()
        {
            using var validationData = TestHelper.GetGmlValidationInputV1("gml.kurve.2-fail.gml");
            using var rule = _validator.GetRule<KurverKanIkkeHaDobbeltpunkter, IGmlValidationInputV1>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.flate.1: Flater skal ha gyldig geometri - FAILED")]
        public async Task FlaterSkalHaGyldigGeometri_RuleWillFail()
        {
            using var validationData = TestHelper.GetGmlValidationInputV1("gml.flate.1-fail.gml");
            using var rule = _validator.GetRule<FlaterSkalHaGyldigGeometri, IGmlValidationInputV1>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.flate.2: Avgrensningen til en flate kan ikke krysse seg selv - FAILED")]
        public async Task AvgrensningenTilEnFlateKanIkkeKrysseSegSelv_RuleWillFail()
        {
            using var validationData = TestHelper.GetGmlValidationInputV1("gml.flate.2-fail.gml");
            using var rule = _validator.GetRule<AvgrensningenTilEnFlateKanIkkeKrysseSegSelv, IGmlValidationInputV1>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.flate.3: Avgrensningene til en flate skal nøstes i riktig retning - FAILED")]
        public async Task AvgrensningeneTilEnFlateSkalNøstesRiktig_RuleWillFail()
        {
            using var validationData = TestHelper.GetGmlValidationInputV1("gml.flate.3-fail.gml");
            using var rule = _validator.GetRule<AvgrensningeneTilEnFlateSkalNøstesRiktig, IGmlValidationInputV1>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.flate.4: Hull i flate må ligge innenfor flatens ytre avgrensning - FAILED")]
        public async Task HullMåLiggeInnenforFlatensYtreAvgrensning_RuleWillFail()
        {
            using var validationData = TestHelper.GetGmlValidationInputV1("gml.flate.4-fail.gml");
            using var rule = _validator.GetRule<HullMåLiggeInnenforFlatensYtreAvgrensning, IGmlValidationInputV1>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.flate.5: Hull i flate kan ikke overlappe andre hull i samme flate - FAILED")]
        public async Task HullKanIkkeOverlappeAndreHullISammeFlate_RuleWillFail()
        {
            using var validationData = TestHelper.GetGmlValidationInputV1("gml.flate.5-fail.gml");
            using var rule = _validator.GetRule<HullKanIkkeOverlappeAndreHullISammeFlate, IGmlValidationInputV1>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.bue.1: Sirkelbuer kan kun inneholde tre punkter - FAILED")]
        public async Task BueKanIkkeHaDobbeltpunkter_RuleWillFail()
        {
            using var validationData = TestHelper.GetGmlValidationInputV1("gml.bue.1-fail.gml");
            using var rule = _validator.GetRule<SirkelbuerKanKunInneholdeTrePunkter, IGmlValidationInputV1>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.bue.1: Sirkelbuer kan kun inneholde tre punkter - PASSED")]
        public async Task BueKanIkkeHaDobbeltpunkter_RuleWillPass()
        {
            using var validationData = TestHelper.GetGmlValidationInputV1("gml.bue.1-pass.gml");
            using var rule = _validator.GetRule<SirkelbuerKanKunInneholdeTrePunkter, IGmlValidationInputV1>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeTrue();
        }

        [Fact(DisplayName = "gml.bue.2: Punktene kan ikke ligge på rett linje for sirkelbue - FAILED")]
        public async Task BueKanIkkeHaPunkterPåRettLinje_RuleWillFail()
        {
            using var validationData = TestHelper.GetGmlValidationInputV1("gml.bue.2-fail.gml");
            using var rule = _validator.GetRule<SirkelbuerKanIkkeHaPunkterPåRettLinje, IGmlValidationInputV1>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }

        [Fact(DisplayName = "gml.bue.2: Punktene kan ikke ligge på rett linje for sirkelbue - PASSED")]
        public async Task BueKanIkkeHaPunkterPåRettLinje_RuleWillPass()
        {
            using var validationData = TestHelper.GetGmlValidationInputV1("gml.bue.2-pass.gml");
            using var rule = _validator.GetRule<SirkelbuerKanIkkeHaPunkterPåRettLinje, IGmlValidationInputV1>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeTrue();
        }

        [Fact(DisplayName = "gml.avgr.1: Samsvarende avgrensingsgeometri - PASSED")]
        public async Task SamsvarendeAvgrensingsgeometri_RuleWillPass()
        {
            using var validationData = TestHelper.GetGmlValidationInputV1("gml.avgr.1-pass.gml");
            using var rule = _validator.GetRule<SamsvarendeAvgrensingsgeometri, IGmlValidationInputV1>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeTrue();
        }

        [Fact(DisplayName = "gml.avgr.1: Samsvarende avgrensingsgeometri - FAILED")]
        public async Task SamsvarendeAvgrensingsgeometri_RuleWillFail()
        {
            using var validationData = TestHelper.GetGmlValidationInputV1("gml.avgr.1-fail.gml");
            using var rule = _validator.GetRule<SamsvarendeAvgrensingsgeometri, IGmlValidationInputV1>();

            await rule.Execute(validationData);
            rule.Passed.Should().BeFalse();
        }
    }
}
