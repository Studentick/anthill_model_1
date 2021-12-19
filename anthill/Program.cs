using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace anthill
{
    class Program
    {
        static ulong day = 0;
        static bool loop = true;
        public static int d = 0;
        public delegate void NtDay();
        public static event NtDay NextMonth;
        static void Main(string[] args)
        {
            //todo: привязать к переключению дней увеличение ТТЛ муравья
            //реализовать создание нового потока при "рождении муравья", 
            //с использованием бесконечного цыкла. Когад элайв == ложно
            //    вызвать функцию уничтожения муравья
            ////Egg e1 = new Egg(1);
            //Egg e2 = new Egg(1);
            //Egg e3 = new Egg(1);
            //Egg e4 = new Egg(1);
            //Egg e5 = new Egg(1);
            //Egg e6 = new Egg(1);
            //Egg e7 = new Egg(2);
            //Egg e8 = new Egg(3);
            //Console.WriteLine(Egg.Clutch.Count);
            ////// Delete all elements, where parrentId == 1
            ////Egg.Clutch.RemoveAll(x => x.ParrentId == 1);
            //e5.Destroy();
            //Console.WriteLine(Egg.Clutch.Count);

            Ant.OnCreateAnt += (ant) => { Console.WriteLine($"Ant {ant.Id} is created with rang {ant.Status}"); };
            Ant.OnCreateAnt += ant => { AntLiveCicle(ant); };
            Egg.OnHatchEgg += (msg) => { Console.WriteLine(msg); };
            NextMonth += () => { Console.WriteLine($"Прошел {d}-й день в муравейнике"); d++; };
            RingOfSansara();
            new Ant();
            new Ant();
            new Ant();
            new Ant();
            new Ant();
            Console.WriteLine("fggfgfdg");
            while (loop)
            {
                var t = Console.ReadLine();
                if (t == "p")
                {
                    loop = false;
                }

            }
            Console.ReadKey();
        }

        static void AntLiveCicle(Ant ant)
        {
            new Thread(() =>
            {
                while (ant.allive && loop)
                {
                    Console.WriteLine($"\tМ-{ant.Id} Start---");
                    ant.AntIteration();
                    Console.WriteLine($"\t---М-{ant.Id} Finish");
                    Thread.Sleep(AntCong.AntLiveCicle);
                }
                Ant.AntHill.Remove(ant);
            }
                ).Start();
        }

        static void RingOfSansara()
        {
            new Thread(() =>
            {
                while (loop)
                {
                    Ant.Writen($"Наступил день {day}-й:\r\n", ConsoleColor.DarkGreen);
                    
                    NextMonth?.Invoke();
                    Thread.Sleep(AntCong.RingOfSansara);
                    day++;
                }
            }).Start();
        }


    }
}
