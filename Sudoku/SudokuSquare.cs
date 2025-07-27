using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class SudokuSquare
    {
        public int digit { set; get; }
        public int box { private set; get; }
        public int col { private set; get; }
        public int row { private set; get; }
        public string name { private set; get; }

        public SudokuGroup boxGroup { set; get; }
        public SudokuGroup colGroup { set; get; }
        public SudokuGroup rowGroup { set; get; }

        private IEnumerable<SudokuSquare> neighbors = null;

        private int[] candidate;

        public int candidates
        {
            get { return getNumberOfCandidates(); }
        }

        public SudokuSquare(int box, int col, int row)
        {
            digit = 0;
            this.box = box;
            this.col = col;
            this.row = row;
            name = "r" + (row + 1) + "c" + (col + 1);
            candidate = new int[9];
        }

        public bool hasCandidate(int digit)
        {
            return (candidate[digit - 1] == 0);
        }

        private int getNumberOfCandidates()
        {
            int number = 0;
            for (int i = 0; i < 9; i++)
            {
                if (candidate[i] == 0)
                {
                    number++;
                }
            }
            return number;
        }

        public List<int> getCandidates()
        {
            List<int> candidateList = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                if (candidate[i] == 0)
                {
                    candidateList.Add(i + 1);
                }
            }
            return candidateList;
        }

        public void updateCandidate(int digit, bool increaseUse)
        {
            candidate[digit - 1] += (increaseUse ? 1 : -1);
        }

        public void reset()
        {
            digit = 0;
            for (int i = 0; i < 9; i++)
            {
                candidate[i] = 0;
            }
        }

        public IEnumerable<SudokuSquare> getNeighbors()
        {
            if (neighbors == null)
            {
                HashSet<SudokuSquare> nbhash = new HashSet<SudokuSquare>();

                foreach (SudokuSquare nb in boxGroup.squares)
                {
                    if (nb != this) nbhash.Add(nb);
                }
                foreach (SudokuSquare nb in colGroup.squares)
                {
                    if (nb != this) nbhash.Add(nb);
                }
                foreach (SudokuSquare nb in rowGroup.squares)
                {
                    if (nb != this) nbhash.Add(nb);
                }

                neighbors = nbhash;
            }

            return neighbors;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(name);
            sb.Append(',');
            if (digit != 0)
            {
                sb.Append(digit);
            }
            else
            {
                sb.Append('(');
                for (int i = 0; i < 9; i++)
                {
                    if (candidate[i] == 0)
                    {
                        sb.Append(i + 1);
                    }
                }
                sb.Append(')');
            }
            sb.Append(']');
            return sb.ToString();
        }
    }
}
