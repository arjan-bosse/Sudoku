using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class SudokuJavaAnalyse
    {
        private SudokuBoard board;

        // result from Java solver
        public string[] lines { private set; get;  }
        public List<string> moves { private set; get;  }
        public string result { private set; get;  }
        public string output { private set; get; }
        public List<SudokuAnalysedSquare> analysedSquareList { private set; get; }


        private string[] prepare(string line)
        {
            moves.Add(line);
            if (moves.Count == 1)
            {
                string[] separator = { " " };
                string[] words = line.Split(separator, StringSplitOptions.None);
                return words;
            }
            return null;
        }

        private void NakedSquareList(SudokuGroup g, List<int> candidates)
        {
            foreach (SudokuSquare square in g.squares)
            {
                List<int> remaining = square.getCandidates().Except(candidates).ToList();

                if (remaining.Count == 0)
                {
                    SudokuAnalysedSquare sas = new SudokuAnalysedSquare(square, square.getCandidates());
                    analysedSquareList.Add(sas);
                }
            }
        }

        private void HiddenSquareList(SudokuGroup g, List<int> candidates)
        {
            foreach (SudokuSquare square in g.squares)
            {
                List<int> remaining = square.getCandidates().Intersect(candidates).ToList();

                if (remaining.Count > 0)
                {
                    SudokuAnalysedSquare sas = new SudokuAnalysedSquare(square, remaining);
                    analysedSquareList.Add(sas);
                }
            }
        }

        // LOCKED CANDIDATE

        private void LockedCandidate(string line)
        {
            string[] words = prepare(line);
            if (words != null)
            {
                //String info = "locked candidate " + values[0] + " in " + g1.getName() + g2.getName();

                List<int> candidates = new List<int>();
                candidates.Add(Int32.Parse(words[2]));
                SudokuGroup g1 = board.getGroup(words[4].Substring(0, 2));
                SudokuGroup g2 = board.getGroup(words[4].Substring(2, 2));

                foreach (SudokuSquare square1 in g1.squares)
                {
                    foreach (SudokuSquare square2 in g2.squares)
                    {
                        if (square1.name == square2.name)
                        {
                            SudokuAnalysedSquare sas = new SudokuAnalysedSquare(square1, candidates);
                            analysedSquareList.Add(sas);
                        }
                    }
                }
            }
        }

        // NAKED SINGLE
        // HIDDEN SINGLE
        // NAKED PAIR
        // HIDDEN PAIR
        // NAKED TRIPLE
        // HIDDEN TRIPLE
        // NAKED QUAD
        // HIDDEN QUAD

        private void NakedHidden(string line)
        {
            string[] words = prepare(line);
            if (words != null)
            {
                //String info = "naked single " + value + " in " + cell.getName();
                //String info = "hidden single " + values[0] + " in " + cell.getName();
                //String info = "naked pair " + values[0] + "," + values[1] + " in " + g.getName();
                //String info = "hidden pair " + values[0] + "," + values[1] + " in " + g.getName();
                //String info = "naked triple " + values[0] + "," + values[1] + "," + values[2] + " in " + g.getName();
                //String info = "hidden triple " + values[0] + "," + values[1] + "," + values[2] + " in " + g.getName();
                //String info = "naked quad " + values[0] + "," + values[1] + "," + values[2] + "," + values[3] + " in " + g.getName();
                //String info = "hidden quad " + values[0] + "," + values[1] + "," + values[2] + "," + values[3] + " in " + g.getName();

                List<int> candidates = new List<int>();
                if (words[2].Length >= 1) candidates.Add(Int32.Parse(words[2].Substring(0, 1)));
                if (words[2].Length >= 3) candidates.Add(Int32.Parse(words[2].Substring(2, 1)));
                if (words[2].Length >= 5) candidates.Add(Int32.Parse(words[2].Substring(4, 1)));
                if (words[2].Length >= 7) candidates.Add(Int32.Parse(words[2].Substring(6, 1)));

                if (words[1] == "single")
                {
                    SudokuSquare square = board.getSquare(words[4].Substring(0, 4));
                    SudokuAnalysedSquare sas = new SudokuAnalysedSquare(square, candidates);
                    analysedSquareList.Add(sas);
                }
                else
                {
                    SudokuGroup g = board.getGroup(words[4].Substring(0, 2));
                    if (words[0] == "naked")
                    {
                        NakedSquareList(g, candidates);
                    }
                    else
                    {
                        HiddenSquareList(g, candidates);
                    }
                }
            }
        }

        // X-WING
        // SWORDFISH
        // YELLYFISH

        private void XFish(string line)
        {
            string[] words = prepare(line);
            if (words != null)
            {
                //String info = "x-wing " + value + " in " + g1.getName() + g2.getName();
                //String info = "swordfish " + value + " in " + g1.getName() + g2.getName() + g3.getName();
                //String info = "yellyfish " + value + " in " + g1.getName() + g2.getName() + g3.getName() + g4.getName();

                List<int> candidates = new List<int>();
                candidates.Add(Int32.Parse(words[1]));

                for (int len = 2; len <= 8; len += 2)
                {
                    if (words[3].Length >= len)
                    {
                        SudokuGroup g = board.getGroup(words[3].Substring(len - 2, 2));
                        HiddenSquareList(g, candidates);
                    }
                }
            }
        }

        // XY-WING

        private void XYWing(string line)
        {
            string[] words = prepare(line);
            if (words != null)
            {
                //String info = "xy-wing " + z + " in " + xy.getName() + "," + xz.getName() + "," + yz.getName();

                List<int> candidates = new List<int>();
                candidates.Add(Int32.Parse(words[1]));

                SudokuSquare xy = board.getSquare(words[3].Substring(0, 4));
                SudokuSquare xz = board.getSquare(words[3].Substring(5, 4));
                SudokuSquare yz = board.getSquare(words[3].Substring(10, 4));

                analysedSquareList.Add(new SudokuAnalysedSquare(xy, xy.getCandidates()));
                analysedSquareList.Add(new SudokuAnalysedSquare(xz, candidates));
                analysedSquareList.Add(new SudokuAnalysedSquare(yz, candidates));
            }
        }

        // GUESS

        private void Guess(string line)
        {
            string[] words = prepare(line);
            if (words != null)
            {
                //String info = "guess " + value + " in " + cell.getName();

                List<int> candidates = new List<int>();
                candidates.Add(Int32.Parse(words[1]));
                SudokuSquare square = board.getSquare(words[3]);

                SudokuAnalysedSquare sas = new SudokuAnalysedSquare(square, candidates);
                analysedSquareList.Add(sas);
            }
        }


        // analyse

        public void analyse(SudokuBoard board, string workingDirectory, string fileName)
        {
            this.board = board;

            //start process (java sodoku solver) that reads from standard input and writes to standard output
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.FileName = fileName;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            //send lines (of the board) to the java solver
            System.IO.StreamWriter writer = process.StandardInput;
            string candidateLines = board.getGridCandidates();
            writer.WriteLine(candidateLines);
            writer.Close();

            //read lines (of the solution) from the java solver
            string readLines = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            string[] separator = { "" + Environment.NewLine };
            lines = readLines.Split(separator, StringSplitOptions.None);
            moves = new List<string>();
            analysedSquareList = new List<SudokuAnalysedSquare>();
            output = "";

            foreach (string line in lines)
            {
                if (line.StartsWith("locked")) LockedCandidate(line);
                else if (line.StartsWith("naked") ||
                    line.StartsWith("hidden")) NakedHidden(line);
                else if (line.StartsWith("x-wing") ||
                    line.StartsWith("swordfish") ||
                    line.StartsWith("yellyfish")) XFish(line);
                else if (line.StartsWith("xy-wing")) XYWing(line);
                else if (line.StartsWith("guess")) Guess(line);

                else if (line.StartsWith("NOTHING") ||
                    line.StartsWith("SOLUTION") ||
                    line.StartsWith("MULTIPLE") ||
                    line.StartsWith("IMPOSSIBLE") ||
                    line.StartsWith("UNSOLVABLE") ||
                    line.StartsWith("UNFINISHED"))
                {
                    result = line;
                }

                else
                {
                    output += line + Environment.NewLine;
                }
            }
        }
    }
}
