using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVRP_SOLVER.CODE
{
    /// <summary>
    /// Represents a route in the solution.
    /// Each routes has:
    /// 1 - List of 'Customers' it serves (the depot is always both the first and last customer is each route)
    /// 2 - its own ID
    /// 3 - total_demand of all the 'Costumer'/s its serves
    /// 4 - the cost of the route ( in our solution it represents the length of the edges between each 2 consecutive 'Customers'(nodes) )
    /// </summary>
    public class Route
    {
        private List<Costumer> nodes;
        private int id;
        private double total_demand, cost;


        public Route(int id)
        {
            this.id = id;
            this.Nodes = new List<Costumer>();
            total_demand = 0;
            cost = 0;
        }



        /// <summary>
        /// Adds a new customer to the end of the customers list.
        /// </summary>
        public void add_Node(Costumer toAdd)
        {

            nodes.Add(toAdd);
            calc_Cost();
            calc_demand();
        }


        /// <summary>
        /// Removes a customer to the end of the customers list.
        /// </summary>
        public void remove_Node(Costumer toRemove)
        {
            nodes.Remove(toRemove);
            calc_Cost();
            calc_demand();
        }

        /// <summary>
        /// inserts a customer to the end of the customers list
        /// and calculates the new cost and demand
        /// </summary>
        public bool insert_Node(int index, Costumer toAdd)
        {
            if (index > Nodes.Count || index < 0)
                return false;
            nodes.Insert(index, toAdd);
            calc_Cost();
            calc_demand();
            return true;
        }

        public List<Costumer> Nodes
        {
            get
            {
                return nodes;
            }

            set
            {
                nodes = value;
            }
        }

        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }


        public override string ToString()
        {
            string result = "\nRoute " + id + ":";

            foreach (Costumer node in Nodes)
            {
                result += "\n   " + node;
            }
            if (Nodes.Count() > 0)
                result += "\n Total Demand : " + total_demand + "\n Cost : " + cost;

            return result;
        }



        /// <summary>
        /// Returns and sets the total demand of the route.
        /// </summary>
        public double get_demand()
        {

            return total_demand;
        }


        /// <summary>
        /// Returns and sets the cost of the route.
        /// </summary>
        public double get_cost()
        {

            return cost;
        }

        /// <summary>
        /// calculates the cost of the route.
        /// </summary>
        public double calc_Cost()
        {
            if (Nodes.Count == 0)
                return 0;
            cost = 0;
            Costumer prevNode = Nodes[0];
            foreach (Costumer node in Nodes)
            {
                cost += node.distance(prevNode);
                prevNode = node;

            }

            return cost;
        }

        /// <summary>
        /// calculates the demand of the route.
        /// </summary>
        public double calc_demand()
        {
            total_demand = 0;
            foreach (Costumer node in Nodes)
            {
                total_demand += node.Demand;
            }

            return total_demand;
        }

        /// <summary>
        /// switches the list of customers with a whole new one
        /// and calculates the new cost and demand of the route.
        /// </summary>
        public void update_route(List<Costumer> new_list)
        {
            this.Nodes.Clear();
            Nodes.AddRange(new_list);
            calc_Cost();
            calc_demand();
        }

        public void update_route(Route new_route)
        {
            Nodes = new_route.Nodes;
            calc_Cost();
            calc_demand();
        }
    }
}
