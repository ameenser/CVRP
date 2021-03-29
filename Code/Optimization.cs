using CVRP_SOLVER.CODE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVRP_SOLVER.LOCAL_SEARCH.Optimization
{
    public static class Optimization
    {

        public static Solution opt_2_1(Solution solution)
        {

            int route_length;
            double cost = 0;
            Solution optimized = Solution.clone(solution);
            Random rnd = new Random();
            Route route;
            while (true)
            {
                bool foundBetterDist = false;
                for (int index = 0; index < solution.Routes.Count(); index++)
                {
                    route = solution.Routes[index];
                    route_length = route.Nodes.Count;
                    if (route_length < 3) continue;

                    for (int i = 1; i < route_length - 1; i++)
                    {
                        for (int j = 1; j < route_length - 1; j++)
                        {
                            if (i == j) continue;
                            foundBetterDist = false;

                            Route new_route = new Route(index);

                            for (int k = 0; k < i; k++)
                            {
                                new_route.add_Node(route.Nodes[k]);
                            }
                            for (int k = j - 1; k >= i; k--)
                            {
                                new_route.add_Node(route.Nodes[k]);
                            }
                            for (int k = j; k < route.Nodes.Count; k++)
                            {
                                new_route.add_Node(route.Nodes[k]);
                            }

                            if (new_route.get_cost() < route.get_cost())
                            {
                                solution.Routes[index].update_route(new_route);
                                route.update_route(new_route);
                                foundBetterDist = true;
                            }

                            if (foundBetterDist) break;

                        }
                        if (foundBetterDist) break;

                    }
                }
                if (!foundBetterDist) break;
            }

            cost = 0;
            foreach (Route r in optimized.Routes)
                cost += r.get_cost();
            optimized.cost = cost;

            optimized.cost = cost;
            return optimized;
        }


        public static Solution opt_2_inter(Solution solution)
        {

            Solution optimized = Solution.clone(solution);
            Route route1, route2;
            for (int route1_index = 0; route1_index < solution.Routes.Count(); route1_index++)
            {
                for (int route2_index = 0; route2_index < solution.Routes.Count(); route2_index++)
                {
                    if (route1_index == route2_index) continue;
                    opt_2_inter_CheckNewRoutes(solution, optimized, out optimized.Routes[route1_index],
                        out optimized.Routes[route2_index], route1_index, route2_index);
                }
            }
            double cost = optimized.get_Cost();
            return optimized;
        }

        private static void opt_2_inter_CheckNewRoutes(Solution solution, Solution optimized, out Route route1, out Route route2, int route1_index, int route2_index)
        {
            route1 = optimized.Routes[route1_index];
            route2 = optimized.Routes[route2_index];
            bool foundBetterRoutes = false;
            for (int i = 2; i < route1.Nodes.Count - 1; i++)
            {
                foundBetterRoutes = false;
                for (int j = 2; j < route2.Nodes.Count - 1; j++)
                {
                    foundBetterRoutes = opt_2_inter_ConstructNewRoutes(solution, optimized, route1, route2, route1_index, route2_index, foundBetterRoutes, i, j);

                    if (foundBetterRoutes) break;
                }

            }
        }

        private static bool opt_2_inter_ConstructNewRoutes(Solution solution, Solution optimized, Route route1, Route route2, int route1_index, int route2_index, bool foundBetterRoutes, int i, int j)
        {
            Route new_r1 = new Route(route1.ID);
            Route new_r2 = new Route(route2.ID);

            // update route 1
            for (int w = 0; w < i; w++)
            {
                new_r1.add_Node(route1.Nodes[w]);
            }
            for (int w = j; w < route2.Nodes.Count; w++)
            {
                new_r1.add_Node(route2.Nodes[w]);
            }

            // update route 2

            for (int w = 0; w < j; w++)
            {
                new_r2.add_Node(route2.Nodes[w]);
            }
            for (int w = i; w < route1.Nodes.Count; w++)
            {
                new_r2.add_Node(route1.Nodes[w]);
            }

            foundBetterRoutes = opt_2_inter_UpdateRoutes(solution, optimized, route1, route2, route1_index, route2_index, foundBetterRoutes, new_r1, new_r2);

            return foundBetterRoutes;
        }

        private static bool opt_2_inter_UpdateRoutes(Solution solution, Solution optimized, Route route1, Route route2, int route1_index, int route2_index, bool foundBetterRoutes, Route new_r1, Route new_r2)
        {
            if (new_r1.get_demand() <= solution.Graph.vehicle_capacity && new_r2.get_demand() <= solution.Graph.vehicle_capacity
                && new_r1.get_cost() + new_r2.get_cost() <= route1.get_cost() + route2.get_cost())
            {
                foundBetterRoutes = true;
                optimized.Routes[route1_index].update_route(new_r1);
                optimized.Routes[route2_index].update_route(new_r2);
                route1.update_route(new_r1);
                route2.update_route(new_r2);
            }

            return foundBetterRoutes;
        }




        public static Solution exchange_2_1(Solution solution, int vehicleCapacity)
        {

            Solution optimized = Solution.clone(solution);
            Route r1, r2;

            for (int i = 0; i < solution.Routes.Count() - 1; i++)
            {
                for (int j = i + 1; j < solution.Routes.Count(); j++)
                {
                    r1 = optimized.Routes[i];
                    r2 = optimized.Routes[j];

                    exchange_2_1_checkForBetterRoute(vehicleCapacity, ref r1, ref r2);

                    optimized.Routes[i].update_route(r1);
                    optimized.Routes[j].update_route(r2);
                }
            }
            double cost = optimized.get_Cost();
            return optimized;
        }

        private static void exchange_2_1_checkForBetterRoute(int vehicleCapacity, ref Route r1, ref Route r2)
        {
            for (int k = 1; k < r1.Nodes.Count() - 1; k++)
            {
                for (int m = 1; m < r2.Nodes.Count() - 1; m++)
                {
                    if (r1.Nodes[k - 1].distance(r2.Nodes[m]) + r2.Nodes[m].distance(r1.Nodes[k]) <= r1.Nodes[k - 1].distance(r1.Nodes[k])
                        && r2.Nodes[m - 1].distance(r2.Nodes[m + 1]) <= r2.Nodes[m - 1].distance(r2.Nodes[m]) + r2.Nodes[m].distance(r2.Nodes[m + 1])
                        && r1.get_demand() + r2.Nodes[m].Demand <= vehicleCapacity
                        )
                    {
                        exchange_2_1_switchNodes(ref r1, ref r2, k, m);
                    }
                }
            }
        }

        private static void exchange_2_1_switchNodes(ref Route r1, ref Route r2, int k, int m)
        {
            Costumer toSwitch = r2.Nodes[m];
            r2.remove_Node(toSwitch);
            r1.insert_Node(k, toSwitch);
        }

        public static Solution exchange_1_1(Solution solution, int vehicleCapacity)
        {
            Solution optimized = Solution.clone(solution);
            Route r1, r2;

            for (int i = 0; i < solution.Routes.Count() - 1; i++)
            {
                for (int j = i + 1; j < solution.Routes.Count(); j++)
                {
                    exchange_1_1_checkForBetterRoutes(vehicleCapacity, optimized, out r1, out r2, i, j);
                }
            }
            double cost = optimized.get_Cost();
            return optimized;
        }

        private static void exchange_1_1_checkForBetterRoutes(int vehicleCapacity, Solution optimized, out Route r1, out Route r2, int i, int j)
        {
            r1 = optimized.Routes[i];
            r2 = optimized.Routes[j];
            bool improvementFound = false;
            do
            {
                improvementFound = exchange_1_1_checkForImprovment(vehicleCapacity, optimized, ref r1, ref r2, i, j);

            } while (improvementFound);
        }

        private static bool exchange_1_1_checkForImprovment(int vehicleCapacity, Solution optimized, ref Route r1, ref Route r2, int i, int j)
        {
            bool improvementFound = false;
            for (int k = 1; k < r1.Nodes.Count() - 1; k++)
            {

                for (int m = 1; m < r2.Nodes.Count() - 1; m++)
                {
                    Route new_r1, new_r2;
                    exchange_1_1_constructNewRoutes(r1, r2, k, m, out new_r1, out new_r2);

                    if (exchange_1_1_improvmentCondition(vehicleCapacity, r1, r2, new_r1, new_r2)
                        )
                    {
                        improvementFound = exchange_1_1_updateRoutes(optimized, i, j, new_r1, new_r2);
                    }
                }
            }

            return improvementFound;
        }

        private static bool exchange_1_1_updateRoutes(Solution optimized, int i, int j, Route new_r1, Route new_r2)
        {
            bool improvementFound = true;
            optimized.Routes[i].update_route(new_r1);
            optimized.Routes[j].update_route(new_r2);
            return improvementFound;
        }

        private static bool exchange_1_1_improvmentCondition(int vehicleCapacity, Route r1, Route r2, Route new_r1, Route new_r2)
        {
            return r1.get_cost() + r2.get_cost() > new_r1.get_cost() + new_r2.get_cost() &&
                                    new_r1.get_demand() <= vehicleCapacity && new_r2.get_demand() <= vehicleCapacity;
        }

        private static void exchange_1_1_constructNewRoutes(Route r1, Route r2, int k, int m, out Route new_r1, out Route new_r2)
        {
            new_r1 = new Route(r1.ID);
            new_r2 = new Route(r2.ID);
            for (int w = 0; w < k; w++)
                new_r1.add_Node(r1.Nodes[w]);
            new_r1.add_Node(r2.Nodes[m]);
            for (int w = k + 1; w < r1.Nodes.Count; w++)
                new_r1.add_Node(r1.Nodes[w]);

            for (int w = 0; w < m; w++)
                new_r2.add_Node(r2.Nodes[w]);
            new_r2.add_Node(r1.Nodes[k]);
            for (int w = m + 1; w < r2.Nodes.Count; w++)
                new_r2.add_Node(r2.Nodes[w]);
        }

        public static Solution switch_nodes(Solution solution, int vehicleCapacity)
        {
            Solution optimized = Solution.clone(solution);
            Route r1, r2;
            for (int i = 0; i < solution.Routes.Count() - 1; i++)
            {
                for (int j = i + 1; j < solution.Routes.Count(); j++)
                {
                    r1 = optimized.Routes[i];
                    r2 = optimized.Routes[j];
                    swich_nodes_getBetterRoutes(vehicleCapacity, ref r1, ref r2);

                    optimized.Routes[i].update_route(r1);
                    optimized.Routes[j].update_route(r2);
                }
            }

            double cost = optimized.get_Cost();
            return optimized;
        }

        private static void swich_nodes_getBetterRoutes(int vehicleCapacity, ref Route r1, ref Route r2)
        {
            for (int k = 1; k < r1.Nodes.Count() - 1; k++)
            {
                for (int m = 1; m < r2.Nodes.Count() - 1; m++)
                {
                    if (swich_nodes_checkCondition(vehicleCapacity, r1, r2, k, m))
                    {
                        swich_nodes_swapNodes(r1, r2, k, m);
                    }
                }
            }
        }


        private static bool swich_nodes_checkCondition(int vehicleCapacity, Route r1, Route r2, int k, int m)
        {
            return r1.Nodes[k - 1].distance(r2.Nodes[m]) + r2.Nodes[m].distance(r1.Nodes[k + 1]) <= r1.Nodes[k - 1].distance(r1.Nodes[k]) + r1.Nodes[k].distance(r1.Nodes[k + 1])
                                    && r2.Nodes[m - 1].distance(r1.Nodes[k]) + r1.Nodes[k].distance(r2.Nodes[m + 1]) <= r2.Nodes[m - 1].distance(r2.Nodes[m]) + r2.Nodes[m].distance(r2.Nodes[m + 1])
                                    && r1.get_demand() + r2.Nodes[m].Demand - r1.Nodes[k].Demand <= vehicleCapacity && r2.get_demand() + r1.Nodes[k].Demand - r2.Nodes[m].Demand <= vehicleCapacity;
        }
        private static void swich_nodes_swapNodes(Route r1, Route r2, int k, int m)
        {
            Costumer toSwitch2 = r2.Nodes[m];
            Costumer toSwitch1 = r1.Nodes[k];
            r2.remove_Node(toSwitch2);
            r1.remove_Node(toSwitch1);
            r1.insert_Node(k, toSwitch2);
            r2.insert_Node(m, toSwitch1);
        }

    }
}
