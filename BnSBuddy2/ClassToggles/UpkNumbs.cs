using System.Collections.Generic;

namespace BnSBuddy2.ClassToggles
{
    public class UpkNumbs
    {
        public Dictionary<string, Dictionary<string, List<string>>> Classes_Toggles = new Dictionary<string, Dictionary<string, List<string>>>();
        private Dictionary<string, List<string>> Effects = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> Animations = new Dictionary<string, List<string>>();
        private List<string> _List = new List<string>();

        public UpkNumbs()
        {
            if (Effects.Count == 0)
            {
                // BM
                _List = new List<string>();
                _List.Add("00013263");
                _List.Add("00060548");
                Effects.Add("Blade Master", _List);
                // KFM
                _List = new List<string>();
                _List.Add("00010771");
                _List.Add("00060549");
                _List.Add("00064821");
                Effects.Add("Kung-Fu Master", _List);
                // FM
                _List = new List<string>();
                _List.Add("00009801");
                _List.Add("00060550");
                _List.Add("00072638");
                Effects.Add("Force Master", _List);
                // DES
                _List = new List<string>();
                _List.Add("00008841");
                _List.Add("00060551");
                _List.Add("00067307");
                Effects.Add("Destroyer", _List);
                // GS
                _List = new List<string>();
                _List.Add("00007307");
                _List.Add("00060552");
                Effects.Add("Gunslinger", _List);
                // ASS
                _List = new List<string>();
                _List.Add("00010504");
                _List.Add("00060553");
                Effects.Add("Assassin", _List);
                // SUM
                _List = new List<string>();
                _List.Add("00006660");
                _List.Add("00060554");
                _List.Add("00080169");
                Effects.Add("Summoner", _List);
                // BD
                _List = new List<string>();
                _List.Add("00031769");
                _List.Add("00060555");
                _List.Add("00072644");
                _List.Add("00072646");
                Effects.Add("Blade Dancer", _List);
                // WL
                _List = new List<string>();
                _List.Add("00023411");
                _List.Add("00023412");
                _List.Add("00060556");
                Effects.Add("Warlock", _List);
                // SF
                _List = new List<string>();
                _List.Add("00034433");
                _List.Add("00060557");
                Effects.Add("Soul Fighter", _List);
                // WD
                _List = new List<string>();
                _List.Add("00056127");
                _List.Add("00060558");
                Effects.Add("Warden", _List);
                // AR
                _List = new List<string>();
                _List.Add("00064738");
                Effects.Add("Archer", _List);
                // AS
                _List = new List<string>();
                _List.Add("00072639");
                _List.Add("00072642");
                Effects.Add("Astromancer", _List);
                // Extras
                _List = new List<string>();
                _List.Add("00003814");
                _List.Add("00007242");
                _List.Add("00008904");
                _List.Add("00009393");
                _List.Add("00009812");
                _List.Add("00010354");
                _List.Add("00010772");
                _List.Add("00010869");
                _List.Add("00011949");
                _List.Add("00012009");
                _List.Add("00024690");
                _List.Add("00026129");
                _List.Add("00059534");
                _List.Add("00060729");
                _List.Add("00069254");
                Effects.Add("Extras", _List);
            }
            if (Animations.Count == 0)
            {
                // BM
                List<string> _List = new List<string>();
                _List.Add("00007911");
                _List.Add("00056567");
                Animations.Add("Blade Master", _List);
                // KFM
                _List = new List<string>();
                _List.Add("00007912");
                _List.Add("00056568");
                _List.Add("00064820");
                Animations.Add("Kung-Fu Master", _List);
                // FM
                _List = new List<string>();
                _List.Add("00007913");
                _List.Add("00056569");
                _List.Add("00068626");
                _List.Add("00068628");
                Animations.Add("Force Master", _List);
                // DES
                _List = new List<string>();
                _List.Add("00007914");
                _List.Add("00056570");
                Animations.Add("Destroyer", _List);
                // GS
                _List = new List<string>();
                _List.Add("00007915");
                _List.Add("00056571");
                Animations.Add("Gunslinger", _List);
                // ASS
                _List = new List<string>();
                _List.Add("00007916");
                _List.Add("00056572");
                _List.Add("00068516");
                Animations.Add("Assassin", _List);
                // SUM
                _List = new List<string>();
                _List.Add("00007917");
                _List.Add("00056573");
                _List.Add("00080266");
                Animations.Add("Summoner", _List);
                // BD
                _List = new List<string>();
                _List.Add("00018601");
                _List.Add("00056574");
                _List.Add("00078303");
                _List.Add("00078533");
                Animations.Add("Blade Dancer", _List);
                // WL
                _List = new List<string>();
                _List.Add("00023439");
                _List.Add("00056575");
                Animations.Add("Warlock", _List);
                // SF
                _List = new List<string>();
                _List.Add("00034408");
                _List.Add("00056576");
                Animations.Add("Soul Fighter", _List);
                // WD
                _List = new List<string>();
                _List.Add("00056126");
                _List.Add("00056566");
                _List.Add("00056577");
                Animations.Add("Warden", _List);
                // AR
                _List = new List<string>();
                _List.Add("00064736");
                Animations.Add("Archer", _List);
                // AS
                _List = new List<string>();
                _List.Add("00069237");
                _List.Add("00069238");
                _List.Add("00076159");
                Animations.Add("Astromancer", _List);
            }
            if (Classes_Toggles.Count == 0) {
                Classes_Toggles.Add("Animations", Animations);
                Classes_Toggles.Add("Effects", Effects);
            }
        }
    }
}
