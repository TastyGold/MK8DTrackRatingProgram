using System.Diagnostics;

namespace Track_Rating_Program
{
    public partial class Form1 : Form
    {
        public static readonly string thumbnailDir = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName)!.FullName)!.FullName + "\\TrackThumbnails\\";

        private Track trackA, trackB;

        private int totalComparisons = 0;

        public Form1()
        {
            InitializeComponent();
            InitializeForm();
            TrackRating.LoadTrackDataFromFile(TrackRating.appDataDir + "ratings_01.csv");
            TrackRating.WriteTrackDataToFile("ratings_01.csv");
            totalComparisons = TrackRating.GetTotalComparisons();
            SetComparisons(totalComparisons);
            SetupNewComparison();
            this.Text = "Track Selector";
        }

        public void SetComparisons(int count)
        {
            label1.Text = "Total Comparisons: " + count.ToString();
        }

        public void InitializeForm()
        {
            button1.Text = string.Empty;
            button2.Text = string.Empty;
        }

        public void ResetTrackThumbnails()
        {
            button1.BackgroundImage = Image.FromFile(trackA.GetThumbnailPath());
            button2.BackgroundImage = Image.FromFile(trackB.GetThumbnailPath());
        }

        public void SetupNewComparison()
        {
            int maxDifference = 500;
            trackA = TrackRating.GetLeastComparedTrack();
            do
            {
                trackB = TrackRating.GetRandomTrack();
                maxDifference += 10;
            } 
            while (ReferenceEquals(trackA, trackB) || Math.Abs(trackA.rating - trackB.rating) > maxDifference);

            ResetTrackThumbnails();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //left choice made
            int tempRating = trackA.rating;
            trackA.UpdateRating(1, trackB.rating);
            trackB.UpdateRating(0, tempRating);
            SetupNewComparison();
            totalComparisons++;
            SetComparisons(totalComparisons);
            this.Text = "Track Selector (Unsaved changes)";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //right choice made
            int tempRating = trackA.rating;
            trackA.UpdateRating(0, trackB.rating);
            trackB.UpdateRating(1, tempRating);
            SetupNewComparison();
            totalComparisons++;
            SetComparisons(totalComparisons);
            this.Text = "Track Selector (Unsaved changes)";
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start(TrackRating.appDataDir);
        }

        private void writeToFileButton_Click(object sender, EventArgs e)
        {
            TrackRating.WriteTrackDataToFile("ratings_01.csv");
            this.Text = "Track Selector";
        }
    }
}