/**************************************************************************
 *                                                                        *
 *  Copyright:   (c) 2016-2020, Florin Leon                               *
 *  E-mail:      florin.leon@academic.tuiasi.ro                           *
 *  Website:     http://florinleon.byethost24.com/lab_ia.html             *
 *  Description: Game playing. Minimax algorithm                          *
 *               (Artificial Intelligence lab 7)                          *
 *                                                                        *
 *  This code and information is provided "as is" without warranty of     *
 *  any kind, either expressed or implied, including but not limited      *
 *  to the implied warranties of merchantability or fitness for a         *
 *  particular purpose. You are free to use this source code in your      *
 *  applications as long as the original copyright notice is included.    *
 *                                                                        *
 **************************************************************************/

using System.Collections.Generic;
using System.Linq;

namespace MinMaxXO
{
    /// <summary>
    /// Reprezinta o configuratie a jocului (o tabla de joc) la un moment dat
    /// </summary>
    public partial class Board
    {
        public int Size { get; set; } // dimensiunea tablei de joc
        public List<Piece> Pieces { get; set; } // lista de piese, atat ale omului cat si ale calculatorului

        public double Score { get; set; }

        public Board()
        {
            Size = 3;
            Pieces = new List<Piece>(Size * Size);
        }

        public void AddPiece(int x, int y, PlayerType player)
        {
            Pieces.Add(new Piece(x, y, Pieces.Count, player));
        }

        public Board(Board b)
        {
            Size = b.Size;
            Pieces = new List<Piece>(Size * Size);

            foreach (Piece p in b.Pieces)
                Pieces.Add(new Piece(p.X, p.Y, p.Id, p.Player));
        }

        public Board(Board b, double score)
        {
            Size = b.Size;
            Pieces = new List<Piece>(Size * Size);

            foreach (Piece p in b.Pieces)
                Pieces.Add(new Piece(p.X, p.Y, p.Id, p.Player));

            Score = score;
        }
        /// <summary>
        /// Creeaza o noua configuratie aplicand mutarea primita ca parametru in configuratia curenta
        /// </summary>
        public Board MakeMove(Move move)
        {
            Board nextBoard = new Board(this); // copy
            nextBoard.AddPiece(move.NewX, move.NewY, PlayerType.Computer);
            return nextBoard;
        }

        public void UndoMove(Move move)
        {
            var pieceToUndo = Pieces.LastOrDefault(p => p.X == move.NewX && p.Y == move.NewY);
            if (pieceToUndo != null)
            {
                Pieces.Remove(pieceToUndo);
            }
        }

        /// <summary>
        /// Metoda care determina daca casuta este libera
        /// </summary>

        public bool IsEmptyCell(int x, int y)
        {
            // Check if there is any piece in the specified cell
            return !Pieces.Any(p => p.X == x && p.Y == y);
        }

        /// <summary>
        /// Verifica daca configuratia curenta este castigatoare
        /// </summary>
        /// <param name="finished">Este true daca cineva a castigat si false altfel</param>
        /// <param name="winner">Cine a castigat: omul sau calculatorul</param>
        public void CheckFinish(out bool finished, out PlayerType winner)
        {
            finished = false;
            winner = PlayerType.None;

            // Check all win conditions for each player
            foreach (var player in new PlayerType[] { PlayerType.Human, PlayerType.Computer })
            {
                // Check rows, columns, and diagonals for a win
                if (HasPlayerWon(player))
                {
                    finished = true;
                    winner = player;
                    return;
                }
            }

            // Check for a draw (board is full and no winner)
            if (Pieces.Count == Size * Size)
            {
                finished = true;
            }
        }

        private bool HasPlayerWon(PlayerType player)
        {
            var playerPieces = Pieces.Where(p => p.Player == player).ToList();

            // Check rows and columns
            for (int i = 0; i < Size; i++)
            {
                if (playerPieces.Count(p => p.X == i) == Size || playerPieces.Count(p => p.Y == i) == Size)
                    return true;
            }

            // Check diagonals
            if (playerPieces.Count(p => p.X == p.Y) == Size || // Diagonal from top-left to bottom-right
                playerPieces.Count(p => p.X + p.Y == Size - 1) == Size) // Diagonal from top-right to bottom-left
                return true;

            return false;
        }

        /// <summary>
        /// Calculeaza functia de evaluare statica pentru configuratia (tabla) curenta
        /// </summary>
        /// 
        public int Evaluate()
        {
            int score = 0;
            // Evaluate rows
            for (int i = 0; i < Size; i++)
            {
                score += EvaluateLine(Pieces.Where(p => p.X == i));
            }
            // Evaluate columns
            for (int i = 0; i < Size; i++)
            {
                score += EvaluateLine(Pieces.Where(p => p.Y == i));
            }
            // Evaluate diagonals
            score += EvaluateLine(Pieces.Where(p => p.X == p.Y));
            score += EvaluateLine(Pieces.Where(p => p.X + p.Y == Size - 1));

            return score;
        }

        private int EvaluateLine(IEnumerable<Piece> line)
        {
            int playerCount = line.Count(p => p.Player == PlayerType.Computer);
            int opponentCount = line.Count(p => p.Player == PlayerType.Human);

            if (playerCount == Size)
                return 100; // Win
            else if (opponentCount == Size)
                return -100; // Opponent win
            else if (playerCount == Size - 1 && opponentCount == 0)
                return 10; // Two in a row, with third cell empty
            else if (playerCount == 0 && opponentCount == Size - 1)
                return -15; // Opponent has two in a row
            else
                return 0; // Neutral or no advantage
        }
    }
}