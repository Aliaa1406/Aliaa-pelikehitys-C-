using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Tehtävä_4___Seikkailijanreppu
{
    public class Item  // Tavara class
    {
        public float paino; // weight
        public float tilavuus; // volume as a single value

        public Item(float paino, float tilavuus)
        {
            this.paino = paino;
            this.tilavuus = tilavuus;
        }
    }

    // Item subclasses
    public class Nuoli : Item { public Nuoli() : base(0.1f, 0.05f) { } }
    public class Jousi : Item { public Jousi() : base(1, 4) { } }
    public class Köysi : Item { public Köysi() : base(1, 1.5f) { } }
    public class Vesi : Item { public Vesi() : base(2, 2) { } }
    public class RuokaAnnos : Item { public RuokaAnnos() : base(1, 0.5f) { } }
    public class Miekka : Item { public Miekka() : base(5, 3) { } }

    public class Reppu
    {
        private List<Item> items = new List<Item>();
        private int maxItem;
        private float maxPaino;
        private float maxTilavuus;

        // Properties to get current state
        public int NykyinenMäärä => items.Count;
        public float NykyinenPaino => items.Sum(t => t.paino);
        public float NykyinenTilavuus => items.Sum(t => t.tilavuus);

        // Properties to get remaining capacity
        public int JäljelläOlevaMäärä => maxItem - NykyinenMäärä;
        public float JäljelläOlevaPaino => maxPaino - NykyinenPaino;
        public float JäljelläOlevaTilavuus => maxTilavuus - NykyinenTilavuus;

        public Reppu(int maxItem, float maxPaino, float maxTilavuus)
        {
            this.maxItem = maxItem;
            this.maxPaino = maxPaino;
            this.maxTilavuus = maxTilavuus;
        }

        public bool Lisää(Item item)
        {
            if (NykyinenMäärä >= maxItem ||
                NykyinenPaino + item.paino > maxPaino ||
                NykyinenTilavuus + item.tilavuus > maxTilavuus)
            {
                return false;
            }

            items.Add(item);
            return true;
        }

        public override string ToString()
        {
            return $"Reppu: {NykyinenMäärä}/{maxItem} item, {NykyinenPaino:0.0}/{maxPaino} paino, {NykyinenTilavuus:0.0}/{maxTilavuus} tilavuus.";
        }
    }

    internal class Program
    {
        public static void Main()
        {
            // Create a backpack with capacity constraints
            Reppu reppu = new Reppu(10, 30, 20);

            while (true)
            {
                Console.WriteLine(reppu);
                Console.WriteLine("Mitä haluat lisätä?\n1 - Nuoli\n2 - Jousi\n3 - Köysi\n4 - Vettä\n5 - Ruokaa\n6 - Miekka\n0 - Lopeta");
                string syote = Console.ReadLine();

                if (syote == "0") break;

                Item item = null;
                switch (syote)
                {
                    case "1": item = new Nuoli(); break;
                    case "2": item = new Jousi(); break;
                    case "3": item = new Köysi(); break;
                    case "4": item = new Vesi(); break;
                    case "5": item = new RuokaAnnos(); break;
                    case "6": item = new Miekka(); break;
                }

                if (item != null)
                {
                    if (reppu.Lisää(item))
                    {
                        Console.WriteLine("Tavara lisätty reppuun.");
                    }
                    else
                    {
                        Console.WriteLine("Tavaraa ei voitu lisätä, koska reppu ylittää kapasiteetin.");
                    }
                }
                else
                {
                    Console.WriteLine("Virheellinen valinta.");
                }
            }
        }
    }
}