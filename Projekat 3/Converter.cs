using Projekat_3.Model;
using System;
using System.Windows;
using System.Xml;

namespace Projekat_3
{
    public class Converter
    {
        //From UTM to Latitude and longitude in decimal
        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }

        public static Point ToPixels(double X, double Y)
        {
            Point returnValue = new Point();

            returnValue.X = (Y - Border.WesternBorder) / (Border.EasternBorder - Border.WesternBorder);
            returnValue.X = returnValue.X * 1175;
            returnValue.X -= 587.5;

            returnValue.Y = (X - Border.SouthernBorder) / (Border.NorthernBorder - Border.SouthernBorder);
            returnValue.Y = returnValue.Y * 775;
            returnValue.Y -= 387.5;

            return returnValue;
        }

        public static void ToSubstation(SubstationEntity substationEntity, XmlNode node)
        {
            substationEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
            substationEntity.Name = node.SelectSingleNode("Name").InnerText;
            substationEntity.X = double.Parse(node.SelectSingleNode("X").InnerText);
            substationEntity.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
            substationEntity.Connections = 0;
        }

        public static void ToNode(NodeEntity nodeEntity, XmlNode node)
        {
            nodeEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
            nodeEntity.Name = node.SelectSingleNode("Name").InnerText;
            nodeEntity.X = double.Parse(node.SelectSingleNode("X").InnerText);
            nodeEntity.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
            nodeEntity.Connections = 0;
        }

        public static void ToSwich(SwitchEntity switchEntity, XmlNode node)
        {
            switchEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
            switchEntity.Name = node.SelectSingleNode("Name").InnerText;
            switchEntity.Status = node.SelectSingleNode("Status").InnerText;
            switchEntity.X = double.Parse(node.SelectSingleNode("X").InnerText);
            switchEntity.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
            switchEntity.Connections = 0;
        }

        public static void ToLine(LineEntity lineEntity, XmlNode node)
        {
            lineEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
            lineEntity.Name = node.SelectSingleNode("Name").InnerText;
            lineEntity.IsUnderground = bool.Parse(node.SelectSingleNode("IsUnderground").InnerText);
            lineEntity.R = float.Parse(node.SelectSingleNode("R").InnerText);
            lineEntity.ConductorMaterial = node.SelectSingleNode("ConductorMaterial").InnerText;
            lineEntity.LineType = node.SelectSingleNode("LineType").InnerText;
            lineEntity.ThermalConstantHeat = long.Parse(node.SelectSingleNode("ThermalConstantHeat").InnerText);
            lineEntity.FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText);
            lineEntity.SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText);
        }

    }
}
