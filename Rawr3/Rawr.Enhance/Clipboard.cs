using System;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rawr.Enhance
{
    // code supplied by http://www.jeff.wilcox.name/2008/05/clipboard-access/
    // clipboard flash from http://code.google.com/p/syntaxhighlighter/
     public static class Clipboard  
     {
         private static bool success = false;
         public static bool Success { get { return success; } }
         const string HostNoClipboard = "The clipboard isn't available in the current host.";  
         const string ClipboardFailure = "The text couldn't be copied into the clipboard.";  
         const string BeforeFlashCopy = "The text will now attempt to be copied...";  
         const string FlashMimeType = "application/x-shockwave-flash";  
   
         // HARD-CODED!  
         const string ClipboardFlashMovie = "clipboard.swf";  
   
         /// <summary>  
         /// Write to the clipboard (IE and/or Flash)  
         /// </summary>  
         public static void SetText(string text)  
         {  
             // document.window.clipboardData.setData(format, data);  
             var clipboardData = (ScriptObject)HtmlPage.Window.GetProperty("clipboardData");  
             if (clipboardData != null) {  
                 success = (bool)clipboardData.Invoke("setData", "text", text);  
                 if (!success) {  
                     HtmlPage.Window.Alert(ClipboardFailure);  
                 }  
             }  
             else {  
                 HtmlPage.Window.Alert(BeforeFlashCopy);  
   
                 // Append a Flash embed element with the data encoded  
                 string safeText = HttpUtility.UrlEncode(text);  
                 var elem = HtmlPage.Document.CreateElement("div");  
                 HtmlPage.Document.Body.AppendChild(elem);  
                 elem.SetProperty("innerHTML", "<embed src=\"" +  
                     ClipboardFlashMovie + "\" " +  
                     "FlashVars=\"clipboard=" + safeText + "\" width=\"0\" " +  
                     "height=\"0\" type=\"" + FlashMimeType + "\"></embed>");  
             }  
         }  
     }  
 }  

