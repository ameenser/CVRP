using CVRP_SOLVER.CODE;
using MA_EAX_CVRP_SOLVER.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA_EAX_CVRP_SOLVER.Singletons
{
    public static class MapFormSingleton
    {
        public static MapForm mapForm = null;

        public static MapForm getInstance(Solution bestSolution, List<Costumer> costumers)
        {
            if (mapForm == null)
            {
                mapForm = new MapForm(bestSolution,costumers);
            }
            else
            {
                if (mapForm.IsDisposed)
                {
                    mapForm = new MapForm(bestSolution, costumers);
                }
            }
            return mapForm;
        }

        public static MapForm getInstance( List<Costumer> costumers)
        {
            if (mapForm == null)
            {
                mapForm = new MapForm(null, costumers);
            }
            else
            {
                if (mapForm.IsDisposed)
                {
                    mapForm = new MapForm(null, costumers);
                }
            }

            return mapForm;
        }
        public static void updatetInstance(Solution bestSolution)
        {
            if (mapForm != null)
            {
                mapForm.sol = bestSolution;
            }
        }
    }
}
