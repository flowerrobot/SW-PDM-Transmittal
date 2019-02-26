#region Copyright Syncfusion Inc. 2001-2018.
// Copyright Syncfusion Inc. 2001-2018. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Controls.RichTextBoxAdv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SyncfusionWpfApp2
{
    /// <summary>
    /// Represents the extension class for SfRichTextBoxAdv.
    /// </summary>
    public class SfRichTextBoxAdvExtension : SfRichTextBoxAdv
    {
        #region Properties
        /// <summary>
        /// Gets or Sets the Html Text.
        /// </summary>
        public string HtmlText
        {
            get
            {
                return (string)GetValue(HtmlTextProperty);
            }
            set
            {
                SetValue(HtmlTextProperty, value);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the instance of SfRichTextBoxAdvExtension class.
        /// </summary>
        public SfRichTextBoxAdvExtension()
        {
           
        }
        #endregion

        #region Static Dependency Properties
        /// <summary>
        /// Using as a backing store for HtmlText dependency property to enable styling, animation etc.
        /// </summary>
        public static readonly DependencyProperty HtmlTextProperty = DependencyProperty.Register("HtmlText", typeof(string), typeof(SfRichTextBoxAdvExtension), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnHtmlTextChanged)));
        #endregion

        #region Static Events
        /// <summary>
        /// Called when Html Text changed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private static void OnHtmlTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            SfRichTextBoxAdvExtension richTextBox = (SfRichTextBoxAdvExtension)obj;
            //Update the document with the Html Text.
            richTextBox.UpdateDocument((string)e.NewValue);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the document.
        /// </summary>
        /// <param name="htmlText"></param>
        private void UpdateDocument(string htmlText)
        {
            //If HtmlText property is set internally means, skip updating the document.
            if (!string.IsNullOrEmpty(htmlText))
            {
                Stream stream = new MemoryStream();
                // Convert the html text to byte array.
                byte[] bytes = Encoding.UTF8.GetBytes(htmlText);
                // Writes the byte array to stream.
                stream.Write(bytes, 0, bytes.Length);
                stream.Position = 0;
                //Load the stream.
                Load(stream, FormatType.Html);
            }
        }
      
        #endregion
    }
}
