using CVRP_SOLVER.CODE;
using MA_EAX_CVRP_SOLVER.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA_EAX_CVRP_SOLVER.Singletons
{
    public static class RouteFormSingleton
    {
        public static RoutesForm routeForm= null;

        public static RoutesForm getInstance(Solution sol)
        {
            if(routeForm == null)
            {
                routeForm = new RoutesForm(sol);
            }
            else
            {
                if (routeForm.IsDisposed)
                {
                    routeForm = new RoutesForm(sol);
                }
            }

            return routeForm;
        }

        public static RoutesForm getInstance()
        {
            if (routeForm == null)
            {
                routeForm = new RoutesForm();
            }
            else
            {
                if (routeForm.IsDisposed)
                {
                    routeForm = new RoutesForm();
                }
            }

            return routeForm;
        }

        public static void updateInstance(Solution sol)
        {
            if (routeForm != null)
            {
                routeForm.sol = sol;
            }
;
        }
    }
}
