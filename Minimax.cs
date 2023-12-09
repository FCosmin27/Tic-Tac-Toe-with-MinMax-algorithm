using System;
using System.Collections.Generic;
using System.Linq;

namespace MinMaxXO
{
    /// <summary>
    /// Implementeaza algoritmul de cautare a mutarii optime
    /// </summary>
    public partial class Minimax
    {
        private static Random _rand = new Random();

        /// <summary>
        /// Primeste o configuratie ca parametru, cauta mutarea optima si returneaza configuratia
        /// care rezulta prin aplicarea acestei mutari optime
        /// </summary>

        public static Board FindNextBoard(Board currentBoard, int depth, double alpha, double beta)
        {
            // If it's the computer's first move, prioritize the center.
            if (currentBoard.Pieces.Count == 0)
            {
                var centerMove = new Move(1, 1);
                var newBoard = currentBoard.MakeMove(centerMove);
                newBoard.Score = 100; // Arbitrary high score for center move
                return newBoard;
            }

            return Maximize(currentBoard, depth, alpha, beta);
        }

        private static Board Maximize(Board currentBoard, int depth, double alpha, double beta)
        {
            currentBoard.CheckFinish(out bool finished, out PlayerType winner);
            if (depth == 0 || finished)
            {
                return new Board(currentBoard, currentBoard.Evaluate());
            }

            double maxEval = double.MinValue;
            Board bestBoard = null;

            foreach (var move in GetAllValidMoves(currentBoard))
            {
                Board newBoard = currentBoard.MakeMove(move);
                double eval = MinValue(newBoard, depth - 1, alpha, beta);
                if (eval > maxEval)
                {
                    maxEval = eval;
                    bestBoard = newBoard;
                }
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha)
                    break;
            }
            return bestBoard != null ? new Board(bestBoard, maxEval) : null;
        }


        private static double MinValue(Board board, int depth, double alpha, double beta)
        {
            board.CheckFinish(out bool finished, out PlayerType winner);
            if (depth == 0 || finished)
            {
                return board.Evaluate();
            }

            double minEval = double.MaxValue;
            foreach (var move in GetAllValidMoves(board))
            {
                Board newBoard = board.MakeMove(move);
                double eval = MaxValue(newBoard, depth - 1, alpha, beta); // Recursive call to MaxValue

                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);
                if (beta <= alpha)
                    break; // Alpha-beta pruning
            }
            return minEval;
        }


        private static double MaxValue(Board board, int depth, double alpha, double beta)
        {
            board.CheckFinish(out bool finished, out PlayerType winner);
            if (depth == 0 || finished)
            {
                return board.Evaluate();
            }

            double maxEval = double.MinValue;
            foreach (var move in GetAllValidMoves(board))
            {
                Board newBoard = board.MakeMove(move);
                double eval = MinValue(newBoard, depth - 1, alpha, beta);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha)
                    break; // Alpha-beta pruning
            }
            return maxEval;
        }


        private static List<Move> GetAllValidMoves(Board board)
        {
            List<Move> validMoves = new List<Move>();
            for (int x = 0; x < board.Size; x++)
                for (int y = 0; y < board.Size; y++)
                    if (board.IsEmptyCell(x, y))
                        validMoves.Add(new Move(x, y));
            return validMoves;
        }

    }


}