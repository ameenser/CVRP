using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVRP_SOLVER.CODE
{
    /// <summary>
    /// Represents a customr/Node is the graph
    /// </summary>
    public class Costumer : IComparable, IEquatable<Costumer>
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Demand { get; set; }
        public int ID { get; set; }


        /// <summary>
        /// Each Costumer has:
        ///  1 - ID (should be unique)
        ///  2 - (X,Y) coordinates
        ///  3 - demand 
        /// </summary>
        public Costumer(int id, double x, double y, double demand)
        {
            this.X = x;
            this.Y = y;
            this.Demand = demand;
            this.ID = id;
        }


        /// <summary>
        /// Returnsthis distance matrix between all costomers(nodes) including the depot.
        /// </summary>
        public static double[,] calc_distance_matrix(Costumer[] costumers)
        {
            double[,] distance_matrix = new double[costumers.Length, costumers.Length];

            for (int i = 0; i < costumers.Length; i++)
            {
                for (int j = 0; j < costumers.Length; j++)
                {
                    distance_matrix[i, j] = costumers[i].distance(costumers[j]);
                }
            }

            return distance_matrix;

        }
        /// <summary>
        /// Returns the distance between the current costomer(node) and other customer
        /// </summary>
        public double distance(Costumer other)
        {
            double distance = 0;
            distance = Math.Sqrt(Math.Pow(this.X - other.X, 2) + Math.Pow(this.Y - other.Y, 2));
            return distance;
        }

        public int CompareTo(object obj)
        {
            try
            {
                Costumer other = (Costumer)obj;

                return (this == other) ? 1 : 0;
            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine(ex.GetType());
                throw new InvalidCastException("Cannot Cast non 'Costumer' object to 'Costomer'");
            }
        }

        public bool Equals(Costumer other)
        {
            if (this.ID == other.ID)
                if (this.X == other.X && this.Y == other.Y && this.Demand == other.Demand)
                    return true;
            return false;
        }

        public static bool operator ==(Costumer costumer, Costumer other)
        {
            if (costumer.ID == other.ID)
                return true;
            return false;
        }
        public static bool operator !=(Costumer costumer, Costumer other)
        {
            return !(costumer == other);
        }



        public override string ToString()
        {
            string typeOfCustomer = ID == 0 ? "Depot " : "Customer " + ID;
            return typeOfCustomer + " : (" + X + "," + Y + ")    Demand : " + Demand;
        }

    }

}
