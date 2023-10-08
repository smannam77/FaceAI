using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.IO;

namespace AI
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        private const string subscriptionKey = "5237e26734d043cf868add67ea47460c";
        private const string baseUri = "https://smfaceverify.cognitiveservices.azure.com/";

        private readonly IFaceClient faceClient = new FaceClient(
            new ApiKeyServiceClientCredentials(subscriptionKey),
            new System.Net.Http.DelegatingHandler[] { });
        String[] faceDescriptions;
        double resizeFactor;

        private string filePath1 = "";
        private string filePath2 = "";
        private string filePath3 = "";
        WebCam webcam;

        public Register()
        {
            InitializeComponent();
            faceClient.Endpoint = baseUri;
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main_window = new MainWindow();
            main_window.Show();
            this.Close();
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtPerson.Text))
            {
                MessageBox.Show("Enter Name", "AI");
                return;
            }

            if (String.IsNullOrEmpty(filePath1))
            {
                MessageBox.Show("Select Picture 1", "AI");
                return;
            }

            if (String.IsNullOrEmpty(filePath2))
            {
                MessageBox.Show("Select Picture 2", "AI");
                return;
            }

            if (String.IsNullOrEmpty(filePath3))
            {
                MessageBox.Show("Select Picture 3", "AI");
                return;
            }

            string personName = txtPerson.Text.Replace(" ", "_").ToLower();
            string personId = txtPerson.Text.Replace(" ", "_").ToLower();
            Person p = await faceClient.PersonGroupPerson.CreateAsync("profiles", personName);

            foreach (string imageFilePath in Directory.GetFiles(@"C:\ScienceProject\profiles\" + txtPerson.Text.Replace(" ", "_").ToLower()))
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    await faceClient.PersonGroupPerson.AddFaceFromStreamAsync("profiles", p.PersonId, imageFileStream);                    
                }
            }            

            RegisterInfo register_info = new RegisterInfo();
            register_info.Show();
            this.Close();
        }       

        private void btnCapture1_Click(object sender, RoutedEventArgs e)
        {
            imgCapture.Source = imgVideo.Source;

            string folderName = @"C:\ScienceProject\profiles\" + txtPerson.Text.Replace(" ", "_").ToLower();
            string fileName = txtPerson.Text.Replace(" ", "_").ToLower();
            filePath1 = folderName + @"\" + fileName + "1.jpg";

            if (File.Exists(filePath1))
            {
                File.Delete(filePath1);
            }
            
            Helper.SaveImageCapture((BitmapSource)imgCapture.Source, filePath1);
            MessageBox.Show("Image 1 Captured!", "AI");
        }

        private void btnCapture2_Click(object sender, RoutedEventArgs e)
        {
            imgCapture.Source = imgVideo.Source;

            string folderName = @"C:\ScienceProject\profiles\" + txtPerson.Text.Replace(" ", "_").ToLower();
            string fileName = txtPerson.Text.Replace(" ", "_").ToLower();
            filePath2 = folderName + @"\" + fileName + "2.jpg";

            if (File.Exists(filePath2))
            {
                File.Delete(filePath2);
            }

            Helper.SaveImageCapture((BitmapSource)imgCapture.Source, filePath2);
            MessageBox.Show("Image 2 Captured!", "AI");
        }

        private void btnCapture3_Click(object sender, RoutedEventArgs e)
        {
            imgCapture.Source = imgVideo.Source;

            string folderName = @"C:\ScienceProject\profiles\" + txtPerson.Text.Replace(" ", "_").ToLower();
            string fileName = txtPerson.Text.Replace(" ", "_").ToLower();
            filePath3 = folderName + @"\" + fileName + "3.jpg";

            if (File.Exists(filePath3))
            {
                File.Delete(filePath3);
            }

            Helper.SaveImageCapture((BitmapSource)imgCapture.Source, filePath3);
            MessageBox.Show("Image 3 Captured!", "AI");
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            string folderName = @"C:\ScienceProject\profiles\" + txtPerson.Text.Replace(" ", "_").ToLower();
            if (String.IsNullOrEmpty(txtPerson.Text))
                {
                MessageBox.Show("Enter Name", "AI");
                return;
            }

            if (Directory.Exists(folderName))
            {
                string[] filePaths = Directory.GetFiles(folderName);
                foreach (string filePath in filePaths)
                    File.Delete(filePath);

                Directory.Delete(folderName);
            }
            Directory.CreateDirectory(folderName);
            MessageBox.Show("Person Created!", "AI");        
        }

        private void btnUpload1_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "JPG (*.jpg)|*.jpg";
            bool? result = dialog.ShowDialog(this);

            if (!(bool)result)
            {
                return;
            }

            filePath1 = dialog.FileName;
            string folderName = @"C:\ScienceProject\profiles\" + txtPerson.Text.Replace(" ", "_").ToLower();
            string fileName = txtPerson.Text.Replace(" ", "_").ToLower();
            Uri uriSource = new Uri(filePath1);
            BitmapImage source = new BitmapImage();

            source.BeginInit();
            source.CacheOption = BitmapCacheOption.None;
            source.UriSource = uriSource;
            source.EndInit();
            imgCapture.Source = source;
            File.Copy(filePath1, (folderName + @"\" + fileName + "1.jpg"));
        }

        private void btnUpload2_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "JPG (*.jpg)|*.jpg";
            bool? result = dialog.ShowDialog(this);

            if (!(bool)result)
            {
                return;
            }

            filePath2 = dialog.FileName;
            string folderName = @"C:\ScienceProject\profiles\" + txtPerson.Text.Replace(" ", "_").ToLower();
            string fileName = txtPerson.Text.Replace(" ", "_").ToLower();
            Uri uriSource = new Uri(filePath2);
            BitmapImage source = new BitmapImage();

            source.BeginInit();
            source.CacheOption = BitmapCacheOption.None;
            source.UriSource = uriSource;
            source.EndInit();
            imgCapture.Source = source;
            File.Copy(filePath2, (folderName + @"\" + fileName + "2.jpg"));
        }

        private void btnUpload3_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "JPG (*.jpg)|*.jpg";
            bool? result = dialog.ShowDialog(this);

            if (!(bool)result)
            {
                return;
            }

            filePath3 = dialog.FileName;
            string folderName = @"C:\ScienceProject\profiles\" + txtPerson.Text.Replace(" ", "_").ToLower();
            string fileName = txtPerson.Text.Replace(" ", "_").ToLower();
            Uri uriSource = new Uri(filePath3);
            BitmapImage source = new BitmapImage();

            source.BeginInit();
            source.CacheOption = BitmapCacheOption.None;
            source.UriSource = uriSource;
            source.EndInit();
            imgCapture.Source = source;
            File.Copy(filePath3, (folderName + @"\" + fileName + "3.jpg"));
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            webcam = new WebCam();
            webcam.InitializeWebCam(ref imgVideo);
            webcam.Start();
        }
    }
}
