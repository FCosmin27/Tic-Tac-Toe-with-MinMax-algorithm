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
        public static Board FindNextBoard(Board currentBoard, int depth, double alpha, double beta)
        {
            // Check for an immediate winning move before evaluating other moves
            if (currentBoard.HasImmediateWinningMove(PlayerType.Computer, out Move winningMove))
            {
                var winningBoard = currentBoard.MakeMove(winningMove);
                winningBoard.Score = double.MaxValue; // Assign maximum score for a winning move
                return winningBoard;
            }

            // Check for an immediate threat and block it
            if (currentBoard.HasImmediateWinningMove(PlayerType.Human, out Move blockingMove))
            {
                var blockingBoard = currentBoard.MakeMove(blockingMove);
                blockingBoard.Score = double.MaxValue - 1; // High score for blocking an immediate threat
                return blockingBoard;
            }

            return Maximize(currentBoard, depth, alpha, beta);
        }
        /// <summary>
        /// Selectează cea mai bună mutare a calculatorului prin maximizarea scorului.
        /// </summary>
        private static Board Maximize(Board currentBoard, int depth, double alpha, double beta)
        {
            currentBoard.CheckFinish(out bool finished, out PlayerType winner);
            if (depth == 0 || finished)
            {
                return new Board(currentBoard, currentBoard.EvaluationFunc(), currentBoard.EvaluationFunc);
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
            return bestBoard != null ? new Board(bestBoard, maxEval, bestBoard.EvaluationFunc) : null;
        }
        /// <summary>
        /// Calculează scorul minim pentru adversar în algoritmul Minimax.
        /// </summary>
        private static double MinValue(Board board, int depth, double alpha, double beta)
        {
            board.CheckFinish(out bool finished, out PlayerType winner);
            if (depth == 0 || finished)
            {
                return board.EvaluationFunc();
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
        /// <summary>
        /// Obține scorul maxim posibil pentru calculator.
        /// </summary>
        private static double MaxValue(Board board, int depth, double alpha, double beta)
        {
            board.CheckFinish(out bool finished, out PlayerType winner);
            if (depth == 0 || finished)
            {
                return board.EvaluationFunc();
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