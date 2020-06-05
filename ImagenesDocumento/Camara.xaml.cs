using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Threading;
using System.Collections.ObjectModel;
using AForge.Video.DirectShow;
using AForge.Video;
using System.Data.SqlClient;
using System.IO;
using System.Data;

namespace ImagenesDocumento
{
    public partial class Camara : Window, INotifyPropertyChanged
    {

        #region Public properties

        public ObservableCollection<FilterInfo> VideoDevices { get; set; }

        public FilterInfo CurrentDevice
        {
            get { return _currentDevice; }
            set { _currentDevice = value; this.OnPropertyChanged("CurrentDevice"); }
        }
        private FilterInfo _currentDevice;

        #endregion


        #region Private fields

        private IVideoSource _videoSource;

        #endregion

        Boolean imageSaveSql = false;

        public int idrowcab = 0;

        dynamic SiaWin;        
        int idemp = 0;
        string cnEmp = "";

        public Camara()
        {
            InitializeComponent();
            this.DataContext = this;
            GetVideoDevices();
            this.Closing += MainWindow_Closing;

            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;

            LoadConfig();

        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);             
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();                
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopCamera();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartCamera();
        }

        private void video_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            try
            {
                BitmapImage bi;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    bi = bitmap.ToBitmapImage();
                }
                bi.Freeze(); // avoid cross thread operations and prevents leaks
                Dispatcher.BeginInvoke(new ThreadStart(delegate { videoPlayer.Source = bi; }));
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StopCamera();
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            imageSaveSql = true;
            StopCamera();

            uploadImage();
        }

        public void uploadImage()
        {
            try
            {
                byte[] imgByteArr = null;
                if (imageSaveSql == true)
                {
                    imgByteArr = ConvertBitmapSourceToByteArray((BitmapSource)videoPlayer.Source);
                }

                using (SqlConnection connection = new SqlConnection(SiaWin.Func.DatosEmp(idemp)))
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    //cmd.CommandText = "insert into documento_ima(cod_ima,cod_doc,image_name,img_cli) values (@cod_ima,@cod_doc,@image_name,@img_cli)";
                    cmd.CommandText = "insert into IMG_ImgDoc(idregDoc,image_desc,imagen,fec_crea) values (@idregDoc,@image_desc,@imagen,@fec_crea)";
                    cmd.Parameters.AddWithValue("@idregDoc", Tx_idrowDoc.Tag);
                    cmd.Parameters.AddWithValue("@image_desc", "DOCUMENTO "+ Tx_idrowDoc.Text);                    
                    cmd.Parameters.AddWithValue("@imagen", imgByteArr);
                    cmd.Parameters.AddWithValue("@fec_crea", DateTime.Now.ToString());
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("imagen guardada en el documento : " + Tx_idrowDoc.Text);
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar la imagen: " + w);
            }


        }


        public static byte[] ConvertBitmapSourceToByteArray(ImageSource imageSource)
        {
            var image = imageSource as BitmapSource;
            byte[] data;
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        private void GetVideoDevices()
        {
            VideoDevices = new ObservableCollection<FilterInfo>();
            foreach (FilterInfo filterInfo in new FilterInfoCollection(FilterCategory.VideoInputDevice))
            {
                VideoDevices.Add(filterInfo);
            }
            if (VideoDevices.Any())
            {
                CurrentDevice = VideoDevices[0];
            }
            else
            {
                MessageBox.Show("No video sources found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartCamera()
        {
            if (CurrentDevice != null)
            {
                _videoSource = new VideoCaptureDevice(CurrentDevice.MonikerString);
                _videoSource.NewFrame += video_NewFrame;
                _videoSource.Start();
            }
        }

        private void StopCamera()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= new NewFrameEventHandler(video_NewFrame);
            }
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }



        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Tx_idrowDoc.Tag = idrowcab.ToString();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;

            string select = "select * from incab_doc where idreg='" + idrowcab + "' ";
            DataTable tabla = SiaWin.Func.SqlDT(select, "Clientes", idemp);
            if (tabla.Rows.Count > 0)
            {
                Tx_idrowDoc.Text = tabla.Rows[0]["num_trn"].ToString().Trim();
            }
        }




    }
}
