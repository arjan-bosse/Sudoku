using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class SudokuAnalyse
    {
        private SudokuBoard board;

        // result from solver
        public bool stopped { private set; get; }
        public string resultType { private set; get; }
        public string resultDetails { private set; get; }
        public List<SudokuAnalysedSquare> analysedSquareList { private set; get; }

        // chain stuff
        private Stack<SudokuSquare> memory;
        private Stack<SudokuSquare> chain;

        private enum ChainType { DEADEND, ELIMINATION};
        private ChainType chainType;

        private string best_chain;
        private int best_chain_length;
        private string best_chain_result;
        private SudokuSquare firstPlayed;

        private List<SudokuSquare> pivot_neighbors;
        private int pivot_digit;

        // constructor
        public SudokuAnalyse(SudokuBoard sudokuBoard)
        {
            board = sudokuBoard;
        }

        // HIDDEN SINGLE functions

        private SudokuGroup SingleHiddenInAnyGroup(int candidate, SudokuSquare square)
        {
            if (!square.boxGroup.unsolvedSquares.Any(sq => sq != square && sq.hasCandidate(candidate))) return square.boxGroup;
            if (!square.colGroup.unsolvedSquares.Any(sq => sq != square && sq.hasCandidate(candidate))) return square.colGroup;
            if (!square.rowGroup.unsolvedSquares.Any(sq => sq != square && sq.hasCandidate(candidate))) return square.rowGroup;

            return null;
        }

        // CHAIN functions

        private List<SudokuSquare> PivotNeighbors(int candidate, SudokuSquare square)
        {
            List<SudokuSquare> nblist = new List<SudokuSquare>();

            foreach (SudokuSquare nb in square.getNeighbors().Where(
                sq => sq.digit == 0 && sq.hasCandidate(candidate)))
            {
                nblist.Add(nb);
            }

            return nblist;
        }

        private void update_best(int pivot_digit, string details)
        {
            if (best_chain == "" || chain.Count < best_chain_length)
            {
                analysedSquareList = new List<SudokuAnalysedSquare>();

                best_chain_length = chain.Count;
                best_chain = "";
                best_chain_result = details;

                foreach (SudokuSquare s in chain.Reverse().ToArray())
                {
                    best_chain += s;

                    List<int> candlist = new List<int>();
                    candlist.Add(s.digit > 0 ? s.digit : pivot_digit);
                    SudokuAnalysedSquare sas = new SudokuAnalysedSquare(s, candlist);
                    analysedSquareList.Add(sas);
                }
            }
        }

        private void solve_chain(SudokuSquare lastPlayed, int max_length)
        {
            //solve_begin:
            chain.Push(lastPlayed);

            if (stopped || board.solved || max_length == 0 || best_chain != "" && chain.Count >= best_chain_length)
            {
                goto solve_end;
            }

            if (chainType == ChainType.ELIMINATION)
            {
                foreach (SudokuSquare sq in pivot_neighbors)
                {
                    if (sq.digit > 0 && sq.digit != pivot_digit ||
                        sq.digit == 0 && !sq.hasCandidate(pivot_digit))
                    {
                        if (sq != lastPlayed)
                        {
                            chain.Push(sq);
                            update_best(pivot_digit, pivot_digit + " can be removed from " + sq.name);
                            chain.Pop();
                        }
                        else
                        {
                            update_best(pivot_digit, pivot_digit + " can be removed from " + sq.name);
                        }
                        goto solve_end;
                    }
                }

                //when all neighbors occupied with pivot then no point to search on
                if (pivot_neighbors.Count(sq => sq.digit == pivot_digit) == pivot_neighbors.Count)
                {
                    goto solve_end;
                }
            }

            foreach (SudokuSquare square in board.unsolvedSquares.Where(sq => sq.candidates == 0))
            {
                if (chainType == ChainType.DEADEND)
                {
                    chain.Push(square);
                    update_best(0, pivot_digit + " can be played at " + firstPlayed.name);
                    chain.Pop();
                }
                goto solve_end;
            }

            //set memory at this level
            int memoryAdded = 0;

            foreach (SudokuSquare square in board.unsolvedSquares.Where(
                sq => !memory.Any(m => m.name == sq.name)))
            {
                //if memorized at higher level then continue

                Boolean memorize_square = false;

                if (square.candidates == 1)
                {
                    //naked single
                    int candidate = square.getCandidates().First();
                    board.Play(square, candidate);
                    solve_chain(square, max_length - 1);
                    board.Undo(square);
                    memorize_square = true;
                }
                else
                {
                    foreach (int candidate in square.getCandidates().Where(
                        cand => SingleHiddenInAnyGroup(cand, square) != null))
                    {
                        //hidden single
                        board.Play(square, candidate);
                        solve_chain(square, max_length - 1);
                        board.Undo(square);
                        memorize_square = true;
                    }
                }

                if (memorize_square)
                {
                    //memorize at this level
                    memory.Push(square);
                    memoryAdded++;
                }
            }

            //reset memory at this level
            for (int m = 0; m < memoryAdded; m++)
            {
                memory.Pop();
            }

            solve_end:
            chain.Pop();
        }

        private void find_chain(int max_length)
        {
            best_chain = "";
            best_chain_length = 0;

            foreach (SudokuSquare square in board.unsolvedSquares)
            {
                if (square.candidates == 2)
                {
                    firstPlayed = square;

                    pivot_digit = square.getCandidates().First();
                    pivot_neighbors = PivotNeighbors(pivot_digit, square);
                    chain = new Stack<SudokuSquare>();
                    board.Play(square, square.getCandidates().Last());
                    solve_chain(square, max_length);
                    board.Undo(square);

                    pivot_digit = square.getCandidates().Last();
                    pivot_neighbors = PivotNeighbors(pivot_digit, square);
                    chain = new Stack<SudokuSquare>();
                    board.Play(square, square.getCandidates().First());
                    solve_chain(square, max_length);
                    board.Undo(square);
                }
            }
        }

        // Stop finding solutions.
        public void stop()
        {
            stopped = true;
        }

        // analyse

        public void analyse()
        {
            analysedSquareList = null;
            resultType = null;
            resultDetails = null;
            memory = new Stack<SudokuSquare>();

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

            // CHAIN

            for (int max_length = 1; max_length < 20; max_length++)
            {
                // ELIMINATION

                chainType = ChainType.ELIMINATION;
                find_chain(max_length);

                if (best_chain_length > 0)
                {
                    resultType = "Elimination";
                    resultDetails = best_chain + "," + best_chain_result;
                    return;
                }

                // DEAD END

                chainType = ChainType.DEADEND;
                find_chain(max_length);

                if (best_chain_length > 0)
                {
                    resultType = "Dead End";
                    resultDetails = best_chain + "," + best_chain_result;
                    return;
                }
            }

            // NOTHING

            resultType = "Nothing";
            resultDetails = "";
            return;
        }
    }
}
