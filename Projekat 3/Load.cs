using Projekat_3.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Xml;

namespace Projekat_3
{
    public class Load
    {
        Viewport3D scena;

        public Load(Viewport3D _scena)
        {
            scena = _scena;
        }

        public void Picture()
        {
            ModelVisual3D myModel = new ModelVisual3D();
            Model3DGroup group = new Model3DGroup();
            AmbientLight light = new AmbientLight() { };

            group.Children.Add(light);
            group.Children.Add(Generate3DGeometry.CreatePictureGeometry());

            myModel.Content = group;
            scena.Children.Add(myModel);
        }

        public void SubstationEntitys(XmlDocument xmlDoc)
        {
            XmlNodeList nodeList;
            double newX, newY;

            nodeList = xmlDoc.DocumentElement.SelectNodes($"/NetworkModel/Substations/SubstationEntity");
            foreach (XmlNode node in nodeList)
            {
                SubstationEntity substationEntity = new SubstationEntity();

                Converter.ToSubstation(substationEntity, node);

                Converter.ToLatLon(substationEntity.X, substationEntity.Y, 34, out newX, out newY);
                substationEntity.X = newX;
                substationEntity.Y = newY;

                if (substationEntity.X <= Border.NorthernBorder && substationEntity.X >= Border.SouthernBorder && substationEntity.Y >= Border.WesternBorder && substationEntity.Y <= Border.EasternBorder)
                {
                    Point pixelsPoint = Converter.ToPixels(substationEntity.X, substationEntity.Y);
                    substationEntity.X = pixelsPoint.X;
                    substationEntity.Y = pixelsPoint.Y;

                    StringHelper.CutString(substationEntity);

                    Generate3DGeometry.CreateCube(substationEntity, "substation", scena);
                    Collections.nodes.Add(substationEntity.Id, substationEntity);
                }
            }
        }

        public void NodeEntitys(XmlDocument xmlDoc)
        {
            XmlNodeList nodeList;
            double newX, newY;

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            foreach (XmlNode node in nodeList)
            {
                NodeEntity nodeEntity = new NodeEntity();

                Converter.ToNode(nodeEntity, node);

                Converter.ToLatLon(nodeEntity.X, nodeEntity.Y, 34, out newX, out newY);
                nodeEntity.X = newX;
                nodeEntity.Y = newY;

                if (nodeEntity.X < Border.NorthernBorder && nodeEntity.X > Border.SouthernBorder && nodeEntity.Y > Border.WesternBorder && nodeEntity.Y < Border.EasternBorder)
                {
                    Point pixelsPoint = Converter.ToPixels(nodeEntity.X, nodeEntity.Y);
                    nodeEntity.X = pixelsPoint.X;
                    nodeEntity.Y = pixelsPoint.Y;

                    StringHelper.CutString(nodeEntity);

                    Generate3DGeometry.CreateCube(nodeEntity, "node", scena);
                    Collections.nodes.Add(nodeEntity.Id, nodeEntity);
                }
            }
        }

        public void SwitchEntitys(XmlDocument xmlDoc)
        {
            XmlNodeList nodeList;
            double newX, newY;

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            foreach (XmlNode node in nodeList)
            {
                SwitchEntity switchEntity = new SwitchEntity();

                Converter.ToSwich(switchEntity, node);

                Converter.ToLatLon(switchEntity.X, switchEntity.Y, 34, out newX, out newY);
                switchEntity.X = newX;
                switchEntity.Y = newY;

                if (switchEntity.X < Border.NorthernBorder && switchEntity.X > Border.SouthernBorder && switchEntity.Y > Border.WesternBorder && switchEntity.Y < Border.EasternBorder)
                {

                    Point pixelsPoint = Converter.ToPixels(switchEntity.X, switchEntity.Y);
                    switchEntity.X = pixelsPoint.X;
                    switchEntity.Y = pixelsPoint.Y;

                    StringHelper.CutString(switchEntity);

                    Generate3DGeometry.CreateCube(switchEntity, "switch", scena);
                    Collections.nodes.Add(switchEntity.Id, switchEntity);
                }
            }
        }

        public void LineEntitys(XmlDocument xmlDoc)
        {
            XmlNodeList lineList;

            lineList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
            foreach (XmlNode node in lineList)
            {
                LineEntity lineEntity = new LineEntity();

                Converter.ToLine(lineEntity, node);

                LoadVertices(lineEntity, node);

                if (!LineAlreadyExists(lineEntity))
                {
                    bool first = false;
                    bool second = false;

                    Point3D prva = new Point3D();
                    Point3D druga = new Point3D();

                    foreach (var pon in Collections.nodes)
                    {
                        if (pon.Key == lineEntity.FirstEnd)
                        {
                            first = true;
                            prva.X = pon.Value.X;
                            prva.Y = pon.Value.Y;
                            prva.Z = pon.Value.Z;
                        }

                        if (pon.Key == lineEntity.SecondEnd)
                        {
                            second = true;
                            druga.X = pon.Value.X;
                            druga.Y = pon.Value.Y;
                            druga.Z = pon.Value.Z;
                        }

                        if (first && second)
                        {
                            string kljuc = prva.X + " " + prva.Y + " " + prva.Z + " " + druga.X + " " + druga.Y + " " + druga.Z; 
                            Collections.lines.Add(kljuc, lineEntity);
       
                            bool connentionFirst = false;
                            bool connectionSecond = false;

                            foreach (var cvor in Collections.nodes)
                            {
                                if (cvor.Key == lineEntity.FirstEnd)
                                {
                                    cvor.Value.Connections++;
                                    connentionFirst = true;

                                    if (cvor.Value.ToString().Contains("Switch"))
                                    {
                                        SwitchEntity switchEntity = (SwitchEntity)cvor.Value;

                                        if (switchEntity.Status.Equals("Open"))
                                        {
                                            if (!Collections.active_lines.ContainsKey(kljuc))
                                            {
                                                Collections.active_lines.Add(kljuc, lineEntity);
                                            }
                                        }
                                    }
                                }
                                if (cvor.Key == lineEntity.SecondEnd)
                                {
                                    cvor.Value.Connections++;
                                    connectionSecond = true;

                                    if (cvor.Value.ToString().Contains("Switch"))
                                    {
                                        SwitchEntity switchEntity = (SwitchEntity)cvor.Value;

                                        if (switchEntity.Status.Equals("Open"))
                                        {
                                            if (!Collections.active_lines.ContainsKey(kljuc))
                                            {
                                                Collections.active_lines.Add(kljuc, lineEntity);
                                            }
                                        }
                                    }
                                }
                                if (connentionFirst && connectionSecond)
                                {
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            Generate3DGeometry.CreateLine(scena);
        }

        private void LoadVertices(LineEntity lineEntity, XmlNode node)
        {
            foreach (XmlNode n in node.SelectNodes("Vertices/Point"))
            {
                Point verticesPoint = new Point();
                verticesPoint.X = double.Parse(n.SelectSingleNode("X").InnerText);
                verticesPoint.Y = double.Parse(n.SelectSingleNode("Y").InnerText);

                double newX, newY;
                Converter.ToLatLon(verticesPoint.X, verticesPoint.Y, 34, out newX, out newY);

                verticesPoint.X = newX;
                verticesPoint.Y = newY;

                lineEntity.Vertices.Add(verticesPoint);
            }
        }

        private bool LineAlreadyExists(LineEntity lineEntity)
        {
            foreach (var l in Collections.lines)
            {
                if ((lineEntity.FirstEnd == l.Value.FirstEnd && lineEntity.SecondEnd == l.Value.SecondEnd) || (lineEntity.FirstEnd == l.Value.SecondEnd && lineEntity.SecondEnd == l.Value.FirstEnd))
                {
                    return true;
                }
            }
            return false;
        }
    }
}