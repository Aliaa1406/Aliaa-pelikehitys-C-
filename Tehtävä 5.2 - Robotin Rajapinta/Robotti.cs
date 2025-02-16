
public interface IRobottiKäsky
{
    void Suorita(Robotti robotti);
}

public class Robotti
{

    private bool onKäynnissä;
    private int x;
    private int y;

    //  Constructor
    public Robotti()
    {
        onKäynnissä = false;
        x = 0;
        y = 0;
    }

    // Energy control methods
    public void Käynnistä() => onKäynnissä = true;
    public void Sammuta() => onKäynnissä = false;
    public bool OnKäynnissä() => onKäynnissä;

    // Motion control methods
    public void SiirryYlös() { if (onKäynnissä) y++; }
    public void SiirryAlas() { if (onKäynnissä) y--; }
    public void SiirryVasemmalle() { if (onKäynnissä) x--; }
    public void SiirryOikealle() { if (onKäynnissä) x++; }

    // How to execute commands
    public void Suorita(IRobottiKäsky[] käskyt)
    {
        foreach (var käsky in käskyt)
        {
            käsky.Suorita(this);
            Console.WriteLine($"Robotti: [{x} {y} {onKäynnissä}]");
        }
    }

    // Internal categories of orders - Inner Classes
    public class KäynnistäKäsky : IRobottiKäsky
    {
        public void Suorita(Robotti robotti) => robotti.Käynnistä();
    }

    public class SammutaKäsky : IRobottiKäsky
    {
        public void Suorita(Robotti robotti) => robotti.Sammuta();
    }

    public class YlösKäsky : IRobottiKäsky
    {
        public void Suorita(Robotti robotti)
        {
            if (robotti.OnKäynnissä()) robotti.SiirryYlös();
        }
    }

    public class AlasKäsky : IRobottiKäsky
    {
        public void Suorita(Robotti robotti)
        {
            if (robotti.OnKäynnissä()) robotti.SiirryAlas();
        }
    }

    public class VasenKäsky : IRobottiKäsky
    {
        public void Suorita(Robotti robotti)
        {
            if (robotti.OnKäynnissä()) robotti.SiirryVasemmalle();
        }
    }

    public class OikeaKäsky : IRobottiKäsky
    {
        public void Suorita(Robotti robotti)
        {
            if (robotti.OnKäynnissä()) robotti.SiirryOikealle();
        }
    }

    // Help method for creating commands
    public static IRobottiKäsky LuoKäsky(string käsky) => käsky.ToLower() switch
    {
        "käynnistä" => new KäynnistäKäsky(),
        "sammuta" => new SammutaKäsky(),
        "ylös" => new YlösKäsky(),
        "alas" => new AlasKäsky(),
        "oikea" => new OikeaKäsky(),
        "vasen" => new VasenKäsky(),
        _ => throw new ArgumentException("Invalid command")
    };
}