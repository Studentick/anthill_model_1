using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anthill
{
    class Ant
    {

        public static List<Ant> AntHill;
        // Для оптимизации думал short использовать, но передумал
        public const string RAB = "Worker"; public const string SW = "retinue";
        public const string QN = "queen";
        protected static short svita_count = 0;
        protected static ulong? Q_id = null;
        protected static ulong ID;
        private ushort fail_counter;
        public bool allive;
        public ulong Id { get; }
        public string Status { get; private set; }
        public short Health { get; } // 1 - Yes, 0 - Can Eat, -1 - No
        public int Ttl { get; }

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
                case QN: allive = QueenDestroy(); break;
                case SW: allive = SwDestroy(); break;
                case RAB: allive = WorkerDestroy(); break;
            }
        }
        public bool QueenDestroy()
        {
            if (Ttl > 65535)
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
            if (egg > 0 && Q_id != null && Q_id != this.Id)
            {
                svita_count--;
                return true;
            }
            return false;
        }
        public bool WorkerDestroy()
        {
            if (fail_counter > 2 && this.Health == 1)
            {
                return true;
            }
            return false;
        }

        // Svita
        public void GetQueenInfo()
        {
            // Search in ants list ant where ant.id = Q_id
            Console.WriteLine("Queen status = {Q.status}, age = {Q.ttl}");
        }

        public void ToQueen()
        {
            if (Q_id == null)
                if (this.Status == SW)
                {
                    this.Status = QN;
                    Q_id = this.Id;
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
        }
        public void ToSvita()
        {
            if (this.Status != SW || this.Status != QN)
            {
                if (svita_count < 10)
                {
                    this.Status = SW;
                    svita_count++;
                }
            }
        }

    }
}
