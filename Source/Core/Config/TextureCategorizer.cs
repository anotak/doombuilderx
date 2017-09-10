using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeImp.DoomBuilder.Data;

// ano
namespace CodeImp.DoomBuilder.Config
{
    public class TextureCategorizer
    {
        int[] tree;
        // tree has 256 + mscount values, 256 for children chars,
        // mscount for matching sets
        const int NUM_CHILDREN = byte.MaxValue;
        const int SETS_START = NUM_CHILDREN + 1;
        int node_size;
        
        int capacity;
        int size;
        List<MatchingTextureSet> sets;
        int mscount;

        internal TextureCategorizer(List<MatchingTextureSet> insets)
        {
            //long startms = General.stopwatch.ElapsedMilliseconds;
            sets = insets;
            capacity = 262144; // 2 ^ 18, will require 0 extend on doom2.wad
                               // this is 1048576 bytes or ~1mb
            size = 1;
            tree = new int[capacity];

            mscount = sets.Count;
            // ano - we could save here on bytes as mscount per node are holding a bool in 32 each :/
            node_size = NUM_CHILDREN + mscount;
            for (int msindex = 0; msindex < mscount; msindex++)
            {
                List<string> filters = sets[msindex].Filters;
                
                int filtercount = filters.Count;
                for (int filterindex = 0; filterindex < filtercount; filterindex++)
                {
                    byte[] curchars = Encoding.ASCII.GetBytes(filters[filterindex]);
                    int curlength = curchars.Length;

                    int curnode = 0;
                    int curptr = 0;
                    bool previous_wildcard = false;
                    for (int charindex = 0; charindex < curlength; charindex++)
                    {
                        // debugcrap
                        /*
                        if (filters[filterindex] == "FLAT1_1")
                        {
                            string logline = charindex + " " + (char)curchars[charindex] + "  :: " + curnode + " in set " + sets[msindex].Name;
                            Logger.WriteLogLine(logline);
                        }*/

                        if (charindex == curlength - 1 && curchars[charindex] == (byte)'*')
                        {
                            tree[curptr + SETS_START + msindex] = 2;
                            break; // for < curlength
                        }


                        int charoffset = curchars[charindex];

                        if (previous_wildcard && charoffset == (byte)'*')
                        {
                            // dont duplicate work for **
                            continue; // for charindex < curlength
                        }

                        int nextnode = tree[curptr + charoffset];

                        if (nextnode == 0)
                        {
                            nextnode = AddNew();

                            if (previous_wildcard) // create circular node for *
                            {
                                for (int i = 0; i < byte.MaxValue; i++)
                                {
                                    if (i != (byte)'*' && i != (byte)'+' && i != charoffset)
                                    {
                                        tree[curptr + i] = curnode;
                                    }
                                }
                            }

                            tree[curptr + charoffset] = nextnode;
                        }

                        if (charoffset == (byte)'*')
                        {
                            previous_wildcard = true;
                        }

                        curnode = nextnode;
                        curptr = nextnode * node_size;

                        // mark if it's in the end of the list
                        if (charindex == curlength - 1 && tree[curptr + SETS_START + msindex] < 2)
                        {
                            if (curchars[charindex] == (byte)'*')
                            {
                                tree[curptr + SETS_START + msindex] = 2;
                            }
                            else
                            {
                                tree[curptr + SETS_START + msindex] = 1;
                            }

                            // debugcrap
                            /*
                            if (sets[msindex].Name == "Rock")
                            {
                                string logline = "ADDED " + filters[filterindex];
                                Logger.WriteLogLine(logline);
                            }*/
                        }
                    }
                }
            }
            
            //Logger.WriteLogLine("elapsed texturecategorizer " + (General.stopwatch.ElapsedMilliseconds - startms));
            //Logger.WriteLogLine("size " + size + " size*nodesize " + (size*node_size) + " capacity " + capacity + " mscount " + mscount);
            
        } // constructor

        void Extend()
        {
            int[] newtree = new int[capacity * 2];

            int realsize = size * node_size;
            for (int i = 0; i < realsize; i++)
            {
                newtree[i] = tree[i];
            }
            tree = newtree;
            capacity *= 2;
            //Logger.WriteLogLine("EXTEND");
        }

        int AddNew()
        {
            if ((size + 1) * node_size > capacity)
            {
                Extend();
            }
            int output = size;
            size++;
            return output;
        }

        static List<int> matchingptrs = new List<int>(8);
        public void PlaceInSets(ImageData img, bool bTexture)
        {
            byte[] inarray = Encoding.ASCII.GetBytes(img.Name.ToUpperInvariant());
            int incount = inarray.Length;
            matchingptrs.Clear();
            matchingptrs.Add(0);
            int ptrcount = 1;

            int[] nextwildcard = new int[byte.MaxValue];
            bool[] added = new bool[mscount];

            for (int stringindex = 0; stringindex < incount; stringindex++)
            {
                bool bNoneAlive = true; // any branches still kickin it?

                int ptrsAddedThisLoop = 0; // dont do it while the loop is going

                for (int i = 0; i < ptrcount; i++)
                {
                    int curptr = matchingptrs[i];
                    if (curptr < 0)
                    {
                        // okay this branch is over
                        continue;
                    }

                    // debug crap
                    /*{
                        if (img.Name == "FLAT1_1")
                        {
                            string logline = stringindex + " " + (char)inarray[stringindex] + "  :: ";
                            for (int j = 0; j < node_size; j++)
                            {
                                if (tree[curptr + j] != 0)
                                {
                                    if (j > NUM_CHILDREN)
                                    {
                                        logline += j.ToString("x") + "=" + sets[j - SETS_START].Name + "-> " + tree[curptr + j] + ", ";
                                    } else {
                                        logline += j.ToString("x") + "=" + (char)j + "-> " + tree[curptr + j] + ", ";
                                    }
                                }
                            }
                            Logger.WriteLogLine(logline);
                            if (stringindex == 6)
                            {
                                Logger.WaitForBufferToClear();
                                Console.WriteLine("HAHA");
                            }
                        }
                    }*/

                    
                    // add a branch for * and +
                    if (tree[curptr + (byte)'+'] > 0)
                    {
                        matchingptrs.Add(tree[curptr + (byte)'+'] * node_size);
                        ptrsAddedThisLoop++;
                        bNoneAlive = false;
                    }

                    if (tree[curptr + (byte)'*'] > 0)
                    {
                        matchingptrs.Add(tree[curptr + (byte)'*'] * node_size);
                        ptrsAddedThisLoop++;
                        bNoneAlive = false;
                    }

                    curptr = tree[curptr + inarray[stringindex]] * node_size;

                    if (curptr == 0)
                    {
                        matchingptrs[i] = -1;
                        continue;
                    }

                    bNoneAlive = false; // in this loop in order to quit if it's in
                    for (int j = 0; j < mscount; j++)
                    {
                        if (added[j])
                        {
                            continue;
                        }

                        if (tree[curptr + SETS_START + j] == 2 // filter ends with *
                            || (stringindex == incount - 1 && tree[curptr + SETS_START + j] == 1) // actual end of string
                            )
                        {
                            if (bTexture)
                            {
                                sets[j].AddTextureForceNoMatching(img);
                            }
                            else
                            {
                                sets[j].AddFlatForceNoMatching(img);
                            }
                            //Logger.WriteLogLine("added " + img.Name + " to set " + sets[j].Name);

                            added[j] = true; // no need to add more than once
                        }
                    } // for < mscount

                    matchingptrs[i] = curptr;

                } // for < ptrcount;

                if (bNoneAlive)
                {
                    return;
                }

                ptrcount += ptrsAddedThisLoop;
            } // for < incount

        } // placeinsets
        
    }
}
