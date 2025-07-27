using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class SudokuMove
    {
        public SudokuSquare square { get; private set; }
        public int candidate { get; private set; }
        public int digit { get; private set; }

        public SudokuMove(SudokuSquare playedSquare)
        {
            square = playedSquare;
            candidate = 0;
            digit = square.digit;
        }

        public SudokuMove(SudokuSquare playedSquare, int removedCandidate)
        {
            square = playedSquare;
            candidate = removedCandidate;
            digit = 0;
        }
    }
}
