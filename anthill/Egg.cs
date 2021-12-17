using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anthill
{
    class Egg
    {
        static public List<Egg> Clutch;
        public ulong ParrentId { get;}
        public Egg(ulong parrent)
        {
            
            this.ParrentId = parrent;
            if (Clutch == null)
            {
                Clutch = new List<Egg>();
            }
            Clutch.Add(this);
        }
        public void Hatch()
        {
            new Ant();

            this.Destroy("вылупления птенца");
        }

        // В теории это можно было сделать при помощи дескриптора,
        // но сборщик мусора у дотнета крайне не предсказуем, ну 
        // или я туплю так что я решил сделать так 
        public void Destroy(string cause = "смены правительства")
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Яйцо было уничтожено по причине {cause}");
            Console.ResetColor();
            Clutch.Remove(this);
        }

    }

    class Clutch
    {
        //private static Clutch 
    }
}
