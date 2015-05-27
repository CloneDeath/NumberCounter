using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberCounter
{
    class Program
    {
        private static List<Cell> Cells = new List<Cell>();

        private static List<List<Cell>> PreviousGuesses = new List<List<Cell>>(); 
 
        static void Main(string[] args)
        {
            Console.WriteLine("Think of 4 random numbers between 1 and 15, inclusive. No repeats. Order doesn't matter.");

            for (int i = 1; i <= 15; i++)
            {
                Cells.Add(new Cell(i, 1 / 15.0));
            }

            do
            {
                MakeGuess();
                AdjustConfidence();
                NormalizeCells();
                if (NumberCorrect == 4 || CellsLeft == 4)
                {
                    DeclareCorrectCells();
                    break;
                }
                else
                {
                    DeclareRemainingCells();
                }
            } while (true);

            Console.ReadLine();
        }

        private static void DeclareRemainingCells()
        {
            Console.Write("Hmm... it's probably one of these: ");
            WriteGuess(Cells.OrderByDescending(c => c.Confidence).ToList());
        }

        private static void NormalizeCells()
        {
            double total = GetTotalSizeOfCells();

            for (int i = 0; i < Cells.Count; i++)
            {
                Cell cell = Cells[i];
                if (cell.Confidence <= 0)
                {
                    Cells.Remove(cell);
                    i--;
                }
                else
                {
                    cell.Confidence /= total;
                }
            }
        }

        private static void AdjustConfidence()
        {
            foreach (var cell in _guessList)
            {
                if (NumberCorrect == 0)
                {
                    cell.Confidence = 0;
                }
                else
                {
                    cell.Confidence *= (NumberCorrect + 1)/4.0;
                }
            }
        }

        private static void DeclareCorrectCells()
        {
            Console.WriteLine("Your sequence must be: ");
            WriteGuess(NumberCorrect == 4 ? _guessList : Cells);
        }

        public static int CellsLeft { get { return Cells.Count; }}

        private static Random random = new Random();
        private static int NumberCorrect;
        private static List<Cell> _guessList;

        private static int GuessNumber = 1;

        private static void MakeGuess()
        {
            _guessList = GetRandomGuess();

            PreviousGuesses.Add(_guessList);

            Console.Write("{0}) How many are correct: ", GuessNumber);
            GuessNumber++;
            WriteGuess(_guessList);
            NumberCorrect = GetFeedback();
        }

        private static List<Cell> GetRandomGuess()
        {
            var ret = new List<Cell>();
            do
            {
                ret = new List<Cell>();

                do
                {
                    double max = GetTotalSizeOfCells();
                    Cell cell = GetCellAtLevel(random.NextDouble()*max);
                    if (ret.Contains(cell)) continue;

                    ret.Add(cell);
                } while (ret.Count < 4);
            } while (HasBeenGuessedBefore(ret));
            return ret;
        }

        private static bool HasBeenGuessedBefore(List<Cell> guessList)
        {
            foreach (var previousGuess in PreviousGuesses)
            {
                if (AreTheSame(previousGuess, guessList)) return true;
            }

            return false;
        }

        private static bool AreTheSame(List<Cell> left, List<Cell> right)
        {
            var l1 = left.OrderBy(c => c.Value).ToList();
            var l2 = right.OrderBy(c => c.Value).ToList();

            for (int i = 0; i < 4; i++)
            {
                if (l1[i].Value != l2[i].Value) return false;
            }

            return true;
        }

        private static int GetFeedback()
        {
            do
            {
                string entry = Console.ReadLine();

                int value = 0;

                if (Int32.TryParse(entry, out value))
                {
                    if (value >= 0 && value <= 4)
                    {
                        return value;
                    }
                    else
                    {
                        Console.WriteLine("Please enter a number between 0 and 4, inclusive.");
                    }
                }
                else
                {
                    Console.WriteLine("Error: Input not recognized. Try again, just enter a number between 0 and 4, inclusive");
                }
            } while (true);
        }

        private static void WriteGuess(List<Cell> guessList)
        {
            guessList = guessList.OrderBy(c => c.Value).ToList();
            for (int i = 0; i < guessList.Count - 1; i++)
            {
                Console.Write(guessList[i].Value + ", ");
            }
            Console.WriteLine(guessList.Last().Value);
        }

        private static Cell GetCellAtLevel(double level)
        {
            double total = 0;
            foreach (var cell in Cells)
            {
                total += cell.Confidence;
                if (total > level) return cell;
            }

            return Cells.Last();
        }

        private static double GetTotalSizeOfCells()
        {
            double ret = 0;
            foreach (var cell in Cells)
            {
                ret += cell.Confidence;
            }
            return ret;
        }
    }

    internal class Cell
    {
        public Cell(int value, double confidence)
        {
            Value = value;
            Confidence = confidence;
        }
        public int Value { get; set; }
        public double Confidence { get; set; }
    }
}
