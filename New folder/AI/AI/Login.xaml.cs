using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace AI
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private const string subscriptionKey = "5237e26734d043cf868add67ea47460c";
        private const string baseUri = "https://smfaceverify.cognitiveservices.azure.com/";

        private readonly IFaceClient faceClient = new FaceClient(
            new ApiKeyServiceClientCredentials(subscriptionKey),
            new System.Net.Http.DelegatingHandler[] { });

        IList<DetectedFace> faceList;
        private string imagePath;
        private double Confidence = 0;
        WebCam webcam;

        public Login()
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
        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            imgCapture.Source = imgVideo.Source;

            string folderName = @"C:\ScienceProject\profiles\TempFaceTest";

            if (Directory.Exists(folderName))
            {
                string[] filePaths = Directory.GetFiles(folderName);
                foreach (string filePhoto in filePaths)
                    File.Delete(filePhoto);

                Directory.Delete(folderName);
            }
            Directory.CreateDirectory(folderName);
            Helper.SaveImageCapture((BitmapSource)imgCapture.Source, folderName + @"\test.jpg");
            imagePath = folderName + @"\test.jpg";
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            faceList = await UploadAndDetectFaces(imagePath); 
           
            // List all the people in this group 
            IList<Person> people = await faceClient.PersonGroupPerson.ListAsync("profiles");
            foreach (Person person in people)
            {
                // Compare Face Id created from upload with Person 
                VerifyResult vr = await faceClient.Face.VerifyFaceToPersonAsync(faceList[0].FaceId.Value, person.PersonId, "profiles");
                Confidence = vr.Confidence;
            }

            LoginInfo login_info = new LoginInfo(Confidence);
            login_info.Show();
            this.Close();
        }
        private async Task<IList<DetectedFace>> UploadAndDetectFaces(string imageFilePath)
        {
            try
            {
                IList<FaceAttributeType> faceAttributes =
                    new FaceAttributeType[]
                    {
                        FaceAttributeType.Age,
                        FaceAttributeType.Blur,
                            FaceAttributeType.Emotion,
                            FaceAttributeType.FacialHair,
                            FaceAttributeType.Gender,
                            FaceAttributeType.Glasses,
                            FaceAttributeType.Hair,
                            FaceAttributeType.HeadPose,
                            FaceAttributeType.Makeup,
                            FaceAttributeType.Noise,
                            FaceAttributeType.Occlusion,
                            FaceAttributeType.Smile
                    };

                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    IList<DetectedFace> faceList =
                        await faceClient.Face.DetectWithStreamAsync(
                            imageFileStream, true, false, faceAttributes);
                    return faceList;
                }
            }

            catch (APIErrorException f)
            {
                MessageBox.Show(f.Message);
                return new List<DetectedFace>();
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
                return new List<DetectedFace>();
            }
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "JPG (*.jpg)|*.jpg";
            bool? result = dialog.ShowDialog(this);

            if (!(bool)result)
            {
                return;
            }

            string filePath = dialog.FileName;
            Uri uriSource = new Uri(filePath);
            BitmapImage source = new BitmapImage();

            source.BeginInit();
            source.CacheOption = BitmapCacheOption.None;
            source.UriSource = uriSource;
            source.EndInit();
            imgCapture.Source = source;

            string folderName = @"C:\ScienceProject\profiles\TempFaceTest";

            if (Directory.Exists(folderName))
            {
                string[] filePaths = Directory.GetFiles(folderName);
                foreach (string filePhoto in filePaths)
                    File.Delete(filePhoto);

                Directory.Delete(folderName);
            }
            Directory.CreateDirectory(folderName);
            File.Copy(filePath, (folderName + @"\test.jpg"));
            imagePath = folderName + @"\test.jpg";
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            webcam = new WebCam();
            webcam.InitializeWebCam(ref imgVideo);
            webcam.Start();
        }
    }
}
