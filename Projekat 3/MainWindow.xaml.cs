using Projekat_3.Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml;
using System.Windows.Media.Animation;


namespace Projekat_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //  Marko Prodanovic PR 23/2015

        private GeometryModel3D hitgeo;
        ModelVisual3D toolTip = new ModelVisual3D();

        private Point start = new Point();
        private Point diffOffset = new Point();

        Point3D p1 = new Point3D();
        Point3D p2 = new Point3D();

        private bool kliknut_je = false;
        Point pocetna = new Point();
        Point krajnja = new Point();

        private int zoomMin = -8;
        private int zoomCurent = 1;

        DoubleAnimation rotate = new DoubleAnimation();
        private bool levo = false;
        private bool desno = false;
        private bool zuta = false;
      

        public MainWindow()
        {
            InitializeComponent();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml");

            Load load = new Load(scena);

            load.Picture();

            load.SubstationEntitys(xmlDoc);
            load.NodeEntitys(xmlDoc);
            load.SwitchEntitys(xmlDoc);
            load.LineEntitys(xmlDoc);


            SortCollections.SortCubes();
            SortCollections.SortLines();
        }

        private void viewport1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            scena.CaptureMouse();
            start = e.GetPosition(this);
            diffOffset.X = translacija.OffsetX;
            diffOffset.Y = translacija.OffsetY;

            if (zuta)
            {
                foreach (var k in Collections.cubes_models)
                {
                    ModelVisual3D model = k.Value;
                    GeometryModel3D g = (GeometryModel3D)model.Content;
                    MeshGeometry3D m = (MeshGeometry3D)g.Geometry;
                    Point3D p = m.Positions[0];

                    if ((p1.X == p.X && p1.Y == p.Y && p1.Z == p.Z) || (p2.X == p.X && p2.Y == p.Y && p2.Z == p.Z))
                    {
                        g.Material = new DiffuseMaterial(Brushes.Red);
                    }
                }
            }

            zuta = false;

            Point mouseposition = e.GetPosition(scena);
            Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);

            PointHitTestParameters pointparams = new PointHitTestParameters(mouseposition);
            RayHitTestParameters rayparams = new RayHitTestParameters(testpoint3D, testdirection);

            hitgeo = null;
            VisualTreeHelper.HitTest(scena, null, HTResult1, pointparams);
        }

        private void viewport1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            scena.ReleaseMouseCapture();
        }

        private void viewport1_MouseMove(object sender, MouseEventArgs e)
        { 
            if (kliknut_je)
            {
                krajnja = e.GetPosition(this);

                if(pocetna.X < krajnja.X)
                {
                    if (!levo)
                    {               
                        if (rotate3D.Angle == 0)
                        {
                            rotate.From = 360;
                        }
                        else
                        {
                            rotate.From = rotate3D.Angle;
                        }
                        
                        rotate.To = -3600;

                        double a = Convert.ToDouble(Math.Abs((double)rotate.To - (double)rotate.From));
                        double sekundi = 0;

                        if (a != 0)
                        {
                            double b = 360 / a;
                            sekundi = 6 / b;
                        }

                        rotate.Duration = TimeSpan.FromSeconds(sekundi);

                        rotate3D.Axis = new Vector3D(0, 0, 1);
                        rotate3D.Angle = 65;

                        rotate3D.BeginAnimation(AxisAngleRotation3D.AngleProperty, rotate);

                        levo = true;
                        desno = false;
                  
                    }
                }
                else if(pocetna.X > krajnja.X)
                {
                    if (!desno)
                    {
                        if (rotate3D.Angle == 360)
                        {
                            rotate.From = 0;
                        }
                        else
                        {
                            rotate.From = rotate3D.Angle;
                        }                   

                        rotate.To = 3600;

                        double a = Convert.ToDouble(Math.Abs((double)rotate.To - (double)rotate.From));
                        double sekundi = 0;

                        if (a != 0)
                        {
                            double b = 360 / a;
                            sekundi = 6 / b;
                        }

                        rotate.Duration = TimeSpan.FromSeconds(sekundi);
                        //rotate.RepeatBehavior = RepeatBehavior.Forever;

                        rotate3D.Axis = new Vector3D(0, 0, 1);
                        rotate3D.Angle = 65;

                        rotate3D.BeginAnimation(AxisAngleRotation3D.AngleProperty, rotate);
                        desno = true;
                        levo = false;
                    }
                }
            }

            if (scena.IsMouseCaptured)
            {
                Point end = e.GetPosition(this);

                double offsetX = start.X - end.X;
                double offsetY = start.Y - end.Y;

                double translateX = offsetX;
                double translateY = 0 - offsetY;

                translacija.OffsetX = diffOffset.X + translateX;
                translacija.OffsetY = diffOffset.Y + translateY;
            }
            else
            {
                Point mouseposition = e.GetPosition(scena);
                Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
                Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);

                PointHitTestParameters pointparams = new PointHitTestParameters(mouseposition);
                RayHitTestParameters rayparams = new RayHitTestParameters(testpoint3D, testdirection);
   
                hitgeo = null;
                VisualTreeHelper.HitTest(scena, null, HTResult, pointparams);
            }
        }

        private Point ReturnCursorCoordinates(Point p)
        {
            Point ret = new Point();

            if (p.X >= 682.5)
            {
                ret.X = (p.X - 682.5) / (1101 - 682.5);
                ret.X = ret.X * 587.5;
            }
            else
            {
                ret.X = (p.X - 264) / (682.5 - 264);
                ret.X = (ret.X * 587.5) - 587.5;
            }

            if (p.Y >= 372)
            {
                ret.Y = (p.Y - 372) / (648 - 372);
                ret.Y = 0 - (ret.Y * 387.5);
            }
            else
            {
                ret.Y = (p.Y - 96) / (372 - 96);
                ret.Y = 387.5 - (ret.Y * 387.5);
            }

            return ret;
        }

        private void viewport1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point p = e.GetPosition(this);

            Point point = ReturnCursorCoordinates(p);

            if (e.Delta < 0 && zoomCurent < 1)
            {
                zoomCurent++;

                skaliranje.ScaleX += 0.1;
                skaliranje.ScaleY += 0.1;
                skaliranje.ScaleZ += 0.1;

                skaliranje.CenterX = point.X;
                skaliranje.CenterY = point.Y;  
            }
            else if (e.Delta > 0 && zoomCurent > zoomMin)
            {       
                zoomCurent--;

                skaliranje.ScaleX -= 0.1;
                skaliranje.ScaleY -= 0.1;
                skaliranje.ScaleZ -= 0.1;

                skaliranje.CenterX = point.X;
                skaliranje.CenterY = point.Y;
            }
        }


        #region Stari Dodatak
        //private void Hide_Click(object sender, RoutedEventArgs e)
        //{
        //    string ime = Hide.Content.ToString();
        //    if (ime == "Sakrij sve objekte")
        //    {
        //        hide(0);

        //        scena.Children.Remove(result);

        //    }else if (ime == "Prikazi sve objekte")
        //    {

        //        //    Ovde dodaj proveru da li je vec dodata ta kolekcija

        //        show(0);

        //        scena.Children.Remove(result);
        //    }
        //}
        #endregion

        private HitTestResultBehavior HTResult1(System.Windows.Media.HitTestResult rawresult)
        {
            RayHitTestResult rayResult = rawresult as RayHitTestResult;

            if (rayResult != null)
            {
                bool gasit = false;

                for (int i = 0; i < Collections.lines_geometry.Count; i++)
                {
                    List<GeometryModel3D> listaGeometrija = new List<GeometryModel3D>();
                    Model3DGroup group = (Model3DGroup)Collections.lines_geometry[i];

                    foreach(var v in group.Children)
                    {
                        listaGeometrija.Add((GeometryModel3D)v);
                    }
             
                    foreach(GeometryModel3D geometrija in listaGeometrija)
                    {
                        if (geometrija == rayResult.ModelHit)
                        {
                            hitgeo = listaGeometrija[0];
                            gasit = true;
                            MeshGeometry3D mesh = (MeshGeometry3D)hitgeo.Geometry;

                            Point3D point1 = mesh.Positions[0];

                            hitgeo = listaGeometrija[listaGeometrija.Count-1];
                            mesh = (MeshGeometry3D)hitgeo.Geometry;

                            Point3D point2 = mesh.Positions[2];

                            p1 = point1;
                            p2 = point2;

                            foreach (var k in Collections.cubes_models)
                            {
                                ModelVisual3D model = k.Value;
                                GeometryModel3D g = (GeometryModel3D)model.Content;
                                MeshGeometry3D m = (MeshGeometry3D)g.Geometry;
                                Point3D p = m.Positions[0];

                                if ((point1.X == p.X && point1.Y == p.Y && point1.Z == p.Z) || (point2.X == p.X && point2.Y == p.Y && point2.Z == p.Z))
                                {
                                    g.Material = new DiffuseMaterial(Brushes.Yellow);
                                    zuta = true;
                                }
                            }
                        }
                    }                  
                }
                if (!gasit)
                {
                    hitgeo = null;            
                }
            }
            return HitTestResultBehavior.Stop;
        }

        private HitTestResultBehavior HTResult(System.Windows.Media.HitTestResult rawresult)
        {
            RayHitTestResult rayResult = rawresult as RayHitTestResult;

            if (rayResult != null)
            {
                bool gasit = false;
                scena.Children.Remove(toolTip);

                for (int i = 0; i < Collections.cubes_geometry.Count; i++)
                {
                    if ((GeometryModel3D)Collections.cubes_geometry[i] == rayResult.ModelHit)
                    {
                        toolTip = new ModelVisual3D();
                        hitgeo = (GeometryModel3D)rayResult.ModelHit;
                        gasit = true;
                        MeshGeometry3D mesh = (MeshGeometry3D)hitgeo.Geometry;
                        Point3D point = mesh.Positions[0];

                        foreach (var k in Collections.cubes_models)
                        {
                            ModelVisual3D model = k.Value;
                            GeometryModel3D g = (GeometryModel3D)model.Content;
                            MeshGeometry3D m = (MeshGeometry3D)g.Geometry;
                            Point3D p = m.Positions[0];

                            if (point.X == p.X && point.Y == p.Y && point.Z == p.Z)
                            {
                                string[] split = k.Key.Split('K');

                                PowerEntity power = new PowerEntity();

                                foreach(var pomocna in Collections.nodes)
                                {
                                    if(pomocna.Value.Id == long.Parse(split[0]))
                                    {
                                        power = pomocna.Value;
                                        break;
                                    }
                                }

                                string text;

                                if (split[2] == "Switch")
                                {
                                    SwitchEntity sw = (SwitchEntity)power;
                                    text = "Id: " + split[0] + " Name: " + split[1] + "  " + split[2] + "  " + sw.Status + "  C: " + power.Connections;
                                }
                                else
                                {
                                    text = "Id: " + split[0] + " Name: " + split[1] + "  " + split[2] + "  C: " + power.Connections;
                                }


                                TextBlock textblock = new TextBlock(new Run(text));
                                textblock.Foreground = Brushes.Orange;
                                textblock.Background = Brushes.Black;
                                textblock.FontFamily = new FontFamily("Arial");

                                DiffuseMaterial mataterialWithLabel = new DiffuseMaterial();
                                mataterialWithLabel.Brush = new VisualBrush(textblock);

                                double width = text.Length * 7;

                                Point3D p0 = new Point3D(point.X - width / 2, point.Y + 20, 180);
                                Point3D p1 = new Point3D(point.X - width / 2, point.Y + 40, 180);
                                Point3D p2 = new Point3D(point.X + width / 2, point.Y + 20, 180);
                                Point3D p3 = new Point3D(point.X + width / 2, point.Y + 40, 180);

                                MeshGeometry3D mg_RestangleIn3D = new MeshGeometry3D();
                                mg_RestangleIn3D.Positions = new Point3DCollection();
                                mg_RestangleIn3D.Positions.Add(p0);
                                mg_RestangleIn3D.Positions.Add(p1);
                                mg_RestangleIn3D.Positions.Add(p2);
                                mg_RestangleIn3D.Positions.Add(p3);

                                mg_RestangleIn3D.TriangleIndices.Add(0);
                                mg_RestangleIn3D.TriangleIndices.Add(3);
                                mg_RestangleIn3D.TriangleIndices.Add(1);
                                mg_RestangleIn3D.TriangleIndices.Add(0);
                                mg_RestangleIn3D.TriangleIndices.Add(2);
                                mg_RestangleIn3D.TriangleIndices.Add(3);

                                mg_RestangleIn3D.TextureCoordinates.Add(new Point(0, 1));
                                mg_RestangleIn3D.TextureCoordinates.Add(new Point(0, 0));
                                mg_RestangleIn3D.TextureCoordinates.Add(new Point(1, 1));
                                mg_RestangleIn3D.TextureCoordinates.Add(new Point(1, 0));

                                toolTip.Content = new GeometryModel3D(mg_RestangleIn3D, mataterialWithLabel);
                                scena.Children.Add(toolTip);
                                break;
                            }
                        }
                    }
                }
                for (int i = 0; i < Collections.lines_geometry.Count; i++)
                {
                    List<GeometryModel3D> listaGeometrija = new List<GeometryModel3D>();
                    Model3DGroup group = (Model3DGroup)Collections.lines_geometry[i];

                    foreach (var v in group.Children)
                    {
                        listaGeometrija.Add((GeometryModel3D)v);
                    }

                    foreach (GeometryModel3D geometrija in listaGeometrija)
                    { 
                        if(geometrija == rayResult.ModelHit)
                        {
                            toolTip = new ModelVisual3D();
                            hitgeo = listaGeometrija[0];
                            gasit = true;
                            MeshGeometry3D mesh = (MeshGeometry3D)hitgeo.Geometry;
                            Point3D point1 = mesh.Positions[0];

                            hitgeo = listaGeometrija[listaGeometrija.Count - 1];
                            mesh = (MeshGeometry3D)hitgeo.Geometry;

                            Point3D point2 = mesh.Positions[2];


                            string kljuc = point1.X + " " + point1.Y + " " + point1.Z + " " + point2.X + " " + point2.Y + " " + point2.Z;
                            string text = "";

                            foreach (var l in Collections.lines)
                            {
                                if (l.Key == kljuc)
                                {
                                    text = "Id: " + l.Value.Id + " Name: " + l.Value.Name + "  " + l.Value.ConductorMaterial + "  R:" + l.Value.R + "   Line";
                                    break;
                                }
                            }
                            
                            TextBlock textblock = new TextBlock(new Run(text));
                            textblock.Foreground = Brushes.Orange;
                            textblock.Background = Brushes.Black;
                            textblock.FontFamily = new FontFamily("Arial");

                            DiffuseMaterial mataterialWithLabel = new DiffuseMaterial();
                            mataterialWithLabel.Brush = new VisualBrush(textblock);

                            double width = text.Length * 7;

                            Point3D p0 = new Point3D(point1.X - width / 2, point1.Y + 20, 180);
                            Point3D p1 = new Point3D(point1.X - width / 2, point1.Y + 40, 180);
                            Point3D p2 = new Point3D(point1.X + width / 2, point1.Y + 20, 180);
                            Point3D p3 = new Point3D(point1.X + width / 2, point1.Y + 40, 180);

                            MeshGeometry3D mg_RestangleIn3D = new MeshGeometry3D();
                            mg_RestangleIn3D.Positions = new Point3DCollection();
                            mg_RestangleIn3D.Positions.Add(p0);
                            mg_RestangleIn3D.Positions.Add(p1);
                            mg_RestangleIn3D.Positions.Add(p2);
                            mg_RestangleIn3D.Positions.Add(p3);

                            mg_RestangleIn3D.TriangleIndices.Add(0);
                            mg_RestangleIn3D.TriangleIndices.Add(3);
                            mg_RestangleIn3D.TriangleIndices.Add(1);
                            mg_RestangleIn3D.TriangleIndices.Add(0);
                            mg_RestangleIn3D.TriangleIndices.Add(2);
                            mg_RestangleIn3D.TriangleIndices.Add(3);

                            mg_RestangleIn3D.TextureCoordinates.Add(new Point(0, 1));
                            mg_RestangleIn3D.TextureCoordinates.Add(new Point(0, 0));
                            mg_RestangleIn3D.TextureCoordinates.Add(new Point(1, 1));
                            mg_RestangleIn3D.TextureCoordinates.Add(new Point(1, 0));

                            toolTip.Content = new GeometryModel3D(mg_RestangleIn3D, mataterialWithLabel);
                            scena.Children.Add(toolTip);
                            break;   
                        }                               
                    }                 
                }
                if (!gasit)
                {
                    hitgeo = null;

                    scena.Children.Remove(toolTip);
                }
            }
            return HitTestResultBehavior.Stop;
        }

        private void viewport1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                kliknut_je = true;
                pocetna = e.GetPosition(this);   
            }
        }

        private void viewport1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (kliknut_je)
            {
                kliknut_je = false;
                levo = false;
                desno = false;
                
                rotate.From = rotate3D.Angle;
                rotate.To = rotate3D.Angle;
                rotate.Duration = TimeSpan.FromSeconds(0.01);

                //rotate3D.Axis = new Vector3D(500, Math.Cos(65), Math.Sin(65));
                rotate3D.Axis = new Vector3D(0, 0, 1);
                rotate3D.Angle = 65;

                rotate3D.BeginAnimation(AxisAngleRotation3D.AngleProperty, rotate);
            }
        }

        
        

        private void dodatak1_Opcija1(object sender, RoutedEventArgs e)
        {
            if (d1o1.Header.ToString().Contains("Hide"))
            {
                hide_elements(1);
            }
            else
            {
                show_elements(1);
            }
        }

        private void dodatak1_Opcija2(object sender, RoutedEventArgs e)
        {
            if (d1o2.Header.ToString().Contains("Hide"))
            {
                hide_elements(2);
            }
            else
            {
                show_elements(2);
            }
        }

        private void dodatak1_Opcija3(object sender, RoutedEventArgs e)
        {
            if (d1o3.Header.ToString().Contains("Hide"))
            {
                hide_elements(3);
            }
            else
            {
                show_elements(3);
            }
        }

        private void hide_elements(int broj)  
        {
            if (broj == 1)
            {
                foreach (var cvor in Collections.nodes0_2)
                {
                    string kljuc1 = cvor.Key + "K" + cvor.Value.Name + "K";

                    foreach (var k in Collections.cubes_models)
                    {
                        string kljuc2 = k.Key.ToString();
                        if (kljuc2.Contains(kljuc1))
                        {
                            scena.Children.Remove(k.Value);
                        }
                    }
                }
                d1o1.Header = "Show objects with 0 to 2 connections";
            }
            else if (broj == 2)
            {
                foreach (var cvor in Collections.nodes3_5)
                {
                    string kljuc1 = cvor.Key + "K" + cvor.Value.Name + "K";

                    foreach (var k in Collections.cubes_models)
                    {
                        string kljuc2 = k.Key.ToString();
                        if (kljuc2.Contains(kljuc1))
                        {
                            scena.Children.Remove(k.Value);
                        }
                    }
                }
                d1o2.Header = "Show objects with 3 to 5 connections";
            }
            else if (broj == 3)
            {
                foreach (var cvor in Collections.nodes5_)
                {
                    string kljuc1 = cvor.Key + "K" + cvor.Value.Name + "K";

                    foreach (var k in Collections.cubes_models)
                    {
                        string kljuc2 = k.Key.ToString();
                        if (kljuc2.Contains(kljuc1))
                        {
                            scena.Children.Remove(k.Value);
                        }
                    }
                }
                d1o3.Header = "Show objects with more than 5 connections";
            }
        }

        private void show_elements(int broj)
        {
            if (broj == 1)
            {
                foreach (var cvor in Collections.nodes0_2)
                {
                    string kljuc1 = cvor.Key + "K" + cvor.Value.Name + "K";

                    foreach (var k in Collections.cubes_models)
                    {
                        string kljuc2 = k.Key.ToString();
                        if (kljuc2.Contains(kljuc1))
                        {
                            scena.Children.Add(k.Value);
                        }
                    }
                    d1o1.Header = "Hide objects with 0 to 2 connections";
                }
            }
            else if (broj == 2)
            {
                foreach (var cvor in Collections.nodes3_5)
                {
                    string kljuc1 = cvor.Key + "K" + cvor.Value.Name + "K";

                    foreach (var k in Collections.cubes_models)
                    {
                        string kljuc2 = k.Key.ToString();
                        if (kljuc2.Contains(kljuc1))
                        {
                            scena.Children.Add(k.Value);
                        }
                    }
                }
                d1o2.Header = "Hide objects with 3 to 5 connections";
            }
            else if (broj == 3)
            {
                foreach (var cvor in Collections.nodes5_)
                {
                    string kljuc1 = cvor.Key + "K" + cvor.Value.Name + "K";

                    foreach (var k in Collections.cubes_models)
                    {
                        string kljuc2 = k.Key.ToString();
                        if (kljuc2.Contains(kljuc1))
                        {
                            scena.Children.Add(k.Value);
                        }
                    }
                }
                d1o3.Header = "Hide objects with more than 5 connections";
            }
        }


        

        private void dodatak2_Opcija1(object sender, RoutedEventArgs e)
        {
            if (d2o1.Header.ToString().Contains("Hide"))
            {
                hide_lines(1);
            }
            else
            {
                show_lines(1);

                if (d3o1.Header.ToString().Contains("Show"))
                {
                    hide_lines(4);
                }
            }
        }

        private void dodatak2_Opcija2(object sender, RoutedEventArgs e)
        {
            if (d2o2.Header.ToString().Contains("Hide"))
            {
                hide_lines(2);
            }
            else
            {
                show_lines(2);

                if (d3o1.Header.ToString().Contains("Show"))
                {
                    hide_lines(4);
                }
            }
        }

        private void dodatak2_Opcija3(object sender, RoutedEventArgs e)
        {    
            if (d2o3.Header.ToString().Contains("Hide"))
            {
                hide_lines(3);
            }
            else
            {
                show_lines(3);

                if (d3o1.Header.ToString().Contains("Show"))
                {
                    hide_lines(4);
                }
            }   
        }

        private void hide_lines(int broj)
        {
            if (broj == 1)
            {
                foreach(var l in Collections.lines0_1)
                {
                    foreach(var t in Collections.lines_models)
                    {
                        if(t.Key == l.Key)
                        {
                            scena.Children.Remove(t.Value);
                        }
                    }
                }
                d2o1.Header = "Show lines with resistance from 0 to 1";
            }
            else if (broj == 2)
            {
                foreach (var l in Collections.lines1_2)
                {
                    foreach (var t in Collections.lines_models)
                    {
                        if (t.Key == l.Key)
                        {
                            scena.Children.Remove(t.Value);
                        }
                    }
                }
                d2o2.Header = "Show lines with resistance from 1 to 2";
            }
            else if (broj == 3)
            {
                foreach (var l in Collections.lines2_)
                {
                    foreach (var t in Collections.lines_models)
                    {
                        if (t.Key == l.Key)
                        {
                            scena.Children.Remove(t.Value);
                        }
                    }
                }
                d2o3.Header = "Show lines with resistance greater than 2";
            }
            else if(broj == 4)
            {
                foreach (var l in Collections.active_lines)
                {
                    foreach (var t in Collections.lines_models)
                    {
                        if (t.Key == l.Key)
                        {
                            scena.Children.Remove(t.Value);
                        }
                    }
                }
                d3o1.Header = "Show active part of the network";
            }
        }

        private void show_lines(int broj)
        {
            if (broj == 1)
            {
                foreach (var l in Collections.lines0_1)
                {
                    foreach (var t in Collections.lines_models)
                    {
                        if (t.Key == l.Key)
                        {
                            scena.Children.Add(t.Value);
                        }
                    }
                }
                d2o1.Header = "Hide lines with resistance from 0 to 1";
            }
            else if (broj == 2)
            {
                foreach (var l in Collections.lines1_2)
                {
                    foreach (var t in Collections.lines_models)
                    {
                        if (t.Key == l.Key)
                        {
                            scena.Children.Add(t.Value);
                        }
                    }
                }
                d2o2.Header = "Hide lines with resistance from 1 to 2";
            }
            else if (broj == 3)
            {
                foreach (var l in Collections.lines2_)
                {
                    foreach (var t in Collections.lines_models)
                    {
                        if (t.Key == l.Key)
                        {
                            scena.Children.Add(t.Value);
                        }
                    }
                }
                d2o3.Header = "Hide lines with resistance greater than 2";
            }
            else if (broj == 4)
            {
                foreach (var l in Collections.active_lines)
                {
                    foreach (var t in Collections.lines_models)
                    {
                        if (t.Key == l.Key)
                        {
                            scena.Children.Add(t.Value);
                        }
                    }
                }
                d3o1.Header = "Hide active part of the network";
            }
        }

       

        private void dodatak3_Opcija1(object sender, RoutedEventArgs e)
        {
            if (d3o1.Header.ToString().Contains("Hide"))
            {
                hide_lines(4);            
            }
            else
            {
                show_lines(4);

                if (d2o1.Header.ToString().Contains("Show"))     
                {
                    hide_lines(1);
                }
                if (d2o2.Header.ToString().Contains("Show"))
                {
                    hide_lines(2);
                }
                if (d2o3.Header.ToString().Contains("Show"))
                {
                    hide_lines(3);
                }
            }
        }
    }
}
