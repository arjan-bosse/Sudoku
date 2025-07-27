using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class SudokuAnalysedSquare
    {
        public SudokuSquare square;
        public List<int> candidates;

        public SudokuAnalysedSquare(SudokuSquare square, List<int> candidates)
        {
            this.square = square;
            this.candidates = candidates;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(square);
            sb.Append(',');
            foreach (int i in candidates)
            {
                sb.Append(i);
            }
            sb.Append(']');
            return sb.ToString();
        }
    }
}
