using System;
using System.Collections.Generic;

namespace Tehtävä2_Ateria
{
    internal class Program
    {
        enum PääRaaka
        {
            nautaa,
            kanaa,
            kasviksia,
        }

        enum Lisuke
        {
            perunaa,
            riisiä,
            pastaa,
        }

        enum Kastike
        {
            curry,
            hapanimelä,
            pippuri,
            chili,
        }

        class Ateria
        {
            private PääRaaka pääraaka;
            private Lisuke lisuke;
            private Kastike kastike;

            // Constructor for creating a meal with specified ingredients
            public Ateria(PääRaaka pääraaka, Lisuke lisuke, Kastike kastike)
            {
                this.pääraaka = pääraaka;
                this.lisuke = lisuke;
                this.kastike = kastike;
            }

            // Public methods to access private properties
            public PääRaaka AnnaPääRaaka()
            {
                return this.pääraaka;
            }

            public Lisuke AnnaLisuke()
            {
                return this.lisuke;
            }

            public Kastike AnnaKastike()
            {
                return this.kastike;
            }

            // Returns a formatted description of the meal
            public override string ToString()
            {
                return $"{pääraaka} ja {lisuke} {kastike}-kastikkeella";
            }
        }

        static void Main(string[] args)
        {
            // Create a list to store meals (for bonus challenge)
            List<Ateria> ateriat = new List<Ateria>();

            // Number of meals to create
            int ateriaCount = 3; // Set to 3 for bonus challenge, or 1 for basic task

            for (int i = 0; i < ateriaCount; i++)
            {
                if (ateriaCount > 1)
                {
                    Console.WriteLine($"\nAteria {i + 1}:");
                }

                // Display the options for the main ingredient
                Console.WriteLine("Valitse pääraaka-aine:");
                Console.WriteLine("0: nautaa");
                Console.WriteLine("1: kanaa");
                Console.WriteLine("2: kasviksia");
                PääRaaka pääraaka = (PääRaaka)int.Parse(Console.ReadLine());

                // Display the options for the side dish
                Console.WriteLine("\nValitse lisuke:");
                Console.WriteLine("0: perunaa");
                Console.WriteLine("1: riisiä");
                Console.WriteLine("2: pastaa");
                Lisuke lisuke = (Lisuke)int.Parse(Console.ReadLine());

                // Display the options for the sauce
                Console.WriteLine("\nValitse kastike:");
                Console.WriteLine("0: curry");
                Console.WriteLine("1: hapanimelä");
                Console.WriteLine("2: pippuri");
                Console.WriteLine("3: chili");
                Kastike kastike = (Kastike)int.Parse(Console.ReadLine());

                // Create a new meal and add it to the list
                Ateria ateria = new Ateria(pääraaka, lisuke, kastike);
                ateriat.Add(ateria);
            }

            // Print the meals
            Console.WriteLine("\nValitsemasi ateriat:");
            foreach (var ateria in ateriat)
            {
                Console.WriteLine(ateria);
            }

            Console.WriteLine("\nPaina Enter lopettaaksesi.");
            Console.ReadLine();
        }
    }
}