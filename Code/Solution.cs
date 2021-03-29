using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace CVRP_SOLVER.CODE
{
    public class Solution : IComparable // a class that holds the final solution
    {
        private Route[] routes;
        private int numberOfRoutes { get; set; }
        public double cost { get; set; }
        public double vehicle_capcity { get; set; }

        private Graph graph;

        public Route[] Routes
        {
            get
            {
                return routes;
            }

            set
            {
                routes = value;

            }
        }

        public Graph Graph
        {
            get
            {
                return graph;
            }

            set
            {
                graph = value;
            }
        }

        public Solution(int number, double capacity, Graph g)
        {
            numberOfRoutes = number;
            vehicle_capcity = capacity;
            Routes = new Route[number];
            Graph = g;
            for (int i = 0; i < number; i++)
            {
                Routes[i] = new Route(i);
            }
        }

        public static Solution clone(Solution sol)
        {

            Solution temp = new Solution(1, sol.vehicle_capcity, sol.Graph);

            List<Route> routes = new List<Route>();

            for (int i = 0; i < sol.Routes.Count(); i++)
            {
                Route tmp = new Route(i);
                tmp.update_route(sol.Routes[i].Nodes);
                routes.Add(tmp);
            }
            temp.Routes = routes.ToArray();
            return temp;
        }
        public static Solution Intermediate_to_Soulution(IntermediateI intermediate)
        {
            Solution sol = new Solution(0, intermediate.vehicle_capcity, intermediate.Graph);
            sol.Routes = intermediate.Routes;
            sol.calc_Cost();
            return sol;
        }
        public double calc_Cost()
        {
            cost = 0;

            foreach (Route route in routes)
            {
                cost += route.calc_Cost();
            }

            return cost;
        }

        /// <summary>
        /// calculates the demand of the route.
        /// </summary>


        public override string ToString()
        {
            String result = "";
            //foreach (Route route in routes)
            //{
            //    result += route;
            //}
            result += $"\ntotal cost of the solution   => {calc_Cost()}";

            return result;
        }


        public static bool compare_solutions(Solution sol2, Solution sol1)
        {
            try
            {
                if (sol2 == null || sol1 == null) return false;
                List<Route> sol1routes = sol1.Routes.ToList();
                List<Route> sol2routes = sol2.Routes.ToList();
                int numOfEqualRoutes = 0;
                foreach (Route r1 in sol1routes)
                {
                    numOfEqualRoutes += compare_routes(r1, sol2routes);
                }

                if (numOfEqualRoutes == sol1routes.Count)
                    return true;
                return false;
            }
            catch (Exception s)
            {
                return false;
            }
        }

        public static int compare_routes(Route r1, List<Route> toCompare)
        {
            int count = 0;
            foreach (Route r2 in toCompare)
            {

                count = 0;
                if (r1.Nodes.Count != r2.Nodes.Count) continue;

                for (int j = 1; j < r1.Nodes.Count - 1; j++)
                {
                    if (r1.Nodes[j] == r2.Nodes[j])
                        count++;
                }

                if (count == r1.Nodes.Count - 2)
                    return 1;

            }

            return 0;
        }

        public int CompareTo(object obj)
        {
            Solution other = obj as Solution;

            if (this >= other)
                return 1;
            return -1;
        }
        static int zz = 0;


        public static List<Cycle> extract_cycles(int[,] adjacency_matrix, CYCLE_FINDING_MODE mode)
        {
            Console.WriteLine("Solution.extract_cycles()");
            int[,] copy = new int[adjacency_matrix.GetLength(0), adjacency_matrix.GetLength(1)];
            for (int i = 0; i < adjacency_matrix.GetLength(0); i++)
            {
                for (int j = i + 1; j < adjacency_matrix.GetLength(1); j++)
                {
                    copy[i, j] = adjacency_matrix[i, j];
                }
            }
            List<Cycle> cycles_list = new List<Cycle>();
            if (mode == CYCLE_FINDING_MODE.Routes)  //extract routes from graph (connected to the depot)
            {
                for (int j = 0; j < adjacency_matrix.GetLength(1); j++)
                {

                    if (adjacency_matrix[0, j] != 0)
                    {
                        
                        cycles_list = Cycle.detect_cycle(adjacency_matrix, 0, j, cycles_list, mode);
                    }
                }
            }
            else  // extract sub toures from graph
            {
                for (int i = 1; i < adjacency_matrix.GetLength(0); i++)
                {
                    for (int j = 1; j < adjacency_matrix.GetLength(1); j++)
                    {

                        if (adjacency_matrix[i, j] != 0)
                        {
                            cycles_list = Cycle.detect_cycle(adjacency_matrix, i, j, cycles_list, mode);
                        }
                    }
                }
            }

            foreach (Cycle cycle in cycles_list)
            {

                int[] nodes = cycle.get_nodes().ToArray();
                for (int k = 0; k < nodes.Length - 1; k++)
                    adjacency_matrix[k, k + 1] = 0;

            }

            return cycles_list;
        }

        public static Route cycle_to_route(Cycle cycle, List<Costumer> customers_list, int routeID)
        {
            Console.WriteLine("Solution.cycle_to_route()");

            int[] nodes_list = cycle.get_nodes().ToArray();
            Route route = new Route(routeID);

            for (int i = 0; i < nodes_list.Length; i++)
            {
                route.add_Node(customers_list[nodes_list[i]]);
           
            }
            return route;
        }

        private static Boolean route_contains_node(List<Costumer> nodes_list, int id)
        {
            foreach (Costumer customer in nodes_list)
            {
                if (customer.ID == id)
                    return true;
            }
            return false;
        }


        public double get_Cost()
        {
            cost = 0;
            calc_Cost();
            return cost;
        }


       
        public static bool operator <(Solution solution1, Solution solution2)
        {
            return solution1.cost < solution2.cost;
        }
        public static bool operator >(Solution solution1, Solution solution2)
        {
            return solution1.cost > solution2.cost;
        }
        public static bool operator >=(Solution solution1, Solution solution2)
        {
            return solution1.cost >= solution2.cost;
        }
        public static bool operator <=(Solution solution1, Solution solution2)
        {
            return solution1.cost <= solution2.cost;
        }
    }

    public class IntermediateI : Solution
    {
        public bool hasSubToures { get; set; }
        public bool isFeasable { get; set; }
        public IntermediateI(int number, double capacity, Graph g) : base(number, capacity, g)
        {
            hasSubToures = false;
            isFeasable = true;
        }
        public static IntermediateI clone(IntermediateI sol)
        {
            IntermediateI clone = new IntermediateI(1, sol.vehicle_capcity, sol.Graph);
            List<Route> routes = new List<Route>();

            for (int i = 0; i < sol.Routes.Count(); i++)
            {
                Route tmp = new Route(i);
                tmp.update_route(sol.Routes[i].Nodes);
                routes.Add(tmp);
            }
            clone.Routes = routes.ToArray();
            clone.hasSubToures = sol.hasSubToures;
            clone.isFeasable = sol.isFeasable;
            return clone;
        }



        public bool check_fesablity(IntermediateI solution, int vehicleCapcity)
        {
            solution.isFeasable = true;
            foreach (Route route in solution.Routes)
            {
                if (route.get_demand() > vehicleCapcity)
                {
                    solution.isFeasable = false;
                }
            }

            return solution.isFeasable;
        }
        public static IntermediateI convert_adjacencyMatrix_to_Solution(int[,] adjacency_matrix, Graph graph)
        {
            Console.WriteLine("Solution.convert_adjacencyMatrix_to_Solution()");

            List<Cycle> cycles_list = Solution.extract_cycles(adjacency_matrix, CYCLE_FINDING_MODE.Routes);
            List<Cycle> sub_toures = Solution.extract_cycles(adjacency_matrix, CYCLE_FINDING_MODE.Sub_Tours);

            List<Costumer> customers_list = graph.getNodes();
            List<Route> routes = new List<Route>();
            for (int i = 0; i < cycles_list.Count; i++)
            {
                routes.Add(cycle_to_route(cycles_list[i], customers_list, i));
            }

            for (int i = 0; i < sub_toures.Count; i++)
            {
                routes.Add(cycle_to_route(sub_toures[i], customers_list, i));
            }
            IntermediateI solution = new IntermediateI(1, graph.vehicle_capacity, graph);

            solution.isFeasable = true;
            if (sub_toures.Count > 0)
            {
                solution.hasSubToures = true;
                solution.isFeasable = false;
            }

            solution.Routes = routes.ToArray();
            double cost = 0;
            foreach (Route route in routes)
            {
                if (route.get_demand() > solution.vehicle_capcity)
                    solution.isFeasable = false;
                cost += route.calc_Cost();
            }
            solution.cost = cost;

           
            Console.WriteLine("Finish convert_adjacencyMatrix_to_Solution()");

            return solution;
        }



    }
}
