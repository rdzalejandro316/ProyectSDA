   M S B A M L   `   `   `  ����I  EImportacion740, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Importacion740Importacion740  5   	   #    Importacion740.BrowDocumentos   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:Importacion740     5   	   a ]Syncfusion.SfGrid.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfGridConverter.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c898
syncfusion!http://schemas.syncfusion.com/wpf     5   	   -   5          � Title$  
Documentos��5   	   $	��400��6   $	��500��6)    � WindowStartupLocation$ CenterScreen=�6L    � 	Resources 5      3  -Syncfusion.UI.Xaml.Grid.GridHeaderCellControl �� 
TargetType   headerStyle �� Property �� Value��%�   &	        �� 5   
   "  6   .�5      ��  � ��$
 Bold"�6+   �� 5       � ��$ 10(�6)   �� 5       � ��
 �����6+   �� 5       � �� �#3737376+   5      5      .��5      �  � RowDefinitions 5   
   �� 5      $��1* �6   �� 5      $��30 �6   .+�5   
   (  "Syncfusion.UI.Xaml.Grid.SfDataGrid  -   5        $��dataGrid��5       $��0��60   $��2q�6=      EnableDataVirtualization . (  "Syncfusion.UI.Xaml.Grid.SfGridBase   AutoGenerateColumns .  	   ShowGroupDropArea	 .  $��12(�6�   
   AllowFiltering
 .    SelectionMode$ Single=�6�      HeaderStyle# [  6�      Columns 5      ,  &Syncfusion.UI.Xaml.Grid.GridTextColumn  ,  &Syncfusion.UI.Xaml.Grid.GridColumnBase   MappingName$ COD_TDO��6,      IsHidden . 6B     5         
HeaderText$ Trn Con��6,   $ COD_TRN��6A      Width$ 65Q�6W     5      $ 	Documento��6,   $ NUM_TRN��6C   $	 120Q�6Y   0  *Syncfusion.UI.Xaml.Grid.GridDateTimeColumn  5      $ Fecha��60   $ FEC_TRN��6C   $	 120Q�6Y   0  *Syncfusion.UI.Xaml.Grid.GridTemplateColumn  5       $ Doc CO��60   $ 80Q�6E   (  "Syncfusion.UI.Xaml.Grid.GridColumn   AllowFiltering .     CellTemplate 5!      �� 5"      .R�5#      ��  ��Name$ BtnGetDocument��6%   -   5        ���#2196F3 � Cursor$
 Hand��5#   o   
�������$	��...��6�   
 � Tag$ co��6�   ��6}   Z� 6�   5$      5&      5(      � 5*   
   $��1��6   ���#373737.+�5+      ��  �� Orientation$ 
Horizontal=�6   .+�5,      �� $��Documentos : ��6   
�������$��Center=�6D   $��5 0 0 0q�6_   $
��Bold"�6p   �� 5-      -   5        $��
Tx_totales��5-      
�������$��Center=�6A   5.      5/      51      53      