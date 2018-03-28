using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace ConsoleApplication1
{   
    public struct PLAY
    {
        public string _PLAY_ID;
        public int _SONG_ID;
        public int _CLIENT_ID;
        public string _PLAY_TS;

        public PLAY(string ar1, int ar2, int ar3, string ar4)
        {
            _PLAY_ID = ar1;
            _SONG_ID = ar2;
            _CLIENT_ID = ar3;
            _PLAY_TS = ar4;
        }
    }

    public struct DISTINCT_PLAYS
    {
        public int DISTINCT_SONG_COUNT;
        public int CLIENT_COUNT;

        public DISTINCT_PLAYS(int ar1, int ar2)
        {
            DISTINCT_SONG_COUNT = ar1;
            CLIENT_COUNT = ar2;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            string filepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                @"Data\exhibitA-input.csv");
            string str = "";
            List<PLAY> list = new List<PLAY>();
            var shortenedList = new List<PLAY>();
            var distinctClients = new List<int>();
            string dateInput = "";

            if (File.Exists(filepath))
            {   //read file
                StreamReader sr = new StreamReader(filepath);
                sr.ReadLine(); //skip first line - column descriptor
                while ((str = sr.ReadLine()) != null)
                {   
                    string[] cols = str.Split('\t');
                    list.Add(new PLAY(cols[0], Convert.ToInt32(cols[1]), Convert.ToInt32(cols[2]), cols[3]));
                    //displayList(list);
                    //Console.ReadKey();
                }
                sr.Close();
                Console.WriteLine("\n!File read success!\n");
                //Main algorithm
                //dateInput = "10/08/2016";
                Console.WriteLine("Enter the date to display distinct song plays per user.");
                Console.WriteLine("Format: 10/08/2016");
                watch.Stop();
                dateInput = Console.ReadLine();
                watch.Start();
                
                for(int i=0; i<list.Count; i++)
                {
                    if(list.ElementAt(i)._PLAY_TS.Contains(dateInput))
                    {
                        shortenedList.Add(list.ElementAt(i));
                        if (!distinctClients.Contains(list.ElementAt(i)._CLIENT_ID))
                        {
                            distinctClients.Add(list.ElementAt(i)._CLIENT_ID);
                        }
                    }
                }

                distinctClients.Sort();
                shortenedList.Sort(delegate(PLAY x, PLAY y)
                {
                    return x._CLIENT_ID.CompareTo(y._CLIENT_ID);
                });


                List<int> playedSongs = new List<int>();
                int prevClient;
                List<DISTINCT_PLAYS> distinct_plays = new List<DISTINCT_PLAYS>();
                DISTINCT_PLAYS updatedItem;
                int index;

                for (int i = 0; i < shortenedList.Count; i++)
                {
                    prevClient = shortenedList.ElementAt(i)._CLIENT_ID;
                    playedSongs.Clear();
                    while(i < shortenedList.Count && shortenedList.ElementAt(i)._CLIENT_ID == prevClient)
                    {
                        prevClient = shortenedList.ElementAt(i)._CLIENT_ID;
                        if (!playedSongs.Contains(shortenedList.ElementAt(i)._SONG_ID))
                        {
                            playedSongs.Add(shortenedList.ElementAt(i)._SONG_ID);
                        }
                        i++;
                    }
                    i--;

                    index = findItem(distinct_plays, playedSongs.Count);
                    if( index != -1)
                    {   
                        updatedItem = new DISTINCT_PLAYS(distinct_plays.ElementAt(index).DISTINCT_SONG_COUNT,
                                                    distinct_plays.ElementAt(index).CLIENT_COUNT+1);
                        distinct_plays[index] = updatedItem;
                    }
                    else
                    {
                        distinct_plays.Add(new DISTINCT_PLAYS(playedSongs.Count,1));
                    }
                }

                distinct_plays.Sort(delegate(DISTINCT_PLAYS x, DISTINCT_PLAYS y)
                {
                    return x.DISTINCT_SONG_COUNT.CompareTo(y.DISTINCT_SONG_COUNT);
                });

                displayDistinctList(distinct_plays);

            }
            else
            {
                Console.WriteLine("File not found");
            }
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Execution time: {0} ms",elapsedMs);

            //Console.WriteLine("Closed");
            Console.ReadKey();
        }

        public static void displayList(List<PLAY> list)
        {   
            string str = "";
            Console.WriteLine();
            for(int i=0; i<list.Count; i++)
            {
                str = list.ElementAt(i)._PLAY_ID + "\t";
                str += list.ElementAt(i)._SONG_ID + "\t";
                str += list.ElementAt(i)._CLIENT_ID + "\t";
                str += list.ElementAt(i)._PLAY_TS ;
                Console.WriteLine(str);
            }
            Console.WriteLine();
        }


        public static int findItem(List<DISTINCT_PLAYS> distinct_plays, int song_count)
        {
            bool found = false;
            int returnIndex = -1;
            for (int i = 0; i < distinct_plays.Count && !found; i++)
            {
                if(distinct_plays.ElementAt(i).DISTINCT_SONG_COUNT == song_count)
                {
                    found = true;
                    returnIndex = i;
                }
            }
            return returnIndex;
        }

        public static void displayDistinctList(List<DISTINCT_PLAYS> list)
        {
            Console.WriteLine();
            string str = "";
            Console.WriteLine("DISTINCT_PLAY_COUNT\tCLIENT_COUNT");
            for (int i = 0; i < list.Count; i++)
            {
                str = list.ElementAt(i).DISTINCT_SONG_COUNT + "\t";
                str += list.ElementAt(i).CLIENT_COUNT;
                Console.WriteLine(str);
            }
            Console.WriteLine();
        }

    }
}
