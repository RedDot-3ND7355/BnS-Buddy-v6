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
                _List.Add("Mod_Remove_SwordMaster_05_SF_p");
                Effects.Add("Blade Master", _List);
                // KFM
                _List = new List<string>();
                _List.Add("Mod_Remove_KungfuFighter_05_SF_p");
                Effects.Add("Kung-Fu Master", _List);
                // FM
                _List = new List<string>();
                _List.Add("Mod_Remove_ForceMaster_05_SF_p");
                Effects.Add("Force Master", _List);
                // DES
                _List = new List<string>();
                _List.Add("Mod_Remove_Destroyer_05_SF_p");
                Effects.Add("Destroyer", _List);
                // GS
                _List = new List<string>();
                _List.Add("Mod_Remove_Shooter_05_SF_p");
                Effects.Add("Gunslinger", _List);
                // ASS
                _List = new List<string>();
                _List.Add("Mod_Remove_Assassin_05_SF_p");
                Effects.Add("Assassin", _List);
                // SUM
                _List = new List<string>();
                _List.Add("Mod_Remove_Summoner_05_SF_p");
                Effects.Add("Summoner", _List);
                // BD
                _List = new List<string>();
                _List.Add("Mod_Remove_Fencer_05_SF_p");
                Effects.Add("Blade Dancer", _List);
                // WL
                _List = new List<string>();
                _List.Add("Mod_Remove_Warlock_05_SF_p");
                Effects.Add("Warlock", _List);
                // SF
                _List = new List<string>();
                _List.Add("Mod_Remove_SoulFighter_05_SF_p");
                Effects.Add("Soul Fighter", _List);
                // WD
                _List = new List<string>();
                _List.Add("Mod_Remove_Warrior_05_SF_p");
                Effects.Add("Warden", _List);
                // AR
                _List = new List<string>();
                _List.Add("Mod_Remove_Archer_05_SF_p");
                Effects.Add("Archer", _List);
                // AS
                _List = new List<string>();
                _List.Add("Mod_Remove_Thunderder_05_SF_p");
                Effects.Add("Astromancer", _List);
                // DUAL BLADE
                _List = new List<string>();
                _List.Add("Mod_Remove_Dualblade_05_SF_p");
                Effects.Add("Dual Blade", _List);
                // Extras
                _List = new List<string>();
                _List.Add("Mod_Remove_GemEffects_p");
                _List.Add("Mod_Remove_Bracelet_Effect_p");
                _List.Add("Mod_Remove_Heart_Effect_p");
                _List.Add("Mod_Remove_Soul_Effect_p");
                _List.Add("Mod_Remove_Talisman_Effect_p");
                _List.Add("Mod_Remove_Gunner_MysticBadge_p");
                _List.Add("Mod_Remove_Unity_Effect_p");
                Effects.Add("Extras", _List);
            }
            if (Animations.Count == 0)
            {
                // BM
                List<string> _List = new List<string>();
                _List.Add("Mod_Remove_SwordMaster_Animset_p");
                Animations.Add("Blade Master", _List);
                // KFM
                _List = new List<string>();
                _List.Add("Mod_Remove_KungfuFighter_Animset_p");
                Animations.Add("Kung-Fu Master", _List);
                // FM
                _List = new List<string>();
                _List.Add("Mod_Remove_ForceMaster_Animset_p");
                Animations.Add("Force Master", _List);
                // DES
                _List = new List<string>();
                _List.Add("Mod_Remove_Destroyer_Animset_p");
                Animations.Add("Destroyer", _List);
                // GS
                _List = new List<string>();
                _List.Add("Mod_Remove_Shooter_Animset_p");
                Animations.Add("Gunslinger", _List);
                // ASS
                _List = new List<string>();
                _List.Add("Mod_Remove_Assassin_Animset_p");
                Animations.Add("Assassin", _List);
                // SUM
                _List = new List<string>();
                _List.Add("Mod_Remove_Summoner_Animset_p");
                _List.Add("Mod_Remove_Summon_Animset_p");
                Animations.Add("Summoner", _List);
                // BD
                _List = new List<string>();
                _List.Add("Mod_Remove_Fencer_Animset_p");
                Animations.Add("Blade Dancer", _List);
                // WL
                _List = new List<string>();
                _List.Add("Mod_Remove_Warlock_Animset_p");
                Animations.Add("Warlock", _List);
                // SF
                _List = new List<string>();
                _List.Add("Mod_Remove_Soulfighter_Animset_p");
                Animations.Add("Soul Fighter", _List);
                // WD
                _List = new List<string>();
                _List.Add("Mod_Remove_Warrior_Animset_p");
                Animations.Add("Warden", _List);
                // AR
                _List = new List<string>();
                _List.Add("Mod_Remove_Archer_Animset_p");
                Animations.Add("Archer", _List);
                // AS
                _List = new List<string>();
                _List.Add("Mod_Remove_Thunderer_Animset_p");
                Animations.Add("Astromancer", _List);
                // DUAL BLADE
                _List = new List<string>();
                _List.Add("Mod_Remove_DualBlade_Animset_p");
                _List.Add("Mod_Remove_DualBlader_Animset_p");
                Animations.Add("Dual Blade", _List);
            }
            if (Classes_Toggles.Count == 0) {
                Classes_Toggles.Add("Animations", Animations);
                Classes_Toggles.Add("Effects", Effects);
            }
        }
    }
}
