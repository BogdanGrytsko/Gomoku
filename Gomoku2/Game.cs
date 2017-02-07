using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gomoku2
{
    public class Game
    {
        private readonly int width;
        private readonly int height;
        private readonly BoardCell[,] board;
        private readonly Stopwatch sw;
        private readonly List<GameState> gameStates;
        private int lastEstimate;

        public Game(BoardCell[,] board)
        {
            width = board.GetLength(0);
            height = board.GetLength(1);
            this.board = (BoardCell[,])board.Clone();
            sw = new Stopwatch();
            gameStates = new List<GameState>();
        }

        public Game(int width, int height)
        {
            this.width = width;
            this.height = height;
            board = new BoardCell[width, height];
            sw = new Stopwatch();
            gameStates = new List<GameState>();
        }

        public EstimatedBoard EstimatedBoard
        {
            get
            {
                return new EstimatedBoard
                {
                    Board = (BoardCell[,]) board.Clone(),
                    Estimate = lastEstimate
                };
            }
        }

        public List<GameState> GameStates
        {
            get { return gameStates; }
        } 

        public void DoOpponentMove(int x, int y, BoardCell boardCell = BoardCell.Second)
        {
            board[x, y] = boardCell;
        }

        public Cell DoMove(BoardCell boardCell = BoardCell.First, int depth = 4, int treeMaxWidth = 16)
        {
            var move = DoMoveInternal(boardCell, depth, treeMaxWidth);
            board[move.X, move.Y] = boardCell;
            return move;
        }

        private Cell DoMoveInternal(BoardCell boardCell, int depth, int maxWidth)
        {
            sw.Start();
            var myLines = GetLines(boardCell);
            var oppLines = GetLines(boardCell.Opponent());
            if (!myLines.Any()) return FirstMoveCase();

            Cell move;
           //TODO needs to move inside AlphaBeta
            //if (WinInThisMove(myLines, BoardCell.First, out move)) return move;
            //if (WinInThisMove(oppLines, BoardCell.Second, out move)) return move;

            BoardState state;
            if (sw.Elapsed < TimeSpan.FromSeconds(5))
                state = new BoardState(myLines, oppLines, boardCell, depth, 0, maxWidth, board);
            else if (sw.Elapsed < TimeSpan.FromSeconds(6.6))
                state = new BoardState(myLines, oppLines, boardCell, depth - 1, 0, maxWidth, board);
            else
                state = new BoardState(myLines, oppLines, boardCell, depth - 2, 0, maxWidth, board);

            //if (GetBestFromNextMoves(state, out move)) return move;

            //todo we may want to remember history for perf improvement
            gameStates.Clear();
            lastEstimate = AlphaBeta(state, int.MinValue, int.MaxValue, out move, null);
            sw.Stop();
            return move;
        }

        private int AlphaBeta(BoardState state, int alpha, int beta, out Cell move, GameState parent)
        {
            var nextCells = state.GetNextCells();
            //if (state.IsTerminal) return LeafCase(state, alpha, beta, out move, parent);

            move = null;
            int bestEstim = state.StartEstimate;
           
            foreach (var estimatedCell in EstimateCells(state, nextCells))
            {
                var cell = estimatedCell.Cell;
                Cell bestMove;

                board[cell.X, cell.Y] = state.MyCellType;
                var currEstim = estimatedCell.Estimate * (state.ItIsFirstsTurn ? 1 : -1);

                var gameState = new GameState {BoardState = state.Clone(), Cell = cell, Estimate = currEstim};
                OnStateChanged(gameState, parent);
                var minMax = state.IsTerminal ? currEstim : AlphaBeta(state.GetNextState(estimatedCell.Lines), alpha, beta, out bestMove, gameState);
                //if (FiveInRow(tuple.Item3) || StraightFour(tuple.Item3))
                //    minMax = currEstim;
                //else
                //    minMax = AlphaBeta(state.GetNextState(tuple.Item2), alpha, beta, out bestMove);
                gameState.Estimate = minMax;

                if (state.ItIsFirstsTurn && minMax > bestEstim)
                {
                    bestEstim = minMax;
                    alpha = minMax;
                    move = cell;
                }
                if (!state.ItIsFirstsTurn && minMax < bestEstim)
                {
                    bestEstim = minMax;
                    beta = minMax;
                    move = cell;
                }
                board[cell.X, cell.Y] = BoardCell.None;
                if (BreakOnFive(state.ItIsFirstsTurn, minMax) || BreakOnStraightFour(state.ItIsFirstsTurn, minMax)
                    || beta <= alpha) break;
            }
            return bestEstim;
        }

        private static bool FiveInRow(int estim)
        {
            return Math.Abs(estim) >= (int)LineType.FiveInRow / 2;
        }

        private static bool StraightFour(int estim)
        {
            return Math.Abs(estim) >= (int)LineType.StraightFour / 2;
        }

        private static bool BreakOnFive(bool movesFirst, int estim)
        {
            return (estim <= -(int)LineType.FiveInRow / 2 && !movesFirst) || (estim >= (int)LineType.FiveInRow / 2 && movesFirst);
        }

        private static bool BreakOnStraightFour(bool movesFirst, int estim)
        {
            return (estim <= -(int)LineType.StraightFour / 2 && !movesFirst) || (estim >= (int)LineType.StraightFour / 2 && movesFirst);
        }

        private void OnStateChanged(GameState gameState, GameState parentState)
        {
            if (parentState == null)
            {
                gameStates.Add(gameState);
            }
            else
            {
                parentState.Children.Add(gameState);
            }
        }

        private Cell FirstMoveCase()
        {
            return board[7, 7] == BoardCell.None ? CellManager.Get(7, 7) : CellManager.Get(8, 8);
        }

        //private bool GetBestFromNextMoves(BoardState state, out Cell move)
        //{
        //    move = null;
        //    MoveResult myThree = MoveResult.NotFound, oppThree = MoveResult.NotFound;
        //    if (OneMoveForwardFour(state, ref myThree, ref move)) return true;
        //    if (DefendFromOpenedThree(state, out move)) return true;
        //    if (OneMoveForwardFour(state.Switch(), ref oppThree, ref move)) return true;
        //    if (myThree.Found)
        //    {
        //        move = myThree.Move;
        //        return true;
        //    }
        //    if (oppThree.Found)
        //    {
        //        move = oppThree.Move;
        //        return true;
        //    }
        //    return false;
        //}

        //private bool OneMoveForwardFour(
        //    BoardState state,
        //    ref MoveResult myThree,
        //    ref Cell move)
        //{
        //    foreach (var line in state.MyLines)
        //    {
        //        var moves = line.GetTwoNextCells(board);
        //        var res = FindBestMove(state, moves.Item1);
        //        if (LineFour(ref move, res)) return true;
        //        if (res.Found) myThree = res;
        //        res = FindBestMove(state, moves.Item2);
        //        if (LineFour(ref move, res)) return true;
        //        if (res.Found) myThree = res;
        //    }
        //    foreach (var cell in state.GetNearEmptyCells())
        //    {
        //        var res = FindBestMove(state, cell);
        //        if (LineFour(ref move, res)) return true;
        //        if (res.Found) myThree = res;
        //    }
        //    return false;
        //}

        //private static bool LineFour(ref Cell move, MoveResult res)
        //{
        //    if (res.FoundFour)
        //    {
        //        move = res.Move;
        //        return true;
        //    }
        //    return false;
        //}

        //private bool DefendFromOpenedThree(BoardState state, out Cell move)
        //{
        //    move = null;
        //    var cellsToDefend = new List<Cell>();
        //    foreach (var oppLine in state.OppLines)
        //    {
        //        Tuple<Cell, Cell> moves;
        //        if (ThreeInRowWithTwoPossibleMovesCase(oppLine, out moves))
        //        {
        //            cellsToDefend.Add(moves.Item1);
        //            cellsToDefend.Add(moves.Item2);
        //        }
        //    }
        //    if (cellsToDefend.Any())
        //    {
        //        AlphaBeta(state, int.MinValue, int.MaxValue, out move, cellsToDefend);
        //        return true;
        //    }
        //    return false;
        //}

        private int SumLines(List<Line> lines, BoardCell type)
        {
            var estims = lines.Select(l => l.Estimate(board, type)).ToList();
            int killerLines = 0;
            foreach (var lineType in estims)
            {
                if (ThreatOfThree(lineType) || ThreatOfFour(lineType)) killerLines++;
            }
            if (killerLines >= 2) return (int)LineType.DoubleThreat + estims.Sum(es => (int)es);
            return estims.Sum(es => (int)es);
        }

        public static bool ThreatOfFour(LineType lineType)
        {
            return lineType == LineType.FourInRow || lineType == LineType.BrokenFourInRow;
        }

        //private bool WinInThisMove(List<Line> lines, BoardCell type, out Cell move)
        //{
        //    move = null;
        //    foreach (var line in lines)
        //    {
        //        var next = line.GetTwoNextCells(board);
        //        var res1 = CanDoWinMove(lines, next.Item1, type);
        //        if (res1.Found)
        //        {
        //            move = res1.Move;
        //            return true;
        //        }
        //        var res2 = CanDoWinMove(lines, next.Item2, type);
        //        if (res2.Found)
        //        {
        //            move = res2.Move;
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //private MoveResult CanDoWinMove(List<Line> existingLines, Cell proposedMove, BoardCell type)
        //{
        //    if (proposedMove == null) return MoveResult.NotFound;
        //    board[proposedMove.X, proposedMove.Y] = type;

        //    var lines = GetLinesByAddingCell(proposedMove, existingLines);
        //    if (lines.Any(l => l.Count >= 5)) return RevertAndReturn(proposedMove, 5);

        //    board[proposedMove.X, proposedMove.Y] = BoardCell.None;
        //    return MoveResult.NotFound;
        //}

        //private MoveResult FindBestMove(BoardState state, Cell move)
        //{
        //    if (move == null) return MoveResult.NotFound;
        //    // do move
        //    board[move.X, move.Y] = state.MyCellType;

        //    var lines = GetLinesByAddingCell(move, state.MyLines);
        //    int lineOfThree = 0, lineOfFour = 0;
        //    foreach (var line in lines)
        //    {
        //        var lineType = line.Estimate(board, state.MyCellType);
        //        if (lineType == LineType.StraightFour) return RevertAndReturn(move, 4);
        //        if ((ThreatOfFour(lineType)) && line.Contains(move))
        //        {
        //            lineOfFour++;
        //            continue;
        //        }
        //        if (ThreatOfThree(lineType) && line.Contains(move)) lineOfThree++;
        //    }
        //    if (lineOfFour >= 2) return RevertAndReturn(move, 4);
        //    if (lineOfFour >= 1 && lineOfThree >= 1) return RevertAndReturn(move, 4);

        //    if (lineOfThree >= 2 && !state.OppLines.Any(l => ThreatOfThree(l.Estimate(board, state.MyCellType))))
        //        return RevertAndReturn(move, 3);
        //    // revert
        //    board[move.X, move.Y] = BoardCell.None;
        //    return MoveResult.NotFound;
        //}

        public static bool ThreatOfThree(LineType lineType)
        {
            return lineType == LineType.ThreeInRow || lineType == LineType.BrokenThree;
        }

        //private MoveResult RevertAndReturn(Cell proposedMove, int lenght)
        //{
        //    board[proposedMove.X, proposedMove.Y] = BoardCell.None;
        //    return new MoveResult(proposedMove, lenght, true);
        //}

        //private bool ThreeInRowWithTwoPossibleMovesCase(Line oppLine, out Tuple<Cell, Cell> moves)
        //{
        //    moves = null;
        //    if (oppLine.Count != 3)
        //    {
        //        return false;
        //    }
        //    var oppBest = oppLine.GetTwoNextCells(board);
        //    if (oppBest.Item1 != null && oppBest.Item2 != null)
        //    {
        //        moves = oppBest;
        //        return true;
        //    }
        //    return false;
        //}

        private IEnumerable<EstimatedCell> EstimateCells(BoardState state, IEnumerable<Cell> cells)
        {
            var list = new List<EstimatedCell>();

            foreach (var cell in cells)
            {
                board[cell.X, cell.Y] = state.MyCellType;
                var myNewLines = GetLinesByAddingCell(cell, state.MyLines);
                var oppEstim = SumLines(state.OppLines, state.OpponentCellType);
                var myEstim = SumLines(myNewLines, state.MyCellType);
                list.Add(new EstimatedCell(cell, myNewLines, myEstim - oppEstim));
                board[cell.X, cell.Y] = BoardCell.None;
            }
            return list.OrderByDescending(ec => ec.Estimate);
        }

        public List<Line> GetLines(BoardCell type)
        {
            var lines = new List<Line>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (board[i, j] != type)
                    {
                        continue;
                    }
                    FillLines(CellManager.Get(i, j), lines);
                }
            }
            var sorted = lines.ToList();
            sorted.Sort();
            return sorted;
        }

        public List<Line> GetLinesByAddingCell(Cell cell, List<Line> existingLines)
        {
            var lines = new List<Line>(existingLines.Select(existingLine => existingLine.Clone()));
            FillLines(cell, lines);
            return lines;
        }

        private void FillLines(Cell cell, List<Line> lines)
        {
            var cellsUsedInAdding = new HashSet<Cell>();
            var addedToSomeLine = false;
            var usedLines = new List<Line>();
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                var line = lines[i];
                if (!line.JoinIfPossible(cell)) continue;
                cellsUsedInAdding.UnionWith(line);
                addedToSomeLine = true;
                MergeLines(lines, usedLines, line);
            }
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                var line = lines[i];
                foreach (var lineCell in line)
                {
                    if (lineCell.DistSqr(cell) > 2 || cellsUsedInAdding.Contains(lineCell)) continue;
                    var newLine = new Line(cell, lineCell);
                    if (!lines.Contains(newLine))
                    {
                        lines.Add(newLine);
                        addedToSomeLine = true;
                        cellsUsedInAdding.Add(lineCell);
                        MergeLines(lines, usedLines, newLine);
                    }
                }
            }
            if (!addedToSomeLine)
            {
                lines.Add(new Line(cell));
            }
        }

        private static void MergeLines(List<Line> lines, List<Line> usedLines, Line line)
        {
            Line mergedLine = null;
            foreach (var usedLine in usedLines.ToList())
            {
                if (!usedLine.HasSameDirection(line)) continue;

                mergedLine = usedLine.GetMergedLine(line);
                usedLines.Remove(usedLine);
                lines.Remove(usedLine);
                lines.Remove(line);
                lines.Add(mergedLine);
            }
            if (mergedLine == null) usedLines.Add(line);
        }

        public bool HasFiveInARow(BoardCell boardCell)
        {
            var lines = GetLines(boardCell);
            return lines.Any(l => l.Count >= 5);
        }
    }
}