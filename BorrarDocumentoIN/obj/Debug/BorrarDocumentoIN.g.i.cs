﻿#pragma checksum "..\..\BorrarDocumentoIN.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "8159C26F64B042E194C6F59A9A2C6DB71CAAF1874F0BAAD697D52010C145268C"
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
using Syncfusion.Windows;
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
    /// BorrarDocumentoIN
    /// </summary>
    public partial class BorrarDocumentoIN : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 80 "..\..\BorrarDocumentoIN.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker TxFecIni;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\BorrarDocumentoIN.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker TxFecFin;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\BorrarDocumentoIN.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TrnIni;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\BorrarDocumentoIN.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TrnFin;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\BorrarDocumentoIN.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox NumIni;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\BorrarDocumentoIN.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox NumFin;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\BorrarDocumentoIN.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnConsutar;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\BorrarDocumentoIN.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnEliminar;
        
        #line default
        #line hidden
        
        
        #line 98 "..\..\BorrarDocumentoIN.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnSalir;
        
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
            System.Uri resourceLocater = new System.Uri("/BorrarDocumentoIN;component/borrardocumentoin.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\BorrarDocumentoIN.xaml"
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
            this.TxFecIni = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 2:
            this.TxFecFin = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 3:
            this.TrnIni = ((System.Windows.Controls.TextBox)(target));
            
            #line 84 "..\..\BorrarDocumentoIN.xaml"
            this.TrnIni.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TextBox_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 84 "..\..\BorrarDocumentoIN.xaml"
            this.TrnIni.LostFocus += new System.Windows.RoutedEventHandler(this.TextBox_LostFocus);
            
            #line default
            #line hidden
            return;
            case 4:
            this.TrnFin = ((System.Windows.Controls.TextBox)(target));
            
            #line 85 "..\..\BorrarDocumentoIN.xaml"
            this.TrnFin.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TextBox_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 85 "..\..\BorrarDocumentoIN.xaml"
            this.TrnFin.LostFocus += new System.Windows.RoutedEventHandler(this.TextBox_LostFocus);
            
            #line default
            #line hidden
            return;
            case 5:
            this.NumIni = ((System.Windows.Controls.TextBox)(target));
            
            #line 88 "..\..\BorrarDocumentoIN.xaml"
            this.NumIni.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TextBox_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 88 "..\..\BorrarDocumentoIN.xaml"
            this.NumIni.LostFocus += new System.Windows.RoutedEventHandler(this.TextBox_LostFocus);
            
            #line default
            #line hidden
            return;
            case 6:
            this.NumFin = ((System.Windows.Controls.TextBox)(target));
            
            #line 89 "..\..\BorrarDocumentoIN.xaml"
            this.NumFin.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TextBox_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 89 "..\..\BorrarDocumentoIN.xaml"
            this.NumFin.LostFocus += new System.Windows.RoutedEventHandler(this.TextBox_LostFocus);
            
            #line default
            #line hidden
            return;
            case 7:
            this.BtnConsutar = ((System.Windows.Controls.Button)(target));
            
            #line 96 "..\..\BorrarDocumentoIN.xaml"
            this.BtnConsutar.Click += new System.Windows.RoutedEventHandler(this.BtnConsutar_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.BtnEliminar = ((System.Windows.Controls.Button)(target));
            
            #line 97 "..\..\BorrarDocumentoIN.xaml"
            this.BtnEliminar.Click += new System.Windows.RoutedEventHandler(this.BtnEliminar_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.BtnSalir = ((System.Windows.Controls.Button)(target));
            
            #line 98 "..\..\BorrarDocumentoIN.xaml"
            this.BtnSalir.Click += new System.Windows.RoutedEventHandler(this.BtnSalir_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

