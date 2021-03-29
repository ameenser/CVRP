using CVRP_SOLVER.CODE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVRP_SOLVER.LOCAL_SEARCH.Modifacation
{
    public static class Modifacation
    {

        public static IntermediateI swich_nodes_infesable(IntermediateI solution, int vehicleCapacity)
        {
            IntermediateI optimized = IntermediateI.clone(solution);
            Route r1, r2;
            for (int i = 0; i < solution.Routes.Count() - 1; i++)
            {
                for (int j = i + 1; j < solution.Routes.Count(); j++)
                {
                    if (i == j) break;
                    if (isThereFesableAndInFesableRoutes(vehicleCapacity, optimized, i, j))
                    {
                        swich_nodes_detectFesableAndInfeasableRoutes(vehicleCapacity, optimized, out r1, out r2, i, j);
                    }
                    else continue;

                    double min;
                    int indexToInsert;
                    swich_nodes_modifyInfesableRouteToBeFesable(vehicleCapacity, r1, r2, out min, out indexToInsert);
                    optimized.Routes[i].update_route(r1);
                    optimized.Routes[j].update_route(r2);

                    min = Double.MaxValue;
                    swich_nodes_modifyInfesableRouteToBeFesable2(vehicleCapacity, r1, r2, ref min, ref indexToInsert);
                    optimized.Routes[i].update_route(r1);
                    optimized.Routes[j].update_route(r2);
                }
            }
            double cost = optimized.get_Cost(); ;
            optimized.isFeasable = optimized.check_fesablity(optimized, vehicleCapacity);
            return optimized;
        }

        private static void swich_nodes_modifyInfesableRouteToBeFesable2(int vehicleCapacity, Route r1, Route r2, ref double min, ref int indexToInsert)
        {
            for (int k = 1; k < r2.Nodes.Count() - 1; k++)
            {
                indexToInsert = -1;
                Costumer toSwitch = null;
                for (int m = 1; m < r1.Nodes.Count() - 1; m++)
                {

                    if (r2.get_demand() + r1.Nodes[m].Demand <= vehicleCapacity
                        && r2.get_cost() - r2.Nodes[k - 1].distance(r2.Nodes[k]) + r2.Nodes[k - 1].distance(r1.Nodes[m])
                        + r2.Nodes[k].distance(r1.Nodes[m]) < min)
                    {
                        min = r2.get_cost() - r2.Nodes[k - 1].distance(r2.Nodes[k]) + r2.Nodes[k - 1].distance(r1.Nodes[m])
                        + r2.Nodes[k].distance(r1.Nodes[m]);
                        indexToInsert = k;
                        toSwitch = r1.Nodes[m];

                    }
                }
                if (indexToInsert != -1)
                {

                    r1.remove_Node(toSwitch);
                    r2.insert_Node(k, toSwitch);
                }

            }
        }

        private static void swich_nodes_modifyInfesableRouteToBeFesable(int vehicleCapacity, Route r1, Route r2, out double min, out int indexToInsert)
        {
            min = Double.MaxValue;
            indexToInsert = -1;
            for (int k = 1; k < r1.Nodes.Count() - 1; k++)
            {
                indexToInsert = -1;
                Costumer toSwitch = null;
                for (int m = 1; m < r2.Nodes.Count() - 1; m++)
                {

                    if (r1.get_demand() + r2.Nodes[m].Demand <= vehicleCapacity
                        && r1.get_cost() - r1.Nodes[k - 1].distance(r1.Nodes[k]) + r1.Nodes[k - 1].distance(r2.Nodes[m])
                        + r1.Nodes[k].distance(r2.Nodes[m]) < min)
                    {
                        min = r1.get_cost() - r1.Nodes[k - 1].distance(r1.Nodes[k]) + r1.Nodes[k - 1].distance(r2.Nodes[m])
                        + r1.Nodes[k].distance(r2.Nodes[m]);
                        indexToInsert = k;
                        toSwitch = r2.Nodes[m];

                    }
                }
                if (indexToInsert != -1)
                {

                    r2.remove_Node(toSwitch);
                    r1.insert_Node(k, toSwitch);
                }

            }
        }

        private static bool isThereFesableAndInFesableRoutes(int vehicleCapacity, IntermediateI optimized, int i, int j)
        {
            return (optimized.Routes[i].get_demand() > (double)vehicleCapacity && optimized.Routes[j].get_demand() <= (double)vehicleCapacity)
                                    || (optimized.Routes[j].get_demand() > (double)vehicleCapacity && optimized.Routes[i].get_demand() <= (double)vehicleCapacity);
        }

        private static void swich_nodes_detectFesableAndInfeasableRoutes(int vehicleCapacity, IntermediateI optimized, out Route r1, out Route r2, int i, int j)
        {
            if (optimized.Routes[i].get_demand() > (double)vehicleCapacity)
            {
                r2 = optimized.Routes[i];
                r1 = optimized.Routes[j];
            }
            else
            {
                r1 = optimized.Routes[i];
                r2 = optimized.Routes[j];
            }
        }

        public static IntermediateI swich_nodes_infesable3(IntermediateI solution, int vehicleCapacity)
        {
            IntermediateI optimized = IntermediateI.clone(solution);
            List<Route> feasable_routes = new List<Route>();
            List<Route> infeasable_routes = new List<Route>();
            foreach (Route route in optimized.Routes)
                if (route.get_demand() > vehicleCapacity)
                    infeasable_routes.Add(route);
                else
                    feasable_routes.Add(route);

            Costumer Depot = feasable_routes[0].Nodes[0];
            while (infeasable_routes.Count > 0)
            {
                Route route = infeasable_routes[0];
                infeasable_routes.Remove(infeasable_routes[0]);
                feasable_routes = infeasable_switch_node_route_to_feasable(feasable_routes, route, vehicleCapacity);
                for (int i = 0; i < feasable_routes.Count; i++)
                {
                    if (feasable_routes[i].get_demand() > vehicleCapacity)
                    {
                     
                        Route new_route = new Route(feasable_routes.Count);
                        new_route.add_Node(Depot);

                        while (feasable_routes[i].get_demand() > vehicleCapacity)
                        {
                            if (feasable_routes[i].Nodes[1].ID != 0)
                            {
                                new_route.add_Node(feasable_routes[i].Nodes[1]);
                                feasable_routes[i].remove_Node(feasable_routes[i].Nodes[1]);

                            }
                        }
                        new_route.add_Node(Depot);
                        feasable_routes.Add(new_route);

                    }
                }
            }

            List<Route> all_routes = new List<Route>(feasable_routes);

            double cost = optimized.get_Cost(); ;
            optimized.isFeasable = optimized.check_fesablity(optimized, vehicleCapacity);
            foreach (Route route in optimized.Routes)
            {
                if (route.Nodes[0].ID != 0 && route.Nodes[route.Nodes.Count - 1].ID != 0)
                    return solution;
            }
            return optimized;
        }

        private static List<Route> infeasable_switch_node_route_to_feasable(List<Route> feasable_routes, Route infeasble, double vehicleCapacity)
        {
            Route new_r1 = new Route(0);
            Route new_r2 = new Route(0);
            for (int k = 0; k < feasable_routes.Count(); k++)
            {
                for (int j = 1; j < infeasble.Nodes.Count() - 1; j++)
                {
                    if (infeasble.Nodes[j].ID != 0 && infeasble.Nodes[j].Demand + feasable_routes[k].get_demand() <= vehicleCapacity && infeasble.get_demand() > vehicleCapacity)
                    {
                        Costumer toSwitch = infeasble.Nodes[j];
                        infeasble.remove_Node(toSwitch);
                        feasable_routes[k].insert_Node(feasable_routes[k].Nodes.Count - 2, toSwitch);
                    }
                }

            }
            feasable_routes.Add(infeasble);
            return feasable_routes;
        }
        public static IntermediateI opt_2_inter_infeasable(IntermediateI solution, int vehicleCapacity)
        {
            IntermediateI optimized = IntermediateI.clone(solution);

            Route r1, r2;
            bool flag = true;

            for (int i = 0; i < solution.Routes.Count() - 1; i++)
            {
                for (int j = i + 1; j < solution.Routes.Count(); j++)
                {
                    if (i == j) break;
                    if ((optimized.Routes[i].get_demand() > (double)vehicleCapacity && optimized.Routes[j].get_demand() <= (double)vehicleCapacity)
                        || (optimized.Routes[j].get_demand() > (double)vehicleCapacity && optimized.Routes[i].get_demand() <= (double)vehicleCapacity))
                    {
                        if (optimized.Routes[i].get_demand() > (double)vehicleCapacity)
                        {
                            r2 = optimized.Routes[i];
                            r1 = optimized.Routes[j];
                            flag = true;
                        }
                        else
                        {
                            r1 = optimized.Routes[i];
                            r2 = optimized.Routes[j];
                            flag = false;
                        }
                    }
                    else continue;
                    double demand1 = 0, demand2 = 0, demand3 = 0, demand4 = 0;
                    bool changed = false;
                    int cnt = 0;
                    int times = 0;
                    int times_to_repeat = (r1.Nodes.Count - 2) * (r2.Nodes.Count - 2);
                    while (times < times_to_repeat)
                    {
                        times++;
                        do
                        {
                            cnt = 0;
                            changed = false;
                            for (int k = 1; k < r1.Nodes.Count() - 2; k++)
                            {
                                if (changed)
                                    break;
                                for (int m = 1; m < r2.Nodes.Count() - 2; m++)
                                {
                                    cnt++;
                                    demand1 = 0;
                                    demand2 = 0;
                                    demand3 = 0;
                                    demand4 = 0;
                                    for (int w = 0; w <= k; w++)
                                        demand1 = demand1 + r1.Nodes[w].Demand;
                                    for (int w = 0; w <= m; w++)
                                        demand2 = demand2 + r2.Nodes[w].Demand;
                                    for (int w = k + 1; w < r1.Nodes.Count(); w++)
                                        demand3 = demand3 + r1.Nodes[w].Demand;
                                    for (int w = m + 1; w < r2.Nodes.Count(); w++)
                                        demand4 = demand4 + r2.Nodes[w].Demand;
                                    if (demand1 + demand4 <= vehicleCapacity && demand2 + demand3 <= vehicleCapacity)
                                    {
                                        changed = true;
                                        Route new_r1 = new Route(i);
                                        Route new_r2 = new Route(j);

                                        for (int w = 0; w <= k; w++)
                                            new_r1.add_Node(r1.Nodes[w]);
                                        for (int w = m + 1; w < r2.Nodes.Count(); w++)
                                            new_r1.add_Node(r2.Nodes[w]);
                                        for (int w = 0; w <= m; w++)
                                            new_r2.add_Node(r2.Nodes[w]);
                                        for (int w = k + 1; w < r1.Nodes.Count(); w++)
                                            new_r2.add_Node(r1.Nodes[w]);

                                        break;
                                    }

                                }

                            }

                        } while (!changed);
                    }

                    if (!flag)
                    {
                        optimized.Routes[i].update_route(r1);
                        optimized.Routes[j].update_route(r2);
                    }
                    else
                    {

                        optimized.Routes[j].update_route(r1);
                        optimized.Routes[i].update_route(r2);

                    }
                }
            }
            double cost = optimized.get_Cost(); ;
            optimized.isFeasable = optimized.check_fesablity(optimized, vehicleCapacity);
            return optimized;
        }

        public static IntermediateI opt_2_inter_infeasable3(IntermediateI solution, int vehicleCapacity)
        {
            IntermediateI optimized = IntermediateI.clone(solution);
            List<Route> feasable_routes = new List<Route>();
            List<Route> infeasable_routes = new List<Route>();
            foreach (Route route in optimized.Routes)
                if (route.get_demand() > vehicleCapacity)
                    infeasable_routes.Add(route);
                else
                    feasable_routes.Add(route);


            foreach (Route route in infeasable_routes)
            {
                feasable_routes = infeasable_route_to_feasable(feasable_routes, route, vehicleCapacity);
            }
            List<Route> all_routes = new List<Route>(feasable_routes);
            if (feasable_routes.Count != optimized.Routes.Length)
            {
                for (int i = 0; i < infeasable_routes.Count; i++)
                {
                    bool found = false;
                    for (int j = 0; j < feasable_routes.Count; j++)
                    {
                        if (infeasable_routes[i].ID == feasable_routes[j].ID)
                            found = true;
                    }
                    if (!found) all_routes.Add(infeasable_routes[i]);

                }
            }

            double cost = optimized.get_Cost(); ;
            optimized.isFeasable = optimized.check_fesablity(optimized, vehicleCapacity);
            return optimized;
        }

        private static List<Route> infeasable_route_to_feasable(List<Route> feasable_routes, Route infeasble, double vehicleCapacity)
        {
            bool changeFound = false;
            Route new_r1 = new Route(0);
            Route new_r2 = new Route(0);
            Route old = null;
            for (int k = 0; k < feasable_routes.Count(); k++)
            {
                for (int i = 0; i < feasable_routes[k].Nodes.Count() - 2; i++)
                {
                    for (int j = 0; j < infeasble.Nodes.Count() - 2; j++)
                    {
                        double demand1 = 0, demand2 = 0, demand3 = 0, demand4 = 0;
                        for (int w = 0; w <= i; w++)
                            demand1 = demand1 + feasable_routes[k].Nodes[w].Demand;

                        for (int w = 0; w <= j; w++)
                            demand2 = demand2 + infeasble.Nodes[w].Demand;

                        for (int w = i + 1; w < feasable_routes[k].Nodes.Count(); w++)
                            demand3 = demand3 + feasable_routes[k].Nodes[w].Demand;

                        for (int w = j + 1; w < infeasble.Nodes.Count(); w++)
                            demand4 = demand4 + infeasble.Nodes[w].Demand;

                        if (demand1 + demand4 <= vehicleCapacity && demand2 + demand3 <= vehicleCapacity)
                        {
                            old = feasable_routes[k];
                            new_r1.ID = feasable_routes[k].ID;
                            new_r2.ID = infeasble.ID;
                            changeFound = true;
                            for (int w = 0; w <= i; w++)
                                new_r1.add_Node(feasable_routes[k].Nodes[w]);
                            for (int w = j + 1; w < infeasble.Nodes.Count(); w++)
                                new_r1.add_Node(infeasble.Nodes[w]);
                            for (int w = 0; w <= j; w++)
                                new_r2.add_Node(infeasble.Nodes[w]);
                            for (int w = k + 1; w < feasable_routes[k].Nodes.Count(); w++)
                                new_r2.add_Node(feasable_routes[k].Nodes[w]);

                            break;
                        }
                        if (changeFound) break;
                    }
                    if (changeFound) break;
                }
                if (changeFound) break;
            }
            if (changeFound)
            {
                feasable_routes.Remove(old);
                feasable_routes.Add(new_r1);
                feasable_routes.Add(new_r2);
                return feasable_routes;
            }
            return feasable_routes;
        }
        public static IntermediateI swich_nodes_infesable2(IntermediateI solution, int vehicleCapacity)
        {
            IntermediateI optimized = IntermediateI.clone(solution);
            Route r1, r2;

            for (int i = 0; i < solution.Routes.Count() - 1; i++)
            {
                for (int j = i + 1; j < solution.Routes.Count(); j++)
                {

                    r2 = optimized.Routes[i];
                    r1 = optimized.Routes[j];

                    double min = Double.MaxValue;
                    int indexToInsert = -1;
                    for (int k = 1; k < r1.Nodes.Count() - 1; k++)
                    {
                        indexToInsert = -1;
                        Costumer toSwitch = null;
                        for (int m = 1; m < r2.Nodes.Count() - 1; m++)
                        {

                            if (r1.get_demand() + r2.Nodes[m].Demand <= vehicleCapacity
                                && r1.get_cost() - r1.Nodes[k - 1].distance(r1.Nodes[k]) + r1.Nodes[k - 1].distance(r2.Nodes[m])
                                + r1.Nodes[k].distance(r2.Nodes[m]) < min)
                            {
                                min = r1.get_cost() - r1.Nodes[k - 1].distance(r1.Nodes[k]) + r1.Nodes[k - 1].distance(r2.Nodes[m])
                                + r1.Nodes[k].distance(r2.Nodes[m]);
                                indexToInsert = k;
                                toSwitch = r2.Nodes[m];

                            }
                        }
                        if (indexToInsert != -1)
                        {

                            r2.remove_Node(toSwitch);
                            r1.insert_Node(k, toSwitch);
                        }

                    }

                    optimized.Routes[i].update_route(r1);
                    optimized.Routes[j].update_route(r2);
                    min = Double.MaxValue;
                    for (int k = 1; k < r2.Nodes.Count() - 1; k++)
                    {
                        indexToInsert = -1;
                        Costumer toSwitch = null;
                        for (int m = 1; m < r1.Nodes.Count() - 1; m++)
                        {

                            if (r2.get_demand() + r1.Nodes[m].Demand <= vehicleCapacity
                                && r2.get_cost() - r2.Nodes[k - 1].distance(r2.Nodes[k]) + r2.Nodes[k - 1].distance(r1.Nodes[m])
                                + r2.Nodes[k].distance(r1.Nodes[m]) < min)
                            {
                                min = r2.get_cost() - r2.Nodes[k - 1].distance(r2.Nodes[k]) + r2.Nodes[k - 1].distance(r1.Nodes[m])
                                + r2.Nodes[k].distance(r1.Nodes[m]);
                                indexToInsert = k;
                                toSwitch = r1.Nodes[m];

                            }
                        }
                        if (indexToInsert != -1)
                        {

                            r1.remove_Node(toSwitch);
                            r2.insert_Node(k, toSwitch);
                        }

                    }

                    optimized.Routes[i].update_route(r2);
                    optimized.Routes[j].update_route(r1);

                }
            }
            double cost = optimized.get_Cost(); ;
            optimized.isFeasable = optimized.check_fesablity(optimized, vehicleCapacity);
            return optimized;
        }

        public static IntermediateI opt_2_inter_infeasable2(IntermediateI solution, int vehicleCapacity)
        {
            IntermediateI optimized = IntermediateI.clone(solution);
            Route r1, r2;
            for (int i = 0; i < solution.Routes.Count() - 1; i++)
            {
                for (int j = i + 1; j < solution.Routes.Count(); j++)
                {
                    r2 = optimized.Routes[i];
                    r1 = optimized.Routes[j];
                    double demand1 = 0, demand2 = 0, demand3 = 0, demand4 = 0;
                    bool changed = false;
                    int cnt = 0;
                    int times = 0;
                    int times_to_repeat = (r1.Nodes.Count - 2) * (r2.Nodes.Count - 2);
                    while (times < times_to_repeat)
                    {
                        times++;
                        do
                        {
                            cnt = 0;
                            changed = false;
                            for (int k = 1; k < r1.Nodes.Count() - 2; k++)
                            {
                                if (changed)
                                    break;
                                for (int m = 1; m < r2.Nodes.Count() - 2; m++)
                                {
                                    cnt++;
                                    demand1 = 0;
                                    demand2 = 0;
                                    demand3 = 0;
                                    demand4 = 0;
                                    for (int w = 0; w <= k; w++)
                                        demand1 = demand1 + r1.Nodes[w].Demand;
                                    for (int w = 0; w <= m; w++)
                                        demand2 = demand2 + r2.Nodes[w].Demand;
                                    for (int w = k + 1; w < r1.Nodes.Count(); w++)
                                        demand3 = demand3 + r1.Nodes[w].Demand;
                                    for (int w = m + 1; w < r2.Nodes.Count(); w++)
                                        demand4 = demand4 + r2.Nodes[w].Demand;
                                    if (demand1 + demand4 <= vehicleCapacity && demand2 + demand3 <= vehicleCapacity)
                                    {
                                        changed = true;
                                        Route new_r1 = new Route(i);
                                        Route new_r2 = new Route(j);

                                        for (int w = 0; w <= k; w++)
                                            new_r1.add_Node(r1.Nodes[w]);
                                        for (int w = m + 1; w < r2.Nodes.Count(); w++)
                                            new_r1.add_Node(r2.Nodes[w]);
                                        for (int w = 0; w <= m; w++)
                                            new_r2.add_Node(r2.Nodes[w]);
                                        for (int w = k + 1; w < r1.Nodes.Count(); w++)
                                            new_r2.add_Node(r1.Nodes[w]);

                                        break;
                                    }

                                }

                            }

                        } while (!changed);
                    }


                    optimized.Routes[j].update_route(r1);
                    optimized.Routes[i].update_route(r2);


                }
            }
            double cost = optimized.get_Cost(); ;
            optimized.isFeasable = optimized.check_fesablity(optimized, vehicleCapacity);
            return optimized;
        }


        public static IntermediateI infeasable_to_feasable(IntermediateI solution, int vehicleCapacity)
        {
            IntermediateI optimized = IntermediateI.clone(solution);
            Costumer Depot = optimized.Routes[0].Nodes[0];
            do
            {
                Route r = new Route(optimized.Routes.Count());
                r.add_Node(Depot);
                foreach (Route route in optimized.Routes)
                {

                    while (route.get_demand() > vehicleCapacity && r.get_demand() + route.Nodes[1].Demand <= vehicleCapacity)
                    {
                        r.add_Node(route.Nodes[1]);
                        route.remove_Node(route.Nodes[1]);
                     
                    }
                }
                r.add_Node(Depot);
                if (r.Nodes.Count > 2)
                {
                    List<Route> routes_ = optimized.Routes.ToList();
                    routes_.Add(r);
                    optimized.Routes = routes_.ToArray();
                }

                List<Route> routes = optimized.Routes.ToList();

                for (int i = 0; i < routes.Count; i++)
                {
                    for (int j = i + 1; j < routes.Count; j++)
                    {
                        if (routes[j].get_demand() + routes[i].get_demand() <= vehicleCapacity)
                        {

                            List<Costumer> customers = routes[j].Nodes;
                            routes.Remove(routes[j]);
                            customers.Remove(Depot);
                            customers.Remove(Depot);
                            for (int k = 0; k < customers.Count; k++)
                            {
                                routes[i].insert_Node(1, customers[k]);
                            }
                        }
                    }
                }
                optimized.isFeasable = true;

                foreach (Route route in optimized.Routes)
                {
                    if (route.get_demand() > vehicleCapacity)
                    {
                        optimized.isFeasable = false;

                    }
                }
            } while (!optimized.isFeasable);

            if (!optimized.isFeasable)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\ninfeasable_to_feasable() does not work");
                Console.ResetColor();
            }

            double cost = 0;
            optimized.isFeasable = true;

            foreach (Route route in optimized.Routes)
            {
                if (route.Nodes[0].ID != 0 || route.Nodes[route.Nodes.Count - 1].ID != 0)
                {
                    route.insert_Node(route.Nodes.Count, Depot);
                    route.insert_Node(0, Depot);
                }

                if (route.get_demand() > vehicleCapacity)
                {


                    optimized.isFeasable = false;
                }
                cost += route.get_cost();
            }
            optimized.cost = cost;

            return optimized;
        }

        public static IntermediateI merge_sub_tours(IntermediateI solution, int vehicleCapacity)
        {
            IntermediateI optimized = IntermediateI.clone(solution);
            List<Route> sub_tours = new List<Route>();
            List<Route> routes = new List<Route>();

            for (int i = 0; i < optimized.Routes.Count(); i++)

            {
                if (optimized.Routes[i].Nodes[0].ID == 0)
                {
                    routes.Add(optimized.Routes[i]);
                }
                else
                {
                    sub_tours.Add(optimized.Routes[i]);
                }
            }


            for (int k = 0; k < sub_tours.Count(); k++)
            {
                sub_tours[k].remove_Node(sub_tours[k].Nodes[0]);
                Tuple<int, int, int> bestMerge = find_best_merge(sub_tours[k], routes);
                if (bestMerge.Item1 == -1) return null;
                Route route = routes[bestMerge.Item1];

                if (bestMerge.Item3 != 0)
                {
                    for (int i = bestMerge.Item3; i < sub_tours[k].Nodes.Count; i++)
                    {
                        route.insert_Node(bestMerge.Item2, sub_tours[k].Nodes[i]);
                    }
                    for (int i = 0; i < bestMerge.Item3; i++)
                    {
                        route.insert_Node(bestMerge.Item2, sub_tours[k].Nodes[i]);
                    }
                }
                else
                {
                    if (bestMerge.Item3 == 0)
                    {
                        for (int i = bestMerge.Item3; i < sub_tours[k].Nodes.Count; i++)
                        {
                            route.insert_Node(bestMerge.Item2, sub_tours[k].Nodes[i]);
                        }
                        for (int i = 0; i < bestMerge.Item3; i++)
                        {
                            route.insert_Node(bestMerge.Item2, sub_tours[k].Nodes[i]);
                        }
                    }
                }

                routes[bestMerge.Item1].update_route(route);
            }
            double cost = 0;
            optimized.Routes = routes.ToArray();
            optimized.hasSubToures = false;
            optimized.isFeasable = true;
            foreach (Route route in optimized.Routes)
            {
                if (route.get_demand() > vehicleCapacity)
                    optimized.isFeasable = false;
                if (route.Nodes[0].ID != 0 || route.Nodes[route.Nodes.Count - 1].ID != 0)
                    return solution;
                cost += route.get_cost();
            }
            optimized.cost = cost;
            optimized.hasSubToures = false;
            return optimized;
        }


        private static Tuple<int, int, int> find_best_merge(Route sub_tour, List<Route> routes)
        {
            double min = Double.MaxValue;
            int routeNum = -1, firstEdge = -1, secondEdge = -1;

            for (int j = 0; j < routes.Count; j++)
            {
                for (int k = 1; k < routes[j].Nodes.Count - 1; k++)
                {
                    for (int i = 0; i < sub_tour.Nodes.Count; i++)
                    {
                        try
                        {
                            if (i != 0)
                            {
                                if (routes[j].Nodes[k].distance(sub_tour.Nodes[i]) + routes[j].Nodes[k + 1].distance(sub_tour.Nodes[i - 1]) < min)
                                {
                                    min = routes[j].Nodes[k].distance(sub_tour.Nodes[i]) + routes[j].Nodes[k + 1].distance(sub_tour.Nodes[i - 1]);
                                    routeNum = j;
                                    firstEdge = k;
                                    secondEdge = i;
                                }
                            }
                            else
                            {
                                if (i == 0)
                                {
                                    if (routes[j].Nodes[k].distance(sub_tour.Nodes[i]) + routes[j].Nodes[k + 1].distance(sub_tour.Nodes[sub_tour.Nodes.Count - 1]) < min)
                                    {
                                        min = routes[j].Nodes[k].distance(sub_tour.Nodes[i]) + routes[j].Nodes[k + 1].distance(sub_tour.Nodes[sub_tour.Nodes.Count - 2]);
                                        routeNum = j;
                                        firstEdge = k;
                                        secondEdge = i;
                                    }
                                }
                            }
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            return new Tuple<int, int, int>(-1, -1, -1);
                        }

                    }
                }
            }
            return new Tuple<int, int, int>(routeNum, firstEdge, secondEdge);

        }

    }
}
