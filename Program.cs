namespace ovi_tehtävä
{
    internal class Program
    {
        enum PlayerDoor
        {
            Locked,
            close,
            open,
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            PlayerDoor state = PlayerDoor.Locked;

            while (true)
            {
                Console.WriteLine($"The Door is {state}. what do you want to do ?");
                string choose = Console.ReadLine()?.ToLower();



                if (state == PlayerDoor.Locked)
                {
                    if (choose == "poista lukitus")
                    {
                        state = PlayerDoor.close;
                    }
                }
                else if (state == PlayerDoor.close)
                {
                    if (choose == "open")
                    {
                        state = PlayerDoor.open;
                    }
                    else if(choose == "lock it")
                    {
                        state = PlayerDoor.Locked;
                    }
                }
                else if (state == PlayerDoor.open)
                {
                    if (choose == "close")
                    {
                        state = PlayerDoor.close;
                    }
                }
                



            }
        }
    }
}

