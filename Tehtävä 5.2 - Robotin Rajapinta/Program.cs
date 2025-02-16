class Program
{
    static void Main(string[] args)
    {
        Robotti robotti = new Robotti();
        IRobottiKäsky[] käskyt = new IRobottiKäsky[3];

        for (int i = 0; i < 3; i++)
        {
            Console.Write("Mitä komentoja syötetään robotille?\n" +
                         "Vaihtoehdot: Käynnistä, Sammuta, Ylös, Alas, Oikea, Vasen: ");
            string syöte = Console.ReadLine();
            käskyt[i] = Robotti.LuoKäsky(syöte);
        }

        robotti.Suorita(käskyt);
    }
}