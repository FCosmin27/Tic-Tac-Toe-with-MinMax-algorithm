using System;
using System.Collections.Generic;
using System.Linq;
using static MinMaxXO.Board;

namespace MinMaxXO
{
    /// <summary>
    /// Implementeaza algoritmul de cautare a mutarii optime
    /// </summary>
    public partial class Minimax
    {
        /// <summary>
        /// Primeste o configuratie ca parametru, cauta mutarea optima si returneaza configuratia
        /// care rezulta prin aplicarea acestei mutari optime
        /// </summary>
        public static Board FindNextBoard(Board currentBoard, int depth, double alpha, double beta, EvaluationFunction evaluationFunc)
        {
            currentBoard.CheckFinish(out bool finished, out PlayerType winner);
            if (depth == 0 || finished)
            {
                return new Board(currentBoard, evaluationFunc(currentBoard));
            }

            double maxEval = double.MinValue;
            Board bestBoard = null;

            foreach (var move in GetAllValidMoves(currentBoard))
            {
                Board newBoard = currentBoard.MakeMove(move, PlayerType.Computer);
                double eval = MinValue(newBoard, depth - 1, alpha, beta, evaluationFunc);
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
        /// <summary>
        /// Calculează scorul minim pentru adversar în algoritmul Minimax.
        /// </summary>
        public static double MinValue(Board board, int depth, double alpha, double beta, EvaluationFunction evaluationFunc)
        {
            board.CheckFinish(out bool finished, out PlayerType winner);
            if (depth == 0 || finished)
            {
                return evaluationFunc(board);
            }

            double minEval = double.MaxValue;
            foreach (var move in GetAllValidMoves(board))
            {
                Board newBoard = board.MakeMove(move, PlayerType.Human);
                double eval = MaxValue(newBoard, depth - 1, alpha, beta, evaluationFunc); // Recursive call to MaxValue

                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);
                if (beta <= alpha)
                    break; // Alpha-beta pruning
            }
            return minEval;
        }
        /// <summary>
        /// Obține scorul maxim posibil pentru calculator.
        /// </summary>
        public static double MaxValue(Board board, int depth, double alpha, double beta, EvaluationFunction evaluationFunc)
        {
            board.CheckFinish(out bool finished, out PlayerType winner);
            if (depth == 0 || finished)
            {
                return evaluationFunc(board);
            }

            double maxEval = double.MinValue;
            foreach (var move in GetAllValidMoves(board))
            {
                Board newBoard = board.MakeMove(move, PlayerType.Computer);
                double eval = MinValue(newBoard, depth - 1, alpha, beta, evaluationFunc);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha)
                    break; // Alpha-beta pruning
            }
            return maxEval;
        }
        /// <summary>
        /// Generează lista tuturor mutărilor valide disponibile.
        /// </summary>
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