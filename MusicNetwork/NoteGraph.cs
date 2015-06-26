using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MusicNetwork
{
    // enumerations for the notes
    // c major scale is used for convenience
    // ready for bitwise stuff
    // letter indicates tonality, number indicates octave
    enum Note
    {
        e2 = 0x1,
        f2 = 0x2,
        g2 = 0x4,
        a2 = 0x8,
        b2 = 0x10,
        c3 = 0x20,
        d3 = 0x40,
        e3 = 0x80,
        f3 = 0x100,
        g3 = 0x200,
        a3 = 0x400,
        b3 = 0x800,
        c4 = 0x1000,
        d4 = 0x2000,
        e4 = 0x4000,
        f4 = 0x8000,
        g4 = 0x10000,
        a4 = 0x20000,
        b4 = 0x40000,
        c5 = 0x80000,
        d5 = 0x100000,
        e5 = 0x200000
    } //DO NOT TYPE HERE CARELESSLY

    class NoteUtil
    {
        public static readonly Note[] notes = new Note[]{Note.e2, Note.f2, Note.g2, Note.a2, Note.b2, 
                                                         Note.c3, Note.d3, Note.e3, Note.f3, Note.g3, 
                                                         Note.a3, Note.b3, Note.c4, Note.d4, Note.e4, 
                                                         Note.f4, Note.g4, Note.a4, Note.b4, Note.c5,
                                                         Note.d5, Note.e5};

        public static readonly int NOTE_COUNT = 22;

        public static Note GetNoteAtIndex(int index)
        {
            if (index < 0 || index > 21)
            {
                throw new IndexOutOfRangeException();
            }

            return notes[index];
        }

        public static string NoteToString(Note note)
        {
            return note.ToString();
        }

        public static int NoteHash(Note note)
        {
            return (int)Math.Log((double)note, 2.00);
        }

    }

    class Edge
    {
        public NoteNode n;
        public double weight;

        public Edge(NoteNode n, double w)
        {
            this.n = n;
            this.weight = w;
        }
    }

    class NoteNode
    {
        private Note note;

        private Edge[] connections = new Edge[22];

        public NoteNode(Note note)
        {
            this.note = note;
            
        }

        // Returns true if adding the node was successful
        // Returns false if the Note in the NoteNode being added is already connected
        public bool AddConnection(NoteNode note, double weight)
        {
            if (!IsConnected(note.GetNote()))
            {
                connections[NoteUtil.NoteHash(note.GetNote())] = new Edge(note,weight);
                return true;
            }
            return false;
        }

        // Returns boolean representing success of the operation
        public bool SetWeight(Note note, double new_weight)
        {
            if (IsConnected(note))
            {
                connections[NoteUtil.NoteHash(note)].weight = new_weight;
                return true;
            }
            return false;
        }

        public bool AddToWeight(Note note, double to_add)
        {
            if (IsConnected(note))
            {
                connections[NoteUtil.NoteHash(note)].weight += to_add;
                return true;
            }
            return false;
        }

        public double GetEdgeWeight(Note note)
        {
            return connections[NoteUtil.NoteHash(note)].weight;
        }

        public Note GetNote()
        {
            return note;
        }
        public void SetNote(Note note)
        {
            this.note = note;
        }

        public NoteNode GetNextNode(Random generator)
        {
            double random_value = generator.NextDouble();
            int node_index = 0;

            // partition based on weights of connections
            for (int i = 0; i < NoteUtil.NOTE_COUNT; ++i)
            {
                double lower_bound = connections[i].weight * i;
                double upper_bound = connections[i].weight * (i + 1);

                if (random_value >= lower_bound && random_value < upper_bound)
                {
                    node_index = i;
                    break;
                }
            }

            // return the NoteNode at the found index
            return connections[node_index].n;
        }

        private bool IsConnected(Note note)
        {
            return connections[NoteUtil.NoteHash(note)] != null;
        }

        
    }
    class NoteGraph
    {
        private NoteNode[] nodes = new NoteNode[22];

        public NoteGraph()
        {
            double initial_weight = 1.00 / 22.00;

            for (int i = 0; i < NoteUtil.NOTE_COUNT; ++i)
            {
                nodes[i] = new NoteNode(NoteUtil.GetNoteAtIndex(i));
            }

            for (int i = 0; i < NoteUtil.NOTE_COUNT; ++i)
            {
                for (int k = 0; k < NoteUtil.NOTE_COUNT; ++k)
                {
                    nodes[i].AddConnection(nodes[k], initial_weight);
                }
            }
        }

        public void RateSequence(List<NoteNode> sequence, int rating)
        {
            if (rating < 0 || rating > 10)
            {
                Console.WriteLine("Unacceptable rating\nMust be in [0,10]");
                return;
            }

            double modification = (double)(rating - 5) / 50.00;

            for (int i = 0; i < sequence.Count; ++i)
            {
                if (i != sequence.Count - 1)
                {
                    ModifyConnectionWeight(sequence.ElementAt<NoteNode>(i).GetNote(), sequence.ElementAt<NoteNode>(i + 1).GetNote(), modification);
                }
            }
        }

        private void ModifyConnectionWeight(Note from, Note to, double by)
        {
            double distribute_to_rest = -((1.00 / (NoteUtil.NOTE_COUNT - 1)) * by);

            nodes[NoteUtil.NoteHash(from)].AddToWeight(to, by);

            for (int i = 0; i < NoteUtil.NOTE_COUNT; ++i)
            {
                if (i == NoteUtil.NoteHash(to))
                    continue;

                nodes[NoteUtil.NoteHash(from)].AddToWeight(nodes[i].GetNote(), distribute_to_rest);
            }
        }

        public List<NoteNode> GetNoteSequenceAsList(Random gen)
        {   
            List<NoteNode> sequence = new List<NoteNode>();
            NoteNode current = nodes[gen.Next(NoteUtil.NOTE_COUNT)];
            
            // eliminate initial behavior
            for (int i = 0; i < 50; ++i)
            {
                current = current.GetNextNode(gen);
            }

            for (int i = 0; i < 8; ++i)
            {
                sequence.Add(current);
                current = current.GetNextNode(gen);
            }

            return sequence;
        }
    }
}
