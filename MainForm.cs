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

using System;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using static MinMaxXO.Board;

namespace MinMaxXO
{
    public partial class MainForm : Form
    {
        private const int SQUARE_DIM = 510;
        private const int CELL_SIZE = 170;
        private const int IMAGE_SIZE = 130;
        private const int OFFSET = (CELL_SIZE - IMAGE_SIZE) / 2;
        private Board _board;
        private PlayerType _currentPlayer; // om sau calculator
        private Bitmap _boardImage;
        private Bitmap _xImage;
        private Bitmap _oImage;
        private bool _playerStarts;
        private bool _playerTurn;
        private bool _gameOver;

        public MainForm()
        {
            InitializeComponent();

            try
            {
                _boardImage = (Bitmap)Image.FromFile("board.png");
                _xImage = (Bitmap)Image.FromFile("X.png");
                _oImage = (Bitmap)Image.FromFile("O.png");
            }
            catch
            {
                MessageBox.Show("Nu se poate incarca board.png");
                Environment.Exit(1);
            }

            _board = new Board();
            _currentPlayer = PlayerType.None;
            _playerStarts = true;
            _playerTurn = true;
            _gameOver = false;

            this.ClientSize = new System.Drawing.Size(927, 600);
            this.pictureBoxBoard.Size = new System.Drawing.Size(SQUARE_DIM, SQUARE_DIM);

            pictureBoxBoard.Refresh();
        }


        private void pictureBoxBoard_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = me.Location;

            // Calculate which cell is clicked
            int x = coordinates.X / CELL_SIZE;
            int y = coordinates.Y / CELL_SIZE;

            // Check if the cell is empty and it's the player's turn
            if (_board.IsEmptyCell(x, y) && _playerTurn)
            {
                _board.AddPiece(x, y, PlayerType.Human); // Add player's move
                _playerTurn = false;
                pictureBoxBoard.Invalidate(); // Redraw the board

                if (!_gameOver)
                {
                    ComputerMove();
                }
            }
        }

        private void pictureBoxBoard_Paint(object sender, PaintEventArgs e)
        {
            var board = new Bitmap(_boardImage);
            var g = e.Graphics;
            g.DrawImage(board, 0, 0);

            if (_board == null)
                return;


            foreach (var p in _board.Pieces)
            {
                var imageToDraw = (p.Player == PlayerType.Human)
                ? (_playerStarts ? _xImage : _oImage)
                : (_playerStarts ? _oImage : _xImage);

                g.DrawImage(imageToDraw, p.X * CELL_SIZE + OFFSET, p.Y * CELL_SIZE + OFFSET, IMAGE_SIZE, IMAGE_SIZE);
            }
        }

        private void ComputerMove()
        {
            Board nextBoard = Minimax.FindNextBoard(_board, 9 - _board.PieceCount, double.MinValue, double.MaxValue);

            _board = nextBoard;
            pictureBoxBoard.Refresh();

            _currentPlayer = PlayerType.Human;
            _playerTurn = true;
            CheckFinish();
        }

        private void CheckFinish()
        {
            PlayerType winner;
            _board.CheckFinish(out _gameOver, out winner);

            if (_gameOver)
            {
                if (winner == PlayerType.Computer)
                {
                    MessageBox.Show("Calculatorul a castigat!");
                    _currentPlayer = PlayerType.None;
                }
                else if (winner == PlayerType.Human)
                {
                    MessageBox.Show("Ai castigat!");
                    _currentPlayer = PlayerType.None;
                }
                else
                {
                    MessageBox.Show("Egalitate!");
                    _currentPlayer = PlayerType.None;
                }

            }
        }

        private void jocNouToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            JocNou();
        }

        private void despreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const string copyright =
                "Algoritmul minimax\r\n" +
                "Inteligenta artificiala, Laboratorul 8\r\n" +
                "(c)2016-2017 Florin Leon\r\n" +
                "http://florinleon.byethost24.com/lab_ia.htm";

            MessageBox.Show(copyright, "Despre jocul Dame simple");
        }

        private void iesireToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Environment.Exit(0);
        }

        private void dificultateUsorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _board.EvaluationFunc = _board.EasyDifficultyEvaluationFunction;
            JocNou();
        }

        private void dificultateGreuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _board.EvaluationFunc = _board.HardDifficultyEvaluationFunction;
            JocNou();
        }

        private void JocNou()
        {
            _board = new Board();
            _currentPlayer = PlayerType.Computer;
            _playerStarts = !_playerStarts;
            _playerTurn = _playerStarts;
            _gameOver = false;
            if (!_playerStarts)
                ComputerMove();
            pictureBoxBoard.Invalidate();
        }
    }
}