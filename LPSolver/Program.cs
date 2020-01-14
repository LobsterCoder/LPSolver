using System;
using System.Collections.Generic;

namespace Examples
{
    public class Program
    {
        #region InputData
        static readonly List<Tuple<int, int>> botPoses = new List<Tuple<int, int>>()
            {
                new Tuple<int, int>(2,2),
                new Tuple<int, int>(7,1),
                new Tuple<int, int>(11,6),
                new Tuple<int, int>(5,3)
            };

        static readonly List<Tuple<int, int>> taskPoses = new List<Tuple<int, int>>()
            {
                new Tuple<int, int>(1,0),
                new Tuple<int, int>(5,0),
                new Tuple<int, int>(10,0),
            };

        /*static readonly List<Tuple<int, int>> botPoses = new List<Tuple<int, int>>()
            {
                new Tuple<int, int>(1,2),
                new Tuple<int, int>(2,9),
                new Tuple<int, int>(3,3),
                new Tuple<int, int>(3,4),
                new Tuple<int, int>(4,18),
                new Tuple<int, int>(5,1),
                new Tuple<int, int>(7,2),
                new Tuple<int, int>(7,4),
                new Tuple<int, int>(7,16),
                new Tuple<int, int>(9,12),
                new Tuple<int, int>(11,2),
                new Tuple<int, int>(13,8),
                new Tuple<int, int>(14,4),
                new Tuple<int, int>(15,17),
                new Tuple<int, int>(16,12),
                new Tuple<int, int>(18,4),
                new Tuple<int, int>(19,8),
                new Tuple<int, int>(23,2),
                new Tuple<int, int>(23,7),
                new Tuple<int, int>(23,9),
                new Tuple<int, int>(23,14),
                new Tuple<int, int>(25,12),
            };

        static readonly List<Tuple<int, int>> taskPoses = new List<Tuple<int, int>>()
            {
                new Tuple<int, int>(1,0),
                new Tuple<int, int>(3,0),
                new Tuple<int, int>(4,0),
                new Tuple<int, int>(8,0),
                new Tuple<int, int>(9,0),
                new Tuple<int, int>(10,0),
                new Tuple<int, int>(16,0),
            };
            */
        #endregion InputData

        static void Main()
        {
            //BotAssignerCP.RunWraper(botPoses, taskPoses); // General Solution with original CP solver  // https://developers.google.com/optimization/cp/cp_solver#original_cp_example
            //BotAssignerGraph.RunWrapper(botPoses, taskPoses); // Solution using Graph Solver // https://developers.google.com/optimization/assignment/simple_assignment#overview
            BotAssignerSAT.RunWraper(botPoses, taskPoses); // General Solution with CP-SAT solver // https://developers.google.com/optimization/assignment/assignment_cp

            Console.Read();
        }
    }
}
