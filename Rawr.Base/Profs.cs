using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr {
    public class Profs {
        #region Functions
        public static Profession IndexToProfession(int i) {
            Profession          p = Profession.None;
            if      (i ==  1) { p = Profession.Alchemy;
            }else if(i ==  2) { p = Profession.Blacksmithing;
            }else if(i ==  3) { p = Profession.Enchanting;
            }else if(i ==  4) { p = Profession.Engineering;
            }else if(i ==  5) { p = Profession.Herbalism;
            }else if(i ==  6) { p = Profession.Inscription;
            }else if(i ==  7) { p = Profession.Jewelcrafting;
            }else if(i ==  8) { p = Profession.Leatherworking;
            }else if(i ==  9) { p = Profession.Mining;
            }else if(i == 10) { p = Profession.Skinning;
            }else if(i == 11) { p = Profession.Tailoring; }
            return p;
        }
        public static int ProfessionToIndex(Profession p) {
            int                                        s =  0;
            if      (p == Profession.Alchemy       ) { s =  1;
            }else if(p == Profession.Blacksmithing ) { s =  2;
            }else if(p == Profession.Enchanting    ) { s =  3;
            }else if(p == Profession.Engineering   ) { s =  4;
            }else if(p == Profession.Herbalism     ) { s =  5;
            }else if(p == Profession.Inscription   ) { s =  6;
            }else if(p == Profession.Jewelcrafting ) { s =  7;
            }else if(p == Profession.Leatherworking) { s =  8;
            }else if(p == Profession.Mining        ) { s =  9;
            }else if(p == Profession.Skinning      ) { s = 10;
            }else if(p == Profession.Tailoring     ) { s = 11; }
            return s;
        }
        public static Profession StringToProfession(string s) {
            Profession                        p = Profession.None;
            if      (s == "Alchemy"       ) { p = Profession.Alchemy;
            }else if(s == "Blacksmithing" ) { p = Profession.Blacksmithing;
            }else if(s == "Enchanting"    ) { p = Profession.Enchanting;
            }else if(s == "Engineering"   ) { p = Profession.Engineering;
            }else if(s == "Herbalism"     ) { p = Profession.Herbalism;
            }else if(s == "Inscription"   ) { p = Profession.Inscription;
            }else if(s == "Jewelcrafting" ) { p = Profession.Jewelcrafting;
            }else if(s == "Leatherworking") { p = Profession.Leatherworking;
            }else if(s == "Mining"        ) { p = Profession.Mining;
            }else if(s == "Skinning"      ) { p = Profession.Skinning;
            }else if(s == "Tailoring"     ) { p = Profession.Tailoring; }
            return p;
        }
        public static string ProfessionToString(Profession p) {
            string                                     s = "None";
            if      (p == Profession.Alchemy       ) { s = "Alchemy";
            }else if(p == Profession.Blacksmithing ) { s = "Blacksmithing";
            }else if(p == Profession.Enchanting    ) { s = "Enchanting";
            }else if(p == Profession.Engineering   ) { s = "Engineering";
            }else if(p == Profession.Herbalism     ) { s = "Herbalism";
            }else if(p == Profession.Inscription   ) { s = "Inscription";
            }else if(p == Profession.Jewelcrafting ) { s = "Jewelcrafting";
            }else if(p == Profession.Leatherworking) { s = "Leatherworking";
            }else if(p == Profession.Mining        ) { s = "Mining";
            }else if(p == Profession.Skinning      ) { s = "Skinning";
            }else if(p == Profession.Tailoring     ) { s = "Tailoring"; }
            return s;
        }
        #endregion
    }
}
