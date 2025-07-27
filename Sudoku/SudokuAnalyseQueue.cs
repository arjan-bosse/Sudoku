using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class SudokuAnalyseQueue
    {
        private SudokuBoard board;

        // result from solver
        public string resultType { private set; get; }
        public string resultDetails { private set; get; }
        public List<SudokuAnalysedSquare> analysedSquareList { private set; get; }


        //private List<SudokuSquare>[] squareList;
        //private SudokuSquare[] squareListIndex;


        private Queue<SudokuSquare> queue;
        private List<SudokuSquare> chain;

        private enum ChainType { WRONG, XY };
        private ChainType chainType;

        private string best_chain;
        private int best_chain_length;

        private HashSet<SudokuSquare> pivot_neighbors;
        private int pivot_digit;


        private bool NotInGroup(int candidate, SudokuGroup group, SudokuSquare square)
        {
            foreach (SudokuSquare sq in group.unsolvedSquares)
            {
                if (sq != square && sq.hasCandidate(candidate))
                {
                    return false;
                }
            }
            return true;
        }

        private SudokuGroup SingleHiddenInAnyGroup(int candidate, SudokuSquare square)
        {
            if (NotInGroup(candidate, square.boxGroup, square)) return square.boxGroup;
            if (NotInGroup(candidate, square.colGroup, square)) return square.colGroup;
            if (NotInGroup(candidate, square.rowGroup, square)) return square.rowGroup;
            return null;
        }


        private HashSet<SudokuSquare> Neighbors(int candidate, SudokuSquare square)
        {
            HashSet<SudokuSquare> nblist = new HashSet<SudokuSquare>();

            foreach (SudokuSquare nb in square.boxGroup.unsolvedSquares)
            {
                if (nb != square && nb.hasCandidate(candidate))
                {
                    nblist.Add(nb);
                }
            }
            foreach (SudokuSquare nb in square.colGroup.unsolvedSquares)
            {
                if (nb != square && nb.hasCandidate(candidate))
                {
                    nblist.Add(nb);
                }
            }
            foreach (SudokuSquare nb in square.rowGroup.unsolvedSquares)
            {
                if (nb != square && nb.hasCandidate(candidate))
                {
                    nblist.Add(nb);
                }
            }

            return nblist;
        }

        // solve

        private void enqueueNext(SudokuSquare square)
        {
            if (square.digit > 0 || square.candidates == 0 || queue.Contains(square))
            {
                return;
            }

            if (square.candidates == 1)
            {
                queue.Enqueue(square);
                return;
            }

            foreach (int candidate in square.getCandidates())
            {
                if (SingleHiddenInAnyGroup(candidate, square) != null)
                {
                    queue.Enqueue(square);
                    return;
                }
            }
        }

        private void update_chain()
        {
            if (best_chain == "" || chain.Count < best_chain_length)
            {
                analysedSquareList = new List<SudokuAnalysedSquare>();

                best_chain_length = chain.Count;
                best_chain = "";
                foreach (SudokuSquare s in chain)
                {
                    best_chain += s;

                    List<int> candlist = new List<int>();
                    candlist.Add(s.digit);
                    SudokuAnalysedSquare sas = new SudokuAnalysedSquare(s, candlist);
                    analysedSquareList.Add(sas);
                }
            }
        }

        private void solve(SudokuSquare lastPlayed, int depth)
        {
            chain.Add(lastPlayed);

            if (board.solved)
            {
                return;
            }

            if (chainType == ChainType.XY)
            {
                foreach (SudokuSquare sq in pivot_neighbors)
                {
                    if (sq.digit > 0 && sq.digit != pivot_digit ||
                        sq.digit == 0 && !sq.hasCandidate(pivot_digit))
                    {
                        if (sq != lastPlayed)
                        {
                            chain.Add(sq);
                        }
                        update_chain();
                        return;
                    }
                }
            }

            foreach (SudokuSquare sq in board.unsolvedSquares)
            {
                if (sq.candidates == 0)
                {
                    if (chainType == ChainType.WRONG)
                    {
                        chain.Add(sq);
                        update_chain();
                    }
                    return;
                }
            }


            //squareList[depth] = new List<SudokuSquare>();
            //squareListIndex[depth] = null;

            foreach (SudokuSquare sq in board.unsolvedSquares)
            {

                enqueueNext(sq);
                /*
                for (int d = 0; d < depth; d++)
                {
                    foreach (SudokuSquare dsq in squareList[depth])
                    {
                        if (board.getIndex(sq) == board.getIndex(dsq))
                        {

                        }
                    }
                }
                */
            }

            if (queue.Count == 0)
            {
                return;
            }

            SudokuSquare square = queue.Dequeue();

            if (square.candidates == 1)
            {
                int candidate = square.getCandidates().First();
                board.Play(square, candidate);
                solve(square, depth + 1);
                board.Undo(square);
                return;
            }

            foreach (int candidate in square.getCandidates())
            {
                if (SingleHiddenInAnyGroup(candidate, square) != null)
                {
                    board.Play(square, candidate);
                    solve(square, depth + 1);
                    board.Undo(square);
                    return;
                }
            }
        }

        private void StrongLink()
        {
            chainType = ChainType.XY;

            foreach (SudokuSquare square in board.unsolvedSquares)
            {
                if (square.candidates == 2)
                {
                    chain = new List<SudokuSquare>();
                    queue = new Queue<SudokuSquare>();

                    pivot_digit = square.getCandidates().First();
                    pivot_neighbors = Neighbors(pivot_digit, square);

                    board.Play(square, square.getCandidates().Last());
                    solve(square, 0);
                    board.Undo(square);

                    chain = new List<SudokuSquare>();
                    queue = new Queue<SudokuSquare>();

                    pivot_digit = square.getCandidates().Last();
                    pivot_neighbors = Neighbors(pivot_digit, square);

                    board.Play(square, square.getCandidates().First());
                    solve(square, 0);
                    board.Undo(square);
                }
            }
        }


        private void DeadEnd()
        {
            chainType = ChainType.WRONG;

            foreach (SudokuSquare square in board.unsolvedSquares)
            {
                if (square.candidates == 2)
                {
                    chain = new List<SudokuSquare>();
                    queue = new Queue<SudokuSquare>();

                    board.Play(square, square.getCandidates().Last());
                    solve(square, 0);
                    board.Undo(square);

                    chain = new List<SudokuSquare>();
                    queue = new Queue<SudokuSquare>();

                    board.Play(square, square.getCandidates().First());
                    solve(square, 0);
                    board.Undo(square);
                }
            }
        }


        // analyse

        public void analyse(SudokuBoard board)
        {
            this.board = board;
            analysedSquareList = null;
            resultType = null;
            resultDetails = null;
            //squareList = new List<SudokuSquare>[81];
            //squareListIndex = new SudokuSquare[81];


            // SOLUTION

            if (board.solved)
            {
                resultType = "Solution";
                resultDetails = "";
                return;
            }

            // IMPOSSIBLE

            if (board.impossible)
            {
                resultType = "Impossible (too few cells filled)";
                resultDetails = "";
                return;
            }

            // UNSOLVABLE

            foreach (SudokuSquare square in board.unsolvedSquares)
            {
                if (square.candidates == 0)
                {
                    resultType = "Unsolvable (cell without candidates)";
                    resultDetails = "" + square;
                    return;
                }
            }

            // NAKED SINGLE

            foreach (SudokuSquare square in board.unsolvedSquares)
            {
                if (square.candidates == 1)
                {
                    analysedSquareList = new List<SudokuAnalysedSquare>();
                    SudokuAnalysedSquare sas = new SudokuAnalysedSquare(square, square.getCandidates());
                    analysedSquareList.Add(sas);
                    resultType = "Naked Single";
                    resultDetails = "" + square;
                    return;
                }
            }

            // HIDDEN SINGLE

            foreach (SudokuSquare square in board.unsolvedSquares)
            {
                foreach (int candidate in square.getCandidates())
                {
                    if (SingleHiddenInAnyGroup(candidate, square) != null)
                    {
                        List<int> candlist = new List<int>();
                        candlist.Add(candidate);
                        analysedSquareList = new List<SudokuAnalysedSquare>();
                        SudokuAnalysedSquare sas = new SudokuAnalysedSquare(square, candlist);
                        analysedSquareList.Add(sas);
                        resultType = "Hidden Single";
                        resultDetails = "" + square + "," + candidate;
                        return;
                    }
                }
            }

            // STRONG LINK

            best_chain = "";
            best_chain_length = 0;
            StrongLink();

            if (best_chain_length > 0)
            {
                resultType = "Strong Link";
                resultDetails = best_chain;
                return;
            }

            // DEAD END

            best_chain = "";
            best_chain_length = 0;
            DeadEnd();

            if (best_chain_length > 0)
            {
                resultType = "Dead End";
                resultDetails = best_chain;
                return;
            }

            // NOTHING

            resultType = "Nothing";
            resultDetails = "";
            return;
        }
    }
}
