using CVRP_SOLVER.CODE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApplication2;
namespace CVRP_SOLVER.CODE
{
    /// <summary>
    /// Class that represents a Cycles in the graph.
    /// Each cycles has:
    /// 1 - list of nodes
    /// 2 - startValue of [1/-1] that detemines that the edge belongs either to solution A/B
    /// 3 - size of the cycles 
    /// </summary>
    public class Cycle
    {
        List<int> Nodes;
        public int startValue { get; set; }
        int size;

        public Cycle()
        {
            size = 0;
            Nodes = new List<int>();
        }

        public void add_Node(int toAdd)
        {

            Nodes.Add(toAdd);
        }

        public List<int> get_nodes()
        {
            return Nodes;
        }

        public void add_nodes_list(List<int> list)
        {
            Nodes = list;
            size = Nodes.Count;

        }

        public static Boolean Contains(Cycle new_cycle, List<Cycle> list_of_cycles)
        {
            int[] cycle_nodes = new_cycle.get_nodes().ToArray();
            HashSet<int> cycle1_nodes_set = new HashSet<int>(cycle_nodes);
            foreach (Cycle cycle in list_of_cycles)
            {
                HashSet<int> other_nodes_set = new HashSet<int>(cycle.get_nodes());
                if (other_nodes_set.SetEquals(cycle1_nodes_set) && cycle1_nodes_set.SetEquals(other_nodes_set))
                    return true;

            }

            return false;
        }


        public static int connected_cycles(Cycle c1, Cycle c2)
        {
            int[] c2_nodes = c2.get_nodes().ToArray();
            int count = 0;
            foreach (int node in c1.get_nodes())
            {
                if (c2_nodes.Contains(node))
                    count++;
            }
            //if (count > 1) return true;
            //return false;
            return count;
        }
        public static List<Cycle> detect_cycle(int[,] solution, int firstNode, int currNode, List<Cycle> cycles_list, CYCLE_FINDING_MODE mode)
        {
            
            Cycle cycle = new Cycle();
            List<int> vertices = new List<int>();
            vertices.Add(firstNode);
            int old, flag = 0;
            do
            {
                old = currNode;
                for (int nextCustomer = 0; nextCustomer < solution.GetLength(0); nextCustomer++)
                {
                    if (solution[currNode, nextCustomer] != 0)
                    {
                        solution[nextCustomer, currNode] = 0;
                        if (vertices.Contains(nextCustomer)) continue;
                        vertices.Add(currNode);
                        currNode = nextCustomer;
                        break;
                    }
                }
                if (old == currNode)
                {
                    vertices.Add(currNode);
                    vertices.Add(firstNode);
                    flag = 1;
                    break;
                }

            } while (vertices[0] != currNode && vertices.Count < solution.GetLength(0));

            if (vertices.Count == solution.GetLength(0))
                return cycles_list;

            if (flag == 0)
                vertices.Add(firstNode);

            cycle.add_nodes_list(vertices);
            if (Cycle.Contains(cycle, cycles_list))
                return cycles_list;


            if (vertices[0] != 0 && vertices.Contains(0))
            {
                int shifts = vertices.Find(vertex => vertex == 0);
                for (int i = shifts; i < vertices.Count; i++)
                {
                    vertices[i - shifts] = vertices[i];
                }

                for (int i = vertices.Count - shifts; i < vertices.Count; i++)
                {
                    vertices[i] = default(int);
                }

                if (vertices[1] == 0)
                    vertices.RemoveAt(1);
            }
            cycles_list.Add(cycle);

            for (int i = 1; i < vertices.Count; i++)
            {
                solution[vertices[i - 1], vertices[i]] = 0;
                if (mode == CYCLE_FINDING_MODE.Sub_Tours)
                    solution[vertices[i], vertices[i - 1]] = 0;

            }

            return cycles_list;


        }



        public static Boolean isDiscovered(Cycle cycle, List<Cycle> cycles_list)
        {
            HashSet<int> cycle_nodes = new HashSet<int>();
            foreach (int vertex in cycle.get_nodes())
                cycle_nodes.Add(vertex);

            int[] cycle1_nodes = cycle_nodes.ToArray();
            Array.Sort(cycle1_nodes);
            int count;
            foreach (Cycle other in cycles_list)
            {
                count = 0;
                HashSet<int> other_nodes = new HashSet<int>();
                foreach (int vertex in other.get_nodes())
                    other_nodes.Add(vertex);
                int[] cycle2_nodes = other_nodes.ToArray();
                Array.Sort(cycle2_nodes);

                if (cycle1_nodes.Length > cycle2_nodes.Length)
                {
                    foreach (int vertex in cycle2_nodes)
                    {
                        if (cycle1_nodes.Contains(vertex))
                            return true;
                    }

                    if (count == cycle2_nodes.Length)
                    {
                        cycles_list.Remove(other);
                        return true;
                    }
                }
                else
                {
                    foreach (int vertex in cycle1_nodes)
                    {
                        if (cycle1_nodes.Contains(vertex))
                            return false;
                    }

                    if (count == cycle1_nodes.Length)
                    {

                        return false;
                    }
                }

            }
            return false;
        }
    }

}
