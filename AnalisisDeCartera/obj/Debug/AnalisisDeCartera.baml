   M S B A M L   `   `   `  ����b  ^Syncfusion.SfChart.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89eGclr-namespace:Syncfusion.UI.Xaml.Charts;assembly=Syncfusion.SfChart.WPFSyncfusion.UI.Xaml.Charts  5      L HAnalisisDeCartera, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:SiasoftAppExtSiasoftAppExt 5      _ [Syncfusion.Grid.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89pKclr-namespace:Syncfusion.Windows.Controls.Grid;assembly=Syncfusion.Grid.Wpf Syncfusion.Windows.Controls.Grid 5   ;   %   SiasoftAppExt.AnalisisDeCartera   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      a ]Syncfusion.SfGrid.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89`	 \Syncfusion.Linq.Base, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b
 ^Syncfusion.SfInput.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89e aSyncfusion.GridCommon.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Chart.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfGridConverter.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89ASiaSf!http://schemas.syncfusion.com/wpf  	 
          5      gchartGclr-namespace:Syncfusion.UI.Xaml.Charts;assembly=Syncfusion.SfChart.WPF  	 
          5      ;localclr-namespace:SiasoftAppExt  	 
          5      p
SFDataGridKclr-namespace:Syncfusion.Windows.Controls.Grid;assembly=Syncfusion.Grid.Wpf  	 
          5   ;   -   5          � 	Resources  5       �� Color   	ColorMain  ColorSecond �� 
TargetType 	 title �� Property �� Value  BtnStlye&   Syncfusion.UI.Xaml.Grid.GridCell3  -Syncfusion.UI.Xaml.Grid.GridHeaderCellControl  headerStyle��%_  &	        &	 &     &	 L     &	     7   '      &	 �    7  �� 5   
   $ #2196F3��6   6*   �� 5   
   $ #373737��6   6*   �� 5   
   " ��6   .�5      ��  � ��$ Center=�62   �� 5       � ��$ Right=�64   �� 5       � ��$
 Bold"�6+   �� 5       � ��$ 5,0,0,0q�6'   5      �� 5   
   " ��6   .�5      ��  � ��8   6+   �� 5       � ��
 �����6+   �� 5       � �� 6,   Z� �� 5       �@�Cursor$
 Hand��6'   �� 5       � ��$	 100��6&   �� 5       � ��$ 25��6'   5      �� 5   
   "  6   .�5      ��  � ��
 �����6,   �� 5        � ��$ Times New Roman)�6+   5!      �� 5"   
   "  6   .�5#      ��  � ��8   6+   �� 5$       � ��
 �����6+   �� 5%       � ��$ 11(�6)   5&      5'      .��5)      � .+�5*   
   5  /Syncfusion.Windows.Tools.Controls.TabControlExt  -   5        $��TabControl1��5*      $��Stretch=�61   $��Stretch=�6O   +  %Syncfusion.Windows.Shared.SkinStorage   VisualStyle$ Metro��6l   '    IsDisableUnloadTabItemExtContent . .@�5+      2  ,Syncfusion.Windows.Tools.Controls.TabItemExt  -   5        $��tabItemExt1��5+      $��Analisis de Cartera��63   $��12(�6P   .��5,      �  � RowDefinitions 5-      �� 5.      $��80 �6(   �� 5/      $��* �6(   �� 50      $	��120 �6(   .+�52      � $��0��6   
������� 53      �� 54      $��1* �6,   �� 55      $��1* �6,    � ColumnDefinitions 57      �� 58      $
��Auto �6/   �� 59      $	��120 �6/   �� 5:      $	��250 �6/   �� 5;      $
��Auto �6/   �� 5<      $	��100 �6/   �� 5=      $	��150 �6/   �� 5>      $
��auto �6/   �� 5?      $	��100 �6/   �� 5@      $	��100 �6/   �� 5A      $	��100 �6/   �� 5B      $	��100 �6/   �� 5C      $	��100 �6/   .+�5F      �� $��Fecha Corte:  ��6$   $��0��6;   $��0��6H   #��[ 6X   (  "System.Windows.Controls.DatePicker  5G      -   5        	  Name$	 FechaIni��5G   %   $��0��67   $��1��6D   $��Center=�6T   6r   3  -Syncfusion.Windows.Tools.Controls.ComboBoxAdv  5I      -   5        
  Name$
 comboBoxCuentas��5I   ;   $��12(�6-   $��5q�6T      AllowMultiSelect . $��2��6w   $��0��6�      DefaultText$ Seleccione Cuenta��6�    �� DisplayMemberPath$ nom_cta��6   �� SelectedValuePath$ cod_cta��6(     SelectedValueDelimiter$ ,��6E     AllowSelectAll . ��6�   �� *	Cuentas+6`  �� 5K      $��	Detallado��6$   $��0��65   $��3��6B   #��[ 6R   �� 5L      -   5        $��Cbx_Detalle��5L   #   $��0��66   $��4��6C   $��5q�6S   .@�5M      �� 
 � Tag$ No��6+   $��No��64   �� 5N      $ Si��6+   $��Si��64   5O      �� 5Q      $��
Cliente:  ��6$   $��1��66   $��0��6C   #��[ 6S   �� 5R      -   5         ��Name$ TextCod_Ter��5R   "   $��1��67   $��1��6D   $��Stretch=�6T   $��2,6,2,6q�6p   $��Stretch=�6�   $��13(�6�   $ 	comae_ter��6�    � ToolTip$ 	F8 buscar��6  �� 5S      -   5         ��Name$ TextNombreTercero��5S   $   $�� ��6?   $��1��6H   $��2��6U   
���   �$��Stretch=�6x   $��Stretch=�6�   $��13(�6�   
�������$��2,6,2,6q�6�   �� 5U      $��5��6%   $��2��65    �� Orientation$ 
Horizontal=�6I   .+�5V      �� -	   5         ��Name$ BtnEjecutar��5V   %   $��Ejecutar��6:   #��[ 6i   $��30��6�   $��5q�6�   �� 5W      -
   5        $ BtnSalir��5W   %   $��Salir��67   #��[ 6^   $��30��6�   $��5q�6�   5X      5Y      � 5Z      $��1��6   .+�5\      (  "Syncfusion.UI.Xaml.Grid.SfDataGrid  -   5          Name$ dataGridCxC��5\   ,   $y�Hidden=�6C      EnableDataVirtualization . (	  "Syncfusion.UI.Xaml.Grid.SfGridBase 	  FrozenColumnCount$ 2��6~    	  AutoGenerateColumns .     ShowGroupDropArea . $��10(�5]   N   $��Times New Roman)�6\      AllowFiltering .    HeaderStyle# [ 5^   3      ItemsSource 6   ��    Columns 5a   "   ,
  &Syncfusion.UI.Xaml.Grid.GridTextColumn
  ,  &Syncfusion.UI.Xaml.Grid.GridColumnBase    
HeaderText$  Nit/cc��67   !   MappingName$! cod_ter��6K   "   Width$" 80Q�6a   
  5b   "   $  Nombre cliente��67   $! nom_ter��6T   $	" 220Q�6j   
  5c   "   $  Cuenta��67   $! cod_cta��6K   $" 70Q�6a   
  5d   "   $
  Tipo��67   $
! tipo��6I   $" 50Q�6\   
  5e   "   $" 85Q�6w   $  	Valor Doc��6�   #   TextAlignment$# Right=�6�   (  "Syncfusion.UI.Xaml.Grid.GridColumn$   AllowFiltering$ .  %   DisplayBinding% 67   �� & �� Path$& valor�' �� StringFormat$' {0:N2}��6�   
  5f   "   $" 80Q�6x   $  
Sin Vencer��6�   $# Right=�6�   $ .  % 67   �� $& sinvenc�$' {0:N2}��6�   
  5g   "   $" 80Q�6v   $
  1-30��6�   $# Right=�6�   $ .  % 67   �� $& ven01�$' {0:N2}��6�   
  5h   "   $" 80Q�6v   $  31-60��6�   $# Right=�6�   $ .  % 67   �� $& ven02�$' {0:N2}��6�   
  5i   "   $" 80Q�6v   $  61-90��6�   $# Right=�6�   $ .  % 67   �� $& ven03�$' {0:N2}��6�   
  5j   "   $" 80Q�6v   $  91-180��6�   $# Right=�6�   $ .  % 67   �� $& ven04�$' {0:N2}��6�   
  5k   "   $" 80Q�6v   $  181-360��6�   $# Right=�6�   $ .  % 67   �� $& ven05�$' {0:N2}��6�   
  5l   "   $" 80Q�6v   $
  +360��6�   $# Right=�6�   $ .  % 67   �� $& ven06�$' {0:N2}��6�   
  5m   "   $" 80Q�6v   $  Saldo��6�   $# Right=�6�   $ .  % 67   �� $& saldo�$' {0:N2}��6�   0  *Syncfusion.UI.Xaml.Grid.GridTemplateColumn  5n   "   $! Detalle��6;   $" 56Q�6Q   $ .  (   CellTemplate( 5o   &   �� 5p   *   .R�5q   .   �� $ 
BtnDetalle��65   -   5        $
��....��5q   I   #��[  6q   ) � Cursor$
) Hand��6�   ��6�   Z� 5r   +   5t   #   5w        5y      -   5        $ dataGridCxCD��5y   ,    . $ 2��6f    .   . $��10(�6�   $��Times New Roman)�6�    . # [ 66   6  ��  5|   "   
  $  Nit/cc��67   $! cod_ter��6K   $" 80Q�6a   
  5}   "   $  Nombre cliente��67   $! nom_ter��6T   $	" 220Q�6j   
  5~   "   $  Cuenta��67   $! cod_cta��6K   $" 70Q�6a   
  5   "   $
  Tipo��67   $
! tipo��6I   $" 50Q�6\   
  5�   "   $	  Trn��67   $! cod_trn��6H   $" 50Q�6^   
  5�   "   $  	Documento��67   $! num_trn��6N   $" 90Q�6d   
  5�   "   $" 90Q�6   $  FechaDoc��6�   % 67   �� $& fec_trn�$' {0:dd/MM/yyyy}��6�   
  5�   "   $" 90Q�6   $  	FechaVenc��6�   % 67   �� $& fec_ven�$' {0:dd/MM/yyyy}��6�   
  5�   "   $
  Dias��67   $
! dias��6I   $" 90Q�6\   
  5�   "   $" 85Q�6w   $  	Valor Doc��6�   $# Right=�6�   $ .  % 67   �� $& valor�$' {0:N2}��6�   
  5�   "   $" 80Q�6v   $  Saldo��6�   $# Right=�6�   $ .  % 67   �� $& saldo�$' {0:N2}��6�     5�   "   $	! Doc��6;   $" 56Q�6M   $ .  ( 5�   &   �� 5�   *   .R�5�   .   �� $ BtnDetalleD��65   -   5        $
��....��5�   J   #��[  6s   $
) Hand��6�   ��6�   Z� 5�   +   5�   #     5�   "   $! Auxiliar��6;   $" 56Q�6R   $ .  ( 5�   &   �� 5�   *   .R�5�   .   �� $ 
BtnAuxliar��65   -   5        $
��....��5�   I   #��[  6q   $
) Hand��6�   ��6�   Z� 5�   +   5�   #   5�      >  8Syncfusion.Windows.Controls.Notification.SfBusyIndicator  5�      -   5        $��sfBusyIndicator��5�   0   $��Bottom=�6G   $��12(�6b   *   AnimationSpeed$	* 100Q�6p   +   AnimationType$+ Rainy=�6�   ,   Header$, Cargando Datos ...��6�   $	��200��6�   -   IsBusy- .  6�   5�      �� 5�      $��2��6   
������$	��0.5q�6C   $��2��6Y   ._�5�      � $��2��6    5�      �� 5�   "   $��30 �60   �� 5�   "   $��30 �60   �� 5�   "   $��30 �60   �� 5�   "   $��30 �60    5�      �� 5�   "   $��80 �63   �� 5�   "   $	��130 �63   �� 5�   "   $	��130 �63   �� 5�   "   $	��130 �63   �� 5�   "   $��20 �63   �� 5�   "   $	��100 �63   �� 5�   "   $	��200 �63   �� 5�   "   $	��110 �63   �� 5�   "   $	��110 �63   �� 5�   "   $	��110 �63   .+�5�      �� 
������$	��0.5q�6?   $��0��6U   $��1��6b   ._�5�   "   �� $	��CxC��6,   $
��Bold"�67   
������$��Center=�6b   $��Center=�6   $��12(�6�   5�      �� 5�      
������$	��0.5q�6>   $��0��6T   $��2��6a   ._�5�   "   �� $��CxC-Anticipo��6,   $
��Bold"�6@   
������$��0��6j   $��2��6w   $��Center=�6�   $��Center=�6�   $��12(�6�   5�      �� 5�      
������$	��0.5q�6>   $��0��6T   $��3��6a   ._�5�   "   �� $��	Saldo CxC��6,   $
��Bold"�6=   
������$��0��6g   $��5��6t   $��Center=�6�   $��Center=�6�   $��12(�6�   5�      �� 5�      $��Causado:��6(   
������$
��Bold"�6P   $��1��6b   $��0��6o   $��Right=�6   $��Center=�6�   $��12(�6�   �� 5�      $��Abonos:��6(   
������$
��Bold"�6O   $��2��6a   $��0��6n   $��Right=�6~   $��Center=�6�   $��12(�6�   �� 5�      $��Saldos:��6(   
������$
��Bold"�6O   $��3��6a   $��0��6n   $��Right=�6~   $��Center=�6�   $��12(�6�   �� 5�      -   5        $ TextCxC��5�   )   
������$��1��6R   $��1��6_   $
��0.00��6p   $��Right=�6|   $��Center=�6�   $��12(�6�   �� 5�      -   5        $ 
TextCxCAnt��5�   )   
������$
��0.00��6U   $��1��6a   $��2��6n   $��Right=�6~   $��Center=�6�   $��12(�6�   �� 5�      -   5        $ TotalCxc��5�   )   
������$
��0.00��6S   $��1��6_   $��3��6l   $��Right=�6|   $��Center=�6�   $��12(�6�   �� 5�      -   5        $ TextCxCAbono��5�   )   
������$��2��6W   $��1��6d   $
��0.00��6u   $��Right=�6�   $��Center=�6�   $��12(�6�   �� 5�      -   5        $ TextCxCAntAbono��5�   )   
������$
��0.00��6Z   $��2��6f   $��2��6s   $��Right=�6�   $��Center=�6�   $��12(�6�   �� 5�      -   5        $ 
TotalAbono��5�   )   
������$
��0.00��6U   $��2��6a   $��3��6n   $��Right=�6~   $��Center=�6�   $��12(�6�   �� 5�      -   5        $ TextCxCSaldo��5�   )   
������$��3��6W   $��1��6d   $
��0.00��6u   $��Right=�6�   $��Center=�6�   $��12(�6�   �� 5�      -   5        $ TextCxCAntSaldo��5�   )   
������$
��0.00��6Z   $��3��6f   $��2��6s   $��Right=�6�   $��Center=�6�   $��12(�6�   �� 5�      -   5        $ 
TotalSaldo��5�   )   
������$
��0.00��6U   $��3��6a   $��3��6n   $��Right=�6~   $��Center=�6�   $��12(�6�   �� 5�      $��Tipo Reporte: ��6(   $��0��6>   $��5��6K   #��[ 6\   �� 5�      -   5        . ��Name$. 
CmbTipoDoc��5�   (   $��2,2,2,2q�6<   $��0��6M   $��6��6[   $��12(�6k   $/ �� IsSynchronizedWithCurrentItem$
/ TrueY�6�   0 �� SelectedItem0 6y   �� *SelectedItem+.@�5�   "   �� $"��Cartera resumida por cliente��60   1 �� 
IsSelected1 . 6W   �� 5�   "   $#��Cartera detallada por cliente��6/   5�      �� 5�      $��Altura Dias:��6(   $��1��6<   $��5��6I   #��[ 6Y   �� 5�      -   5        $ 	TxtAltura��5�   &   $��0��69   $��50��6B   $
��Left=�6M   $
��Left=�6o   $��1��6�   $��6��6�   $��2q�6�   �� 5�      $��Registros :��6(   $��2��6;   $��5��6H   #��[ 6X   �� 5�      -   5        $ 
TxtRecords��5�   (   $��0��6<   $��50��6E   $
��Left=�6P   $��Center=�6k   $��2��6�   $��6��6�   $��2q�6�   �� 5�      -   5        $ Imprimir��5�   %   $��Imprimir��67   $��7��6J   $��0��6Z   #��[ 6~   �� 5�      -   5        $ ExportarXls��5�   %   $��Exportar XLS��6:   $��8��6Q   $��0��6a   #��[ 6�   �� 5�      -   5        $ ConciliarCxcCo��5�   %   $��Conciliar cuentas��6=   $��9��6Y   $��0��6i   #��[ 6�   �� 5�      -    5        $ BtnvrAbonado��5�   %   $��Abonado��6;   $��7��6M   $��1��6]   $	��100��6j   #��[ 6�   �� 5�      -!   5        $ BtnCalcularInt��5�   %   $��Calcular Interes��6=   $��8��6X   $��1��6h   $	��100��6u   #��[ 6�   5�      5�      5�      5�      5�      5�      5�      