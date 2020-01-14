using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Google.OrTools.Sat;

namespace Examples
{
    class BotAssignerSAT
    {
        public static void RunWraper(List<Tuple<int, int>> botPoses, List<Tuple<int, int>> taskPoses)
        {
            // Generate costs matrix from input poses
            int[][] costs = BuildCostMatrix(taskPoses, botPoses);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Main method
            var result = Run(costs, out string statistic, out double cost);

            stopwatch.Stop();
            Console.WriteLine(" *** Final Search time {0}", stopwatch.Elapsed);
            Console.WriteLine(" ");

            foreach (var asgmt in result)
            {
                Console.WriteLine($" Task {asgmt.Item2} assigned to the bot {asgmt.Item1}");
            }

            Console.WriteLine(" ");
            Console.WriteLine($"The best cost = {cost}");
            Console.WriteLine(" ");
            Console.WriteLine("*** Slover statistic:");
            Console.WriteLine(statistic);
        }
        private static List<Tuple<int, int>> Run(int[][] costs, out string statistic, out double bestCost)
        {
            var result = new List<Tuple<int, int>>();

            var botsCnt = costs.Length;
            var tasksCnt = costs[0].Length;

            CpModel model = new CpModel();

            // Decision variables
            IntVar[,] x = new IntVar[botsCnt, tasksCnt];

            for (int i = 0; i < botsCnt; i++)
            {
                for (int j = 0; j < tasksCnt; j++)
                {
                    x[i, j] = model.NewIntVar(0, 1, "x");
                }
            }
            // Constraints
            if (botsCnt > tasksCnt)
            {
                // Each bot is assigned to at most 1 task
                for (int i = 0; i < botsCnt; i++)
                {
                    model.Add((from j in Enumerable.Range(0, tasksCnt)
                               select x[i, j]).ToArray().Sum() <= 1);
                }

                // Each task is assigned to exactly one bot
                for (int j = 0; j < tasksCnt; j++)
                {
                    model.Add((from i in Enumerable.Range(0, botsCnt)
                               select x[i, j]).ToArray().Sum() == 1);
                }
            }
            else
            {
                // Each task is assigned to exactly one bot
                for (int i = 0; i < botsCnt; i++)
                {
                    model.Add((from j in Enumerable.Range(0, tasksCnt)
                               select x[i, j]).ToArray().Sum() == 1);
                }

                // Each bot is assigned to at most 1 task
                for (int j = 0; j < tasksCnt; j++)
                {
                    model.Add((from i in Enumerable.Range(0, botsCnt)
                               select x[i, j]).ToArray().Sum() <= 1);
                }
            }
            // Objective goals
            model.Minimize(new SumArray((from i in Enumerable.Range(0, botsCnt)
                                         from j in Enumerable.Range(0, tasksCnt)
                                         select (costs[i][j] * x[i, j]))));

            // Solver
            CpSolver solver = new CpSolver();
            var solveStatus = solver.Solve(model);

            // Fetching results
            for (int i = 0; i < botsCnt; i++)
            {
                for (int j = 0; j < tasksCnt; j++)
                {
                    if (solver.Value(x[i, j]) == 1)
                    {
                        result.Add(new Tuple<int, int>(j, i));
                    }
                }
            }
            bestCost = solver.ObjectiveValue;
            statistic = solver.ResponseStats();

            return result;
        }

        private static int CalcDistance(Tuple<int, int> a, Tuple<int, int> b)
        {
            return Math.Abs(b.Item1 - a.Item1) + Math.Abs(b.Item2 - a.Item2);
        }

        // Build Costs Matrix based on Manhattan Distance between Bot pose (x,y) and Task item pose (x,y)
        public static int[][] BuildCostMatrix(List<Tuple<int, int>> taskPoses, List<Tuple<int, int>> botPoses)
        {
            int[][] costMatrix = new int[botPoses.Count][];

            for (int bp = 0; bp < botPoses.Count; bp++)
            {
                costMatrix[bp] = new int[taskPoses.Count];
                for (int tp = 0; tp < taskPoses.Count; tp++)
                {
                    costMatrix[bp][tp] = CalcDistance(botPoses[bp], taskPoses[tp]);
                }
            }

            // Display the costs matrix (test)
            Console.WriteLine("Costs Matrix:");
            foreach (var row in costMatrix)
            {
                foreach (var col in row)
                {
                    Console.Write("{0,2} ", col);
                }
                Console.WriteLine(Environment.NewLine);
            }

            return costMatrix;
        }

    }
}
