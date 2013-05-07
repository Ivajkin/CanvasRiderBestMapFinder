using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;

namespace CanvasRiderBestMapFinder
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        int iterationsCount = 10000;

        private void MainForm_Load(object sender, EventArgs e)
        {
            bestTrack.likeCount = 4394;
            bestTrack.dislikeCount = 5000;
            bestTrack.track = "mystery box";
            bestTrack.author = "webster";

            MainCycle();

        }
        void MainCycle()
        {
            var wc = new WebClient();
            wc.DownloadStringCompleted += (s, ea) =>
            {
                var m = Regex.Match(ea.Result, @"<title>Canvas Rider - (.+) by (.+)</title>");
                string track = m.Groups[1].Value;
                string author = m.Groups[2].Value;

                m = Regex.Match(ea.Result, @"<a href=.users/.+>.+</a> ?-? ?([0-9]+k) ?-? ?<a href=.tracks/.+/down.><img src=.images/down.jpg. alt=.. /></a><span class=.green.>([0-9]+k?)</span><a href=.tracks/.+/up.>");
                double dislikeCount = convertRating(m.Groups[1].Value);
                double likeCount = convertRating(m.Groups[2].Value);

                double rating = likeCount / dislikeCount;

                if (bestTrack.rating < rating)
                {
                    bestTrack.track = track;
                    bestTrack.author = author;
                    bestTrack.dislikeCount = dislikeCount;
                    bestTrack.likeCount = likeCount;
                }

                if (--iterationsCount > 0)
                {
                    MainCycle();
                }
                else
                {
                    Finish();
                }
            };
            wc.DownloadStringAsync(new Uri("http://canvasrider.com/tracks/random"));
            
        }

        void Finish()
        {
            label1.Text = "Track: " + bestTrack.track;
            label2.Text = "Author: " + bestTrack.author;
            label3.Text = "Dislikes: " + bestTrack.dislikeCount;
            label4.Text = "Likes: " + bestTrack.likeCount;
            label5.Text = "Rating: " + bestTrack.rating;
        }

        static int convertRating(string rating)
        {
            if (rating[rating.Length - 1] == 'k')
            {
                return int.Parse(rating.Substring(0, rating.Length - 1)) * 1000;
            }
            else
            {
                return int.Parse(rating);
            }
        }

        struct Track
        {
            public string track;
            public string author;
            public double dislikeCount;
            public double likeCount;
            public double rating
            {
                get
                {
                    return likeCount / dislikeCount;
                }
            }
        }
        Track bestTrack;
    }
}
