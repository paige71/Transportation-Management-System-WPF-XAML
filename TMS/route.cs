//  NAME    :   route
//  PURPOSE :   This class models the attributes and behaviours for the route. There are properties
//              included for oriTodes,distance,tripTime


using System;
using System.Collections.Generic;

namespace TMS
{
    public class route
    {
        public Tuple<string,string> oriToDes { get; set; }

        public int distance { get; set; }

        public double tripTime { get; set; }

        public static List<route> timetable()
        {
            // Create a list of routes.
            List<route> routes = new List<route>();

            // Add routes to the list.
            routes.Add(new route() { oriToDes = new Tuple<string, string>("Windsor", "London"), distance = 191, tripTime = 2.5 });
            routes.Add(new route() { oriToDes = new Tuple<string, string>("London", "Hamilton"), distance = 128, tripTime = 1.75 });
            routes.Add(new route() { oriToDes = new Tuple<string, string>("Hamilton", "Toronto"), distance = 68, tripTime = 1.25 });
            routes.Add(new route() { oriToDes = new Tuple<string, string>("Toronto", "Oshawa"), distance = 60, tripTime = 1.3 });
            routes.Add(new route() { oriToDes = new Tuple<string, string>("Oshawa", "Belleville"), distance = 134, tripTime = 1.65 });
            routes.Add(new route() { oriToDes = new Tuple<string, string>("Belleville", "Kingston"), distance = 82, tripTime = 1.2 });
            routes.Add(new route() { oriToDes = new Tuple<string, string>("Kingston", "Ottawa"), distance = 196, tripTime = 2.5 });
            routes.Add(new route() { oriToDes = new Tuple<string, string>("Ottawa", "Kingston"), distance = 196, tripTime = 2.5 });
            routes.Add(new route() { oriToDes = new Tuple<string, string>("Kingston", "Belleville"), distance = 82, tripTime = 1.2 });
            routes.Add(new route() { oriToDes = new Tuple<string, string>("Belleville", "Oshawa"), distance = 134, tripTime = 1.65 });
            routes.Add(new route() { oriToDes = new Tuple<string, string>("Oshawa", "Toronto"), distance = 60, tripTime = 1.3 });
            routes.Add(new route() { oriToDes = new Tuple<string, string>("Toronto", "Hamilton"), distance = 68, tripTime = 1.25 });
            routes.Add(new route() { oriToDes = new Tuple<string, string>("Hamilton", "London"), distance = 128, tripTime = 1.75 });
            routes.Add(new route() { oriToDes = new Tuple<string, string>("London", "Windsor"), distance = 191, tripTime = 2.5 });
            return routes;
        }
    }
}
