﻿#pragma checksum "..\..\ReclasificacionCuentasXLS.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "075CF853B2F50C4097AE4032EE27BF1F3C4234701E18D6AEC46E97305E211E11"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using SiasoftAppExt;
using Syncfusion;
using Syncfusion.UI.Xaml.Controls.DataPager;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.Grid.RowFilter;
using Syncfusion.UI.Xaml.TreeGrid;
using Syncfusion.Windows;
using Syncfusion.Windows.Controls.Notification;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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
    /// ReclasificacionCuentasXLS
    /// </summary>
    public partial class ReclasificacionCuentasXLS : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 77 "..\..\ReclasificacionCuentasXLS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnImport;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\ReclasificacionCuentasXLS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnPlantilla;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\ReclasificacionCuentasXLS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnEjecuter;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\ReclasificacionCuentasXLS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.Windows.Controls.Notification.SfBusyIndicator sfBusyIndicator;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\ReclasificacionCuentasXLS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.UI.Xaml.Grid.SfDataGrid dataGridExcel;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\ReclasificacionCuentasXLS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Tx_total;
        
        #line default
        #line hidden
        
        
        #line 109 "..\..\ReclasificacionCuentasXLS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Tx_errores;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\ReclasificacionCuentasXLS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnErrores;
        
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
            System.Uri resourceLocater = new System.Uri("/ReclasificacionCuentasXLS;component/reclasificacioncuentasxls.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ReclasificacionCuentasXLS.xaml"
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
            this.BtnImport = ((System.Windows.Controls.Button)(target));
            
            #line 77 "..\..\ReclasificacionCuentasXLS.xaml"
            this.BtnImport.Click += new System.Windows.RoutedEventHandler(this.BtnImport_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.BtnPlantilla = ((System.Windows.Controls.Button)(target));
            
            #line 78 "..\..\ReclasificacionCuentasXLS.xaml"
            this.BtnPlantilla.Click += new System.Windows.RoutedEventHandler(this.BtnPlantilla_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.BtnEjecuter = ((System.Windows.Controls.Button)(target));
            
            #line 79 "..\..\ReclasificacionCuentasXLS.xaml"
            this.BtnEjecuter.Click += new System.Windows.RoutedEventHandler(this.BtnEjecuter_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.sfBusyIndicator = ((Syncfusion.Windows.Controls.Notification.SfBusyIndicator)(target));
            return;
            case 5:
            this.dataGridExcel = ((Syncfusion.UI.Xaml.Grid.SfDataGrid)(target));
            return;
            case 6:
            this.Tx_total = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.Tx_errores = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.BtnErrores = ((System.Windows.Controls.Button)(target));
            
            #line 110 "..\..\ReclasificacionCuentasXLS.xaml"
            this.BtnErrores.Click += new System.Windows.RoutedEventHandler(this.BtnErrores_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

