using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
// text drawing in picturebox
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Add Reference: System.Configuration

// Sudoku (C#)
//
// Version: 0.1 (original, GUI)
// Date   : 2016-09-20
// Author : Arjan Bosse
//
// Version: 0.2 (SudokuSolver, SudokuJavaAnalyse)
// Date   : 2018-02-08
// Author : Arjan Bosse
//
// Version: 0.3 (SudokuAnalyse)
// Date   : 2018-03-06
// Author : Arjan Bosse

namespace Sudoku
{
    partial class SudokuForm : Form
    {
        private int boardWidth, boardHeight;

        // Show square candidates or not.
        private bool showCandidates = true;

        // Calculate a solution. When calculating only the stop button is enabled. 
        private bool calculating = false;
        private Cursor current;
        private SudokuSolver solver = null;
        private SudokuAnalyse analyser = null;
        private List<SudokuAnalysedSquare> analysedSquareList = null;

        // State of the 9 * 9 sudoku board: rows, columns, boxes, digits, candidates, etc.
        private SudokuBoard board;
        private Stack<SudokuMove> history;
        private SudokuMove lastmove = null;

        // Show textual information.
        private InputForm input;
        private OutputForm output;

        // Java Analyser
        string workingDirectory = null;
        string fileName = null;

        public SudokuForm()
        {
            InitializeComponent();

            // Square size depends on board size.
            boardWidth = pictureBox1.Size.Width;
            boardHeight = pictureBox1.Size.Height;

            input = new InputForm();
            output = new OutputForm();
            board = new SudokuBoard();
            history = new Stack<SudokuMove>();
        }

        // User clicked a candidate at a square on the board.
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (!calculating)
            {
                Boolean left = (e.Button == MouseButtons.Left);
                Boolean right = (e.Button == MouseButtons.Right);
                Point location = e.Location;
                int x = location.X;
                int y = location.Y;
                int clicks = e.Clicks;

                // Determine candidate, cell, digit and place digit in cell.
                // Each cell is subdivided in 3 * 3 candidates.
                // Each row and column has 9 cells and therefore 27 candidates.
                // First calculate candidate with highest accuracy, then cell, and finally digit.

                // calculate candidate: 0-26
                int wcand = (27000 * x / boardWidth) / 1000;
                int hcand = (27000 * y / boardHeight) / 1000;

                // calculate cell: 0-8
                int wcell = wcand / 3;
                int hcell = hcand / 3;

                // 1-9
                int digit = 3 * (hcand % 3) + wcand % 3 + 1;

                SudokuSquare square = board.getSquare(wcell, hcell);

                // on left-click play chosen candidate digit at cell
                if (left)
                {
                    if (square.digit == 0 && square.hasCandidate(digit))
                    {
                        board.Play(square, digit);
                        //output.WriteLines("play " + digit + " in " + square.name + Environment.NewLine);
                        lastmove = new SudokuMove(square);
                        history.Push(lastmove);
                        Redraw();
                    }
                }

                // on right-click disable chosen candidate digit at cell
                if (right)
                {
                    if (square.digit == 0 && square.hasCandidate(digit))
                    {
                        board.DisableCandidate(square, digit);
                        lastmove = new SudokuMove(square, digit);
                        history.Push(lastmove);
                        Redraw();
                    }
                }
            }
        }

        // DRAWING THE BOARD

        // Draw lines of the 9 * 9 sudoku board
        private void DrawRaster(Graphics g)
        {
            int height, width, penWidth;

            for (int row = 0; row <= 9; row++)
            {
                height = (row == 9) ? boardHeight - 1 : boardHeight * row / 9;
                penWidth = (row == 3 || row == 6) ? 3 : 1;
                g.DrawLine(new Pen(Color.Blue, penWidth), new Point(0, height), new Point(boardWidth - 1, height));
            }
            for (int col = 0; col <= 9; col++)
            {
                width = (col == 9) ? boardWidth - 1 : boardWidth * col / 9;
                penWidth = (col == 3 || col == 6) ? 3 : 1;
                g.DrawLine(new Pen(Color.Blue, penWidth), new Point(width, 0), new Point(width, boardHeight - 1));
            }  
        }

        // Draw digits on the 9 * 9 sudoku board
        private void DrawDigits(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // for each square either show its digit or (when digit is 0) show its candidates (optional)
            // show all squares
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    SudokuSquare square = board.getSquare(col, row);
                    int digit = square.digit;

                    // show digit
                    if (digit > 0)
                    {
                        Brush color = Brushes.Black;
                        if (lastmove != null && lastmove.square == square)
                        {
                            color = Brushes.Green;
                        }

                        g.DrawString(digit.ToString(), new Font("Tahoma", 20), color,
                            boardWidth * col / 9 + (boardWidth / 25),
                            boardHeight * row / 9 + (boardHeight / 32));
                    }

                    // show candidates
                    if (digit == 0 && showCandidates)
                    {
                        List<int> candidateList = square.getCandidates();
                        foreach (int candidate in candidateList)
                        {
                            Brush color = Brushes.LightGray;
                            if (lastmove != null && lastmove.square == square &&
                                (lastmove.candidate == candidate || lastmove.digit == candidate))
                            {
                                color = Brushes.LightGreen;
                            }

                            if (analysedSquareList != null)
                            {
                                foreach (SudokuAnalysedSquare sas in analysedSquareList)
                                {
                                    if (sas.square == square)
                                    {
                                        foreach (int cand in sas.candidates)
                                        {
                                            if (cand == candidate)
                                            {
                                                color = Brushes.Blue;
                                                break;
                                            }
                                        }
                                    }
                                    if (color == Brushes.Blue)
                                    {
                                        break;
                                    }
                                }
                            }

                            g.DrawString(candidate.ToString(), new Font("Tahoma", 12), color,
                                boardWidth * col / 9 + (boardWidth / 25) * ((candidate - 1) % 3) + 4,
                                boardHeight * row / 9 + (boardHeight / 32) * ((candidate - 1) / 3) + 4);
                        }
                    }
                }
            }
        }

        // Render the board and texts.
        private void Redraw()
        {
            // get (re)painted board bitmap from the State
            Bitmap bitmap = new Bitmap(boardWidth, boardHeight);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                DrawRaster(g);
                DrawDigits(g);
                g.Flush();
            }
            pictureBox1.Image = bitmap;
            this.Controls.Add(pictureBox1);
        }

        // Show the board and textual output dialog.
        private void Board_Load(object sender, EventArgs e)
        {
            Redraw();
            input.Show();
            output.Show();
        }

        // START, STOP CALCULATING

        // Worker thread has been started, let it do the requested work.
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            if (solver != null) solver.findSolution();
            if (analyser != null) analyser.analyse();
        }

        // Worker thread finished calculating.
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (solver != null)
            {
                // Check if there is exactly one solution.
                // Because it can also happen there is no solution or multiple solutions.
                // Only in case of one solution the sudoku is valid.

                if (solver.solutions == 0)
                {
                    output.WriteLines("No solution found." + Environment.NewLine);
                }
                if (solver.solutions >= 1)
                {
                    output.WriteLines(solver.solution1);
                    output.WriteLines("Solution found." + Environment.NewLine);
                }
                if (solver.solutions >= 2)
                {
                    output.WriteLines(solver.solution2);
                    output.WriteLines("Multiple solutions found." + Environment.NewLine);
                }

                // leave calculating state
                output.WriteLines((solver.stopped ? "Stopped" : "Finished") + " solving." + Environment.NewLine);

                solver = null;
                this.Cursor = current;
                calculating = false;
            }

            if (analyser != null)
            {
                analysedSquareList = analyser.analysedSquareList;
                output.WriteLines(analyser.resultType + " found (" + analyser.resultDetails + ")." + Environment.NewLine);

                // leave calculating state
                output.WriteLines((analyser.stopped ? "Stopped" : "Finished") + " analysing." + Environment.NewLine);

                analyser = null;
                this.Cursor = current;
                calculating = false;

                lastmove = null;
                Redraw();
            }
        }

        // HANDLE BUTTONS

        private void loadButton_Click(object sender, EventArgs e)
        {
            if (!calculating)
            {
                openFileDialog1.Title = "Open Sudoku File";
                openFileDialog1.Filter = "TXT files|*.txt";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    output.WriteLines("Loading..." + Environment.NewLine);
                    System.IO.StreamReader sr;
                    sr = new System.IO.StreamReader(openFileDialog1.FileName);
                    string lines = sr.ReadToEnd();
                    sr.Close();
                    output.WriteLines(lines + Environment.NewLine);
                    output.WriteLines("Sodoku loaded." + Environment.NewLine);

                    // cleanup existing and (re)load board
                    board.setGrid(lines);

                    // reset history; keep loaded board as base
                    while (history.Count > 0)
                    {
                        history.Pop();
                    }
                    lastmove = null;

                    // show loaded board
                    Redraw();
                }
            }
        }

        private void reportButton_Click(object sender, EventArgs e)
        {
            if (!calculating)
            {
                // just show the board in textual output
                string gridLines = board.getGrid();
                string candidateLines = board.getGridCandidates();
                output.WriteLines("Reporting..." + Environment.NewLine);
                output.WriteLines(gridLines);
                output.WriteLines(candidateLines);
                output.WriteLines("Sudoku reported." + Environment.NewLine);
            }
        }

        private void solveButton_Click(object sender, EventArgs e)
        {
            if (!calculating)
            {
                // enter calculating state
                calculating = true;
                current = Cursor.Current;
                output.WriteLines("Solving..." + Environment.NewLine);

                // solve in the background
                solver = new SudokuSolver(board);
                backgroundWorker1.RunWorkerAsync();
                this.Cursor = Cursors.WaitCursor;
            }
        }

        private void analyseButton_Click(object sender, EventArgs e)
        {
            if (!calculating && showCandidates)
            {
                // enter calculating state
                calculating = true;
                current = Cursor.Current;
                output.WriteLines("Analysing..." + Environment.NewLine);

                // analyse in the background
                analyser = new SudokuAnalyse(board);
                backgroundWorker1.RunWorkerAsync();
                this.Cursor = Cursors.WaitCursor;
            }
        }

        public void debug(string s)
        {
            output.WriteLines("" + s + Environment.NewLine);
        }

        private void javaButton_Click(object sender, EventArgs e)
        {
            if (!calculating && showCandidates)
            {
                output.WriteLines("Java analysing..." + Environment.NewLine);
                SudokuJavaAnalyse javaAnalyser = new SudokuJavaAnalyse();

                if (workingDirectory == null || fileName == null)
                {
                    openFileDialog1.Title = "Select Java Analyser";
                    openFileDialog1.Filter = "BAT files|*.bat";

                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        fileName = openFileDialog1.FileName;
                        workingDirectory = Path.GetDirectoryName(fileName);
                    }
                }

                javaAnalyser.analyse(board, workingDirectory, fileName);

                analysedSquareList = javaAnalyser.analysedSquareList;
                foreach (string line in javaAnalyser.lines)
                {
                    output.WriteLines(line + Environment.NewLine);
                }
                output.WriteLines("Moves: " + javaAnalyser.moves.Count + Environment.NewLine);
                output.WriteLines("Result: " + javaAnalyser.result + Environment.NewLine);
                if (javaAnalyser.moves.Count > 0) output.WriteLines("First: " + javaAnalyser.moves[0] + Environment.NewLine);

                //input.Clear();
                //input.WriteLines(javaAnalyser.output + Environment.NewLine);

                output.WriteLines("Java analyse finished." + Environment.NewLine);

                lastmove = null;
                Redraw();
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (calculating)
            {
                if (solver != null ) solver.stop();
                if (analyser != null) analyser.stop();
            }
        }

        private void hideButton_Click(object sender, EventArgs e)
        {
            if (!calculating)
            {
                showCandidates = !showCandidates;
                if (showCandidates)
                {
                    output.WriteLines("Show candidates." + Environment.NewLine);
                }
                else
                {
                    output.WriteLines("Hide candidates." + Environment.NewLine);
                }
                Redraw();
            }
        }

        private void UndoLastMove()
        {
            lastmove = history.Pop();
            if (lastmove.candidate == 0)
            {
                board.Undo(lastmove.square);
            }
            else
            {
                board.EnableCandidate(lastmove.square, lastmove.candidate);
            }
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            if (!calculating && history.Count > 0)
            {
                while (history.Count > 0)
                {
                    UndoLastMove();
                }
                Redraw();
            }
        }

        private void inputButton_Click(object sender, EventArgs e)
        {
            if (!calculating)
            {
                output.WriteLines("Loading from input..." + Environment.NewLine);
                string lines = input.ReadLines();
                output.WriteLines("Sodoku input loaded." + Environment.NewLine);

                // cleanup existing and (re)load board
                board.setGrid(lines);

                // reset history; keep loaded board as base
                while (history.Count > 0)
                {
                    history.Pop();
                }
                lastmove = null;

                // show loaded board
                Redraw();
            }
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            if (!calculating && history.Count > 0)
            {
                UndoLastMove();
                Redraw();
            }
        }
    }
}
