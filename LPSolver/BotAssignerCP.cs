using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Google.OrTools.ConstraintSolver;

namespace Examples
{
    class BotAssignerCP
    {
        private static List<Tuple<int, int>> output = new List<Tuple<int, int>>();

        public static void RunWraper(List<Tuple<int, int>> botPoses, List<Tuple<int, int>> taskPoses)
        {
            // Generate Matrix from Input
            int[][] costs = BotAssignerSAT.BuildCostMatrix(taskPoses, botPoses);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine(" *** Check for the best total cost: ");
            long bestCost = Run(costs);
            stopwatch.Stop();
            Console.WriteLine(" *** All Cases time {0}", stopwatch.Elapsed);
            Console.WriteLine("");
            Console.WriteLine(" *** Final Search *** ");
            stopwatch.Reset();
            stopwatch.Start();
            bestCost = Run(costs, bestCost);
            stopwatch.Stop();

            DisplayResults();

            Console.WriteLine(" *** Final Search time {0}", stopwatch.Elapsed);
        }

        private static long Run(int[][] costs, long bestCost = 0)
        {
            bool finalSearch = bestCost > 0;
            long result = 0;
            Solver solver = new Solver("Assignment");

            int bots = costs.Length; // rows
            int tasks = costs[0].Length; // cols

            // Decision variables
            IntVar[,] x = solver.MakeBoolVarMatrix(bots, tasks, "x");
            IntVar[] x_flat = x.Flatten();

            // Dynamic Constraints
            if (bots > tasks)
            {
                // Each bot is assigned to at most 1 task
                for (int i = 0; i < bots; i++)
                {
                    solver.Add((from j in Enumerable.Range(0, tasks)
                                select x[i, j]).ToArray().Sum() <= 1);
                }

                // Each task is assigned to exactly one bot
                for (int j = 0; j < tasks; j++)
                {
                    solver.Add((from i in Enumerable.Range(0, bots)
                                select x[i, j]).ToArray().Sum() == 1);
                }
            }
            else
            {
                // Each task is assigned to exactly one bot
                for (int i = 0; i < bots; i++)
                {
                    solver.Add((from j in Enumerable.Range(0, tasks)
                                select x[i, j]).ToArray().Sum() == 1);
                }

                // Each bot is assigned to at most 1 task
                for (int j = 0; j < tasks; j++)
                {
                    solver.Add((from i in Enumerable.Range(0, bots)
                                select x[i, j]).ToArray().Sum() <= 1);
                }
            }

            // Total cost
            IntVar total_cost = (from i in Enumerable.Range(0, bots)
                                 from j in Enumerable.Range(0, tasks)
                                 select (costs[i][j] * x[i, j])).ToArray().Sum().Var();

            if (bestCost > 0)
            {
                solver.Add(total_cost == bestCost);
            }

            // Search
            DecisionBuilder db = solver.MakePhase(x_flat,
                                                  Solver.INT_VAR_DEFAULT,
                                                  Solver.INT_VALUE_DEFAULT);

            if (!finalSearch)
            {
                // Objective
                OptimizeVar objective = total_cost.Minimize(1);
                solver.NewSearch(db, objective);
            }
            else
            {
                solver.NewSearch(db);
            }

            while (solver.NextSolution())
            {
                output.Clear();
                result = total_cost.Value();
                Console.WriteLine($"total_cost: {result}");
                if (finalSearch)
                {
                    for (int i = 0; i < bots; i++)
                    {
                        for (int j = 0; j < tasks; j++)
                        {
                            Console.Write(x[i, j].Value() + " ");
                        }
                        Console.WriteLine();
                    }
                }

                for (int i = 0; i < bots; i++)
                {
                    bool isAssigned = false;
                    for (int j = 0; j < tasks; j++)
                    {
                        if (x[i, j].Value() == 1)
                        {
                            output.Add(new Tuple<int, int>(i, j));
                            isAssigned = true;
                        }
                    }
                    if (!isAssigned) output.Add(new Tuple<int, int>(i, -1));
                }
            }

            Console.WriteLine("\nSolutions: {0}", solver.Solutions());
            Console.WriteLine("WallTime: {0}ms", solver.WallTime());
            Console.WriteLine("Failures: {0}", solver.Failures());
            Console.WriteLine("Branches: {0} ", solver.Branches());
            Console.WriteLine("Number of constraints = " + solver.Constraints());

            solver.EndSearch();
            return result;
        }

        private static void DisplayResults()
        {
            Console.WriteLine();
            Console.WriteLine("Assignments:");
            foreach (var a in output)
            {
                if (a.Item2 >= 0)
                {
                    Console.WriteLine($"Bot {a.Item1} is with task { a.Item2 }");
                }
                else
                {
                    Console.WriteLine($"Bot {a.Item1} is free");
                }
            }
            Console.WriteLine();
        }
    }
}
