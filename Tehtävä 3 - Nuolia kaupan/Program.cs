namespace Tehtävä_3___Nuolia_kaupan
{
    internal class Program
    {

        static void Main(string[] args)
        {
            int Hinta = 0;

            KärkiMateriaali valittuKärki = KärkiMateriaali.Puu;
            SulkeMateriaali valittuSukla = SulkeMateriaali.Lehti;
            sbyte pituusCm = 0;
            while (true)
            {
                Console.WriteLine("Valitse kärki materiaali \nPuu\nTeräs\nTimantti");
                string? chooser = Console.ReadLine();
                if (chooser.ToLower() == "puu")
                {
                    valittuKärki = KärkiMateriaali.Puu;
                    break;
                }
                else if (chooser.ToLower() == "Timantti")
                {
                    valittuKärki = KärkiMateriaali.Timantti;
                    break;
                }
                else if (chooser.ToLower() == "Teräs")
                {
                    valittuKärki = KärkiMateriaali.Teräs;
                    break;
                }
                else
                {
                    Console.WriteLine("Incorrect choice");
                }
            }
            while (true)
            {
                Console.WriteLine("Opstin sulke Maeriaal: \n  Lehti\n  Kanansulka\n   Kotkanasulka " );
                string? chooser = Console.ReadLine().ToLower();
                if (chooser == "lehti")
                {
                    valittuSukla = SulkeMateriaali.Lehti;
                    break;

                }
                else if (chooser == "Kanansulka")
                {
                    valittuSukla = SulkeMateriaali.Kanansulka;
                    break;
                }
                else if (chooser == "Kotkanasulka")
                {
                    valittuSukla = SulkeMateriaali.Kotkanasulka;
                    break;
                }
                else
                {
                    Console.WriteLine("Incorrect choice");
                    
                }
            }

            while (true)
            {
                Console.WriteLine("anna pituus nuola (60-100 CM): ");

                try
                {
                    pituusCm = sbyte.Parse(Console.ReadLine().ToLower());
                    if (pituusCm >= 60 && pituusCm <= 100)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("pituuus on vällllä 60-100 Cm.");
                    }
                }
                catch
                {
                    Console.WriteLine("Syotä kwlvollinen numero.");
                }
            }

            Nuoli nuoli = new Nuoli(valittuKärki, valittuSukla, pituusCm);
            float hinta = nuoli.AnnaHinta();

            Console.WriteLine($"Nuola hinta on:{hinta}€ kultaa");



            //kysy nuolen kärki, sukla ja pituus
            // kysy pituua uudentaan jos se on <60|| >100

            //Tulosta nuolen hinta
        }
    }
}
