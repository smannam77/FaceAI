using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AI
{
    /// <summary>
    /// Interaction logic for LoginInfo.xaml
    /// </summary>
    public partial class LoginInfo : Window
    {
        private const string subscriptionKey = "5237e26734d043cf868add67ea47460c";
        private const string baseUri = "https://smfaceverify.cognitiveservices.azure.com/";

        private readonly IFaceClient faceClient = new FaceClient(
            new ApiKeyServiceClientCredentials(subscriptionKey),
            new System.Net.Http.DelegatingHandler[] { });

        IList<DetectedFace> faceList;
        String[] faceDescriptions;
        double resizeFactor;

        public LoginInfo()
        {
            InitializeComponent();
            faceClient.Endpoint = baseUri;
        }
        public LoginInfo(double Confidence) : this()
        {
            if (Confidence > 0.5)
            {
                lblResult.Content ="Logged in successfully!";               
                Uri uriSource = new Uri(@"C:\ScienceProject\profiles\TempFaceTest\Test.jpg");
                BitmapImage source = new BitmapImage();

                source.BeginInit();
                source.CacheOption = BitmapCacheOption.None;
                source.UriSource = uriSource;
                source.EndInit();
                imgImage1.Source = source;                
            }
            else
            {
                lblResult.Content = "Failed to login!";
            }           
        }
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main_window = new MainWindow();
            main_window.Show();
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

        private async void btnDetect_Click(object sender, RoutedEventArgs e)
        {
            string filePath = @"C:\ScienceProject\profiles\TempFaceTest\Test.jpg";
            Uri uriSource = new Uri(filePath);
            BitmapImage source = new BitmapImage();

            source.BeginInit();
            source.CacheOption = BitmapCacheOption.None;
            source.UriSource = uriSource;
            source.EndInit();
            imgImage1.Source = source;

            faceList = await UploadAndDetectFaces(filePath);

            if (faceList.Count > 0)
            {
                DrawingVisual visual = new DrawingVisual();
                DrawingContext context = visual.RenderOpen();
                context.DrawImage(source, new Rect(0, 0, source.Width, source.Height));
                double dpi = source.DpiX;
                resizeFactor = (dpi > 0) ? 96 / dpi : 1;
                faceDescriptions = new String[faceList.Count];

                for (int i = 0; i < faceList.Count; ++i)
                {
                    DetectedFace face = faceList[i];
                    context.DrawRectangle(
                        Brushes.Transparent,
                        new Pen(Brushes.Green, 5),
                        new Rect(
                            face.FaceRectangle.Left * resizeFactor,
                            face.FaceRectangle.Top * resizeFactor,
                            face.FaceRectangle.Width * resizeFactor,
                            face.FaceRectangle.Height * resizeFactor
                            )
                    );
                }
                context.Close();

                RenderTargetBitmap facewithRectangle = new RenderTargetBitmap(
                    (int)(source.PixelWidth * resizeFactor),
                    (int)(source.PixelHeight * resizeFactor),
                    96,
                    96,
                    PixelFormats.Default);

                facewithRectangle.Render(visual);
                imgImage1.Source = facewithRectangle;
            }

            ImageSource imageSource = imgImage1.Source;
            BitmapSource bitmapSource = (BitmapSource)imageSource;
            var scale = imgImage1.ActualWidth / (bitmapSource.PixelWidth / resizeFactor);

            for (int i = 0; i < faceList.Count; ++i)
            {
                FaceRectangle fr = faceList[i].FaceRectangle;
                double left = fr.Left * scale;
                double top = fr.Top * scale;
                double width = fr.Width * scale;
                double height = fr.Height * scale;
                faceDescriptions[i] = FaceDescription(faceList[i]);
                TextBlock1.Text = TextBlock1.Text + string.Format("Person {0}: {1} \n", i.ToString(), faceDescriptions[i]);
            }
        }
        private string FaceDescription(DetectedFace face)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(face.FaceAttributes.Gender);
            sb.Append(", ");
            sb.Append(face.FaceAttributes.Age);
            sb.Append(", ");
            sb.Append(String.Format("smile {0:F1}%, ", face.FaceAttributes.Smile * 100));
            sb.Append("Emotion: ");
            Emotion emotionScores = face.FaceAttributes.Emotion;
            if (emotionScores.Anger >= 0.1f)
                sb.Append(String.Format("anger {0:F1}%, ", emotionScores.Anger * 100));

            if (emotionScores.Contempt >= 0.1f)
                sb.Append(String.Format("contempt {0:F1}%, ", emotionScores.Contempt * 100));

            if (emotionScores.Disgust >= 0.1f)
                sb.Append(String.Format("disgust {0:F1}%, ", emotionScores.Disgust * 100));

            if (emotionScores.Fear >= 0.1f)
                sb.Append(String.Format("fear {0:F1}%, ", emotionScores.Fear * 100));

            if (emotionScores.Happiness >= 0.1f)
                sb.Append(String.Format("happiness {0:F1}%, ", emotionScores.Happiness * 100));

            if (emotionScores.Neutral >= 0.1f)
                sb.Append(String.Format("neutral {0:F1}%, ", emotionScores.Neutral * 100));

            if (emotionScores.Sadness >= 0.1f)
                sb.Append(String.Format("sadness {0:F1}%, ", emotionScores.Sadness * 100));

            if (emotionScores.Surprise >= 0.1f)
                sb.Append(String.Format("surprise {0:F1}%, ", emotionScores.Surprise * 100));

            sb.Append(face.FaceAttributes.Glasses);
            sb.Append(", ");

            sb.Append("Hair: ");
            if (face.FaceAttributes.Hair.Bald >= 0.01f)
                sb.Append(String.Format("bald {0:F1}% ", face.FaceAttributes.Hair.Bald * 100));

            IList<HairColor> hairColors = face.FaceAttributes.Hair.HairColor;
            foreach (HairColor hairColor in hairColors)
            {
                if (hairColor.Confidence >= 0.1f)
                {
                    sb.Append(hairColor.Color.ToString());
                    sb.Append(String.Format(" {0:F1}% ", hairColor.Confidence * 100));
                }
            }

            sb.Append(string.Format("Blur: {0}", face.FaceAttributes.Blur.BlurLevel));
            sb.Append(string.Format("Facial Hair: {0}", face.FaceAttributes.FacialHair.Beard));
            sb.Append(string.Format("Head Pose: {0}", face.FaceAttributes.HeadPose.Roll));
            sb.Append(string.Format("Makeup: {0}", face.FaceAttributes.Makeup.LipMakeup));
            sb.Append(string.Format("Noise: {0}", face.FaceAttributes.Noise.NoiseLevel));
            sb.Append(string.Format("Occlusion: {0}", face.FaceAttributes.Occlusion.ForeheadOccluded));
            sb.Replace(",", Environment.NewLine);
            return sb.ToString();
        }
    }
}
