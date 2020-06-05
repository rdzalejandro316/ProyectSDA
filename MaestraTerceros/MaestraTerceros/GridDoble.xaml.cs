using System;
using System.Collections.Generic;
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

namespace MaestraTerceros
{
    
    public partial class GridDoble : Grid
    {
        public GridDoble()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MyPropertyProperty = DependencyProperty.Register(
            nameof(columnas),
            typeof(int),
            typeof(GridDoble),
            (PropertyMetadata)new UIPropertyMetadata((object)0)
        );
        
        
        public int columnas
        {
            get
            {
                return (int)this.GetValue(GridDoble.MyPropertyProperty);
            }
            set
            {     
                this.SetValue(GridDoble.MyPropertyProperty, (object)value);                
            }
        }

        private void GridPrin_Loaded(object sender, RoutedEventArgs e)
        {
            if (columnas>0)
            {                
                this.ColumnDefinitions.Clear();
                for (int i = 1; i <= columnas; i++)
                {
                    ColumnDefinition col = new ColumnDefinition() { };
                    this.ColumnDefinitions.Add(col);
                }
            }
            else
            {
                for (int i = 1; i <= 2; i++)
                {
                    ColumnDefinition col = new ColumnDefinition() { };
                    this.ColumnDefinitions.Add(col);
                }
            }            
        }



    }
}
