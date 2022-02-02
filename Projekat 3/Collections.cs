using Projekat_3.Model;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Projekat_3
{
    public class Collections
    {

        public static Dictionary<string, ModelVisual3D> cubes_models = new Dictionary<string, ModelVisual3D>();
        public static Dictionary<string, ModelVisual3D> lines_models = new Dictionary<string, ModelVisual3D>();

        public static Dictionary<long, PowerEntity> nodes = new Dictionary<long, PowerEntity>();
        public static Dictionary<string, LineEntity> lines = new Dictionary<string, LineEntity>();

        public static ArrayList cubes_geometry = new ArrayList();
        public static ArrayList lines_geometry = new ArrayList();

        public static Dictionary<long, PowerEntity> nodes0_2 = new Dictionary<long, PowerEntity>();
        public static Dictionary<long, PowerEntity> nodes3_5 = new Dictionary<long, PowerEntity>();
        public static Dictionary<long, PowerEntity> nodes5_ = new Dictionary<long, PowerEntity>();

        public static Dictionary<string, LineEntity> lines0_1 = new Dictionary<string, LineEntity>();
        public static Dictionary<string, LineEntity> lines1_2 = new Dictionary<string, LineEntity>();
        public static Dictionary<string, LineEntity> lines2_ = new Dictionary<string, LineEntity>();

        public static Dictionary<string, LineEntity> active_lines = new Dictionary<string, LineEntity>();


    }
}
