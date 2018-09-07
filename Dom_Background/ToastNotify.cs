using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Dom_Background
{
    static class ToastNotify
    {
        public static void Send(String msg)
        {
            String xmlToastTemplate = "<toast launch=\"app-defined-string\">" +
             "<visual>" +
               "<binding template =\"ToastGeneric\">" +
                 "<text>Bram wjazdowa</text>" +
                 "<text>" +
                   msg +
                 "</text>" +
                  "<image placement=\"appLogoOverride\" hint-crop=\"circle\" src=\"Assets\\Square44x44Logo.altform-unplated_targetsize-256.png\"/> " +
               "</binding>" +
             "</visual>" +
           "</toast>";

            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(xmlToastTemplate);
            var Toastnotify = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(Toastnotify);
        }
    }
}
