﻿#pragma checksum "..\..\MenuReporte.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "DC62F200CE82D084EA0235A07BBD64FE102BD771C9FE66A4190164741972A3FA"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using SiasoftAppExt;
using Syncfusion;
using Syncfusion.UI.Xaml.Maps;
using Syncfusion.Windows;
using Syncfusion.Windows.Chart;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Notification;
using Syncfusion.Windows.Data;
using Syncfusion.Windows.Gauge;
using Syncfusion.Windows.PropertyGrid;
using Syncfusion.Windows.Reports.Viewer;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace SiasoftAppExt {
    
    
    /// <summary>
    /// MenuReporte
    /// </summary>
    public partial class MenuReporte : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 31 "..\..\MenuReporte.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button MenuBTN;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\MenuReporte.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BTNParameter;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\MenuReporte.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BTNsetting;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\MenuReporte.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid PanelMenu;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\MenuReporte.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TreeView Menu;
        
        #line default
        #line hidden
        
        
        #line 90 "..\..\MenuReporte.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid conte;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\MenuReporte.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.Windows.Controls.Notification.SfBusyIndicator sfBusyIndicator;
        
        #line default
        #line hidden
        
        
        #line 92 "..\..\MenuReporte.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.Windows.Tools.Controls.TabControlExt TabControlPricipal;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MenuReporte;component/menureporte.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MenuReporte.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.MenuBTN = ((System.Windows.Controls.Button)(target));
            
            #line 31 "..\..\MenuReporte.xaml"
            this.MenuBTN.Click += new System.Windows.RoutedEventHandler(this.Button_Vis);
            
            #line default
            #line hidden
            return;
            case 2:
            this.BTNParameter = ((System.Windows.Controls.Button)(target));
            
            #line 40 "..\..\MenuReporte.xaml"
            this.BTNParameter.Click += new System.Windows.RoutedEventHandler(this.BTNParameter_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.BTNsetting = ((System.Windows.Controls.Button)(target));
            
            #line 43 "..\..\MenuReporte.xaml"
            this.BTNsetting.Click += new System.Windows.RoutedEventHandler(this.BTNsetting_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.PanelMenu = ((System.Windows.Controls.Grid)(target));
            return;
            case 5:
            this.Menu = ((System.Windows.Controls.TreeView)(target));
            return;
            case 6:
            this.conte = ((System.Windows.Controls.Grid)(target));
            return;
            case 7:
            this.sfBusyIndicator = ((Syncfusion.Windows.Controls.Notification.SfBusyIndicator)(target));
            return;
            case 8:
            this.TabControlPricipal = ((Syncfusion.Windows.Tools.Controls.TabControlExt)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

