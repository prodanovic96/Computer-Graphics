using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Projekat_3
{
    public class CreateDiffuse
    {
        public static DiffuseMaterial Line(string material)
        {
            DiffuseMaterial diffuseMaterial = new DiffuseMaterial();

            if (material == "Steel")
            {
                diffuseMaterial.Brush = Brushes.Blue;
            }
            else if (material == "Copper")
            {
                diffuseMaterial.Brush = Brushes.Green;
            }
            else if (material == "Acsr")
            {
                diffuseMaterial.Brush = Brushes.Coral;
            }

            return diffuseMaterial;
        }
    }
}
