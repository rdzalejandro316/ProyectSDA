﻿#pragma checksum "..\..\DocumentosReportes.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "A6D3400B32F3A8447B7DE1C0F52E0AB4245A99A0DED6CFF4BF4A878DF8C5AEDE"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Reporting.WinForms;
using SiasoftAppExt;
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
    /// DocumentosReportes
    /// </summary>
    public partial class DocumentosReportes : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\DocumentosReportes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Forms.Integration.WindowsFormsHost winFormsHost;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\DocumentosReportes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Reporting.WinForms.ReportViewer viewer;
        
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
            System.Uri resourceLocater = new System.Uri("/DocumentosReportes;component/documentosreportes.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\DocumentosReportes.xaml"
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
            
            #line 11 "..\..\DocumentosReportes.xaml"
            ((SiasoftAppExt.DocumentosReportes)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.winFormsHost = ((System.Windows.Forms.Integration.WindowsFormsHost)(target));
            
            #line 13 "..\..\DocumentosReportes.xaml"
            this.winFormsHost.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.winFormsHost_PreviewKeyDown);
            
            #line default
            #line hidden
            return;
            case 3:
            this.viewer = ((Microsoft.Reporting.WinForms.ReportViewer)(target));
            
            #line 14 "..\..\DocumentosReportes.xaml"
            this.viewer.Print += new Microsoft.Reporting.WinForms.ReportPrintEventHandler(this.viewer_Print);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

