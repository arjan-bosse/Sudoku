using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    //
    // Solve sudoku using brute-force.
    // This is meant to validate the sudoku whether or not there is exactly one solution.
    // As a side-effect maximal two found solutions are kept.
    //
    class SudokuSolver
    {
        private SudokuBoard board;

        public bool stopped { private set; get; }
        public int solutions { private set; get; }
        public string solution1 { private set; get; }
        public string solution2 { private set; get; }

        public SudokuSolver(SudokuBoard sudokuBoard)
        {
            board = sudokuBoard;
        }

        //
        // Find unoccupied square with the fewest number of candidates.
        // If all squared are occupied then the board contains a solution and no unoccupied square can be found.
        // if an unoccupied square with no candidates is found then immediately return since no solution is possible.
        //
        private SudokuSquare findSquare()
        {
            SudokuSquare found = null;

            foreach (SudokuSquare square in board.unsolvedSquares)
            {
                if (found == null || square.candidates < found.candidates)
                {
                    found = square;
                }
                if (found.candidates == 0)
                {
                    return found;
                }
            }

            return found;
        }

        private void solve()
        {
            SudokuSquare square = findSquare();
            if (square == null)
            {
                // solution found
                solutions++;
                if (solutions == 1)
                {
                    solution1 = board.getGrid();
                }
                if (solutions == 2)
                {
                    solution2 = board.getGrid();
                }
                return;
            }
            if (square.candidates == 0)
            {
                // no solution possible...
                return;
            }

            // for each candidate try whether or not it leads to a solution
            List<int> candidates = square.getCandidates();
            foreach (int digit in candidates)
            {
                // no need to solve further if stop requested or multiple solutions found
                if (!stopped && solutions < 2)
                {
                    // play the digit
                    board.Play(square, digit);

                    // solve the resulting board
                    solve();

                    // undo played digit before trying next
                    board.Undo(square);
                }
            }
        }

        // Stop finding solutions.
        public void stop()
        {
            stopped = true;
        }

        //
        // Find 0, 1 or 2 solutions.
        // More than 2 can happen but doesn't matter.
        //
        public void findSolution()
        {
            solutions = 0;
            solution1 = null;
            solution2 = null;
            stopped = false;
            solve();
        }
    }
}
