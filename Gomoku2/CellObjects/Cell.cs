using System;

namespace Gomoku2.CellObjects
{
    public class Cell : IEquatable<Cell>
    {
        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public BoardCell BoardCell { get; set; }

        public int DistSqr(Cell other)
        {
            var xx = X - other.X;
            var yy = Y - other.Y;
            return xx * xx + yy * yy;
        }

        public Cell Normalize()
        {
            int nx = 0, ny = 0;
            if (X > 0) nx = 1;
            if (X < 0) nx = -1;
            if (Y > 0) ny = 1;
            if (Y < 0) ny = -1;
            return CellManager.Get(nx, ny);
        }

        public static Cell operator +(Cell first, Cell second)
        {
            return CellManager.Get(first.X + second.X, first.Y + second.Y);
        }

        public static Cell operator -(Cell first, Cell second)
        {
            return CellManager.Get(first.X - second.X, first.Y - second.Y);
        }

        public static Cell operator *(int a, Cell cell)
        {
            return CellManager.Get(cell.X * a, cell.Y * a);
        }

        public static bool operator ==(Cell first, Cell second)
        {
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }
            if (ReferenceEquals(first, null))
            {
                return false;
            }
            return first.Equals(second);
        }

        public static bool operator !=(Cell first, Cell second)
        {
            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((Cell)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public bool IsEmptyWithBoard(BoardCell[,] board)
        {
            return InTheBoard && board[X, Y] == BoardCell.None;
        }

        public bool IsEmpty
        {
            get { return BoardCell == BoardCell.None; }
        }

        public bool InTheBoard
        {
            get { return X >= 0 && X < 15 && Y >= 0 && Y < 15; }
        }

        //TODO :do we need this? we should use reference comparision instead.
        //maybe X == other.X && Y == other.Y; is never called, investigate
        public bool Equals(Cell other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return X == other.X && Y == other.Y;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", X, Y);
        }

        public bool IsType(BoardCell[,] board, BoardCell cellType)
        {
            return InTheBoard && board[X, Y] == cellType;
        }
    }
}