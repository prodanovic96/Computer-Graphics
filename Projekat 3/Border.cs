using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat_3
{
    public class Border
    {
        private static readonly double northernBorder = 45.277031;
        private static readonly double southernBorder = 45.2325;
        private static readonly double easternBorder = 19.894459;
        private static readonly double westernBorder = 19.793909;
        
        public static double NorthernBorder => northernBorder;
        public static double SouthernBorder => southernBorder;
        public static double EasternBorder => easternBorder;
        public static double WesternBorder => westernBorder;
    }
}
