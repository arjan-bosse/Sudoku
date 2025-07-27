using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class SudokuBoard
    {
        // maintain 9 x 9 board
        public SudokuSquare[] squares { private set; get; }
        public IEnumerable<SudokuSquare> unsolvedSquares { private set; get; }
        public int digitsPlayed { private set; get; }

        public Boolean solved
        {
            get { return digitsPlayed == 81; }
        }

        public Boolean impossible
        {
            get { return digitsPlayed < 17; }
        }

        // maintain rows, columns and boxes on the board
        private SudokuGroup[] boxGroup;
        private SudokuGroup[] colGroup;
        private SudokuGroup[] rowGroup;

        public SudokuBoard()
        {
            squares = new SudokuSquare[81];
            unsolvedSquares = squares.Where(x => x.digit == 0);
            digitsPlayed = 0;

            boxGroup = new SudokuGroup[9];
            colGroup = new SudokuGroup[9];
            rowGroup = new SudokuGroup[9];

            for (int idx = 0; idx < 81; idx++)
            {
                int col = idx % 9;
                int row = idx / 9;
                int box = ((col < 3) ? 0 : (col < 6) ? 1 : 2) + 3 * ((row < 3) ? 0 : (row < 6) ? 1 : 2);

                squares[idx] = new SudokuSquare(box, col, row);
            }

            for (int box = 0; box < 9; box++)
            {
                boxGroup[box] = new SudokuGroup("b" + (box + 1), 9);

                int startCol = 3 * (box % 3);
                int startRow = 3 * (box / 3);
                int idx = 0;

                for (int row = startRow; row < startRow + 3; row++)
                {
                    for (int col = startCol; col < startCol + 3; col++)
                    {
                        boxGroup[box].squares[idx] = squares[col + 9 * row];
                        idx++;
                        squares[col + 9 * row].boxGroup = boxGroup[box];
                    }
                }
            }

            for (int col = 0; col < 9; col++)
            {
                colGroup[col] = new SudokuGroup("c" + (col + 1), 9);
                for (int row = 0; row < 9; row++)
                {
                    colGroup[col].squares[row] = squares[col + 9 * row];
                    squares[col + 9 * row].colGroup = colGroup[col];
                }
            }
            
            for (int row = 0; row < 9; row++)
            {
                rowGroup[row] = new SudokuGroup("r" + (row + 1), 9);
                for (int col = 0; col < 9; col++)
                {
                    rowGroup[row].squares[col] = squares[col + 9 * row];
                    squares[col + 9 * row].rowGroup = rowGroup[row];
                }
            }
        }

        private void updateDirections(SudokuSquare sq, int digit, bool increaseUse)
        {
            for (int i = 0; i < 9; i++)
            {
                sq.boxGroup.squares[i].updateCandidate(digit, increaseUse);
                sq.colGroup.squares[i].updateCandidate(digit, increaseUse);
                sq.rowGroup.squares[i].updateCandidate(digit, increaseUse);
            }
        }

        public void Play(SudokuSquare sq, int digit)
        {
            if (sq.digit == 0 && sq.hasCandidate(digit))
            {
                updateDirections(sq, digit, true);
                sq.digit = digit;
                digitsPlayed++;
            }
        }

        public void Undo(SudokuSquare sq)
        {
            int digit = sq.digit;
            if (digit != 0)
            {
                updateDirections(sq, digit, false);
                sq.digit = 0;
                digitsPlayed--;
            }
        }

        public void DisableCandidate(SudokuSquare sq, int digit)
        {
            if (sq.digit == 0 && sq.hasCandidate(digit))
            {
                sq.updateCandidate(digit, true);
            }
        }

        public void EnableCandidate(SudokuSquare sq, int digit)
        {
            if (sq.digit == 0 && !sq.hasCandidate(digit))
            {
                sq.updateCandidate(digit, false);
            }
        }

        public SudokuSquare getSquare(int idx)
        {
            return squares[idx];
        }

        public SudokuSquare getSquare(int col, int row)
        {
            return squares[col + 9 * row];
        }

        public SudokuSquare getSquare(string name)
        {
            for (int idx = 0; idx < 81; idx++)
            {
                if (squares[idx].name == name) return squares[idx];
            }
            return null;
        }


        public int getIndex(SudokuSquare sq)
        {
            return sq.col + 9 * sq.row;
        }

        public SudokuGroup getGroup(string name)
        {
            for (int idx = 0; idx < 9; idx++)
            {
                if (boxGroup[idx].name == name) return boxGroup[idx];
                if (colGroup[idx].name == name) return colGroup[idx];
                if (rowGroup[idx].name == name) return rowGroup[idx];
            }
            return null;
        }

        public string getGrid()
        {
            return getGrid(false);
        }

        public string getGridCandidates()
        {
            return getGrid(true);
        }

        public string getGrid(bool candidates)
        {
            StringBuilder sb = new StringBuilder();
            for (int row = 0; row < 9; row++)
            {
                if (row == 3 || row == 6)
                {
                    sb.Append("---+---+---" + Environment.NewLine);
                }
                for (int col = 0; col < 9; col++)
                {
                    if (col == 3 || col == 6)
                    {
                        sb.Append("|");
                    }
                    if (squares[col + 9 * row].digit == 0)
                    {
                        if (candidates)
                        {
                            List<int> candidateList = squares[col + 9 * row].getCandidates();
                            sb.Append("(");
                            foreach (int candidate in candidateList) sb.Append(candidate);
                            sb.Append(")");
                        }
                        else
                        {
                            sb.Append(".");
                        }
                    }
                    else
                    {
                        sb.Append(squares[col + 9 * row].digit);
                    }
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        private void reset()
        {
            foreach (SudokuSquare sq in squares)
            {
                sq.reset();
            }
        }

        public void setGrid(string lines)
        {
            reset();
            digitsPlayed = 0;

            int digitsRead = 0;
            Boolean readingCandidates = false;
            string candidatesLeft = null;

            foreach (char digit in lines)
            {
                if (digit == '(')
                {
                    readingCandidates = true;
                    candidatesLeft = "123456789";
                }
                if (digit == ')')
                {
                    readingCandidates = false;
                    if (digitsRead < 81)
                    {
                        foreach (char cand in candidatesLeft)
                        {
                            DisableCandidate(squares[digitsRead], Convert.ToInt32(cand.ToString()));
                        }
                    }
                    digitsRead++;
                }
                if (digit >= '1' && digit <= '9')
                {
                    if (readingCandidates)
                    {
                        candidatesLeft = candidatesLeft.Replace("" + digit, "");
                    }
                    else
                    {
                        if (digitsRead < 81)
                        {
                            Play(squares[digitsRead], Convert.ToInt32(digit.ToString()));
                        }
                        digitsRead++;
                    }
                }
                if (digit == '.' || digit == '0' || digit =='o' || digit == 'O')
                {
                    if (!readingCandidates)
                    {
                        digitsRead++;
                    }
                }
            }        
        }
    }
}
