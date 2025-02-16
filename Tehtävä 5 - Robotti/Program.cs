class Program
{
    static RobottiKäsky LuoKäsky(string käsky) => käsky.ToLower() switch
    {
        "käynnistä" => new KäynnistäKäsky(),
        "sammuta" => new SammutaKäsky(),
        "ylös" => new YlösKäsky(),
        "alas" => new AlasKäsky(),
        "oikea" => new OikeaKäsky(),
        "vasen" => new VasenKäsky(),
        _ => throw new ArgumentException("Tuntematon komento")
    };

    static void Main(string[] args)
    {
        // Luo uusi robotti
        Robotti robotti = new Robotti();
        RobottiKäsky[] käskyt = new RobottiKäsky[3];

        // Kysy kolme komentoa käyttäjältä
        for (int i = 0; i < 3; i++)
        {
            Console.Write("Mitä komentoja syötetään robotille?\n" +
                          "Vaihtoehdot: Käynnistä, Sammuta, Ylös, Alas, Oikea, Vasen: ");
            string syöte = Console.ReadLine();
            try
            {
                käskyt[i] = LuoKäsky(syöte);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                i--; // Kysy komento uudelleen, jos annettiin virheellinen syöte
            }
        }

        // Suorita komennot
        robotti.Suorita(käskyt);
    }
}