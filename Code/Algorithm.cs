using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using WindowsFormsApplication2;
using CVRP_SOLVER.LOCAL_SEARCH.Optimization;
using CVRP_SOLVER.LOCAL_SEARCH.Modifacation;
using MA_EAX_CVRP_SOLVER.GUI;

namespace CVRP_SOLVER.CODE
{
    public static class MA_EAX
    {
        private const int N_POP_SIZE = 30;
        private const int MAX_GENERATION = 5;

        public static Solution solve(Graph graph,MainForm form)
        {
            Solution[] N_pop = new Solution[N_POP_SIZE];
            form.sendNewPrgressBarValues("Creating initial population" ,0,MAX_GENERATION);

            Parallel.For(0, N_POP_SIZE, i =>
            {
                N_pop[i] = Create_initial_solution(graph);
            });
           
            Mutex _mutex = new Mutex();
            Mutex _replace_parent_mutex = new Mutex();
            ESET_CREATE_MODE eset_creation_mode = ESET_CREATE_MODE.ESET_1AB;
            int no_imporvement_count = 0;
            Solution parentA, parentB, C_best;
            for (int generation = 0; generation < MAX_GENERATION; generation++)
            {
               // form.UpdateLabel("Creating geneartion "+ (generation+1),generation,MAX_GENERATION);
                no_imporvement_count = 0;

                Parallel.For(0, N_POP_SIZE - 1, i =>
                {
                    parentA = N_pop[i];
                    parentB = N_pop[i + 1];
                    C_best = parentA;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(" MA_EAX solve() generation {0} interation {1}", i + 1, generation + 1);
                    Console.ResetColor();
                    Solution child = start(parentA, parentB, graph, eset_creation_mode);
                    if (child != null)
                    {
                        if (child.calc_Cost() < parentA.calc_Cost())
                            C_best = child;
                        else
                        {
                            _mutex.WaitOne();
                            no_imporvement_count++;
                            _mutex.ReleaseMutex();
                        }
                    }
                    _replace_parent_mutex.WaitOne();
                    form.sendNewPrgressBarValues("Creating geneartion " + (generation + 1) , generation*(N_POP_SIZE-1) + i, MAX_GENERATION * (N_POP_SIZE - 1));
                    N_pop[i] = C_best;
                    _replace_parent_mutex.ReleaseMutex();

                });
                Solution[] currIter = new Solution[N_POP_SIZE];
                for (int i = 0; i < N_POP_SIZE; i++)
                    currIter[i] = N_pop[i];
                Array.Sort(currIter);
                form.sendNewBestSolution(currIter[0]);
                if (no_imporvement_count >= N_POP_SIZE / 2)
                    eset_creation_mode = ESET_CREATE_MODE.ESET_BLOCK;
            }

            Array.Sort(N_pop);
            form.sendNewPrgressBarValues("Algorithm Finished\nBest solution Found :" + N_pop[0], MAX_GENERATION,MAX_GENERATION);

            return N_pop[0];
        }

        private static Solution Create_initial_solution(Graph graph)
        {
            Solution sol = graph.random_solver();
            sol = Optimization.opt_2_inter(sol);
            sol = Optimization.exchange_1_1(Optimization.opt_2_1(Optimization.exchange_1_1(Optimization.switch_nodes(Optimization.opt_2_1(sol), sol.Graph.vehicle_capacity), sol.Graph.vehicle_capacity)), sol.Graph.vehicle_capacity);
            sol = Optimization.opt_2_inter(sol);
            return sol;
        }
        public static Solution start(Solution parentA, Solution parentB, Graph graph, ESET_CREATE_MODE eset_mode)
        {
            int[,] G_ab = new int[parentA.Graph.getNodes().Count, parentA.Graph.getNodes().Count];
            int[,] G_a = new int[parentA.Graph.getNodes().Count, parentA.Graph.getNodes().Count];
            int[,] G_b = new int[parentA.Graph.getNodes().Count, parentA.Graph.getNodes().Count];
            int[,] G_a_original = new int[parentA.Graph.getNodes().Count, parentA.Graph.getNodes().Count];
            int[,] G_b_original = new int[parentA.Graph.getNodes().Count, parentA.Graph.getNodes().Count];
            int[,] G_a_reverse = new int[parentA.Graph.getNodes().Count, parentA.Graph.getNodes().Count];
            int[,] G_b_reverse = new int[parentA.Graph.getNodes().Count, parentA.Graph.getNodes().Count];


            construct_Gab(parentA, parentB, graph, G_a, G_b, G_ab, G_a_original, G_b_original, G_a_reverse, G_b_reverse);
            List<Cycle> cycles_list = construct_AB_cycles(G_ab);

         
            IntermediateI[] solutions = Construct_intermediate_I(parentA, cycles_list, graph, G_a, G_b, eset_mode).ToArray();

            List<IntermediateI> feasable_solutions = new List<IntermediateI>();
            Console.WriteLine($"number  of new solutions { solutions.Length}");
            List<Solution> feasableSolution_list = new List<Solution>();

            solutions = Modifaction_phase(graph, solutions);
            feasableSolution_list = getFeasableSolutions(solutions, feasableSolution_list);

            Console.WriteLine("\n there is {0} feasable solutions ", feasableSolution_list.Count);
            List<Solution> optimizedSolutions_list = new List<Solution>();
            optimizedSolutions_list = Optimaztion_phase(graph, feasableSolution_list);

            Solution[] final_solutions = optimizedSolutions_list.ToArray();
            Array.Sort(final_solutions);
            if (final_solutions.Length > 0)
                return final_solutions[0];

            return null;
        }

        private static List<Solution> Optimaztion_phase(Graph graph, List<Solution> feasableSolution_list)
        {
            List<Solution> optimizedSolutions_list = new List<Solution>();
           
            Parallel.For(0, feasableSolution_list.Count, i =>
            {
                
                Console.WriteLine("\n Solution {0} optimzation", i);


                var task = Task.Run(() =>
                {
                    if (i > feasableSolution_list.Count) return null;
                    Solution sol = feasableSolution_list[i];
                    sol = Optimization.opt_2_inter(sol);
                   
                    sol = Optimization.exchange_1_1(Optimization.opt_2_1(Optimization.exchange_1_1(Optimization.switch_nodes(Optimization.opt_2_1(sol), graph.vehicle_capacity), graph.vehicle_capacity)), graph.vehicle_capacity);
                    sol = Optimization.opt_2_1(Optimization.opt_2_inter(Optimization.exchange_1_1(Optimization.opt_2_1(Optimization.exchange_1_1(Optimization.switch_nodes(Optimization.opt_2_1(sol), graph.vehicle_capacity), graph.vehicle_capacity)), graph.vehicle_capacity)));
                    sol = Optimization.exchange_1_1(Optimization.opt_2_inter(sol), graph.vehicle_capacity);
                    return sol;
                });

                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(3 * 60));
                if (isCompletedSuccessfully)
                {
                    if (task.Result != null)
                        try
                        {
                            Solution sol = task.Result;
                            while(modify_Solution(graph, ref sol));
                            sol.calc_Cost();
                            optimizedSolutions_list.Add(sol);
                        }
                        catch (Exception e)
                        {

                        }
                }

            });
            return optimizedSolutions_list;
        }

        private static bool modify_Solution(Graph graph, ref Solution sol)
        {
            int[] nodes = new int[graph.nodes.Count];

            foreach (Route r in sol.Routes)
            {
                for (int j = 1; j < r.Nodes.Count - 1; j++)
                {
                    if (nodes[r.Nodes[j].ID] == 1)
                    {
                        r.remove_Node(r.Nodes[j]);
                        return true;
                    }
                    else
                    {
                        nodes[r.Nodes[j].ID]++;
                    }
                }
            }
            return false;
        }

        private static List<Solution> getFeasableSolutions(IntermediateI[] solutions, List<Solution> solutions_list)
        {
            for (int i = 0; i < solutions.Length; i++)
            {
                if (solutions[i].isFeasable)
                    solutions_list.Add(Solution.Intermediate_to_Soulution(solutions[i]));
            }

            return solutions_list;
        }

        private static IntermediateI[] Modifaction_phase(Graph graph, IntermediateI[] solutions)
        {
            for (int i = 0; i < solutions.Length; i++)
            {
                
                if (solutions[i].hasSubToures && !solutions[i].isFeasable)
                {
                    IntermediateI temp = solutions[i], res;

                    var task = Task.Run(() =>
                    {
                        if (i < solutions.Length)
                        {

                            return Modifacation.merge_sub_tours(solutions[i], graph.vehicle_capacity);
                        }
                        else
                            return null;


                    });

                    bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(3 * 10));
                    if (isCompletedSuccessfully)
                    {
                        if (task.Result != null)
                            solutions[i] = task.Result;
                        else
                        { solutions[i] = temp; }    // break;

                    }
                }



                if (!solutions[i].isFeasable && !solutions[i].hasSubToures)
                {

                    IntermediateI temp = solutions[i], res;
                    var task = Task.Run(() =>
                    {
                        if (i < solutions.Length)
                        {
                            res = Modifacation.swich_nodes_infesable3(solutions[i], graph.vehicle_capacity);
                            return res;
                        }
                        else
                            return null;
                        
                    });

                    bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(5 * 10));
                    if (isCompletedSuccessfully)
                    {
                        if (task.Result != null)
                            solutions[i] = task.Result;
                        else
                        { solutions[i] = Modifacation.swich_nodes_infesable3(solutions[i], graph.vehicle_capacity); }    // break;
                    }
                    else
                    {
                        solutions[i] = Modifacation.swich_nodes_infesable3(solutions[i], graph.vehicle_capacity);

                        Console.Write($"\n\nIntermediateI[{i}] modifacation");
                        solutions[i].check_fesablity(solutions[i], graph.vehicle_capacity);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($" failed!!");
                        Console.ResetColor();
                        solutions[i] = temp;
                    }

                    string feasable = solutions[i].isFeasable ? " " : " not ";
                    Console.Write($"\nIntermediateI[{i}] is");
                    if (feasable == " ")
                        Console.ForegroundColor = ConsoleColor.Green;
                    else
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{ feasable}");
                    Console.Write($" feasable");
                    Console.ResetColor();

                }

            }
            return solutions;
        }
    
        private static void construct_Gab(Solution solution1, Solution solution2, Graph graph, int[,] G_a, int[,] G_b, int[,] G_ab, int[,] G_a_original, int[,] G_b_original, int[,] G_a_reverse, int[,] G_b_reverse)
        {

            foreach (Route route in solution1.Routes)
            {
                for (int i = 1; i < route.Nodes.Count; i++)
                {
                    G_a[route.Nodes[i - 1].ID, route.Nodes[i].ID] = 1;
                    G_a[route.Nodes[i].ID, route.Nodes[i - 1].ID] = 1;
                    G_a_original[route.Nodes[i - 1].ID, route.Nodes[i].ID] = 1;
                    G_a_reverse[route.Nodes[i].ID, route.Nodes[i - 1].ID] = 1;
                }
            }

            foreach (Route route in solution2.Routes)
            {
                for (int i = 1; i < route.Nodes.Count; i++)
                {
                    G_b[route.Nodes[i - 1].ID, route.Nodes[i].ID] = -1;
                    G_b[route.Nodes[i].ID, route.Nodes[i - 1].ID] = -1;
                    G_b_original[route.Nodes[i - 1].ID, route.Nodes[i].ID] = -1;
                    G_b_reverse[route.Nodes[i].ID, route.Nodes[i - 1].ID] = -1;

                }
            }

            for (int i = 0; i < G_a.GetLength(0); i++)
            {
                for (int j = 0; j < G_a.GetLength(0); j++)
                {
                    
                    G_ab[i, j] = G_a[i, j] + G_b[i, j];

                }
            }
     
            Console.WriteLine("Finish construct_Gab()");
        }
        private static List<Cycle> construct_AB_cycles(int[,] G_ab)
        {
            List<Route> AB_cycles = new List<Route>();
            List<Cycle> cycles = new List<Cycle>();

            for (int i = 0; i < G_ab.GetLength(0); i++)
            {
                for (int j = 0; j < G_ab.GetLength(1); j++)
                {
                    if (G_ab[i, j] != 0)
                    {

                        List<int> x = search_cycle(new List<int> { i }, j, G_ab, G_ab[i, j]);
                        if (x != null)
                        {
                            Cycle c = new Cycle();
                            c.add_nodes_list(x);
                            c.startValue = G_ab[i, j];
                            List<int> nodes = new List<int>(x);
                            nodes.RemoveAt(nodes.Count - 1);
                            nodes.RemoveAt(0);
                            nodes.Sort();
                            Boolean can_Add = true;
                            for (int k = 1; k < nodes.Count; k++)
                                if (nodes[k - 1] == nodes[k])
                                {
                                    can_Add = false;
                                    break;
                                }
                            if (!Cycle.Contains(c, cycles) && can_Add)
                                cycles.Add(c);
                        }
                    }
                }
            }
         
            return cycles;

        }
        private static List<int> search_cycle(List<int> vertices, int currNode, int[,] G_ab, int value)
        {

            Console.WriteLine($"search_cycle() ");
            int temp = currNode;
            do
            {
                temp = currNode;
                for (int nextCustomer = 0; nextCustomer < G_ab.GetLength(0); nextCustomer++)
                {
                    if ((G_ab[currNode, nextCustomer] < 0 && value > 0) || (G_ab[currNode, nextCustomer] > 0 && value < 0))
                    {
                        if (nextCustomer != vertices[0] && vertices.Contains(nextCustomer)) continue;
                        vertices.Add(currNode);
                        value = G_ab[currNode, nextCustomer];
                        currNode = nextCustomer;
                        break;
                    }
                }
                if (temp == currNode) return null;
            } while (vertices[0] != currNode && vertices.Count < G_ab.GetLength(0));

            if (vertices.Count >= G_ab.GetLength(0) || vertices.Count % 2 == 1)
                return null;
            vertices.Add(currNode);
            for (int i = 0; i < vertices.Count - 1; i++)
            {
                G_ab[vertices[i], vertices[i + 1]] = 0;
            }
            return vertices;
        }
        private static List<IntermediateI> Construct_intermediate_I(Solution solution1, List<Cycle> cycles_list, Graph graph, int[,] G_a, int[,] G_b, ESET_CREATE_MODE eset_mode)
        {

            List<int[,]> list_of_intermeidates_I = new List<int[,]>();
            int[,] eset_matrix = new int[1, 1];
            if (eset_mode == ESET_CREATE_MODE.ESET_BLOCK)
            {
                for (int k = 0; k < cycles_list.Count; k++)
                {
                    for (int m = k + 1; m < cycles_list.Count; m++)
                    {
                        if (Cycle.connected_cycles(cycles_list[m], cycles_list[k]) > 2)
                        {
                            eset_matrix = constuct_Eset_Block(cycles_list[m], cycles_list[k], G_a.GetLength(0));
                        }
                  
                        else continue;

                        eset_matrix = merge_eset_with_Graph_a(G_a, eset_matrix);

                        if (eset_matrix != null)
                            list_of_intermeidates_I.Add(eset_matrix);
                    }
                }
            }
            else
            {
                for (int m = 0; m < cycles_list.Count; m++)
                {
                    eset_matrix = constuct_Eset_1AB(cycles_list[m], G_a, G_b);
                    eset_matrix = merge_eset_with_Graph_a(G_a, eset_matrix);

                    if (eset_matrix != null)
                        list_of_intermeidates_I.Add(eset_matrix);
                }
            }

            List<IntermediateI> Solution_list = new List<IntermediateI>();
            int quanity = (list_of_intermeidates_I.Count > 30) ? 30 : list_of_intermeidates_I.Count;
            for (int i = 0; i < quanity; i++)
            {
                Solution_list.Add(IntermediateI.convert_adjacencyMatrix_to_Solution(list_of_intermeidates_I[i], graph));
            }
            return Solution_list;
        }

        private static int[,] merge_eset_with_Graph_a(int[,] G_a, int[,] eset_matrix)
        {
            for (int i = 0; i < G_a.GetLength(0); i++)
            {
                for (int j = 0; j < G_a.GetLength(1); j++)
                {
                    if (i == j) continue;
                    eset_matrix[i, j] = G_a[i, j] + eset_matrix[i, j];
                 
                }
            }
            int[] node_edges = new int[G_a.GetLength(0)];
            Boolean kill_solution = false;
            for (int i = 0; i < G_a.GetLength(0); i++)
            {
                for (int j = 0; j < G_a.GetLength(1); j++)
                {
                    if (eset_matrix[i, j] != 0)
                        node_edges[i] += 1;
                }

            }
            for (int i = 1; i < node_edges.Length; i++)
            {
                if (node_edges[i] > 2)
                    kill_solution = true;

            }
            if (kill_solution)
                return null;
            return eset_matrix;
        }
        private static int[,] constuct_Eset_Block(Cycle cycle1, Cycle cycle2, int size)
        {
            int[] cycle_nodes;
            int value;
            int[,] Eset_matrix = new int[size, size];

            cycle_nodes = cycle1.get_nodes().ToArray();
            value = -cycle1.startValue;
            for (int k = 1; k < cycle_nodes.Length; k++)
            {

                Eset_matrix[cycle_nodes[k], cycle_nodes[k - 1]] = value;
                Eset_matrix[cycle_nodes[k - 1], cycle_nodes[k]] = value;
                value *= (-1);
            }




            cycle_nodes = cycle2.get_nodes().ToArray();
            value = -cycle2.startValue;
            for (int k = 1; k < cycle_nodes.Length; k++)
            {
                Eset_matrix[cycle_nodes[k], cycle_nodes[k - 1]] = value;
                Eset_matrix[cycle_nodes[k - 1], cycle_nodes[k]] = value;
                value *= (-1);
            }
            return Eset_matrix;
        }

        private static int[,] constuct_Eset_1AB(Cycle cycle, int[,] g_a, int[,] g_b)
        {
            int[] cycle_nodes;
            int value;
            int[,] Eset_matrix = new int[g_b.GetLength(0), g_b.GetLength(1)];


            cycle_nodes = cycle.get_nodes().ToArray();
            value = -cycle.startValue;
            for (int k = 1; k < cycle_nodes.Length; k++)
            {
                Eset_matrix[cycle_nodes[k], cycle_nodes[k - 1]] = value;
                Eset_matrix[cycle_nodes[k - 1], cycle_nodes[k]] = value;

                value *= (-1);
            }


            return Eset_matrix;
        }


    }

}
