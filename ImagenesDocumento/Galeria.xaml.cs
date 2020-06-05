using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImagenesDocumento
{

    public partial class Galeria : Window
    {
        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";

        private Point origin;
        private Point start;


        public int idrowcab = 0;
        int rotate = 0;


        List<string> lista = new List<string>();
        int incre = 0;

        public Galeria()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            //LoadConfig();

            TransformGroup group = new TransformGroup();

            ScaleTransform xform = new ScaleTransform();
            group.Children.Add(xform);

            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);

            image.RenderTransform = group;

            image.MouseWheel += image_MouseWheel;
            image.MouseLeftButtonDown += image_MouseLeftButtonDown;
            image.MouseLeftButtonUp += image_MouseLeftButtonUp;
            image.MouseMove += image_MouseMove;
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        public void cargarIdImage(string id)
        {
            string select = "select idrow from IMG_ImgDoc where idregDoc = '" + id + "'";
            DataTable tabla = SiaWin.Func.SqlDT(select, "Clientes", idemp);

            if (tabla.Rows.Count > 0)
            {
                foreach (DataRow item in tabla.Rows)
                    lista.Add((item["idrow"].ToString()));
            }

        }

        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                image.ReleaseMouseCapture();
            }
            catch (Exception w) { MessageBox.Show("image_MouseLeftButtonUp:" + w); }

        }

        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (!image.IsMouseCaptured) return;

                var tt = (TranslateTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is TranslateTransform);
                Vector v = start - e.GetPosition(border);
                tt.X = origin.X - v.X;
                tt.Y = origin.Y - v.Y;
            }
            catch (Exception w) { MessageBox.Show("image_MouseMove:" + w); }

        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                image.CaptureMouse();
                var tt = (TranslateTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is TranslateTransform);
                start = e.GetPosition(border);
                origin = new Point(tt.X, tt.Y);
            }
            catch (Exception w) { MessageBox.Show("image_MouseLeftButtonDown:" + w); }
        }

        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                TransformGroup transformGroup = (TransformGroup)image.RenderTransform;
                ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];

                double zoom = e.Delta > 0 ? .2 : -.2;
                transform.ScaleX += zoom;
                transform.ScaleY += zoom;
            }
            catch (Exception w) { MessageBox.Show("image_MouseWheel:" + w); }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadConfig();
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicator.IsBusy = true;

                string idreg = idrowcab.ToString();
                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(idreg, source.Token), source.Token);
                await slowTask;
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    byte[] blob = (byte[])((DataSet)slowTask.Result).Tables[0].Rows[0]["imagen"];
                    MemoryStream stream = new MemoryStream();
                    stream.Write(blob, 0, blob.Length);
                    stream.Position = 0;
                    System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                    System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
                    bi.BeginInit();
                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                    bi.EndInit();
                    image.Source = bi;

                    cargarIdImage(idreg);
                }

                this.sfBusyIndicator.IsBusy = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("error al cargar : " + ex);

            }
        }


        private DataSet LoadData(string id, CancellationToken cancellationToken)
        {
            try
            {
                string cadena = "select top 1 * from  IMG_ImgDoc where idregDoc='" + id + "' ";
                DataTable dt = SiaWin.Func.SqlDT(cadena, "Imagen", idemp);

                if (dt == null || dt.Rows.Count <= 0)
                {
                    MessageBox.Show("no contienn mas imagenes");
                }

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
                return ds;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el loadData:" + e);
                return null;
            }
        }

        private async void BTNbefore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicator.IsBusy = true;


                string idreg = itemlist(false);
                bloquear(idreg);
                if (string.IsNullOrEmpty(idreg)) return;

                image.Source = null;

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadDataBeforeAndAfter(idreg, source.Token), source.Token);
                await slowTask;
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    byte[] blob = (byte[])((DataSet)slowTask.Result).Tables[0].Rows[0]["imagen"];
                    MemoryStream stream = new MemoryStream();
                    stream.Write(blob, 0, blob.Length);
                    stream.Position = 0;
                    System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                    System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
                    bi.BeginInit();
                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                    bi.EndInit();
                    image.Source = bi;
                }

                this.sfBusyIndicator.IsBusy = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("error al cargar : " + ex);

            }
        }

        private async void BTNafter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicator.IsBusy = true;


                string idreg = itemlist(true);
                bloquear(idreg);

                if (string.IsNullOrEmpty(idreg)) return;

                image.Source = null;

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadDataBeforeAndAfter(idreg, source.Token), source.Token);
                await slowTask;
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    byte[] blob = (byte[])((DataSet)slowTask.Result).Tables[0].Rows[0]["imagen"];
                    MemoryStream stream = new MemoryStream();
                    stream.Write(blob, 0, blob.Length);
                    stream.Position = 0;
                    System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                    System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
                    bi.BeginInit();
                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                    bi.EndInit();
                    image.Source = bi;
                }

                this.sfBusyIndicator.IsBusy = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("error al cargar : " + ex);

            }
        }


        private DataSet LoadDataBeforeAndAfter(string id, CancellationToken cancellationToken)
        {
            try
            {
                string cadena = "select * from IMG_ImgDoc where idrow='" + id + "' ";
                DataTable dt = SiaWin.Func.SqlDT(cadena, "Imagen", idemp);

                if (dt == null || dt.Rows.Count <= 0)
                {
                    MessageBox.Show("no contienn mas imagenes");
                }

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
                return ds;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el loadData:" + e);
                return null;
            }
        }


        public string itemlist(bool c)
        {
            string item = string.Empty;
            try
            {
                if (c)
                {
                    incre++;
                    item = lista[incre].ToString();
                }
                else
                {
                    incre--;
                    item = lista[incre].ToString();
                }
            }

            catch (Exception)
            {
                if (c == true) incre--;
                else incre++;

                MessageBox.Show("no hay mas valores siguientes");
            }

            return item;
        }

        public void bloquear(string valor)
        {
            string first = lista[0].ToString().Trim();
            string last = lista[lista.Count - 1].ToString().Trim();

            if (first == valor)
                BTNbefore.IsEnabled = false;
            else
                BTNbefore.IsEnabled = true;

            if (last == valor)
                BTNafter.IsEnabled = false;
            else
                BTNafter.IsEnabled = true;
        }

        private void BTNZoomIn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BTNZoomOut_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BTNFilter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BTNSave_Click(object sender, RoutedEventArgs e)
        {


        }


        public static void SaveClipboardImageToFile(string filePath)
        {
            var image = Clipboard.GetImage();
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fileStream);
            }
        }

        private void BTNRoteRight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rotate += 90;
                RotateTransform rotateTransform = new RotateTransform(rotate);
                image.RenderTransform = rotateTransform;
            }
            catch (Exception w)
            {
                MessageBox.Show("error BTNRoteRight_Click:" + w);
            }

        }

        private void BTNRoteLeft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //rotate -= 90;
                //RotateTransform rotateTransform = new RotateTransform(rotate);
                //image.RenderTransform = rotateTransform;

                SkewTransform skewTransform1 = new SkewTransform(45, 0, -50, 50);
                image.RenderTransform = skewTransform1;

            }
            catch (Exception w)
            {
                MessageBox.Show("error BTNRoteRight_Click:" + w);
            }
        }

        private void BTNDrop_Click(object sender, RoutedEventArgs e)
        {

        }



    }
}
