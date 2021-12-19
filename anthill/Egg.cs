using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace anthill
{
    class Egg
    {
        public const int max_egg = 40;
        public delegate void HchEgg(string msg);
        public static event HchEgg OnHatchEgg;
        int ttl;
        int ttlHatch = 3;//10
        static public List<Egg> Clutch;
        public ulong ParrentId { get;}
        public Egg(ulong parrent)
        {
            ttl = 0;
            this.ParrentId = parrent;
            if (Clutch == null)
            {
                Clutch = new List<Egg>();
            }
            Clutch.Add(this);
            // Яичный костыль? (подробнее в объекте Муравей)
            Program.NextMonth += () => { ttl++; if (ttl > ttlHatch) Hatch(); };
        }
        public void Hatch()
        {
            if (Ant.AntHill.Count <= Ant.MaxAntIntHill)
            {
                bool need_hatch = false;
               
                if (this.ParrentId == Ant.Queen_id)
                {
                        need_hatch = true;
                }
                else
                if (Ant.Queen_id == null)
                {
                    List<Ant> tmp = Ant.AntHill.Where(x => x.Id == this.ParrentId).ToList<Ant>();
                    if (tmp[0] != null && tmp[0].allive == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Debug: Egg try update queen");
                        Console.ResetColor();
                        tmp[0].ToQueen();
                        need_hatch = true;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Debug: Egg can-t update queen");
                        Console.ResetColor();
                    }
                    
                }
                if (need_hatch)
                {
                    var ant = new Ant();
                    OnHatchEgg?.Invoke($"Было вылуплено яйцо муравья ({this.ParrentId}) и рождён муравей ({ant.Id})");
                    this.Destroy("вылупления птенца");
                }

            }
            else
            {
                this.Destroy("переполнения муравейника и было разобрано на детали");
            }
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
            Thread.Sleep(AntCong.DestroyEgg);
        }

    }

    class Clutch
    {
        //private static Clutch 
    }
}
