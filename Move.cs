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

namespace MinMaxXO
{
    /// <summary>
    /// Reprezinta mutarea unei singure piese
    /// </summary>
    public class Move
    {
        //public int PieceId { get; set; } // id-ul piesei mutate
        public int NewX { get; set; } // noua pozitie X
        public int NewY { get; set; } // noua pozitie Y

        public double Score { get; set; }

        public Move(int newX, int newY)
        {
            //PieceId = pieceId;
            NewX = newX;
            NewY = newY;
        }
    }
}