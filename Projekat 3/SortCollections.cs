
namespace Projekat_3
{
    public class SortCollections
    {
        public static void SortCubes()
        {
            foreach (var cvor in Collections.nodes)
            {
                if (cvor.Value.Connections >= 0 && cvor.Value.Connections <= 2)
                {
                    Collections.nodes0_2.Add(cvor.Key, cvor.Value);
                }
                else if (cvor.Value.Connections >= 3 && cvor.Value.Connections <= 5)
                {
                    Collections.nodes3_5.Add(cvor.Key, cvor.Value);
                }
                else
                {
                    Collections.nodes5_.Add(cvor.Key, cvor.Value);
                }
            }
        }

        public static void SortLines()
        {
            foreach (var l in Collections.lines)
            {
                if (l.Value.R >= 0 && l.Value.R < 1)
                {
                    Collections.lines0_1.Add(l.Key, l.Value);
                }
                else if (l.Value.R >= 1 && l.Value.R < 2)
                {
                    Collections.lines1_2.Add(l.Key, l.Value);
                }
                else if (l.Value.R >= 2)
                {
                    Collections.lines2_.Add(l.Key, l.Value);
                }
            }
        }

    }
}
