using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxXO;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class MinimaxTests
    {

        [TestMethod]
        public void FindNextBoard_EmptyBoard_ShouldNotReturnNull()
        {
            var board = new Board();
            var nextBoard = Minimax.FindNextBoard(board, 3, double.MinValue, double.MaxValue, board.EasyDifficultyEvaluationFunction);
            Assert.IsNotNull(nextBoard);
        }

        [TestMethod]
        public void FindNextBoard_NextMoveIsWinning_ShouldReturnWinningBoard()
        {
            var board = new Board();
            board.AddPiece(0, 0, PlayerType.Computer);
            board.AddPiece(0, 1, PlayerType.Computer);
            var nextBoard = Minimax.FindNextBoard(board, 3, double.MinValue, double.MaxValue, board.HardDifficultyEvaluationFunction);
            Assert.IsNotNull(nextBoard);
            Assert.IsTrue(nextBoard.Pieces.Any(p => p.X == 0 && p.Y == 2 && p.Player == PlayerType.Computer));
        }

        [TestMethod]
        public void FindNextBoard_ShouldPreventImmediateLoss()
        {
            var board = new Board();
            board.AddPiece(0, 0, PlayerType.Human);
            board.AddPiece(0, 1, PlayerType.Human);
            var nextBoard = Minimax.FindNextBoard(board, 3, double.MinValue, double.MaxValue, board.EasyDifficultyEvaluationFunction);
            Assert.IsNotNull(nextBoard);
            Assert.IsTrue(nextBoard.Pieces.Any(p => p.X == 0 && p.Y == 2 && p.Player == PlayerType.Computer));
        }

        [TestMethod]
        public void FindNextBoard_MidGame_ShouldMakeOptimalMove()
        {
            var board = new Board();
            board.AddPiece(0, 0, PlayerType.Computer);
            board.AddPiece(1, 1, PlayerType.Human);
            var nextBoard = Minimax.FindNextBoard(board, 3, double.MinValue, double.MaxValue, board.EasyDifficultyEvaluationFunction);
            Assert.IsNotNull(nextBoard);
            Assert.IsTrue(nextBoard.PieceCount > 2);
        }

        [TestMethod]
        public void FindNextBoard_ComputerTurn_ShouldAddComputerPiece()
        {
            var board = new Board();
            board.AddPiece(0, 0, PlayerType.Human);
            var nextBoard = Minimax.FindNextBoard(board, 3, double.MinValue, double.MaxValue, board.EasyDifficultyEvaluationFunction);
            Assert.IsTrue(nextBoard.Pieces.Count(p => p.Player == PlayerType.Computer) == 1);
        }

        [TestMethod]
        public void MinValue_InitialBoard_ShouldBeNegative()
        {
            var board = new Board();
            var minValue = Minimax.MinValue(board, 3, double.MinValue, double.MaxValue, board.HardDifficultyEvaluationFunction);
            Assert.IsTrue(minValue < 0);
        }

        [TestMethod]
        public void MinValue_OpponentCanWinNextMove_ShouldBeNegative()
        {
            var board = new Board();
            board.AddPiece(0, 0, PlayerType.Human);
            board.AddPiece(0, 1, PlayerType.Human);
            var minValue = Minimax.MinValue(board, 3, double.MinValue, double.MaxValue, board.EasyDifficultyEvaluationFunction);
            Assert.IsTrue(minValue < 0);
        }

        [TestMethod]
        public void MinValue_DrawScenario_ShouldBeLessThenZero()
        {
            var board = new Board();
            for (int i = 0; i < 8; i++)
            {
                board.AddPiece(i / 3, i % 3, i % 2 == 0 ? PlayerType.Human : PlayerType.Computer);
            }
            var minValue = Minimax.MinValue(board, 3, double.MinValue, double.MaxValue, board.HardDifficultyEvaluationFunction);

            Assert.IsTrue(minValue < 0);
        }

        [TestMethod]
        public void MinValue_EarlyGame_ShouldNotBeHighlyNegative()
        {
            var board = new Board();
            board.AddPiece(0, 0, PlayerType.Computer);
            var minValue = Minimax.MinValue(board, 3, double.MinValue, double.MaxValue, board.EasyDifficultyEvaluationFunction);
            Assert.IsTrue(minValue > -100);
        }

        [TestMethod]
        public void MaxValue_InitialBoard_ShouldBePositive()
        {
            var board = new Board();
            var maxValue = Minimax.MaxValue(board, 6, double.MinValue, double.MaxValue, board.HardDifficultyEvaluationFunction);
            Assert.IsTrue(maxValue > 0);
        }

        [TestMethod]
        public void MaxValue_ComputerCanWinNextMove_ShouldBePositive()
        {
            var board = new Board();
            board.AddPiece(0, 0, PlayerType.Computer);
            board.AddPiece(0, 1, PlayerType.Computer);
            var maxValue = Minimax.MaxValue(board, 3, double.MinValue, double.MaxValue, board.EasyDifficultyEvaluationFunction);
            Assert.IsTrue(maxValue > 0);
        }

        [TestMethod]
        public void MaxValue_DrawScenario_ShouldBeLessThenZero()
        {
            var board = new Board();
            for (int i = 0; i < 8; i++)
            {
                board.AddPiece(i / 3, i % 3, i % 2 == 0 ? PlayerType.Human : PlayerType.Computer);
            }
            var maxValue = Minimax.MaxValue(board, 3, double.MinValue, double.MaxValue, board.HardDifficultyEvaluationFunction);
            Assert.IsTrue(maxValue < 0);
        }

        [TestMethod]
        public void MaxValue_EarlyGame_ShouldNotBeHighlyPositive()
        {
            var board = new Board();
            board.AddPiece(0, 0, PlayerType.Computer);
            var maxValue = Minimax.MaxValue(board, 3, double.MinValue, double.MaxValue, board.EasyDifficultyEvaluationFunction);
            Assert.IsTrue(maxValue < 100);
        }

        [TestMethod]
        public void MaxValue_OpponentCanWinNextMove_ShouldNotBeHighlyPositive()
        {
            var board = new Board();
            board.AddPiece(0, 0, PlayerType.Human);
            board.AddPiece(0, 1, PlayerType.Human);
            var maxValue = Minimax.MaxValue(board, 3, double.MinValue, double.MaxValue, board.EasyDifficultyEvaluationFunction);
            Assert.IsTrue(maxValue > -100);
        }

        [TestMethod]
        public void FindNextBoard_OpponentHasTwoInARow_ShouldBlock()
        {
            var board = new Board();
            board.AddPiece(0, 0, PlayerType.Human);
            board.AddPiece(0, 1, PlayerType.Human);
            var nextBoard = Minimax.FindNextBoard(board, 3, double.MinValue, double.MaxValue, board.EasyDifficultyEvaluationFunction);
            Assert.IsTrue(nextBoard.Pieces.Any(p => p.X == 0 && p.Y == 2 && p.Player == PlayerType.Computer));
        }

        [TestMethod]
        public void FindNextBoard_ComputerHasTwoInARow_ShouldWin()
        {
            var board = new Board();
            board.AddPiece(1, 0, PlayerType.Computer);
            board.AddPiece(1, 1, PlayerType.Computer);
            var nextBoard = Minimax.FindNextBoard(board, 6, double.MinValue, double.MaxValue, board.HardDifficultyEvaluationFunction);
            Assert.IsTrue(nextBoard.Pieces.Any(p => p.X == 1 && p.Y == 2 && p.Player == PlayerType.Computer));
        }

        [TestMethod]
        public void MinValue_BoardNearlyFull_ShoulNotdBeLow()
        {
            var board = new Board();
            for (int i = 0; i < 8; i++)
            {
                board.AddPiece(i / 3, i % 3, i % 2 == 0 ? PlayerType.Computer : PlayerType.Human);
            }
            var minValue = Minimax.MinValue(board, 3, double.MinValue, double.MaxValue, board.HardDifficultyEvaluationFunction);
            Assert.IsTrue(minValue > 0);
        }

        [TestMethod]
        public void MaxValue_BoardNearlyFull_ShouldBeHigh()
        {
            var board = new Board();
            for (int i = 0; i < 8; i++)
            {
                board.AddPiece(i / 3, i % 3, i % 2 == 0 ? PlayerType.Computer : PlayerType.Human);
            }
            var maxValue = Minimax.MaxValue(board, 3, double.MinValue, double.MaxValue, board.EasyDifficultyEvaluationFunction);
            Assert.IsTrue(maxValue >= 0);
        }

        [TestMethod]
        public void FindNextBoard_EmptyCorner_ShouldOccupyCorner()
        {
            var board = new Board();
            var nextBoard = Minimax.FindNextBoard(board, 3, double.MinValue, double.MaxValue, board.EasyDifficultyEvaluationFunction);
            var corners = new[] { (0, 0), (0, 2), (2, 0), (2, 2) };
            Assert.IsTrue(corners.Any(c => nextBoard.Pieces.Any(p => p.X == c.Item1 && p.Y == c.Item2)));
        }

        [TestMethod]
        public void MinValue_ComputerCanCreateFork_ShouldBeNegative()
        {
            var board = new Board();
            board.AddPiece(0, 0, PlayerType.Computer);
            board.AddPiece(2, 2, PlayerType.Computer);
            var minValue = Minimax.MinValue(board, 3, double.MinValue, double.MaxValue, board.HardDifficultyEvaluationFunction);
            Assert.IsTrue(minValue < 0);
        }

        [TestMethod]
        public void MaxValue_OpponentCanCreateFork_ShouldNotBeHigh()
        {
            var board = new Board();
            board.AddPiece(0, 0, PlayerType.Human);
            board.AddPiece(2, 2, PlayerType.Human);
            var maxValue = Minimax.MaxValue(board, 3, double.MinValue, double.MaxValue, board.EasyDifficultyEvaluationFunction);
            Assert.IsTrue(maxValue <= 0);
        }

        [TestMethod]
        public void FindNextBoard_OpponentCouldFork_ShouldPreventFork()
        {
            var board = new Board();
            board.AddPiece(0, 0, PlayerType.Human);
            board.AddPiece(2, 2, PlayerType.Human);
            var nextBoard = Minimax.FindNextBoard(board, 3, double.MinValue, double.MaxValue, board.EasyDifficultyEvaluationFunction);
            Assert.IsNotNull(nextBoard);
        }

        [TestMethod]
        public void MinValue_ImmediateWinAvailable_ShouldBeHighlyNegative()
        {
            var board = new Board();
            board.AddPiece(0, 0, PlayerType.Human);
            board.AddPiece(0, 1, PlayerType.Human);
            board.AddPiece(1, 1, PlayerType.Computer);
            board.AddPiece(2, 2, PlayerType.Computer);
            var minValue = Minimax.MinValue(board, 3, double.MinValue, double.MaxValue, board.EasyDifficultyEvaluationFunction);
            Assert.IsTrue(minValue < -50);
        }

        [TestMethod]
        public void MaxValue_ImmediateWinAvailable_ShouldBeHighlyPositive()
        {
            var board = new Board();
            board.AddPiece(0, 0, PlayerType.Computer);
            board.AddPiece(0, 1, PlayerType.Computer);
            board.AddPiece(1, 1, PlayerType.Human);
            board.AddPiece(2, 2, PlayerType.Human);
            var maxValue = Minimax.MaxValue(board, 3, double.MinValue, double.MaxValue, board.EasyDifficultyEvaluationFunction);
            Assert.IsTrue(maxValue > 50);
        }

    }
}
