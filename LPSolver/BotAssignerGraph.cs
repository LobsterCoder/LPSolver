using System;
using System.Collections.Generic;
using Google.OrTools.Graph;

namespace Examples
{
    class BotAssignerGraph
    {
        public static void RunWrapper(List<Tuple<int, int>> botPoses, List<Tuple<int, int>> taskPoses)
        {
            int[][] costs = BotAssignerSAT.BuildCostMatrix(taskPoses, botPoses);

            Run(costs);
        }
        private static void Run(int[][] costs)
        {
            var botsCount = costs.Length;
            var tasksCount = costs[0].Length;

            Console.WriteLine($"*** Start with Bots = {botsCount} , tasks = {tasksCount} ***");

            // Solver usage
            var assignment = new LinearSumAssignment();

            for (var bot = 0; bot < botsCount; bot++)
            {
                for (var task = 0; task < tasksCount; task++)
                {
                    assignment.AddArcWithCost(bot, task, costs[bot][task]);
                }
            }

            var solveStatus = assignment.Solve();

            // Check the result
            if (solveStatus == LinearSumAssignment.Status.OPTIMAL)
            {
                Console.WriteLine($" Optimal Cost = {assignment.OptimalCost()}");

                for (int i = 0; i < assignment.NumNodes(); i++)
                {
                    Console.WriteLine($"Task {assignment.RightMate(i)} assigned to Bot {i}, distance {assignment.AssignmentCost(i)}");
                }
            }
            else if (solveStatus == LinearSumAssignment.Status.INFEASIBLE)
            {
                Console.WriteLine("No assignment is possible.");
            }
            else if (solveStatus == LinearSumAssignment.Status.POSSIBLE_OVERFLOW)
            {
                Console.WriteLine("Some input costs are too large and may cause an integer overflow.");
            }
        }
    }
}
