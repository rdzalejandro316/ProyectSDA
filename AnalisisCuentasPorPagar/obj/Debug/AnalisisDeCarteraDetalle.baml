   M S B A M L   `   `   `  ����  AnalisisDeCartera0    *AnalisisDeCartera.AnalisisDeCarteraDetalle   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   a ]Syncfusion.SfGrid.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89e aSyncfusion.GridCommon.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfGridConverter.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfChart.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b	 ^Syncfusion.SfInput.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89`
 \Syncfusion.Tools.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89_ [Syncfusion.Grid.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Chart.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Linq.Base, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89ASiaSf!http://schemas.syncfusion.com/wpf     	 
      5   	     � Title$  AnalisisDeCarteraDetalle��5	   	   $	��650��6*   $
��1100��67    � WindowStartupLocation$ CenterOwner=�6D    � ShowInTaskbar .  .��5
      �  � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      $��* �6   �� 5      $	��120 �6   .+�5   
   � $��0��6    5      �� 5      $��28 �6    �� 5      $��28 �6     � ColumnDefinitions 5      �� 5      $��60 �6#   �� 5      $	��100 �6#   �� 5      $	��270 �6#   �� 5      $	��100 �6#   �� 5      $	��100 �6#   �� 5      $	��100 �6#   �� 5      $��* �6#   �� 5      $	��100 �6#   �� 5      $��50 �6#   .+�5       �� $��0��6   $
��Bold"�6%   $��0��67   $��	Cliente: ��6G   $��Right=�6X   $��Center=�6t   �� 5!      -   5         ��Name$ 
TextCodigo��5!   5   $��0��6   $��1��6%   $	��-- ��6I   $
��Left=�6T   $��Center=�6o   �� 5"      -   5        $ 
TextNombre��5"   5   $��0��6   $��2��6%   $	��-- ��6I   $
��Left=�6T   $��Center=�6o   �� 5#      $��0��6   $
��Bold"�6%   $��3��67   $��Cuenta: ��6H   $��Right=�6X   $��Center=�6t   �� 5$      -   5        $ 
TextCuenta��5$   5   $��0��6   $��4��6%   $	��-- ��6I   $
��Left=�6T   $��Center=�6o   �� 5%      -   5        $��Salir��5%      $��0��6%   $��7��62    �� IsCancel . 6W   5&      � 5'   
   $��1��6    � 	Resources 5(      &   Syncfusion.UI.Xaml.Grid.GridCell �� 
TargetType	 �� Property
 �� Value3  -Syncfusion.UI.Xaml.Grid.GridHeaderCellControl   headerStyle��%#  '        &	  {     �� 5)      "  6   .�5*      �� 	 � ��

 �����64   �� 5+      	 � ��$
 Times New Roman)�63   5,      �� 5.      "  6   .�5/      �� 	 � ��

 �����63   �� 50      	 � ��

 ��  �63   �� 51      	 � ��$
 10(�61   52      54      .+�56      (  "Syncfusion.UI.Xaml.Grid.SfDataGrid  -   5          Name$ dataGridCxC��56       $��5,5,5,5q�66      EnableDataVirtualization . (  "Syncfusion.UI.Xaml.Grid.SfGridBase   AutoGenerateColumns .     ShowGroupDropArea . $��11(�58   N      AllowFiltering .    HeaderStyle# [  6�      ItemsSource 6s   �� *DtTraslados+   Columns 5:      ,  &Syncfusion.UI.Xaml.Grid.GridTextColumn  ,  &Syncfusion.UI.Xaml.Grid.GridColumnBase   
HeaderText$	 Trn��6+      MappingName$ cod_trn��6<      Width$ 50Q�6R     5;      $ 	Documento��6+   $ num_trn��6B   $	 110Q�6X     5<      $	 100Q�6s   $ 	Fecha Doc��6      DisplayBinding 6+   ��  �� Path$ fec_trn� �� StringFormat$ {0:dd/MM/yyyy}��6     5=      $	 100Q�6s   $ 	Fecha Ven��6    6+   �� $ fec_ven�$ {0:dd/MM/yyyy}��6     5>      $
 Tipo��6+   $
 tipo��6=   $ 50Q�6R     5@      $ Cuenta��6+   $ cod_cta��6?   $ 90Q�6W     5A      $
 Dias��6+   $
 dias��6=   $ 50Q�6P     5B      $	 110Q�6j   $ 	Valor Doc��6v      TextAlignment$ Right=�6�   (  "Syncfusion.UI.Xaml.Grid.GridColumn   AllowFiltering .   6+   �� $ valor�$ {0:N2}��6�     5C      $	 110Q�6j   $ Saldo��6v   $ Right=�6�    .   6+   �� $ saldo�$ {0:N2}��6�     5D      $
 Vend��6+   $ cod_ven��6=   $ 60Q�6S     5E      $ C.Costo��6+   $ nom_cco��6@   $	 100Q�6V   0  *Syncfusion.UI.Xaml.Grid.GridTemplateColumn  5F      $ Abonos��6/   $ 76Q�6D    .     CellTemplate 5G      �� 5H      .R�5I   "   ��  ��Name$ 
BtnDetalle��6)   $
��....��6=   
����İ�6M   5K      5M      5P      5Q      �� 5R   
   $��2��6   
������$	��0.5q�69   $��3,3,3,3q�6O   $��2��6`   ._�5S      � $��2��6    5T      �� 5U      $��25 �6$   �� 5V      $��25 �6$   �� 5W      $��25 �6$   �� 5X      $��25 �6$    5Z      �� 5[      $	��130 �6'   �� 5\      $	��130 �6'   �� 5]      $	��130 �6'   �� 5^      $	��130 �6'   �� 5_      $��20 �6'   �� 5`      $	��120 �6'   �� 5a      $	��200 �6'   �� 5b      $	��100 �6'   �� 5c      $	��100 �6'   .+�5e      �� 
������$	��0.5q�63   $��0��6I   $��1��6V   ._�5f      �� $	��CxC��6    
������$��Center=�6D   $��Center=�6a   $��12(�6|   5g      �� 5h      
������$	��0.5q�62   $��0��6H   $��2��6U   ._�5i      �� $��CxC-Anticipo��6    
������$��0��6L   $��2��6Y   $��Center=�6i   $��Center=�6�   $��12(�6�   5j      �� 5k      
������$	��0.5q�62   $��0��6H   $��3��6U   ._�5l      �� $��	Saldo CxC��6    
������$��0��6I   $��5��6V   $��Center=�6f   $��Center=�6�   $��12(�6�   5m      �� 5n      $��Causado:��6   
������$��1��6D   $��0��6Q   $��Right=�6a   $��Center=�6}   $��12(�6�   �� 5o      $��Abonos:��6   
������$��2��6C   $��0��6P   $��Right=�6`   $��Center=�6|   $��12(�6�   �� 5p      $��Saldos:��6   
������$��3��6C   $��0��6P   $��Right=�6`   $��Center=�6|   $��12(�6�   �� 5q      -   5        $ TextCxC��5q      
������$��1��6F   $��1��6S   $
��0.00��6d   $��Center=�6p   $��Center=�6�   $��12(�6�   �� 5r      -   5        $ 
TextCxCAnt��5r      
������$
��0.00��6I   $��1��6U   $��2��6b   $��Center=�6r   $��Center=�6�   $��12(�6�   �� 5s      -   5        $ TotalCxc��5s      
������$
��0.00��6G   $��1��6S   $��3��6`   $��Center=�6p   $��Center=�6�   $��12(�6�   �� 5t      -	   5        $ TextCxCAbono��5t      
������$��2��6K   $��1��6X   $
��0.00��6i   $��Center=�6u   $��Center=�6�   $��12(�6�   �� 5u      -
   5        $ TextCxCAntAbono��5u      
������$
��0.00��6N   $��2��6Z   $��2��6g   $��Center=�6w   $��Center=�6�   $��12(�6�   �� 5v      -   5        $ 
TotalAbono��5v      
������$
��0.00��6I   $��2��6U   $��3��6b   $��Center=�6r   $��Center=�6�   $��12(�6�   �� 5w      -   5        $ TextCxCSaldo��5w      
������$��3��6K   $��1��6X   $
��0.00��6i   $��Center=�6u   $��Center=�6�   $��12(�6�   �� 5x      -   5        $ TextCxCAntSaldo��5x      
������$
��0.00��6N   $��3��6Z   $��2��6g   $��Center=�6w   $��Center=�6�   $��12(�6�   �� 5y      -   5        $ 
TotalSaldo��5y      
������$
��0.00��6I   $��3��6U   $��3��6b   $��Center=�6r   $��Center=�6�   $��12(�6�   �� 5z      $��Tipo Reporte: ��6   
������$��0��6K   $��5��6X   $��Center=�6i   $��Right=�6�   $��12(�6�   �� 5{      -   5         ��Name$ 
CmbTipoDoc��5{      $��2,2,2,2q�60   $��0��6A   $��6��6O   $��12(�6_   $ �� IsSynchronizedWithCurrentItem$
 TrueY�6�    �� SelectedItem 6m   �� *SelectedItem+.@�5|      �� $"��Cartera resumida por cliente��6$   �� 5}      $#��Cartera detallada por cliente��6#   5~      �� 5      -   5        $ 
ReImprimir��5      $��Imprimir��6-   $��7��6@   $��1,1,1,1q�6P   $��0��6a   $��12(�6p   
��� ���$��90��6�   �� 5�      -   5        $ ExportarXls��5�      $��Exportar XLS��6.   $��8��6E   $��1,1,1,1q�6U   $��0��6f   $��12(�6s   
��� ���$��90��6�   5�      5�      5�      5�      