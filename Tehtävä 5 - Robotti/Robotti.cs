using System;
using Raylib_cs;

public class Robotti
{
    // Robotin tilan seuranta
    private bool onKäynnissä; // Onko robotti päällä
    private int x; // X-koordinaatti
    private int y; // Y-koordinaatti

    public Robotti()
    {
        onKäynnissä = false;
        x = 0;
        y = 0;
    }

    // Virran hallinta
    public void Käynnistä() => onKäynnissä = true;
    public void Sammuta() => onKäynnissä = false;
    public bool OnKäynnissä() => onKäynnissä;

    // Liikkuminen
    public void SiirryYlös() { if (onKäynnissä) y++; }
    public void SiirryAlas() { if (onKäynnissä) y--; }
    public void SiirryVasemmalle() { if (onKäynnissä) x--; }
    public void SiirryOikealle() { if (onKäynnissä) x++; }

    public void Suorita(RobottiKäsky[] käskyt)
    {
        foreach (var käsky in käskyt)
        {
            käsky.Suorita(this);
            Console.WriteLine($"Robotti: [{x}, {y}, {onKäynnissä}]");
        }
    }
}

public abstract class RobottiKäsky
{
    public abstract void Suorita(Robotti robotti);
}

// Käynnistyskomento
public class KäynnistäKäsky : RobottiKäsky
{
    public override void Suorita(Robotti robotti) => robotti.Käynnistä();
}

// Sammutuskomento
public class SammutaKäsky : RobottiKäsky
{
    public override void Suorita(Robotti robotti) => robotti.Sammuta();
}

// Liikkumiskomennot
public class YlösKäsky : RobottiKäsky
{
    public override void Suorita(Robotti robotti) { if (robotti.OnKäynnissä()) robotti.SiirryYlös(); }
}

public class AlasKäsky : RobottiKäsky
{
    public override void Suorita(Robotti robotti) { if (robotti.OnKäynnissä()) robotti.SiirryAlas(); }
}

public class VasenKäsky : RobottiKäsky
{
    public override void Suorita(Robotti robotti) { if (robotti.OnKäynnissä()) robotti.SiirryVasemmalle(); }
}

public class OikeaKäsky : RobottiKäsky
{
    public override void Suorita(Robotti robotti) { if (robotti.OnKäynnissä()) robotti.SiirryOikealle(); }
}
