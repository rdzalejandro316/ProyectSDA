   M S B A M L   `   `   `  ����T  PDeterioroCuentasPorCobrar, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:SiasoftAppExtSiasoftAppExt  5      -    'SiasoftAppExt.DeterioroCuentasPorCobrar   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      a ]Syncfusion.SfGrid.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfGridConverter.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89_ [Syncfusion.Grid.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j	 fSyncfusion.SfBusyIndicator.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a
 ]Syncfusion.Shared.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c897SiaSf!http://schemas.syncfusion.com/wpf     	 
 5      1localclr-namespace:SiasoftAppExt     	 
 5      + 
 %Syncfusion.Windows.Shared.SkinStorage    VisualStyle$  Metro��5	      
������� � 	Resources 5       �� Color   	ColorMain  ColorSecond �� 
TargetType 	 title �� Property �� Value 
 values  BtnStlye&   Syncfusion.UI.Xaml.Grid.GridCell3  -Syncfusion.UI.Xaml.Grid.GridHeaderCellControl  headerStyle  	StyleDias  StyleDiasValor��%�  &	        &	 &     &	 L     &	     &	 }    7   '  �    &	 %    7  &	 �    &	 t    7   �� 5   
   $ #2196F3��6   6*   �� 5   
   $ #373737��6   6*   �� 5   
   " ��6   .�5      ��  � ��$ Center=�62   �� 5       � ��$ Right=�64   �� 5       � ��$
 Bold"�6+   �� 5       � ��$ 5,0,0,0q�6'   5      �� 5   
   " ��6   .�5      ��  � ��$ Center=�62   �� 5       � ��$ 5q�6(   5      �� 5   
   " ��6   .�5      ��  � ��8   6+   �� 5       � ��
 �����6+   �� 5       � �� 6,   Z� �� 5       �@�Cursor$
 Hand��6'   �� 5       � ��$	 120��6&   �� 5       � ��$ 32��6'   �� 5       � ��$ 2,0,2,0q�6'   5       �� 5!   
   "  6   .�5"      ��  � ��
 �����6,   �� 5#       � ��$ Times New Roman)�6+   5$      �� 5%   
   "  6   .�5&      ��  � ��8   6+   �� 5'       � ��
 �����6+   �� 5(       � ��$ 11(�6)   5)      �� 5*   
   " ��6   .�5+      ��  � ��$ Center=�62   �� 5,       � ��$ Right=�64   �� 5-       � ��$ DemiBold"�6+   �� 5.       � ��$ 10(�6)   5/      �� 50   
   " ��6   .�51      ��  � ��$ Center=�62   �� 52       � ��$
 Left=�64   �� 53       � ��8   6+   �� 54       � ��$ 10(�6)   �� 55       � ��$ 5,0,0,0q�6'   56      57      .��59      5  /Syncfusion.Windows.Tools.Controls.TabControlExt  -   5        $��TabControl1��59      $��Stretch=�6-   $��Stretch=�6K   $  Metro��6h   '    IsDisableUnloadTabItemExtContent . .@�5:   
   2  ,Syncfusion.Windows.Tools.Controls.TabItemExt  -   5        $��tabItemExt1��5:      $��Estado de Cartera��6/   $��12(�6J   .��5;      �  � RowDefinitions 5<      �� 5=      $��80 �6$   �� 5>      $��* �6$   �� 5?      $��50 �6$   �� 5@      $��30 �6$   .+�5C      � $��5q�6    5D      �� 5E      $��1* �6(   �� 5F      $��1* �6(    � ColumnDefinitions 5H      �� 5I      $��80 �6+   �� 5J      $	��150 �6+   �� 5K      $��70 �6+   �� 5L      $	��200 �6+   �� 5M      $��1* �6+   .+�5P      �� $��Fecha Corte:  ��6    $��0��67   $��0��6D   #��[ 6T   (  "System.Windows.Controls.DatePicker  5Q      -   5        	  Name$	 FechaIni��5Q   !   $��0��63   $��1��6@   $��Center=�6P   6n   �� 5T      $��
Cliente:  ��6    $��1��62   $��0��6?   #��[ 6O   �� 5U      -   5        
 ��Name$
 TxCodTer��5U      $��1��60   $��1��6=   #��[ 6M   
 � Tag$ 	comae_ter��6m    � ToolTip$ 	F8 buscar��6�   �� 5V      -   5         ��Name$ TextNombreTercero��5V       $�� ��6;   $��1��6D   $��2��6Q   $��2��6a   
���   �$��Center=�6�   $��Stretch=�6�   $��13(�6�   
������� �� Padding$ 5q�6�   $��5,0,0,0q�6�   �� 5X      $��	Cuentas: ��6    $��0��61   $��2��6>   #��[ 6N   3 
 -Syncfusion.Windows.Tools.Controls.ComboBoxAdv  5Y      -   5          Name$ comboBoxCuentas��5Y   7   $��12(�6)   $��5q�6P      AllowMultiSelect . $��3��6s      DefaultText$ Seleccione Cuenta��6�    �� DisplayMemberPath$ nom_cta��6�    �� SelectedValuePath$ cod_cta��6�      SelectedValueDelimiter$ ,��6�      AllowSelectAll . ��6�   �� *	Cuentas+6  �� 5[      -   5        $��PanelA��5[   !   $��5��6/   $��2��6?    �� Orientation$ 
Horizontal=�6S   $��10,0,0,0q�6l   .+�5\      �� -   5         ��Name$ BtnConsultar��5\   !   $��	Consultar��67   #��[ 6f   �� 5]      -	   5        $ BtnExportar��5]   !   $��Exportar��66   #��[ 6c   �� 5^      -
   5        $ BtnImprimir��5^   !   $��Imprimir��66   #��[ 6c   5_      �� 5a      -   5        $��PanelB��5a   !   $��5��6/   $��1��6?   $ 
Horizontal=�6L   $��10,0,0,0q�6e   .+�5b      �� -   5        $ BtnPlantillas��5b   !   $��	Plantilla��68   #��[ 6h   �� 5c      -   5        $ BtnImportar��5c   !   $��Importar��66   #��[ 6c   �� 5d      -   5        $ BtnCrear��5d   !   $��Crear Ajuste��63   #��[ 6k   �� 5e      -   5        $ BtnCrearRecup��5e   !   $��Crear Recuperacion��68   #��[ 6q   5f      5h      > 	 8Syncfusion.Windows.Controls.Notification.SfBusyIndicator  5k      -   5        $��sfBusyIndicator��5k   5   $��1��6(   $��Bottom=�6L   $��12(�6g      AnimationSpeed$	 100Q�6u      AnimationType$ Rainy=�6�      Header$ Cargando ...��6�   $	��200��6�      IsBusy .  6�   � 5m      -   5        $��	PanelGrid��5m   $   $��1��6    5o      �� 5p      $��1* �6+   �� 5q      $	��200 �6+   .+�5u      (	  "Syncfusion.UI.Xaml.Grid.SfDataGrid	  -   5         	 Name$ dataGridCxC��5u   (   (
  "Syncfusion.UI.Xaml.Grid.SfGridBase 
  FrozenColumnCount$ 2��6>    
  AutoGenerateColumns .   
  AllowResizingColumns .   	  ShowGroupDropArea  .  $��10(�6�   $��Times New Roman)�6�   ! 	  AllowFiltering! . " 	  HeaderStyle#" [ 6�   # 	  Columns# 5w      ,  &Syncfusion.UI.Xaml.Grid.GridTextColumn  ,  &Syncfusion.UI.Xaml.Grid.GridColumnBase$   
HeaderText$$ Nit/cc��63   %   MappingName$% cod_ter��6G   &   Width$& 80Q�6]     5x      $$ Nombre cliente��63   $% nom_ter��6P   $	& 150Q�6f     5y      $$ 	Doc Conta��63   $% doc_ref��6J   $& 90Q�6`     5z      $$ Doc Ref��63   $% doc_mov��6H   $& 90Q�6^     5{      $$ Cuenta��63   $% cod_cta��6G   $	& 100Q�6]     5|      $% nom_cta��63   '   IsHidden' . 6I     5}      $$ 	Dias Venc��63   $% vencido��6J   $& 80Q�6`     5~      $$ 
Sin Vencer��63   $% sin_ven��6K   $& 80Q�6a     5      $& 85Q�6r   $$ Vencido��6}   (   TextAlignment$( Right=�6�   )   DisplayBinding) 63   �� * �� Path$* valor�+ �� StringFormat$+ {0:N2}��6�     5�      $& 80Q�6r   $$ Saldo��6}   $( Right=�6�   ) 63   �� $* saldo�$+ {0:N2}��6�     5�      $& 80Q�6v   $$ Det Causado��6�   $( Right=�6�   ) 63   �� $* 	deterioro�$+ {0:N2}��6�     5�      $	& 100Q�6t   $$ Valor Pres Archivo��6�   $( Right=�6�   ) 63   �� $* valarch�$+ {0:N2}��6�     5�      $	& 100Q�6u   $$ Deterioro Archivo��6�   $( Right=�6�   ) 63   �� $* det_arch�$+ {0:N2}��6�     5�      $	& 100Q�6r   $$ Ajuste��6~   $( Right=�6�   ) 63   �� $* difer�$+ {0:N2}��6�   5�      � 5�      $��1��6   .+�5�      � 
�������$��5q�67   .+�5�      �� 
�������, J� Effect, 5�   "   3  -System.Windows.Media.Effects.DropShadowEffect  5�   &   -   	Direction$- 0Q�67   .   ShadowDepth$	. 0.5Q�6E   /   
BlurRadius$/ 5Q�6W   0   Color$0 Black��6f   5�      � 5�       5�   "   �� 5�   &   $��40 �64   �� 5�   &   $��40 �64   �� 5�   &   $��40 �64   �� 5�   &   $��40 �64   �� 5�   &   $��40 �64   �� 5�   &   $��40 �64   �� 5�   &   $��40 �64   �� 5�   &   $��40 �64   �� 5�   &   $��40 �64    5�   "   �� 5�   &   $��70 �67   �� 5�   &   $��1* �67   .+�5�   "   �� $��Tasas % mensuales��6,   $��2��6E   $��Center=�6Y   $��Center=�6t   #��[  6�   �� 5�   "   $��1��6,   $��	30 DIAS :��69   #��[ 6J   �� 5�   "   -   5        $
��Tx30��5�   ,   $��1��68   $��2��6E   $	��---��6U   #��[ 6`   �� 5�   "   $��2��6,   $��	60 DIAS :��69   #��[ 6J   �� 5�   "   -   5        $
��Tx60��5�   ,   $��2��68   $��2��6E   $	��---��6U   #��[ 6`   �� 5�   "   $��3��6,   $��	90 DIAS :��69   #��[ 6J   �� 5�   "   -   5        $
��Tx90��5�   ,   $��3��68   $��2��6E   $	��---��6U   #��[ 6`   �� 5�   "   $��4��6,   $��
120 DIAS :��69   #��[ 6K   �� 5�   "   -   5        $��Tx120��5�   ,   $��4��69   $��2��6F   $	��---��6V   #��[ 6a   �� 5�   "   $��5��6,   $��
150 DIAS :��69   #��[ 6K   �� 5�   "   -   5        $��Tx150��5�   ,   $��5��69   $��2��6F   $	��---��6V   #��[ 6a   �� 5�   "   $��6��6,   $��
180 DIAS :��69   #��[ 6K   �� 5�   "   -   5        $��Tx180��5�   ,   $��6��69   $��2��6F   $	��---��6V   #��[ 6a   �� 5�   "   $��7��6,   $��
360 DIAS :��69   #��[ 6K   �� 5�   "   -   5        $��Tx360��5�   ,   $��7��69   $��2��6F   $	��---��6V   #��[ 6a   �� 5�   "   $��8��6,   $��	MAS 360 :��69   #��[ 6J   �� 5�   "   -   5        $��Txm360��5�   ,   $��8��6:   $��2��6G   $	��---��6W   #��[ 6b   5�      5�      5�      5�      � 5�      $��2��6   $��2,5,2,5q�6$   .+�5�      �� 
�������, 5�        5�      $- 0Q�6/   $	. 0.5Q�6=   $/ 5Q�6O   $0 Black��6^   5�      � 5�       5�      �� 5�      $��70 �6/   �� 5�      $	��100 �6/   �� 5�      $��70 �6/   �� 5�      $	��100 �6/   .+�5�      �� $��Cuenta :��6$   $��Center=�64   $��Right=�6O   $��DemiBold"�6k   �� 5�      -   5        $��TxCodCta��5�   $   $��1��64   $	��---��6D   $��Center=�6O   $
��Left=�6j   $��5,0,0,0q�6�   $��10(�6�   �� 5�      $��2��6$   $��Nombre :��64   $��Center=�6D   $��Right=�6_   $��DemiBold"�6{   �� 5�      -   5        $��TxNomCta��5�   $   $��3��64   $	��---��6D   $��Center=�6O   $
��Left=�6j   $��5,0,0,0q�6�   $��10(�6�   5�      5�      � 5�      -   5        1 �Name$1 PanelC��5�      $��3��6'   #��[ 64    5�      �� 5�      $��1* �6+   �� 5�      $��1* �6+   .+�5�      �� $ 
Horizontal=�6!   $��2��6:   .+�5�      �� $��
Registros:��6$   
�������$��Center=�6I   $
��Bold"�6d   $��5,0,0,0q�6v   �� 5�      -   5        $��TxRegistros��5�   -   $��0��6$   
�������$��Center=�6S   $��5,0,0,0q�6n   �� 5�      $��Registros Importados:��6$   
�������$��Center=�6T   $
��Bold"�6o   $��50,0,0,0q�6�   �� 5�      -   5        $��TxImportados��5�   -   $��0��6$   
�������$��Center=�6T   $��5,0,0,0q�6o   5�      �� 5�      $��1��6!   $ 
Horizontal=�61   .+�5�      �� $��Filas con errores: ��6$   
�������$
��Bold"�6R   $��Center=�6d   $��5 0 0 0q�6   �� 5�      -    5        $��
Tx_errores��5�   $   $��0��66   
���  ��$��Center=�6P   $
��Left=�6k   �� 5�      -!   5        $��
BtnErrores��5�   !   $��lista errores��63   $��Center=�6K   $��10,0,0,0q�6f   $��5,2,5,2q�6x   
����������#2196F3��6�   Z� 6�   5�      5�      5�      5�      5�      5�      