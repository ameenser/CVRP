using CVRP_SOLVER;
using MA_EAX_CVRP_SOLVER.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApplication2;
namespace CVRP_SOLVER.CODE
{
    /// <summary>
    /// Represents the graph.
    /// Each Graph has :
    /// 1 - list of all customers
    /// 2 - vehicle capacity
    /// 3 - total demand
    /// </summary>
    public class Graph   // class that reads a parses info from file
    {
        public List<Costumer> nodes { get; set; }
        public int vehicle_capacity { get; set; }
        double total_demand, cost = 0;



        public void add_Node(Costumer toAdd)
        {
            nodes.Add(toAdd);
        }

        public Graph()
        {
            this.nodes = new List<Costumer>();
        }


        /// <summary>
        /// Parses the info about the graph from the file.
        /// </summary>
        public bool parse_file(string filePath, int vehicle_Cap,MainForm form)
        {
            if (!filePath.EndsWith(".txt"))
            {
                form.sendWrongFileFormat();
                return false;
            }
            string[] lines = File.ReadAllLines(filePath);

            var commentLine = lines[2].Split(',');
            if (vehicle_Cap == 0)
            {
                try
                {
                 vehicle_capacity = int.Parse(commentLine[1]);
                }
                catch(Exception e)
                {
                    form.sendWrongFileFormat();
                    return false;
                }
            }
            else if(vehicle_Cap > 0)
            {
                vehicle_capacity = vehicle_Cap;
            }
            else
            {
                return false;
            }

            for (int i = 4; i < lines.Length; i++)
            {
                string clean_line = "";

                foreach (char c in lines[i])
                {
                    if (!char.IsWhiteSpace(c))
                        clean_line += c;
                }
                var words = lines[i].Split(',');
                try
                {
                    Costumer new_node = new Costumer(int.Parse(words[0].Trim()), double.Parse(words[1].Trim()), double.Parse(words[2].Trim()), double.Parse(words[3].Trim()));
                    nodes.Add(new_node);

                }
                catch(Exception e)
                {
                    form.sendWrongFileFormat();
                    return false;
                }
            }
            return true;
        }

        public List<Costumer> getNodes()
        {
            return nodes;
        }


        /// <summary>
        /// Prints the data of the graph.
        /// </summary>
        public void Print()
        {
            Console.WriteLine("[Vehicle capacity : " + vehicle_capacity + "]");
            int i = 0;
            foreach (Costumer node in nodes)
            {
                Console.WriteLine(node);
            }
        }

        /// <summary>
        /// Returns the total demand of all the customers.
        /// </summary>
        public double get_total_demand()
        {
            total_demand = 0;
            foreach (Costumer node in nodes)
            {
                total_demand += node.Demand;
            }
            return total_demand;

        }

        /// <summary>
        /// Create a random solution for the given CVRP instance
        /// </summary>
        public Solution random_solver() // create a random solution
        {
            Console.WriteLine("CVRP_ Random_solution()");
            int numOfVehiles = ((int)(get_total_demand())) / vehicle_capacity + 1;

            Solution solution = new Solution(numOfVehiles, vehicle_capacity, this);

            Route[] routes = solution.Routes;
            Random rnd = new Random();
            int[] visitedNodes = new int[nodes.Count];

            double[,] distance_matrix = Costumer.calc_distance_matrix(nodes.ToArray());

            for (int t = 0; t < visitedNodes.Length; t++) visitedNodes[t] = 0;

            int served_customers = 0, flag = 0;
            cost = 0;
            for (int i = 0; i < routes.Length; i++)
            {

                double comulativeDemand = 0;
                routes[i] = new Route(i);
                routes[i].add_Node(nodes[0]);  // the first node of each route is the depot
                int nextRow = 0;
               
                while (comulativeDemand < vehicle_capacity)
                {

                    if (flag == 0)
                    {
                      
                        using (System.Security.Cryptography.RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
                        {
                            byte[] randomBytes = new byte[sizeof(int)];
                            crypto.GetBytes(randomBytes);
                            int unique = Math.Abs((int)BitConverter.ToUInt32(randomBytes, 0) % nodes.Count) % nodes.Count;
                            while (visitedNodes[unique] == 1)
                            {
                                crypto.GetBytes(randomBytes);
                                unique = Math.Abs((int)BitConverter.ToUInt32(randomBytes, 0) % nodes.Count);
                            }
                            nextRow = unique;
                        }
                        flag = 1;

                    }

                    double[] dist_arr = distance_matrix.GetRow(nextRow);

                    int[] index_arr = new int[dist_arr.Length];
                    for (int k = 0; k < dist_arr.Length; k++)
                    {
                        index_arr[k] = k;
                    }

                    for (int k = 1; k < dist_arr.Length; k++) // sort the dist arr of one node to all others
                    {
                        for (int j = 0; j < k; j++)
                        {
                            if (dist_arr[j + 1] < dist_arr[j])
                            {
                                double temp = dist_arr[j + 1];
                                dist_arr[j + 1] = dist_arr[j];
                                dist_arr[j] = temp;
                                int temp_index = index_arr[j + 1];
                                index_arr[j + 1] = index_arr[j];
                                index_arr[j] = temp_index;

                            }
                        }
                    }

             
                    int index = 0;

                    try
                    {
                        while (visitedNodes[index_arr[index]] == 1 || comulativeDemand + nodes[index_arr[index]].Demand > vehicle_capacity || dist_arr[index] == 0 || index_arr[index] == 0)
                        {
                            index++;
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        if (index >= visitedNodes.Length)
                            break;
                    }

                    if (visitedNodes[index_arr[index]] == 1 || dist_arr[index] == 0 || index_arr[index] == 0)
                        Console.WriteLine("Error 404 ");

                    // add node to the route if it does not violet the restrictions
                    visitedNodes[index_arr[index]] = 1;
                    routes[i].add_Node(nodes[index_arr[index]]);
                    comulativeDemand += nodes[index_arr[index]].Demand;
                    nextRow = index;
                    served_customers++;

                    if (served_customers == visitedNodes.Length - 1) break;


                }
                routes[i].add_Node(nodes[0]);
                cost += routes[i].get_cost();
                flag = 0;


            }
            solution.cost = cost;
            solution.Routes = (Route[])((Object)routes.Clone());

            return solution;
        }



    }
}
