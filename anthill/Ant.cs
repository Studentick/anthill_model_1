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
        int QnMxTtl = 55; int SwMxTtl = 35; int WkMXTtl = 20;

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
                allive = false;
                return;
            }
            switch (this.Status)
            {
                case QN: allive = !QueenDestroy();
                    break;
                case SW: allive = !SwDestroy();
                    break;
                case RAB: allive = !WorkerDestroy();
                    break;
            }
        }
        public bool QueenDestroy()
        {
            if (Ttl > QnMxTtl)
            {
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
                svita_count--;
                return true;
            }
            return false;
        }
        public bool WorkerDestroy()
        {
            if ((fail_counter > 2 && this.Health == 1) || (Ttl > WkMXTtl))
            {
                return true;
            }
            return false;
        }

        // Svita
        public void GetQueenInfo()
        {
            // Search in ants list ant where ant.id = Q_id
            Ant Q = null;
            if (AntHill != null)
            {
                var temp = AntHill.Where(x => x.Id == Q_id).ToList<Ant>();
                if(temp.Count > 0)
                {
                    Q = temp[0];
                }
            }
            if (Q != null)
                Console.WriteLine($"Queen status = {Q.allive}, age = {Q.Ttl}. by ant-{this.Id}");
            else
            {
                int i = new Random().Next(8,10);
                Writen($"Королева отсутствует. by ant-{this.Id}",ConsoleColor.Red);
                if (i == 8)
                    PutEggs();
            }

            Thread.Sleep(AntCong.GetQueenInfo);
            
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
                    Ant.svita_count--;
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
            Writen($"Ant {this.Id} on tudy in hive (в улее)",ConsoleColor.DarkGray);
            Thread.Sleep(AntCong.CareHive);
        }
        private void Forage()
        {
            Console.WriteLine($"Ant {this.Id} search food(в поисках еды)",ConsoleColor.DarkGray);
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
            
            if (this.Status != SW || this.Status != QN)
            {
                if (svita_count < AntCong.MaxSvitaCount) //10
                {
                    this.Status = SW;
                    svita_count++;
                    Thread.Sleep(AntCong.ToSvita);
                    Writen($"{this.Id} стаёт свитой", ConsoleColor.Blue);
                }
            }
        }

        void PutEggs()
        {
            if(this.Status != RAB)
            {
                if (Egg.Clutch == null || Egg.Clutch.Count < Egg.max_egg)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    int count = new Random().Next(3, 9);
                    Console.WriteLine($"Муравей-{this.Id} с рангом {this.Status} откладывает яйца ({count}шт)");
                    for (int i = 0; i < count; i++)
                        new Egg(this.Id);
                    Thread.Sleep(AntCong.PutEgg);
                    Console.ResetColor();
                }
            }
        }

        public void AntIteration()
        {
            this.Destroy();
            this.Eat();
            switch (this.Status)
            {
                case QN: QueenIteration();
                    break;
                case SW: SwitaIteration();
                    break;
                case RAB: WorkerIteration();
                    break;
            }
        }
        void QueenIteration()
        {
            Writen($"Королева - {this.Id} контролирует улий",ConsoleColor.Magenta);
            PutEggs();

        }
        void SwitaIteration()
        {
            
            GetQueenInfo(); // not complite
        }
        void WorkerIteration()
        {
            ToSvita();
            Work();

        }

        public static void Writen(string msg, ConsoleColor color)
        {
            var tmp = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = tmp;
        }

    }
}
