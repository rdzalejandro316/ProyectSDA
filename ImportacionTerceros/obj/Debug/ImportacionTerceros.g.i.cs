﻿#pragma checksum "..\..\ImportacionTerceros.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "F0EF4BE42A00AAD75F9800125D1A90333B6123779E9621A3D8FEBCE7A2AC4843"
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
    /// ImportacionTerceros
    /// </summary>
    public partial class ImportacionTerceros : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 249 "..\..\ImportacionTerceros.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnTercero;
        
        #line default
        #line hidden
        
        
        #line 250 "..\..\ImportacionTerceros.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnPlantilla;
        
        #line default
        #line hidden
        
        
        #line 262 "..\..\ImportacionTerceros.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.UI.Xaml.Grid.SfDataGrid dataGridExcel;
        
        #line default
        #line hidden
        
        
        #line 267 "..\..\ImportacionTerceros.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnEjecuter;
        
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
            System.Uri resourceLocater = new System.Uri("/ImportacionTerceros;component/importacionterceros.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ImportacionTerceros.xaml"
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
            this.BtnTercero = ((System.Windows.Controls.Button)(target));
            
            #line 249 "..\..\ImportacionTerceros.xaml"
            this.BtnTercero.Click += new System.Windows.RoutedEventHandler(this.BtnTercero_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.BtnPlantilla = ((System.Windows.Controls.Button)(target));
            
            #line 250 "..\..\ImportacionTerceros.xaml"
            this.BtnPlantilla.Click += new System.Windows.RoutedEventHandler(this.BtnPlantilla_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.dataGridExcel = ((Syncfusion.UI.Xaml.Grid.SfDataGrid)(target));
            return;
            case 4:
            this.BtnEjecuter = ((System.Windows.Controls.Button)(target));
            
            #line 267 "..\..\ImportacionTerceros.xaml"
            this.BtnEjecuter.Click += new System.Windows.RoutedEventHandler(this.BtnEjecuter_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

