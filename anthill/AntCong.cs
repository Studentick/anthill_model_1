using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anthill
{
    class AntCong
    {
        // Время на откладывание яиц
        public const int PutEgg = 10000;
        // Переход в свиту
        public const int ToSvita = 1000;
        // Работа по уходу за ульем
        public const int CareHive = 100;
        // Рабаота в поиске еды (фуражирование)
        public const int Forage = 300;
        // Время на получение информации о королеве
        public const int GetQueenInfo = 150;
        // Максимальное значение свиты
        public const int MaxSvitaCount = 2; // 10



        // Время на уничтожение яйца
        public const int DestroyEgg = 100;

        // Время между днями
        public const int RingOfSansara = 55000;
        // Ожидать после того как муравей отработал 1 цыкл
        public const int AntLiveCicle =9000; 
    }
}
