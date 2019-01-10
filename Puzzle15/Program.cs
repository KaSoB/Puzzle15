using System;

namespace Puzzle15 {
    class Program {
        static void Main(string[] args) {
            AStarAlgorithm strategy = new AStarAlgorithm();

            int[] states = new int[] {
                1, 4, 3,
                2, 5, 8,
                7, -1, 6
            };

            Solve15PuzzleProblem(strategy, states, HeuristicMethod.ManhattanDistance);
            Solve15PuzzleProblem(strategy, states, HeuristicMethod.MisplacedTiles);

            Console.ReadKey();
        }

        static public void Solve15PuzzleProblem(AStarAlgorithm algorithm, int[] initStates, HeuristicMethod heuristic) {
            var results = algorithm.Run(initStates, heuristic);
            Console.WriteLine($"Heuristic method selected: {heuristic}");
            foreach (var item in results) {
                Console.WriteLine(item);
            }
            Console.WriteLine("End");
        }
    }
}
