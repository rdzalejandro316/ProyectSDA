using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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

namespace ImagenesDocumento
{

    public partial class InsertarImage : Window
    {

        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";


        private Point origin;
        private Point start;

        //imagen
        string strName = "", imageName;
        bool imageSave = false;

        public int idrowcab = 0;

        public InsertarImage()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();


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
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            image.ReleaseMouseCapture();
        }

        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            if (!image.IsMouseCaptured) return;

            var tt = (TranslateTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is TranslateTransform);
            Vector v = start - e.GetPosition(border);
            tt.X = origin.X - v.X;
            tt.Y = origin.Y - v.Y;
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            image.CaptureMouse();
            var tt = (TranslateTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is TranslateTransform);
            start = e.GetPosition(border);
            origin = new Point(tt.X, tt.Y);
        }

        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            TransformGroup transformGroup = (TransformGroup)image.RenderTransform;
            ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];

            double zoom = e.Delta > 0 ? .2 : -.2;
            transform.ScaleX += zoom;
            transform.ScaleY += zoom;
        }


        private void BTNimage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileDialog fldlg = new OpenFileDialog();
                fldlg.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
                fldlg.Filter = "Image File (*.jpg;*.bmp;*.gif;*.png)|*.jpg;*.bmp;*.gif;*.png";
                fldlg.ShowDialog();
                {
                    strName = fldlg.SafeFileName;
                    imageName = fldlg.FileName;
                    ImageSourceConverter isc = new ImageSourceConverter();
                    image.SetValue(System.Windows.Controls.Image.SourceProperty, isc.ConvertFromString(imageName));
                    imageSave = true;
                    BTNsubirFoto.IsEnabled = true;
                }
                fldlg = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

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

        private void BTNsubirFoto_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                byte[] imgByteArr = null;
                if (imageSave == true)
                {
                    FileStream fs = new FileStream(imageName, FileMode.Open, FileAccess.Read);
                    imgByteArr = new byte[fs.Length];
                    fs.Read(imgByteArr, 0, Convert.ToInt32(fs.Length));
                    fs.Close();
                }

                //MessageBox.Show("cnEmp:"+ cnEmp);

                using (SqlConnection connection = new SqlConnection(SiaWin.Func.DatosEmp(idemp)))
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    //cmd.CommandText = "insert into IMG_ImgDoc(cod_ima,cod_doc,image_name,img_cli) values (@cod_ima,@cod_doc,@image_name,@img_cli)";
                    cmd.CommandText = "insert into IMG_ImgDoc(idregDoc ,image_desc ,imagen ,fec_crea) values (@idregDoc , @image_desc ,@imagen ,@fec_crea)";
                    cmd.Parameters.AddWithValue("@idregDoc", Tx_idrowDoc.Tag);
                    cmd.Parameters.AddWithValue("@image_desc", "DOCUMENTO " + Tx_idrowDoc.Text);
                    cmd.Parameters.AddWithValue("@imagen", imgByteArr);
                    cmd.Parameters.AddWithValue("@fec_crea", DateTime.Now.ToString());

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("imagen guardada en el documento : "+ Tx_idrowDoc.Text);

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar la imagen: " + w);
            }

        }





    }
}
