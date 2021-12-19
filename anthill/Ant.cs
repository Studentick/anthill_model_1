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
            //Console.WriteLine($"{this.allive}-31");
            if (this.Health == -1)
            {
                //Console.WriteLine($"{this.allive}-32");
                //Console.WriteLine("Ошибка в дестрое муравья");
                allive = false;
                return;
            }
            //Console.WriteLine($"{this.allive}-33");
            switch (this.Status)
            {
                case QN: allive = !QueenDestroy();
                    //Console.WriteLine($"{this.allive}-331");
                    break;
                case SW: allive = !SwDestroy();
                    //Console.WriteLine($"{this.allive}-332");
                    break;
                case RAB: allive = !WorkerDestroy();
                    //Console.WriteLine($"{this.allive}-333");
                    break;
            }
            //Console.WriteLine($"{this.allive}-34");
        }
        public bool QueenDestroy()
        {
            if (Ttl > QnMxTtl)
            {
                //Console.WriteLine("Ошибка в дестрое королевы");
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
                //Console.WriteLine("Ошибка в дестрое свиты");
                svita_count--;
                return true;
            }
            return false;
        }
        public bool WorkerDestroy()
        {
            //Console.WriteLine($"{this.allive}-333-1");
            if ((fail_counter > 2 && this.Health == 1) || (Ttl > WkMXTtl))
            {
                //Console.WriteLine($"{this.allive}-333-11");
                //Console.WriteLine("Ошибка в дестрое рабочего");
                return true;
            }
            //Console.WriteLine($"{this.allive}-333-12");
            return false;
        }

        // Svita
        public void GetQueenInfo()
        {
            // Search in ants list ant where ant.id = Q_id
            Console.WriteLine("Queen status = {Q.status}, age = {Q.ttl}. by ant-{this.Id}");
            Thread.Sleep(AntCong.GetQueenInfo);
            //PutEggs();
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
            Thread.Sleep(AntCong.CareHive);
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
            Thread.Sleep(AntCong.Forage);
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
                    Thread.Sleep(AntCong.ToSvita);
                }
            }
        }

        void PutEggs()
        {
            if(this.Status != RAB)
            {
                if (Egg.Clutch == null || Egg.Clutch.Count < Egg.max_egg)
                {
                    int count = new Random().Next(3, 9);
                    for (int i = 0; i < count; i++)
                        new Egg(this.Id);
                    Thread.Sleep(AntCong.PutEgg);
                }
            }
        }

        public void AntIteration()
        {
            //Console.WriteLine($"{this.allive}-2");
            this.Destroy();
            //Console.WriteLine($"{this.allive}-3");
            this.Eat();
            //Console.WriteLine($"{this.allive}-4");
            //Console.WriteLine($"GGGGGGGGGGGGGGGG");
            switch (this.Status)
            {
                case QN: QueenIteration();
                    //Console.WriteLine($"{this.allive}-5");
                    break;
                case SW: SwitaIteration();
                    //Console.WriteLine($"{this.allive}-6");
                    break;
                case RAB: WorkerIteration();
                    //Console.WriteLine($"{this.allive}-7");
                    break;
            }
            //Console.WriteLine($"{this.allive}-8");
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
