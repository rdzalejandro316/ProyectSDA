   M S B A M L   `   `   `  ����F  BMenuReporte, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:SiasoftAppExtSiasoftAppExt  5          SiasoftAppExt.MenuReporte   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ` \Syncfusion.Linq.Base, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89_ [Syncfusion.Grid.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89`	 \Syncfusion.Tools.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89`
 \Syncfusion.Chart.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Gauge.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.SfMaps.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89g cSyncfusion.PropertyGrid.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89g cSyncfusion.ReportViewer.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89?SiaSf!http://schemas.syncfusion.com/wpf
    	 
      5      V RMaterialDesignThemes.Wpf, Version=2.5.0.1205, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      'localclr-namespace:SiasoftAppExt  5        � 	Resources  5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $e _pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml?�6%   �� 5      $h bpack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml?�6%   �� 5      $t npack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Blue.xaml?�6%   �� 5      $u opack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Accent/MaterialDesignColor.Indigo.xaml?�6%   %    5      .��5      � 
������� � ColumnDefinitions 5   
   �� 5      $��50 �6   �� 5      .+�5   
   � 
���   � K� ZIndex$ 4��6"   .+�5      �� -   5        $��MenuBTN��5      
 � Tag$ 1��6$   $	��Top=�6?   ��6W   Z� ��6m   Z� .��5       '  !MaterialDesignThemes.Wpf.PackIcon  �  �ControlzEx.PackIconBase`1[[MaterialDesignThemes.Wpf.PackIconKind, MaterialDesignThemes.Wpf, Version=2.5.0.1205, Culture=neutral, PublicKeyToken=null]]   Kind$
 Menu=�6*    � LayoutTransform 5!      �� 5"       �� ScaleX$	 1.5Q�6*   	 �� ScaleY$		 1.5Q�67   5$      5%      �� 5'      $��Bottom=�6   .+�5(      �� -   5        $��BTNParameter��5(      $ 1��6-   ��6P   Z� ��6f   Z� .��5)        $ BookmarkTick=�6.   5*      �� 5+      -   5        $��
BTNsetting��5+      $ 1��6+   ��6L   Z� ��6b   Z� .��5,        $ Settings=�6.   5-      5.      5/      � 51   
   -   5        $��	PanelMenu��51   -   $��1��6   $��1��6    
���   �$	��350��6Q   $
��Left=�6]   $��
-350,0,0,0q�6x   $ 3��6�   $y�Visible=�6�   
 J� Opacity$	
 0.8Q�6�   .+�52      � $��1��6   $	��Top=�67   $��0,0,0,0q�6O   ��6    Z�   53       �� 
TargetType �� Property �� Value �� 
TargetType �� Triggers P� Property P� Value 	  panel��%�  '��       &	  c    �� 55      " ��6   .�56      ��  � �� 6?   Z� �� 57       � �� 6@   Z� �� 58       � ��
 �����6?   �� 59       � ��$ Center=�6F   �� 5:       � ��$
 Left=�6H   �� 5;       �@�Cursor$
 Hand��6;   �� 5<       � �� 5=      �� 5>   "   " ��62   .R�5?   &   �� #��z��6-   $��1q�6n   ��6W   Z� ._�5@   *   �� $��Center=�6;   $��Center=�6X   5A   '   5B   #   5D       5E      P� 5F       �@J�IsMouseOver . . �5G   "   ��  � �� 6G   Z� �� 5H   "    �@�Cursor$
 Hand��6C   5I      5K      �� 5L      " ��6   .�5M      ��  �@��Orientation$ 
Horizontal=�68   �� 5N       � ��$
 Left=�6@   5O      5P      .+�5R      R� -   5        $
��Menu��5R      $
��Auto��6(   
�������$��18(�6H   5T      5W      5X      � 5Z   
   -   5        $��conte��5Z      $��1��6   $ 1��6,   
�������.+�5[      >  8Syncfusion.Windows.Controls.Notification.SfBusyIndicator  -   5        $��sfBusyIndicator��5[   $   $��5q�6;   $��Center=�6F   $��12(�6a      AnimationSpeed$	 100Q�6o      AnimationType$ Rainy=�6�      Header$! Cargando Datos - SiasoftApp��6�   $	��200��6�      IsBusy .  $ 2��6�   |�.  6�   5 	 /Syncfusion.Windows.Tools.Controls.TabControlExt  5\      -   5        $��TabControlPricipal��5\   "   $��Stretch=�6<   $��Stretch=�6Z   +  %Syncfusion.Windows.Shared.SkinStorage   VisualStyle$ Metro��6w   '    IsDisableUnloadTabItemExtContent .    EnableLabelEdit .     RotateTextWhenVertical .   �� TabStripPlacement$	 Top=�5]   3       TabItemSelectedBackground �#373737   CloseButtonType$
 Both=�6o   5`      5a      5c      5e      