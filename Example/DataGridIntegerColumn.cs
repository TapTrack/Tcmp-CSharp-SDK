﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Controls;

//namespace TapTrack.Demo
//{
//    public class DataGridIntegerColumn : DataGridTextColumn
//    {
//        protected override object PrepareCellForEdit(System.Windows.FrameworkElement editingElement, System.Windows.RoutedEventArgs editingEventArgs)
//        {
//            TextBox edit = editingElement as TextBox;
//            edit.PreviewTextInput += OnPreviewTextInput;

//            return base.PrepareCellForEdit(editingElement, editingEventArgs);
//        }

//        void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
//        {
//            try
//            {
//                Convert.ToInt32(e.Text);
//            }
//            catch
//            {
//                // Show some kind of error message if you want

//                // Set handled to true
//                e.Handled = true;
//            }
//        }
//    }
//}
