   M S B A M L   `   `   `  ����   AnalisisDeCuentasPorPagar8    2AnalisisDeCartera.AnalisisDeCuentasPorPagarDetalle   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     a ]Syncfusion.SfGrid.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89e aSyncfusion.GridCommon.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfGridConverter.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b	 ^Syncfusion.SfChart.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b
 ^Syncfusion.SfInput.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89_ [Syncfusion.Grid.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Chart.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Linq.Base, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89ASiaSf!http://schemas.syncfusion.com/wpf    	 
         � Title$"  AnalisisDeProveedoresDetalle��$	��650��$
��1100�� � WindowStartupLocation$ CenterOwner=� � ShowInTaskbar .  +  %Syncfusion.Windows.Shared.SkinStorage   VisualStyle$ 	ShinyBlue��.���  � RowDefinitions �� $��40 ��� $��* ��� $	��120 �.+�� $��0�� �� $��28 ��� $��28 � � ColumnDefinitions �� $��80 ��� $	��100 ��� $	��270 ��� $	��100 ��� $	��100 ��� $	��100 ��� $��* ��� $	��100 ��� $��50 �.+��� $��0��$
��Bold"�$��0��$��Proveedor: ��$��Right=�$��Center=��� -    ��Name$ 
TextCodigo��$��0��$��1��$	��-- ��$
��Left=�$��Center=��� -   $ 
TextNombre��$��0��$��2��$	��-- ��$
��Left=�$��Center=��� $��0��$
��Bold"�$��3��$��Cuenta: ��$��Right=�$��Center=��� -   $ 
TextCuenta��$��0��$��4��$	��-- ��$
��Left=�$��Center=��� -   $��Salir��$��0��$��7�� �� IsCancel . � $��1�� � 	Resources &   Syncfusion.UI.Xaml.Grid.GridCell	 �� 
TargetType
 �� Property �� Value3  -Syncfusion.UI.Xaml.Grid.GridHeaderCellControl   headerStyle��%�   '        &	  H     �� "	  .��� 
 � ��
 ������� 
 � ��$ Times New Roman)��� "	  .��� 
 � ��
 ������� 
 � ��
 ��  ��� 
 � ��$ 10(�.+�(  "Syncfusion.UI.Xaml.Grid.SfDataGrid  -     Name$ dataGridCxC��$��5,5,5,5q�   EnableDataVirtualization . (  "Syncfusion.UI.Xaml.Grid.SfGridBase   AutoGenerateColumns .     ShowGroupDropArea . $��11(�   AllowFiltering .    HeaderStyle# [     ItemsSource �� *DtTraslados+   Columns ,  &Syncfusion.UI.Xaml.Grid.GridTextColumn  ,  &Syncfusion.UI.Xaml.Grid.GridColumnBase   
HeaderText$	 Trn��   MappingName$ cod_trn��   Width$ 50Q�  $ 	Documento��$ num_trn��$	 110Q�  $	 100Q�$ 	Fecha Doc��   DisplayBinding ��  �� Path$ fec_trn� �� StringFormat$ {0:dd/MM/yyyy}��  $	 100Q�$ 	Fecha Ven�� �� $ fec_ven�$ {0:dd/MM/yyyy}��  $
 Tipo��$
 tipo��$ 50Q�  $ Cuenta��$ cod_cta��$ 90Q�  $
 Dias��$
 dias��$ 50Q�  $	 110Q�$ 	Valor Doc��   TextAlignment$ Right=�(  "Syncfusion.UI.Xaml.Grid.GridColumn   AllowFiltering .   �� $ valor�$ {0:N2}��  $	 110Q�$ Saldo��$ Right=� .   �� $ saldo�$ {0:N2}��  $ C.Costo��$ nom_cco��$	 100Q�0	  *Syncfusion.UI.Xaml.Grid.GridTemplateColumn	  $ Abonos��$ 76Q� .     CellTemplate �� .R���  ��Name$ 
BtnDetalle��$
��....��
����İ��� $��2�����#2980b9$	��0.5q�$��3,3,3,3q�$��2��._�� $��2�� �� $��25 ��� $��25 ��� $��25 ��� $��25 � �� $	��130 ��� $	��130 ��� $	��130 ��� $	��130 ��� $��20 ��� $	��120 ��� $	��200 ��� $	��100 ��� $	��100 �.+��� ���#2980b9$	��0.5q�$��0��$��1��._��� $	��CxP�����#2980b9$��Center=�$��Center=�$��12(��� ���#2980b9$	��0.5q�$��0��$��2��._��� $��CxP-Anticipo�����#2980b9$��0��$��2��$��Center=�$��Center=�$��12(��� ���#2980b9$	��0.5q�$��0��$��3��._��� $��	Saldo CxP�����#2980b9$��0��$��5��$��Center=�$��Center=�$��12(��� $��Causado:�����#2980b9$��1��$��0��$��Right=�$��Center=�$��12(��� $��Abonos:�����#2980b9$��2��$��0��$��Right=�$��Center=�$��12(��� $��Saldos:�����#2980b9$��3��$��0��$��Right=�$��Center=�$��12(��� -   $ TextCxP�����#2980b9$��1��$��1��$
��0.00�� �� TextAlignment$ Right=�$��Center=�$��12(��� -   $ 
TextCxPAnt�����#2980b9$
��0.00��$��1��$��2��$ Right=�$��Center=�$��12(��� -   $ TotalCxP�����#2980b9$
��0.00��$��1��$��3��$ Right=�$��Center=�$��12(��� -	   $ TextCxPAbono�����#2980b9$��2��$��1��$
��0.00��$ Right=�$��Center=�$��12(��� -
   $ TextCxPAntAbono�����#2980b9$
��0.00��$��2��$��2��$ Right=�$��Center=�$��12(��� -   $ 
TotalAbono�����#2980b9$
��0.00��$��2��$��3��$ Right=�$��Center=�$��12(��� -   $ TextCxPSaldo�����#2980b9$��3��$��1��$
��0.00��$ Right=�$��Center=�$��12(��� -   $ TextCxPAntSaldo�����#2980b9$
��0.00��$��3��$��2��$ Right=�$��Center=�$��12(��� -   $ 
TotalSaldo�����#2980b9$
��Bold"�$
��0.00��$��3��$��3��$ Right=�$��Center=�$��12(��� $��Tipo Reporte: ��$y�Hidden=�
������$��0��$��5��$��Center=�$��Right=�$��12(��� -    ��Name$ 
CmbTipoDoc��$y�Hidden=�$��2,2,2,2q�$��0��$��6��$��12(�$  �� IsSynchronizedWithCurrentItem$
  TrueY�! �� SelectedItem! �� *SelectedItem+.@��� $��Cartera resumida ���� $��Cartera detallada ���� -   $ 
ReImprimir��$��Imprimir��$��7��$��1,1,1,1q�$��0��$��12(�
��� ���$��90���� -   $ ExportarXls��$��Exportar XLS��$��8��$��1,1,1,1q�$��0��$��12(�
��� ���$��90��