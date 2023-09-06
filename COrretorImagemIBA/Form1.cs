using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CorretorImagemIBA
{
    public partial class Form1 : Form
    {
        private Timer timerCorrectImages = new Timer();
        public Form1()
        {
            InitializeComponent();
            timerCorrectImages.Interval = 15000;
            timerCorrectImages.Tick += TimerCorrectImages_Tick;

            this.WindowState = FormWindowState.Minimized;
        }

        private void TimerCorrectImages_Tick(object sender, EventArgs e)
        {
            StartCorrecting();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            StartCorrecting();
            timerCorrectImages.Start();
        }

        private void StartCorrecting()
        {
            this.Hide();

            var allDirectories = GetAllDirectories();

            foreach (var dir in allDirectories)
            {
                var files = Directory.GetFiles(dir);
                EditImages(files);
            }
        }

        private static void EditImages(string[] files)
        {
            foreach (var file in files)
            {
                if (!File.Exists(file)) continue;
                if (!(file.Contains(".jpg") || file.Contains(".jpeg") || file.Contains(".png"))) continue;
                if (file.Contains("_@corrected")) continue;
                if (file.Contains("@.jpg") || file.Contains("@.jpeg") || file.Contains("@.png")) continue;
                
                Image img = Image.FromFile(file);
                int width = (int)(img.Height * 1.777777777777778);

                Bitmap bitmap = new Bitmap(width, img.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.Clear(Color.Green);
                graphics.DrawImage(img, (width - img.Width), 0, img.Width, img.Height);
                var tempName = file.Replace(".jpg", "@.jpg")
                    .Replace(".jpeg", "@.jpeg")
                    .Replace(".png", "@.png");
                bitmap.Save(tempName);

                img.Dispose();

                File.Delete(file);
            }
        }

        private List<string> GetAllDirectories()
        {
            var directories = Directory.GetDirectories("C:\\Holyrics\\Holyrics\\files\\media\\image").ToList();
            directories.Add("C:\\Holyrics\\Holyrics\\files\\media\\image");
            List<string> referenceDirectory = new List<string>(directories);
            List<string> allDirectories = new List<string>(directories);

            bool allDirectoriesFound = false;

            while (!allDirectoriesFound)
            {
                allDirectoriesFound = true;
                directories.ForEach(x =>
                {
                    var dirs = Directory.GetDirectories(x).ToList();
                    if (dirs.Count > 0)
                        allDirectoriesFound = false;

                    referenceDirectory.Remove(x);
                    referenceDirectory.AddRange(dirs);

                    allDirectories.AddRange(dirs);
                });

                directories.Clear();
                referenceDirectory.ForEach(x => directories.Add(x));
            }

            return allDirectories;
        }
    }
}
