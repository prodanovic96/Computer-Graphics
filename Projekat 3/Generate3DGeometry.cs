using Projekat_3.Model;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Projekat_3
{
    public class Generate3DGeometry
    {

        private static int cubeSize = 10;

        public static GeometryModel3D CreatePictureGeometry()
        {
            GeometryModel3D geometry = new GeometryModel3D();
            MeshGeometry3D mesh = new MeshGeometry3D();

            Point3DCollection point = new Point3DCollection();

            point.Add(new Point3D(-587.5, -387.5, 0));
            point.Add(new Point3D(-587.5, 387.5, 0));
            point.Add(new Point3D(587.5, -387.5, 0));
            point.Add(new Point3D(587.5, 387.5, 0));
            mesh.Positions = point;

            Int32Collection collenton = new Int32Collection();
            collenton.Add(0);
            collenton.Add(2);
            collenton.Add(1);
            collenton.Add(1);
            collenton.Add(2);
            collenton.Add(3);
            mesh.TriangleIndices = collenton;

            mesh.TextureCoordinates.Add(new Point(0, 1));
            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(1, 0));

            geometry.Geometry = mesh;

            var path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path += @"\PZ3-map.jpg";

            ImageBrush imgBrush = new ImageBrush(new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute)));

            DiffuseMaterial difuse = new DiffuseMaterial(imgBrush);
            geometry.Material = difuse;

            return geometry;
        }

        public static void CreateCube(PowerEntity powerEntity, string type, Viewport3D scena)
        {

            ModelVisual3D model = new ModelVisual3D();
            GeometryModel3D geometry = new GeometryModel3D();
            
            double x, y;
            int z = 0;

            if (findFreeZ(powerEntity.X, powerEntity.Y, z))
            {
                do
                {
                    z += 15;

                } while (findFreeZ(powerEntity.X, powerEntity.Y, z));
            }

            string kljuc;
            if (type == "substation")
            {
                SubstationEntity substationEntity = (SubstationEntity)powerEntity;
                x = substationEntity.X;
                y = substationEntity.Y;
                substationEntity.Z = z;
                kljuc = substationEntity.Id + "K" + substationEntity.Name + "K" + "Substation";
            }
            else if (type == "node")
            {
                NodeEntity nodeEntity = (NodeEntity)powerEntity;
                x = nodeEntity.X;
                y = nodeEntity.Y;
                nodeEntity.Z = z;
                kljuc = nodeEntity.Id + "K" + nodeEntity.Name + "K" + "Node";
            }
            else
            {
                SwitchEntity switchEntity = (SwitchEntity)powerEntity;
                x = switchEntity.X;
                y = switchEntity.Y;
                switchEntity.Z = z;
                kljuc = switchEntity.Id + "K" + switchEntity.Name + "K" + "Switch";
            }


            DiffuseMaterial difuse = new DiffuseMaterial();
            difuse.Brush = Brushes.Red;

            geometry.Material = difuse;
            geometry.Geometry = CreateMesh.Cube(x,y,z,cubeSize);
            model.Content = geometry;

            scena.Children.Add(model);

            Collections.cubes_geometry.Add(geometry);
            Collections.cubes_models.Add(kljuc, model);
        }

        public static void CreateLine(Viewport3D scena)
        {
            foreach (var lin in Collections.lines)
            {
                ModelVisual3D model = new ModelVisual3D();
                Model3DGroup group = new Model3DGroup();

                GeometryModel3D geometry;
                MeshGeometry3D mesh;

                string[] split = lin.Key.Split(' ');

                Point3D firstEnd = new Point3D();
                Point3D secondEnd = new Point3D();

                firstEnd.X = double.Parse(split[0]);
                firstEnd.Y = double.Parse(split[1]);
                firstEnd.Z = Int32.Parse(split[2]);

                secondEnd.X = double.Parse(split[3]);
                secondEnd.Y = double.Parse(split[4]);
                secondEnd.Z = Int32.Parse(split[5]);
                

                DiffuseMaterial difuse = CreateDiffuse.Line(lin.Value.ConductorMaterial);

                Point3D pomocna1 = firstEnd;
                Point pomocna2;

                foreach (Point p in lin.Value.Vertices)
                {
                    mesh = CreateMesh.Line();
                    geometry = new GeometryModel3D();

                    mesh.Positions.Add(pomocna1);
                    mesh.Positions.Add(new Point3D(pomocna1.X + 5, pomocna1.Y, pomocna1.Z));

                    pomocna2 = Converter.ToPixels(p.X, p.Y);

                    mesh.Positions.Add(new Point3D(pomocna2.X, pomocna2.Y, 5));
                    mesh.Positions.Add(new Point3D(pomocna2.X + 5, pomocna2.Y, 5));

                    geometry.Material = difuse;
                    geometry.Geometry = mesh;

                    group.Children.Add(geometry);

                    pomocna1.X = pomocna2.X;
                    pomocna1.Y = pomocna2.Y;
                    pomocna1.Z = 5;
                }

                mesh = CreateMesh.Line();
                geometry = new GeometryModel3D();

                mesh.Positions.Add(pomocna1);
                mesh.Positions.Add(new Point3D(pomocna1.X + 5, pomocna1.Y, pomocna1.Z));

                mesh.Positions.Add(new Point3D(secondEnd.X, secondEnd.Y, secondEnd.Z));
                mesh.Positions.Add(new Point3D(secondEnd.X + 5, secondEnd.Y, secondEnd.Z));

                geometry.Material = difuse;
                geometry.Geometry = mesh;

                group.Children.Add(geometry);

                model.Content = group;

                Collections.lines_models.Add(lin.Key, model);
                scena.Children.Add(model);
                Collections.lines_geometry.Add(group);
            }
        }

        public static bool findFreeZ(double x, double y, int z)
        {
            foreach (var k in Collections.cubes_models)
            {
                GeometryModel3D geometry = (GeometryModel3D)k.Value.Content;
                MeshGeometry3D mesh = (MeshGeometry3D)geometry.Geometry;

                Point3D point = mesh.Positions[0];

                if (point.Z == z)
                {
                    if (point.X == x && point.Y == y && point.Z == z)
                    {
                        return true;
                    }

                    if (overlap(x, y, z, point))
                    {
                        return true;
                    }
                    if (overlap(x + cubeSize, y, z, point))
                    {
                        return true;
                    }
                    if (overlap(x, y + cubeSize, z, point))
                    {
                        return true;
                    }
                    if (overlap(x + cubeSize, y + cubeSize, z, point))
                    {
                        return true;
                    }
                    if (overlap(x, y, z + cubeSize, point))
                    {
                        return true;
                    }
                    if (overlap(x + cubeSize, y, z + cubeSize, point))
                    {
                        return true;
                    }
                    if (overlap(x, y + cubeSize, z + cubeSize, point))
                    {
                        return true;
                    }
                    if (overlap(x + cubeSize, y + cubeSize, z + cubeSize, point))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool overlap(double X, double Y, int z, Point3D point)
        {
            if (X >= point.X && X <= (point.X + cubeSize) && Y >= point.Y && Y <= (point.Y + cubeSize) && z >= point.Z && z <= (point.Z + cubeSize))
            {
                return true;
            }
            return false;
        }
    }
}
