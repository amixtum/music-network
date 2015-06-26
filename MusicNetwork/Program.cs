using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            NoteGraph test = new NoteGraph();

            Random gen = new Random();

            string user_input = "";

            while (true)
            {
                if (user_input == "-1")
                    break;

                List<NoteNode> sequence = test.GetNoteSequenceAsList(gen);

                PrintSequence(sequence);

                Console.WriteLine("Rate this sequence on a 0-10 scale from bad to good\nEnter -1 to quit");

                test.RateSequence(sequence, Int32.Parse(Console.ReadLine()));
            }
        }
        static void PrintSequence(List<NoteNode> notes)
        {
            foreach (NoteNode note in notes)
            {
                Console.WriteLine(note.GetNote().ToString());
            }
        }
    }
}
