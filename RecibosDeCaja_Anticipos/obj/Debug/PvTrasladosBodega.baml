   M S B A M L   `   `   `  ����L  HPvTrasladosBodega, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:SiasoftAppExtSiasoftAppExt  5   	   %    SiasoftAppExt.PvTrasladosBodega   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   a ]Syncfusion.SfGrid.WPF, Version=15.2460.0.43, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfGridConverter.WPF, Version=15.2460.0.43, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89_ [Syncfusion.Grid.Wpf, Version=15.2460.0.43, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=15.2460.0.43, Culture=neutral, PublicKeyToken=3d67ed1f87d44c898
syncfusion!http://schemas.syncfusion.com/wpf     5   	   -localclr-namespace:SiasoftAppExt     5   	   -   5          � Title$,  &SiasoftApp  - Traslados Punto de Venta��5	   	    � WindowStyle$ SingleBorderWindow=�68   $	��620��6Y   $
��1150��6f    � 	Resources 5
      3  -Syncfusion.UI.Xaml.Grid.GridHeaderCellControl �� 
TargetType   headerStyle �� Property �� Value��%�   &	        �� 5   
   "  6   .�5      ��  � ��$
 Bold"�6+   �� 5       � ��$ 12(�6)   �� 5       � ��
 �����6+   �� 5       � �� �#3737376+   5      5      .��5      �� $��5,5,5,5q�6   .@�5   
   ��  �� 
IsSelected . $��Documentos de Traslados��6$   $	��200��6F   
������.��5      �  � RowDefinitions 5      �� 5      $	��210 �6$   �� 5      $��* �6$   �� 5      $��40 �6$   .+�5      � $��0��6    � ColumnDefinitions 5      �� 5      $	��380 �6+   �� 5      $	��480 �6+   �� 5       $	��260 �6+   .+�5#      � -   5        $��0��5#       5$      �� 5%      $��28 �6,   �� 5&      $��28 �6,   �� 5'      $��28 �6,   �� 5(      $��28 �6,   �� 5)      $��28 �6,   �� 5*      $��28 �6,   �� 5+      $��28 �6,   �� 5,      $��15 �6,    5.      �� 5/      $	��120 �6/   �� 50      $��* �6/   .+�52      �� 
�����w�$��0��6@   $��0,5,5,2q�6N   $��0��6_   $��Fecha:��6o   $��Right=�6~   �� 53      
�����w�$��1��6A   $��0,5,5,2q�6N   $��0��6_   $��Tipo de Documento:��6o   $��Right=�6�   �� 54      
�����w�$��2��6A   $��0,5,5,2q�6N   $��0��6_   $��
Documento:��6o   $��Right=�6�   �� 55      -   5        	 ��Name$	 LabelBodegaOrigen��55   n   
�����w�$��3��6@   $��0,5,5,2q�6M   $��0��6^   $��Bodega Origen:��6�   $��Right=�6�   �� 56      -   5        $	 LabelBodegaDestino��56   n   
�����w�$��4��6@   $��0,5,5,2q�6M   $��0��6^   $��Bodega Destino:��6�   $��Right=�6�   �� 57      
�����w�$��5��6@   $��0,5,5,2q�6M   $��0��6^   $��Nota:��6n   $��Right=�6{   �� 58      
�����w�$��6��6A   $��0,5,5,2q�6N   $��0��6_   $��Tipo de Traslado:��6o   $��Right=�6�   �� 59      -   5        
 ��Name$
 	TextFecha��59   P   $��0��6"   $��0,1,0,1q�6/   $��1��6@   |�.  $��12(�6u   �� 5:      -   5         ��Name$ 
CmbTipoDoc��5:   $   $��0,1,0,1q�68   $��1��6I   $��1��6W   $ �� IsSynchronizedWithCurrentItem$
 TrueY�6�    �� SelectedItem 6�   �� *SelectedItem+.@�5;      �� $��Salida Trasaldo Bodega��6,   �� 5<      $)��#Salida Traslado Bodega Consignacion��6+   �� 5=      $��Entrada Traslado Bodega ��6+   �� 5>      $*��$Entrada Traslado Bodega Consignacion��6+   5?      �� 5@      -   5        $
 TextNumeroDoc��5@   P   $��2��6"   $��0,1,0,1q�6/   $��1��6@   |�.  $��12(�6y   �� 5A      -   5        $ CmbBodOrigen��5A   #   $��0,1,0,1q�6n   $��3��6   $��1��6�   $
 TrueY�6�    �� SelectedValue 69   �� *SelectedValue+ �� Mode$ TwoWay=� 6�   �� *SelectedItem+6�   �� 5B      -	   5        $ CmbBodDestino��5B   #   $��0,1,0,1q�6:   $��4��6K   $��1��6X   $
 TrueY�6   %� IsVirtualizing .  %� VirtualizationMode$ 	Recycling=�6c   6h   �� *	cod_bod+$ TwoWay=� �� ValidatesOnDataErrors .  �� NotifyOnValidationError .  �� ValidatesOnExceptions .  6�   �� *SelectedItem+6�  �� 5C      -
   5        $
 TextNota��5C   P   $��5��6"   $��1��6/   $��0,1,0,1q�6?   $��12(�6c   �� 5D      -   5        $ CmbTipoTraslado��5D   $   $��0,1,0,1q�6=   $��6��6N   $��1��6\   $
 TrueY�6�    6�   �� *SelectedItem+.@�5E      �� $��Traslado entre tiendas ��6,   �� 5F      $"��Traslado gerente de producto��6+   �� 5G      $%��Traslado gerente administrativo��6+   5H      �� 5J      -   5        $	 TextKey��5J   o   $��F3=Eliminar registro ��6$   $��7��6A   $��0��6N   $��5,0,0,1q�6^   $��10(�6�   �� 5K      -   5        $	 TextKey1��5K   }   $'��!F8=Cambiar cantidad     F5=Grabar��6$   $��7��6M   $��1��6Z   $��	100,0,1,1q�6j   $��10(�6�   5M      � 5N      $��0��6   $��1��6(   .+�5O      �� -   5         ��Name$ BtnCargarEntradas��5O   !   $��1��6<   $��2��6I   $
��Left=�6Y   $	��Top=�6t   $��25��6�   $	��100��6�   $��10,30q�6�   
��� ���
���   �$��Cargar Entradas��6�   $y�Hidden=�6�   6  5P      �� 5Q      $��0��6   $��2��6*   $	��125��6:   $	��Top=�6G   $��2q�6_   $��5,5,5,5q�6s   
�������._�5R      � 
������� 5S      �� 5T   "   $��60 �63   �� 5U   "   $	��120 �63    5W      �� 5X   "   $��28 �60   �� 5Y   "   $��28 �60   �� 5Z   "   $��28 �60   �� 5[   "   $��28 �60   .+�5]      �� 
�����w�$��Empresa:��6D   $��0��6T   $��0��6a   $��Right=�6q   �� 5^      
�����w�$��PVenta:��6D   $��1��6S   $��0��6`   $��Right=�6p   �� 5_      -   5        $	 
TxtEmpresa��5_   E   
�����w�$	��5,1q�6Y   $	��---��6f   $��0��6r   $��1��6   $
��Left=�6�   �� 5`      -   5        $	 	TxtPVenta��5`   D   
�����w�$	��5,1q�6W   $
��---:��6d   $��1��6p   $��1��6}   $
��Left=�6�   �� 5a      
�����w�$��Bodega:��6D   $��2��6T   $��0��6a   $��Right=�6q   �� 5b      -   5        $	 TxtBod��5b   D   
�����w�$	��5,1q�6T   $
��---:��6a   $��2��6m   $��1��6z   $
��Left=�6�   �� 5d      
�����w�$��Usuario:��6D   $��3��6U   $��0��6b   $��Right=�6r   �� 5e      -   5        $	 TxtUser��5e   D   
�����w�$	��5,1q�6U   $
��---:��6b   $��3��6n   $��1��6{   $
��Left=�6�   5i      5j      5k      &   System.Windows.Controls.DataGrid  5l      -   5        $��dataGrid��5l      $��1��6,      AutoGenerateColumns .  
�������   RowBackground
 �����   AlternatingRowBackground
 �����   
IsReadOnly .    CanUserAddRows .     CanUserDeleteRows .     HorizontalGridLinesBrush
 �����$��5,5,5,5q�5m   �   $��14(�6�      VerticalGridLinesBrush
 �����   HeadersVisibility$ Column=�5n   .      SelectionMode$ Single=�6I       SelectionUnit$  FullRow=�6`   $
 TrueY�6�    .  !   EnableRowVirtualization! .  !"   EnableColumnVirtualization" .  ��5l   W   �� *RefGDCSource+# �� UpdateSourceTrigger$# PropertyChanged=�$ TwoWay=� 6�   �� *SelectedItem+$ TwoWay=�$# PropertyChanged=�$ �� SelectedIndex$ 6  �� *SelectedIndex+$ TwoWay=�$# PropertyChanged=� 5o        	CellStyle*  $System.Windows.Controls.DataGridCell=  7System.Windows.Controls.Primitives.DataGridColumnHeader��%d  &	       '  F     �� 5p      "  62   .�5q      ��  � |� .  6:   5r      �� 5t      "  6    .�5u      ��  � �� �#3737376;   �� 5v       � ��
 �����6;   �� 5w       � ��$
 Bold"�6;   �� 5x       � ��$ 12(�69   �� 5y       � ��$ 22��67   �� 5z       � ��$ Center=�6K   5{      5|      %   Columns% 5}      0  *System.Windows.Controls.DataGridTextColumn  5~      ,  &System.Windows.Controls.DataGridColumn&   Width5  /System.Windows.Controls.DataGridLengthConverter$	& 140 6-   '   MinWidth$	' 150Q�69   (   Header$( Codigo��6H   1  +System.Windows.Controls.DataGridBoundColumn)   Binding) 6X   �� *	cod_ref+* �� NotifyOnTargetUpdated* . $# PropertyChanged=�$ TwoWay=�  5      $( Nombre��6-   +   	CellStyle#+ [ 6>   ,   
IsReadOnly, . $	& 500 6�   ) 6w   �� *	nom_ref+* . $# PropertyChanged=�$ TwoWay=�6�     5�      $( Cantidad��6-   $& 70 6�   ) 6?   �� *
cantidad+* . $# 	LostFocus=�$ TwoWay=�- �� StringFormat$- N2��+ 5�      �� 5�      .�5�      ��  �@��TextAlignment$ Right=�62   5�      5�      5�      � 5�      $��2��6   
���   �.+�5�      �� $��Total Cantidades: ��6    
��� ���$��12(�6N   $��Center=�6\   $
��15,5q�6w   $	��100��6�   $
��Left=�6�   �� 5�      -   5        $	 TextCantidades��5�       
�������$
��Bold"�6L   $��12(�6_   $��Center=�6m   $��120,5q�6�   $	��100��6�   $
��Left=�6�   . �� TextAlignment$
. Left=�6�   �� 5�      $��Total Item(s): ��6    
��� ���$��12(�6K   $��Center=�6Y   $��250,5q�6t   $��70��6�   $
��Left=�6�   �� 5�      -   5        $	 TextItem��5�       
�������$
��Bold"�6F   $��12(�6Y   $��Center=�6g   $��330,5q�6�   $��80��6�   $
��Left=�6�   �� 5�      $��Saldos Unidades: ��6    
��� ���$��12(�6M   $��Center=�6[   $��	450,5,5,5q�6v   $	��100��6�   $
��Left=�6�   �� 5�      -   5        $	 
TextSaldoU��5�       $��0��65   
�������$
��Bold"�6Q   $��12(�6d   $��Center=�6r   $��	550,5,5,5q�6�   $��80��6�   $
��Left=�6�   �� 5�      -   5        $ 	BtbGrabar��5�      
��� ���
���   �$��Nuevo��6o   $��25��6   $��	650,5,5,5q�6�   $	��100��6�   $
��Left=�6�   $	��Top=�6�   6�   �� 5�      -   5        $ BtbCancelar��5�      
��� ���
���   �$��Salir��6u   $��25��6�   $	��100��6�   $
��Left=�6�   $	��Top=�6�   $��	760,5,5,5q�6�   6�   5�      5�      5�      �� 5�   
    . $'��!Consultas Documentos de Traslados��6$   
������$	��250��6g   $��0,2,0,-4q�6s   .��5�      �  5�      �� 5�      $��45 �6$   �� 5�      $��* �6$   �� 5�      $��50 �6$   .+�5�      � $��0��6   
���   � 5�      �� 5�      $��40 �6(   �� 5�      $��10 �6(    5�      �� 5�      $	��100 �6+   �� 5�      $	��120 �6+   �� 5�      $	��120 �6+   �� 5�      $	��120 �6+   �� 5�      $	��120 �6+   �� 5�      $	��120 �6+   �� 5�      $	��120 �6+   .+�5�      �� $��Fecha:  ��6    $��0��61   $��0��6>   
��� ���$��Center=�6b   $��Right=�6}   $��13(�6�   (	  "System.Windows.Controls.DatePicker	  5�      -   5        / 	 Name$/ FechaIni��5�   !   $��0��63   $��1��6@   $��Center=�6P   6n   	  5�      -   5        $/ FechaFin��5�   !   $��0��63   $��2��6@   $��Center=�6P   6n   �� 5�      -   5        $ Ejecutar��5�      $��Ejecutar��6/   $��3��6B   $��5,5,5,5q�6R   
��� ���6w   �� 5�      -   5        $ 
ReImprimir��5�      $��Re-Imprimir��61   $��4��6G   $��5,5,5,5q�6W   
��� ���6|   �� 5�      -   5        $ 
ExportaXLS��5�      $��Exportar XLS��61   $��5��6H   $��5,5,5,5q�6X   
��� ���6}   5�      � 5�      $��1��6   .+�5�      (
  "Syncfusion.UI.Xaml.Grid.SfDataGrid
  -   5        0 
 Name$0 
dataGridSF��5�   -   1 
  EnableDataVirtualization1 . (  "Syncfusion.UI.Xaml.Grid.SfGridBase2   AutoGenerateColumns2 .  3 
  ShowGroupDropArea3 . $��11(�5�   N   4 
  AllowFiltering4 . 5 
  HeaderStyle#5 [  5�      6 
  ItemsSource6 5�      �� *DtTraslados+7 
  Columns7 5�      ,  &Syncfusion.UI.Xaml.Grid.GridTextColumn  ,  &Syncfusion.UI.Xaml.Grid.GridColumnBase8   
HeaderText$
8 Tipo��68   9   MappingName$9 cod_trn��6J   :   Width$: 40Q�6`     5�      $8 	Documento��68   $9 num_trn��6O   $	: 100Q�6e     5�      $8 Tipo Traslado��68   $9 tipotraslado��6S   $	: 100Q�6n     5�      $8 Fecha��68   $9 fec_trn��6K     5�      $8 	Orig/Dest��68   $9 bod_dest��6O   $	: 100Q�6f     5�      $8 Codigo Producto��68   $9 cod_ref��6U   $	: 120Q�6k     5�      $8 Nombre Producto��68   $9 nom_ref��6U   $	: 350Q�6k   /  )Syncfusion.UI.Xaml.Grid.GridNumericColumn  5�      $8 Cantidad��6;   $9 cantidad��6Q   $: 70Q�6h   5�      5�      � 5�      $��2��6   .+�5�      �� $��Total Entradas:��6    $
��Left=�67   $	��Top=�6R   $��10,5,0,0q�6j   �� 5�      -   5        $	 TextTotalEntradas��5�   !   $��0��6<   $
��Left=�6E   $	��Top=�6`   $��	100,5,0,0q�6x   $	��100��6�   �� 5�      $��Total Salidas...:��6    $
��Left=�69   $	��Top=�6T   $��	10,25,0,0q�6l   �� 5�      -    5        $	 TextTotalSalidas��5�   !   $��0��6;   $
��Left=�6D   $	��Top=�6_   $��
100,25,0,0q�6w   $	��100��6�   5�      5�      5�      5�      5�      