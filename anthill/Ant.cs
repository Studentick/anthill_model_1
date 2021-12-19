using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace anthill
{
    class Ant
    {
        public delegate void CrAnt(Ant junior_ant);
        public static event CrAnt OnCreateAnt;
        public static List<Ant> AntHill;
        // Для оптимизации думал short использовать, но передумал
        public const string RAB = "Worker"; public const string SW = "retinue";
        public const string QN = "queen";
        protected static short svita_count = 0;
        protected static ulong? Q_id = null;
        public static ulong? Queen_id
        {
            get
            {
                return Q_id;
            }
        }
        protected static ulong ID;
        private ushort fail_counter;
        public bool allive;
        public ulong Id { get; }
        public string Status { get; private set; }
        public short Health { get; } // 1 - Yes, 0 - Can Eat, -1 - No
        public int Ttl { get; private set; }
        public const int MaxAntIntHill = 30;

        // Царица 15 - 20 лет, Свита и рабочие 7 лет
        int QnMxTtl = 65535; int SwMxTtl = 55535; int WkMXTtl = 10000;

        public Ant()
        {
            allive = true;
            this.Id = ID; ID++;
            this.Status = RAB;
            this.Health = 1;
            this.Ttl = 0;
            this.fail_counter = 0;
            if (AntHill == null)
            {
                AntHill = new List<Ant>();
            }
            AntHill.Add(this);

            OnCreateAnt?.Invoke(this);
            // Я так понимаю сейчас будет костыль в этой реализации, 
            // а правильная реализация - это использование расширений
            Program.NextMonth += () => { this.Ttl++; };
        }
        
        // To AllAnts
        public void Eat()
        {
            if (this.Health != -1)
                Console.WriteLine($"Муравей {this.Id} ({this.Status}) покушал");
        }

        public void CreateEgg()
        {
            if (this.Status == QN || (this.Status == SW && Q_id == null))
            {
                // Create Egg. Push new egg to egg stack
            }
        }

        public void Destroy()
        {
            if (this.Health == -1)
            {
                Console.WriteLine("Ошибка в дестрое муравья");
                allive = false;
                return;
            }
            switch (this.Status)
            {
                case QN: allive = QueenDestroy(); break;
                case SW: allive = SwDestroy(); break;
                case RAB: allive = WorkerDestroy(); break;
            }
        }
        public bool QueenDestroy()
        {
            if (Ttl > QnMxTtl)
            {
                Console.WriteLine("Ошибка в дестрое королевы");
                Q_id = null;
                return true;
            }
            return false;
        }

        public bool SwDestroy()
        {
            int egg = 0;
            // egg = EggCount();
            if ((egg > 0 && Q_id != null && Q_id != this.Id) || (Ttl > SwMxTtl))
            {
                Console.WriteLine("Ошибка в дестрое свиты");
                svita_count--;
                return true;
            }
            return false;
        }
        public bool WorkerDestroy()
        {
            if ((fail_counter > 2 && this.Health == 1) || (Ttl > WkMXTtl))
            {
                Console.WriteLine("Ошибка в дестрое рабочего");
                return true;
            }
            return false;
        }

        // Svita
        public void GetQueenInfo()
        {
            // Search in ants list ant where ant.id = Q_id
            Console.WriteLine("Queen status = {Q.status}, age = {Q.ttl}. by ant-{this.Id}");
            PutEggs();
        }

        public void ToQueen()
        {
            short cr = (short)new Random().Next(1,4);
            if (Q_id == null)
                if (this.Status == SW && cr == 2)
                {
                    this.Status = QN;
                    Q_id = this.Id;
                    List<Ant> tmp = AntHill.Where(x => x.Status == SW).ToList<Ant>();
                    tmp.AsParallel().ForAll(x => x.Destroy());
                    List<Egg> temp2 = Egg.Clutch.Where(x => x.ParrentId != this.Id).ToList<Egg>();
                    temp2.AsParallel().ForAll(x=>x.Destroy());
                }
        }
        // Workers
        public void Work()
        {
            if (this.Ttl < 5)
            {
                CareHive();
            }
            else
            {
                Forage();
            }
        }
        private void CareHive()
        {
            Console.WriteLine($"Ant {this.Id} on tudy in hive");
            Thread.Sleep(100);
        }
        private void Forage()
        {
            Console.WriteLine($"Ant {this.Id} search food");
            Random rand = new Random();
            int fail_chanse = rand.Next(0, 100);
            if (fail_chanse == 50)
            {
                this.fail_counter++;
            }
            Thread.Sleep(300);
        }
        public void ToSvita()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{this.Id} пытается стать свитой");
            Console.ResetColor();
            if (this.Status != SW || this.Status != QN)
            {
                if (svita_count < 5) //10
                {
                    this.Status = SW;
                    svita_count++;
                    Thread.Sleep(1000);
                }
            }
        }

        void PutEggs()
        {
            if(this.Status != RAB)
            {
                if (Egg.Clutch.Count < Egg.max_egg)
                {
                    int count = new Random().Next(3, 9);
                    for (int i = 0; i < count; i++)
                        new Egg(this.Id);
                    Thread.Sleep(10000);
                }
            }
        }

        public void AntIteration()
        {
            DB(1);
            this.Destroy();
            DB(2);
            this.Eat();
            DB(3);
            switch (this.Status)
            {
                case QN: QueenIteration(); DB(4); break;
                case SW: SwitaIteration(); DB(5); break;
                case RAB: WorkerIteration(); DB(6); break;
            }
            DB(10);
        }
        public static void DB(int i)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Compl {i}");
            Console.ResetColor();
        }
        void QueenIteration()
        {
            PutEggs();

        }
        void SwitaIteration()
        {
            GetQueenInfo(); // not complite
        }
        void WorkerIteration()
        {
            DB(7);
            ToSvita();
            DB(8);
            Work();
            DB(9);
        }

    }
}
