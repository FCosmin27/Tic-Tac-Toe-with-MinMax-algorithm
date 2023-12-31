﻿/**************************************************************************
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

        public delegate int EvaluationFunction(Board board);

        public int PieceCount
        {
            get { return Pieces.Count; }
        }

        public Board()
        {
            Size = 3;
            Pieces = new List<Piece>(Size * Size);
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
        }

        public void AddPiece(int x, int y, PlayerType player)
        {
            Pieces.Add(new Piece(x, y, Pieces.Count, player));
        }
        /// <summary>
        /// Creeaza o noua configuratie aplicand mutarea si tipul jucatolui primiti ca parametrii in configuratia curenta
        /// </summary>
        public Board MakeMove(Move move, PlayerType player)
        {
            Board nextBoard = new Board(this);
            nextBoard.Pieces = new List<Piece>(Pieces);// Create a copy of the current board
            nextBoard.Pieces.Add(new Piece (move.NewX, move.NewY, Pieces.Count, player )); // Make the move for the specified player
            return nextBoard;
        }
        /// <summary>
        /// Metoda care determina daca casuta este libera
        /// </summary>
        public bool IsEmptyCell(Board board, int x, int y)
        {
            // Check if there is any piece in the specified cell
            return !board.Pieces.Any(p => p.X == x && p.Y == y);
        }
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
        /// <summary>
        /// Calculeaza functia care verifica ce player a castigat
        /// </summary>
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
        /// Calculeaza functia de evaluare statica pentru configuratia (tabla) curenta luand in considerare fiecare linie/diagonala
        /// </summary>
        public int EasyDifficultyEvaluationFunction(Board board)
        {
            int score = 0;
            for (int i = 0; i < board.Size; i++)
            {
                // Evaluate rows
                score += EvaluateLine(board.Pieces.Where(p => p.X == i));
                // Evaluate columns
                score += EvaluateLine(board.Pieces.Where(p => p.Y == i));
            }

            // Evaluate diagonals
            score += EvaluateLine(board.Pieces.Where(p => p.X == p.Y));
            score += EvaluateLine(board.Pieces.Where(p => p.X + p.Y == board.Size - 1));
            return score;
        }
        /// <summary>
        /// Calculeaza functia de evaluare statica pentru fiecare linie de pe tabla de jos
        /// </summary>
        private int EvaluateLine(IEnumerable<Piece> line)
        {
            int computerCount = line.Count(p => p.Player == PlayerType.Computer);
            int humanCount = line.Count(p => p.Player == PlayerType.Human);

            if (computerCount == Size)
                return 100; // Win
            else if (humanCount == Size)
                return -100; // Opponent win
            else if (computerCount == Size - 1 && humanCount == 0)
                return 10; // Two in a row, with third cell empty
            else if (computerCount == 0 && humanCount == Size - 1)
                return -10; // Opponent has two in a row, prioritize blocking winning move of opponent
            else
                return 0; // Neutral or no advantage
        }
        /// <summary>
        /// Calculeaza functia de evaluare statica pentru configuratia (tabla) curenta
        /// </summary>
        public int HardDifficultyEvaluationFunction(Board board)
        {
            int computerScore = 0;
            int humanScore = 0;

            // Evaluate control of strategic positions
            var strategicScores = EvaluateStrategicPositions(board);
            computerScore += strategicScores.computer;
            humanScore += strategicScores.human;

            var potentialLineScores = EvaluatePotentialWinningLines(board);
            computerScore += potentialLineScores.computer;
            humanScore += potentialLineScores.human;

            var blockingScores = EvaluateBlockingAndThreats(board);
            computerScore += blockingScores.computer;
            humanScore += blockingScores.human;


            // The final score is the difference between the computer's and human's scores
            return computerScore - humanScore;
        }
        /// <summary>
        /// Verificararea centrului sau a colturilor pentru o potentiala miscare 
        /// </summary>
        private (int computer, int human) EvaluateStrategicPositions(Board board)
        {
            int computerScore = 0;
            int humanScore = 0;

            // If it's the computer's first move, prioritize the center.
            if (!IsEmptyCell(board, 1, 1))
            {
                if (GetOccupant(board, 1, 1) == PlayerType.Computer)
                    computerScore += 10;
                else
                    humanScore += 10;
            }

            // Evaluate corners control
            var corners = new List<(int, int)> { (0, 0), (0, 2), (2, 0), (2, 2) };
            foreach (var corner in corners)
            {
                if (!IsEmptyCell(board, corner.Item1, corner.Item2))
                {
                    if (GetOccupant(board, corner.Item1, corner.Item2) == PlayerType.Computer)
                        computerScore += 3;
                    else
                        humanScore += 3;
                }
            }

            return (computerScore, humanScore);
        }
        /// <summary>
        /// Calcularea scorului pentru liniile ce ar putea fi considerate urmatoarea miscare
        /// </summary>
        private (int computer, int human) EvaluatePotentialWinningLines(Board board)
        {
            int computerScore = 0;
            int humanScore = 0;

            foreach (var line in GetLines(board))
            {
                var scores = EvaluatePotentialLine(line);
                computerScore += scores.computer;
                humanScore += scores.human;
            }

            return (computerScore, humanScore);
        }
        /// <summary>
        /// Evaluarea liniei pentru a verifica daca poate fi luata in considerare ca fiind urmatoarea miscare
        /// </summary>
        private (int computer, int human) EvaluatePotentialLine(IEnumerable<Piece> line)
        {
            int computerCount = line.Count(p => p.Player == PlayerType.Computer);
            int humanCount = line.Count(p => p.Player == PlayerType.Human);

            if (computerCount > 0 && humanCount == 0)
            {
                // More weight to lines where the computer has more pieces
                return (computerCount * 2, 0);
            }
            else if(humanCount > 0 && computerCount == 0)
            {
                // Score for the human player
                return (0, humanCount * 2);
            }

            return (0, 0);
        }

        /// <summary>
        /// Verificarea tuturor liniilor pentru a scoate in evidenta posibile avantaje ale oponentului
        /// </summary>
        private (int computer, int human) EvaluateBlockingAndThreats(Board board)
        {
            int computerScore = 0;
            int humanScore = 0;

            foreach (var line in GetLines(board))
            {
                var scores = EvaluateBlockingLine(line);
                computerScore += scores.computer;
                humanScore += scores.human;
            }

            return (computerScore, humanScore);
        }
        /// <summary>
        /// Verificarea liniei cu scop de a scoate in evidenta o miscare castigatoare a oponentului
        /// </summary>
        private (int computer, int human) EvaluateBlockingLine(IEnumerable<Piece> line)
        {
            int computerCount = line.Count(p => p.Player == PlayerType.Computer);
            int humanCount = line.Count(p => p.Player == PlayerType.Human);

            if (humanCount == Size - 1 && computerCount == 0)
            {
                // High score for the computer blocking opponent's almost complete line
                return (5, 0);
            }
            else if (computerCount == Size - 1 && humanCount == 0)
            {
                // Score for the human player blocking the computer's almost complete line
                return (0, 5);
            }

            return (0, 0);
        }
        /// <summary>
        /// Checks if there is an immediate win available for the specified player.
        /// </summary>
        /// <param name="player">The player to check for a winning move.</param>
        /// <param name="winningMove">The move that will lead to an immediate win.</param>
        /// <returns>true if there is a winning move; otherwise, false.</returns>
        public bool HasImmediateWinningMove(PlayerType player, out Move winningMove)
        {
            winningMove = null;
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    if (IsEmptyCell(x, y))
                    {
                        var tempMove = new Move(x, y);
                        var tempBoard = MakeMove(tempMove, player);

                        if (tempBoard.HasPlayerWon(player))
                        {
                            winningMove = tempMove;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Returneaza type-ul player-ului ce ocupa casuta respectiva
        /// </summary>
        public PlayerType? GetOccupant(Board board, int x, int y)
        {
            var piece = board.Pieces.FirstOrDefault(p => p.X == x && p.Y == y);
            return piece != null ? (PlayerType?)piece.Player : null;
        }
        /// <summary>
        /// Returneaza toate liniile
        /// </summary>
        private IEnumerable<IEnumerable<Piece>> GetLines(Board board)
        {
            // Rows
            for (int i = 0; i < Size; i++)
            {
                yield return board.Pieces.Where(p => p.X == i);
            }

            // Columns
            for (int i = 0; i < Size; i++)
            {
                yield return board.Pieces.Where(p => p.Y == i);
            }

            // Main diagonal
            yield return board.Pieces.Where(p => p.X == p.Y);

            // Anti-diagonal
            yield return board.Pieces.Where(p => p.X + p.Y == Size - 1);
        }

    }
}