   M S B A M L   `   `   `  ����I  EImportacion740, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:SiasoftAppExtSiasoftAppExt  5      "    SiasoftAppExt.Importacion740   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      -localclr-namespace:SiasoftAppExt     5      a ]Syncfusion.SfGrid.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfGridConverter.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c898
syncfusion!http://schemas.syncfusion.com/wpf     5        � 	Resources  5
      3  -Syncfusion.UI.Xaml.Grid.GridHeaderCellControl �� 
TargetType   headerStyle �� Property �� Value 	 borde3  -System.Windows.Media.Effects.DropShadowEffect   	Direction   ShadowDepth   
BlurRadius   Color/  )Syncfusion.UI.Xaml.Grid.GridRowHeaderCell �� 
TargetType	 ��Name
 �� TextAlignment �� RelativeSource��%�  &	        &	 �     '  �    �� 5   
   "  6   .�5      ��  � ��$
 Bold"�6+   �� 5       � ��$ 10(�6)   �� 5       � ��
 �����6+   �� 5       � �� �#3737376+   5      �� 5   
   " ��6   .�5      ��  � ��
 �����6+   �� 5       � ��$
 Auto��6'   �� 5       � ��$ 1,1,1,1q�60   �� 5       � ��$ 5q�6'   �� 5       �@J�Effect 5        5      $ 0Q�6'   $	 0.5Q�65   $ 5Q�6G   $ Black��6V   5      5      �� 5   
   "  6   .�5      ��  � �� 5      �� 5      "  6&   .R�5       �� $	 PART_RowHeaderCellBorder��6!   #��z��5!      #��z��5"      #��z��5#      ._�5$      � .+�5%   "   �� $��Center=�6,   $��Center=�5&   $   $
 Center=�5(   $   ��5'   $   �� *
RowIndex+ �� *TemplatedParent+5(   $   5)      5*      5+      5-      5.      5/      .��51      �  � RowDefinitions 52   
   �� 53      $��60 �6   �� 54      $��1* �6   �� 55      $	��100 �6   .+�58   
   � 
���   �.+�59      ��  �� Orientation$ 
Horizontal=�6   $��Right=�62   $��0 0 5 0q�6N   .+�5:      �� -   5        $��BtnImportar��5:       � ToolTip$< 6Importar archivo xls segun el formato de plantilla xls��6,   $	��130��6m   $��40��6y   ���#2196F3
�������$��5q�6�   $��Importa XLS��6�    � Cursor$
 Hand��6�   ��6�   Z� 6�   �� 5;      -   5        $��
BtnGenerar��5;      $O IPlantilla con la cual funciona el proceso de importacion de esta pantalla��6+   $	��130��6   $��40��6�   ���#2196F3
�������$��3q�6�   $��Generar Plantilla XLS��6�   $
 Hand��6  ��6�   Z� 6  �� 5<      -   5        $��BtnCrear��5<      $. (Generar documentos de retiros de activos��6)   $	��130��6\   $��40��6h   ���#2196F3
�������$��3q�6�   $��Generar Documentos��6�   $
 Hand��6�   ��6�   Z� 6�   5=      5>      �� 5@   
   $��1��6   #��[ 6   ._�5A      � .+�5B      >  8Syncfusion.Windows.Controls.Notification.SfBusyIndicator  -   5        $��sfBusyIndicator��5B   :   $��1��6-   $��5q�6Q   $��Bottom=�6\   $��12(�6w      AnimationSpeed$	 100Q�6�      AnimationType$	 Pen=�6�      Header$ Importando Datos��6�   $	��100��6�      IsBusy .  ���#2196F36�   (  "Syncfusion.UI.Xaml.Grid.SfDataGrid  5C      -   5        $��dataGridRefe��5C   (   (  "Syncfusion.UI.Xaml.Grid.SfGridBase   ShowRowHeader . $��0��6Q   $��2q�6^      EnableDataVirtualization .    AllowResizingColumns .    AutoGenerateColumns .     ShowGroupDropArea .  $��10(�6�      AllowFiltering .    SelectionMode$ Single=�6     HeaderStyle# [  6     Columns 5E      ,  &Syncfusion.UI.Xaml.Grid.GridTextColumn  ,  &Syncfusion.UI.Xaml.Grid.GridColumnBase   
HeaderText$	 TRN��64      MappingName$ COD_TRN��6E      Width$ 60Q�6[     5F      $ NUM_TRN��64   $ NUM_TRN��6I   $	 100Q�6_     5G      $ FEC_TRN��64   $ FEC_TRN��6I   $ 80Q�6_     5H      $ COD_CTA��64   $ COD_CTA��6I   $ 80Q�6_     5I      $ COD_TER��64   $ COD_TER��6I   $ 80Q�6_     5J      $ DES_MOV��64   $ DES_MOV��6I   $	 150Q�6_     5K      $ DOC_MOV��64   $ DOC_MOV��6I   $ 90Q�6_   /	  )Syncfusion.UI.Xaml.Grid.GridNumericColumn	  5L      $ BAS_MOV��67   $ BAS_MOV��6L   $ 90Q�6b     	  NumberDecimalDigits$  0��6m   ! 	  NumberDecimalSeparator$! .��6�   " 	  NumberGroupSizes" �      # 	  NumberGroupSeparator$# ,��6�   	  5M      $ DEB_MOV��67   $ DEB_MOV��6L   $ 90Q�6b   $  0��6m   $! .��6�   " �      $# ,��6�   	  5N      $ CRE_MOV��67   $ CRE_MOV��6L   $ 90Q�6b   $  0��6m   $! .��6�   " �      $# ,��6�     5O      $ DOC_CRUC��64   $ DOC_CRUC��6J   $ 90Q�6a     5P      $ ORD_PAG��64   $ ORD_PAG��6I   $ 90Q�6_     5Q      $ COD_BANC��64   $ COD_BANC��6J   $ 90Q�6a     5R      $ FEC_VENC��64   $ FEC_VENC��6J   $ 90Q�6a     5S      $ NUM_CHQ��64   $ NUM_CHQ��6I   $ 90Q�6_     5T      $ FACTURA��64   $ FACTURA��6I   $ 90Q�6_     5U      $ COD_CIU��64   $ COD_CIU��6I   $ 90Q�6_     5V      $ COD_SUC��64   $ COD_SUC��6I   $ 90Q�6_     5W      $ COD_CCO��64   $ COD_CCO��6I   $ 90Q�6_     5X      $ DOC_REF��64   $ DOC_REF��6I   $ 90Q�6_     5Y      $ FEC_SUSC��64   $ FEC_SUSC��6J   $ 90Q�6a   5[      5\      5]      � 5_   
   $��2��6   
���   � 5`      �� 5a      $��1* �6    �� 5b      $��30 �6    .+�5e      � 
���   �$ � ColumnDefinitions$ 5f      �� 5g      $��1* �6'   �� 5h      $��1* �6'   .+�5l      � $ 5m      �� 5n      $	��100 �6+   �� 5o      $��1* �6+    5q      �� 5r      $��1* �6(   �� 5s      $��1* �6(   .+�5v      �� $��0��6    $��
TERCERO : ��6-   $��Center=�6?   $��Right=�6Z   
�������$
��Bold"�6�   �� 5w      -   5        % ��Name$% Tx_ter��5w       $��0��60   $��1��6=   $��Center=�6M   
�������$��5 0 0 0q�6{   �� 5y      $��1��6    $��	CUENTA : ��6-   $��Center=�6>   $��Right=�6Y   
�������$
��Bold"�6�   �� 5z      -   5        $% Tx_cuen��5z       $��1��61   $��1��6>   $��Center=�6N   
�������$��5 0 0 0q�6|   5|      � 5�      $��1��6   $ 5�      �� 5�      $	��100 �6+   �� 5�      $��1* �6+    5�      �� 5�      $��1* �6(   �� 5�      $��1* �6(   .+�5�      �� $ 
Horizontal=�6!   $��2��6:   .+�5�      �� $��0��6$   $��	DEBITOS: ��61   $��Center=�6B   $��Right=�6]   
�������$
��Bold"�6�   $	��100��6�   $
 Right=�6�   �� 5�      -   5        $% 	TxTot_deb��5�   -   $��-��6$   $��Center=�6@   ���#2196F3$��5 0 0 0q�6p   $	��150��6�   �� 5�      $��0��6$   $��
CREDITOS: ��61   $��Center=�6C   $��Right=�6^   
�������$
��Bold"�6�   $��10 0 0 0q�6�   �� 5�      -	   5        $% 	TxTot_cre��5�   -   $��-��6$   $��Center=�6@   ���#2196F3$��5 0 0 0q�6p   $	��150��6�   5�      �� 5�      $��2��6    $��DIFERENCIA: ��6.   $��Center=�6B   $��Right=�6]   
�������$
��Bold"�6�   �� 5�      -
   5        $% Txdif��5�   )   $��-��6    $��1��68   $��1��6E   $
��Left=�6U   $��Center=�6p   ���#2196F3$��5 0 0 0q�6�   $	��150��6�   5�      5�      � 5�      $��1��6   ���#373737$ 5�      �� 5�      $��1* �6'   �� 5�      $��1* �6'   .+�5�      �� $ 
Horizontal=�6   .+�5�      �� $��Total Registros: ��6    
�������$
��Bold"�6L   $��Center=�6^   $��5 0 0 0q�6y   �� 5�      -   5        $��Tx_total��5�       $��0��60   
�������$��Center=�6L   $
��Left=�6g   5�      �� 5�      $��1��6   $ 
Horizontal=�6-   .+�5�      �� $��Filas con errores: ��6    
�������$
��Bold"�6N   $��Center=�6`   $��5 0 0 0q�6{   �� 5�      -   5        $��
Tx_errores��5�       $��0��62   
���  ��$��Center=�6L   $
��Left=�6g   �� 5�      -   5        $��
BtnErrores��5�      $��lista errores��6/   $��Center=�6G   $��10,0,0,0q�6b   $��5,2,5,2q�6t   
����������#2196F3��6�   Z� 6�   5�      5�      5�      5�      5�      