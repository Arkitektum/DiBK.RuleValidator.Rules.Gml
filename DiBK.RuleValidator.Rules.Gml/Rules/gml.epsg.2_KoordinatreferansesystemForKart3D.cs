using System.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class KoordinatreferansesystemForKart3D : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.epsg.2";
            Name = "Koordinatreferansesystem for kart i 3D";
            Description = "Koordinatsystemet for 3D-kart må være i UTM 32, 33 eller 35 (EPSG-kode 5972, 5973, 5975).";
        }

        protected override Status Validate(IGmlValidationData data)
        {
            if (!data.Solids.Any())
                return Status.NOT_EXECUTED;

            var espgCodes = GetSetting<int[]>("EspgCodes3D");

            data.Solids.ForEach(document => Messages.AddRange(KoordinatreferansesystemForKart2D.Validate(document, espgCodes)));

            return HasMessages ? Status.FAILED : Status.PASSED;
        }
    }
}
