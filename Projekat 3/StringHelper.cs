using Projekat_3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Projekat_3
{
    public class StringHelper
    {
        public static void CutString(PowerEntity entity)
        {
            string X = entity.X.ToString("#.0000000000");
            string Y = entity.Y.ToString("#.0000000000");

            entity.X = double.Parse(X);
            entity.Y = double.Parse(Y);
        }

        
    }
}
