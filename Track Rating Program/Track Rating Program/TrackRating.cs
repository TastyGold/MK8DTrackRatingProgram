using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Track_Rating_Program
{
    static class TrackRating
    {
        public static string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MK8DTrackRatings\\";

        private static List<Track> trackData = new List<Track>();

        private static string currentTrackDataFilePath = string.Empty;

        private static Random rand = new Random();

        public static void WriteTrackDataToFile(string fileName)
        {
            currentTrackDataFilePath = appDataDir + fileName;
            Directory.CreateDirectory(appDataDir);
            FileStream s = File.Create(appDataDir + fileName);
            s.Close();

            string[] lines = new string[trackData.Count];

            for (int i = 0; i < trackData.Count; i++)
            {
                lines[i] = $"{trackData[i].name}, {trackData[i].rating}, {trackData[i].comparisons}";
            }

            File.WriteAllLines(appDataDir + fileName, lines);
        }

        public static void LoadTrackDataFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                FileStream s = File.Create(filePath);
                s.Close();
            }

            string[] lines = File.ReadAllLines(filePath);

            string[] thumbnails = Directory.GetFiles(Form1.thumbnailDir);
            for (int i = 0; i < thumbnails.Length; i++)
            {
                thumbnails[i] = Path.GetFileNameWithoutExtension(thumbnails[i]);
            }
            thumbnails.OrderBy(t => t);

            int lineIndex = 0;
            for (int i = 0; i < thumbnails.Length; i++)
            {
                if (lineIndex < lines.Length)
                {
                    string[] elements = lines[lineIndex].Split(new char[2] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (elements[0] == thumbnails[i])
                    {
                        lineIndex++;

                        trackData.Add(new Track(elements[0], Convert.ToInt32(elements[1]), Convert.ToInt32(elements[2])));
                    }
                    else
                    {
                        trackData.Add(new Track(thumbnails[i], 1300, 0));
                    }
                }
                else
                {
                    trackData.Add(new Track(thumbnails[i], 1300, 0));
                }
            }
        }

        public static Track GetRandomTrack()
        {
            int trackIndex = rand.Next(0, trackData.Count);
            return trackData[trackIndex];
        }

        public static Track GetLeastComparedTrack()
        {
            int lowestIndex = 0;
            for (int i = 0; i < trackData.Count; i++)
            {
                if (trackData[i].comparisons < trackData[lowestIndex].comparisons)
                {
                    lowestIndex = i;
                }
            }
            return trackData[lowestIndex];
        }
    }

    public class Track
    {
        public readonly string name;
        public int rating;
        public int comparisons;

        private const int maxEloGainPerComparison = 128;

        public void UpdateRating(int score, int opponentRating)
        {
            int maxEloChange = maxEloGainPerComparison * (comparisons < 3 ? 4 - comparisons : 1);

            double expectedScore = 1 / (1 + Math.Pow(10, (opponentRating - rating) / 400));

            double newRating = rating + (maxEloChange * (score - expectedScore));

            rating = (int)newRating;
            comparisons++;
        }

        public string GetThumbnailPath()
        {
            return Form1.thumbnailDir + name + ".png";
        }
        
        public Track(string name)
        {
            this.name = name;
        }

        public Track(string name, int rating, int comparisons) : this(name)
        {
            this.rating = rating;
            this.comparisons = comparisons;
        }
    }
}
