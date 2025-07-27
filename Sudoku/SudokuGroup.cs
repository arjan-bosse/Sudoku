using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class SudokuGroup
    {
        public string name { private set; get; }
        public SudokuSquare[] squares { private set; get; }
        public IEnumerable<SudokuSquare> unsolvedSquares { private set; get; }

        public SudokuGroup(string name, int len)
        {
            this.name = name;
            squares = new SudokuSquare[len];
            unsolvedSquares = squares.Where(x => x.digit == 0);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(name);
            sb.Append(',');
            foreach (SudokuSquare sq in squares)
            {
                sb.Append(sq);
            }
            sb.Append(']');
            return sb.ToString();
        }
    }
}
