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
using System;

namespace MinMaxXO
{
    public enum PlayerType { None, Computer, Human };

    /// <summary>
    /// Reprezinta o piesa de joc
    /// </summary>
    public partial class Piece
    {
        public int Id { get; set; } // identificatorul piesei
        public int X { get; set; } // pozitia X pe tabla de joc
        public int Y { get; set; } // pozitia Y pe tabla de joc
        public PlayerType Player { get; set; } // carui tip de jucator apartine piesa (om sau calculator)

        public Piece(int x, int y, int id, PlayerType player)
        {
            X = x;
            Y = y;
            Id = id;
            Player = player;
        }
        /// <summary>
        /// Testeaza daca o mutare este valida intr-o anumita configuratie
        /// </summary>
        public bool IsValidMove(Board currentBoard, Move move)
        {
            // Check if the move is within the 3x3 grid
            if (move.NewX >= 3 || move.NewY >= 3) return false;

            // Check if the cell is already occupied
            return !currentBoard.Pieces.Any(piece => piece.X == move.NewX && piece.Y == move.NewY);
        }
    }
}