namespace Tehtävä_3___Nuolia_kaupan
{


    enum KärkiMateriaali
    {
        Puu,
        Teräs,
        Timantti,
        Kivi,
    }

    enum SulkeMateriaali
    {
        Lehti,
        Kanansulka,
        Kotkanasulka,

    }

    internal class Nuoli
    {
        sbyte pituusCm;
        SulkeMateriaali sulka;
        KärkiMateriaali Kärki;

        // tee konstruktori
        public Nuoli(KärkiMateriaali kärki, SulkeMateriaali sulke, float pituusCm)
        {

            this.Kärki = kärki;
            this.sulka = sulke;
            this.pituusCm = (sbyte)pituusCm;

        }
        // prise account 
        public float AnnaHinta()
        {
            float hinta = 0;
            hinta += Kärki switch
            {
                KärkiMateriaali.Puu => 3,
                KärkiMateriaali.Teräs => 5,
                KärkiMateriaali.Timantti => 50,
            };
            hinta += sulka switch
            {
                SulkeMateriaali.Lehti => 0,
                SulkeMateriaali.Kotkanasulka => 1,
                SulkeMateriaali.Kanansulka => 5,
            };
            
            hinta += pituusCm * 0.05f;

            return hinta;
        }

    }

}


