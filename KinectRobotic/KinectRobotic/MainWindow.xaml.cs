using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LegoNXTLibrary;
using Microsoft.Kinect;

namespace KinectRobotic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly KinectSensor kinectSensor;
        byte[] pixelData;
        Skeleton[] skeletons;
        private LegoNXTDriver legoNxt;
        private bool isConnectedToRobot = false;


        private DateTime whenBeReady = DateTime.Now;
        private bool IsRobotReady
        {
            get
            {
                if (whenBeReady < DateTime.Now)
                {
                    return true;
                }
                return false;
            }
        }

        public MainWindow()
        {

            kinectSensor = KinectSensor.KinectSensors[0];

            Loaded += MainWindowLoaded;
            Closing += MainWindowClosing;

            kinectSensor.ColorStream.Enable();
            kinectSensor.SkeletonStream.Enable();

            InitializeComponent();
        }

        void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            kinectSensor.SkeletonFrameReady += KinectSensorSkeletonFrameReady;
            kinectSensor.ColorFrameReady += KinectSensorColorFrameReady;
            kinectSensor.Start();
        }

        void MainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            kinectSensor.Stop();
        }

        void KinectSensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            bool receivedData = false;

            using (ColorImageFrame cFrame = e.OpenColorImageFrame())
            {
                if (cFrame == null)
                {
                    // The image processing took too long. More than 2 frames behind.
                }
                else
                {
                    pixelData = new byte[cFrame.PixelDataLength];
                    cFrame.CopyPixelDataTo(pixelData);
                    receivedData = true;
                }
            }

            if (receivedData)
            {
                BitmapSource source = BitmapSource.Create(640, 480, 96, 96,
                        PixelFormats.Bgr32, null, pixelData, 640 * 4);
                videoImage.Source = source;
            }
        }

        void KinectSensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            bool receivedData = false;

            using (SkeletonFrame sFrame = e.OpenSkeletonFrame())
            {
                if (sFrame == null)
                {
                    // The image processing took too long. More than 2 frames behind.
                }
                else
                {
                    skeletons = new Skeleton[sFrame.SkeletonArrayLength];
                    sFrame.CopySkeletonDataTo(skeletons);
                    receivedData = true;
                }
            }

            if (receivedData)
            {

                Skeleton currentSkeleton = (from s in skeletons
                                            where s.TrackingState == SkeletonTrackingState.Tracked
                                            select s).FirstOrDefault();
                if (currentSkeleton != null)
                {
                    MoveDetected(currentSkeleton);
                }
            }
        }

        private void MoveDetected(Skeleton currentSkeleton)
        {
            if (IsRobotReady)
            {
                if (currentSkeleton.Joints[JointType.Spine].Position.Z < 1.5)
                {
                    Warnings.Text = "Do przodu";
                    if (isConnectedToRobot) legoNxt.MoveOneStepForward();
 
                    whenBeReady = DateTime.Now + TimeSpan.FromSeconds(1);
                }
                else if (currentSkeleton.Joints[JointType.Spine].Position.Z > 2.5)
                {
                    Warnings.Text = "Do tyłu";
                    if (isConnectedToRobot) legoNxt.MoveOneStepBackwards();
                    
                    whenBeReady = DateTime.Now + TimeSpan.FromSeconds(1);
                }
                else if (currentSkeleton.Joints[JointType.HandRight].Position.X < currentSkeleton.Joints[JointType.Spine].Position.X - 0.2)
                {
                    Warnings.Text = "Skręt w lewo";
                    if (isConnectedToRobot) legoNxt.TurnLeft();
                    
                    whenBeReady = DateTime.Now + TimeSpan.FromSeconds(5);
                }
//                else if (currentSkeleton.Joints[JointType.HandRight].Position.X >
//                         (currentSkeleton.Joints[JointType.Spine].Position.X + 0.4))
                else if (currentSkeleton.Joints[JointType.HandLeft].Position.X > currentSkeleton.Joints[JointType.Spine].Position.X + 0.2)
                {
                    Warnings.Text = "Skręt w prawo";
                    if (isConnectedToRobot) legoNxt.TurnRight();
                    
                    whenBeReady = DateTime.Now + TimeSpan.FromSeconds(5);

                }
                else if (currentSkeleton.Joints[JointType.HandRight].Position.Z + 0.6 <
                         currentSkeleton.Joints[JointType.Spine].Position.Z)
                {
                    Warnings.Text = "Prawa reka do przodu";
                    if (isConnectedToRobot) legoNxt.MoveArms(Direction.Left, 0); //drugi parametr nie używany :p

                    whenBeReady = DateTime.Now + TimeSpan.FromSeconds(0.5);

                }
                else if (currentSkeleton.Joints[JointType.HandLeft].Position.Z + 0.6 <
                         currentSkeleton.Joints[JointType.Spine].Position.Z)
                {
                    Warnings.Text = "Lewa reka do przodu";
                    if (isConnectedToRobot) legoNxt.MoveArms(Direction.Right, 0); //drugi parametr nie używany :P 

                    whenBeReady = DateTime.Now + TimeSpan.FromSeconds(0.5);
                }
                else
                {
                    Warnings.Text = "";
                }
            }
        }

        private void AboutClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(
                "Program stworzony przez: Dawid Cichy, Marcin Dziarmaga, Damian Zawada, Jakub Izydorczyk, Emil Pytka",
                "Grupa .NET FTIMS");
        }

        private void StopKinectClick(object sender, RoutedEventArgs e)
        {
            kinectSensor.Stop();
        }

        private void StartKinectClick(object sender, RoutedEventArgs e)
        {
            kinectSensor.Start();
        }

        private void ConnectToRobot(object sender, RoutedEventArgs e)
        {
            legoNxt = new LegoNXTDriver(0,"COM3");
            isConnectedToRobot = true;
        }

        private void DisconnectRobot(object sender, RoutedEventArgs e)
        {
            isConnectedToRobot = false;
            legoNxt.DisconnectNXT();
        }

        
    }
}
