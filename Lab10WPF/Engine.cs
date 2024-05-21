using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace lab10
{
    public class Engine : IComparable
    {
        public double Displacement { get; set; }
        public double HorsePower { get; set; }

        [XmlAttribute]
        public string Model { get; set; }

        public Engine()
        {
            Displacement = 0.0;
            HorsePower = 0.0;
            Model = "";
        }

        public Engine(double displacement, double horsePower, string model)
        {
            Displacement = displacement;
            HorsePower = horsePower;
            Model = model;
        }

        public int CompareTo(object other)
        {
            if (other == null) return 1;
            if (!(other is Engine)) throw new ArgumentException("Object is not an Engine");

            Engine otherEngine = (Engine)other;
            return this.HorsePower.CompareTo(otherEngine.HorsePower);
        }


        public override string ToString()
        {
            return string.Format("{0} L {1} HP {2}", Displacement, HorsePower, Model);
        }
    }
}