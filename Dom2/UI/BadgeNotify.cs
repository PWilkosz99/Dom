using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Dom
{
    public enum BadgeValue
    {
        none = 1,
        activity = 2,
        alarm = 3,
        alert = 4,
        attention = 5,
        available = 6,
        away = 7,
        busy = 8,
        error = 9,
        newMessage = 10,
        paused  = 11,
        playing = 12,
        unavailable = 13,
    }

    class  BadgeNotify
    {
        static private int lastnr = 0;

        static private String[] SBadgeValue = new String[] { "null", "none", "activity", "alarm", "alert","attention","available","away","busy","error","newMessage","paused","playing","unavailable"};

        static public void Set(BadgeValue value)
        {  
            // Get the blank badge XML payload for a badge glyph
            XmlDocument badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeGlyph);

            // Set the value of the badge in the XML to our glyph value
            XmlElement badgeElement = badgeXml.SelectSingleNode("/badge") as XmlElement;
            badgeElement.SetAttribute("value", SBadgeValue[(int) value ]);

            // Create the badge notification
            BadgeNotification badge = new BadgeNotification(badgeXml);

            // Create the badge updater for the application
            BadgeUpdater badgeUpdater = BadgeUpdateManager.CreateBadgeUpdaterForApplication();

            // And update the badge
            badgeUpdater.Update(badge);

            if( (int) value == 1)
            {
                if (lastnr != 0)
                {
                    Set(lastnr);
                }
            }
        }

        static public void Set(int Number)
        {

            // Get the blank badge XML payload for a badge number
            XmlDocument badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);

            // Set the value of the badge in the XML to our number
            XmlElement badgeElement = badgeXml.SelectSingleNode("/badge") as XmlElement;
            badgeElement.SetAttribute("value", Number.ToString());

            // Create the badge notification
            BadgeNotification badge = new BadgeNotification(badgeXml);

            // Create the badge updater for the application
            BadgeUpdater badgeUpdater = BadgeUpdateManager.CreateBadgeUpdaterForApplication();

            // And update the badge
            badgeUpdater.Update(badge);

            lastnr = Number;

        }

        static public void Clear()
        {
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
            Windows.Storage.ApplicationData.Current.RoamingSettings.Values["BadgeIndex"] = null;
        }
    }
}
