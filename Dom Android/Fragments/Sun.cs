using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Dom_android
{
    class Sun
    {
        public Sun()
        {
            Calculate(DateTime.Now, 0, 5);
            Calculate(DateTime.Now, 1, 3);
        }

        public int[] Wschod
        {
            get
            {
                return _Wschod;
            }
        }

        public int[] Zachod
        {
            get
            {
                return _Zachod;
            }
        }

        int[] _Wschod = new int[2];
        int[] _Zachod = new int[2];


        double longitude = 50.061389;
        double latitude = 19.938333;

        double D2R = Math.PI / 180;
        double R2D = 180 / Math.PI;



        public void Calculate(DateTime date, int set_rise, double offset)
        {
            //Convert longitude into hour value
            double long_hour = longitude / 15;
            double t = 0;

            //sunset = 0, sunrise = 1
            //calculate approximate time
            if (set_rise == 1)
            {
                t = Convert.ToDouble(date.DayOfYear) + ((6 - long_hour) / 24);
            }
            else if (set_rise == 0)
            {
                t = Convert.ToDouble(date.DayOfYear) + ((18 - long_hour) / 24);
            }

            //Calculate Sun's mean anomaly time
            double mean = (0.9856 * t) - 3.289;

            //Calculate Sun's true longitude
            double sun_true_long = mean + (1.916 * Math.Sin(mean * D2R)) + (0.020 * Math.Sin(2 * mean * D2R)) + 282.634;
            if (sun_true_long > 360)
                sun_true_long = sun_true_long - 360;
            else if (sun_true_long < 0)
                sun_true_long = sun_true_long + 360;

            //Calculate Sun's right ascension
            double right_ascension = R2D * Math.Atan(0.91764 * Math.Tan(sun_true_long * D2R));
            if (right_ascension > 360)
                right_ascension = right_ascension - 360;
            else if (right_ascension < 0)
                right_ascension = right_ascension + 360;

            //Adjust right ascension value to be in the same quadrant as Sun's true longitude
            double Lquadrant = (Math.Floor(sun_true_long / 90)) * 90;
            double RAquadrant = (Math.Floor(right_ascension / 90)) * 90;
            right_ascension = right_ascension + (Lquadrant - RAquadrant);

            //Convert right ascension value into hours
            right_ascension = right_ascension / 15;

            //Calculate Sun's declination
            double sinDec = 0.39782 * Math.Sin(sun_true_long * D2R);
            double cosDec = Math.Cos(Math.Asin(sinDec));

            //Setting Sun's zenith value
            double zenith = 90 + (50 / 60);

            //Calculate Sun's local hour angle
            double cosH = (Math.Cos(zenith * D2R) - (sinDec * Math.Sin(latitude * D2R))) / (cosDec * Math.Cos(latitude * D2R));

            if (cosH > 1)
            {
                Console.Write("Sun never rises on this day. " + date.Year + "/" + date.Month + "/" + date.Day + "<br />");
            }
            else if (cosH < -1)
            {
                Console.Write("Sun never sets on this day. " + date.Year + "/" + date.Month + "/" + date.Day + "<br />");
            }

            //Calculate and convert into hour of sunset or sunrise
            double hour = 0;
            if (set_rise == 1)
            {
                hour = 360 - R2D * Math.Acos(cosH);
            }
            else if (set_rise == 0)
            {
                hour = R2D * Math.Acos(cosH);
            }

            hour = hour / 15;

            //Calculate local mean time of rising or setting
            double local_mean_time = hour + right_ascension - (0.06571 * t) - 6.622;

            //Adjust time to UTC
            double utc = local_mean_time - long_hour;

            //Convert time from UTC to local time zone
            double local_time = utc + offset;
            if (local_time > 24)
                local_time = local_time - 24;
            else if (local_time < 0)
                local_time = local_time + 24;

            //Convert the local_time into time format
            int s_hour = Convert.ToInt32(Math.Floor(local_time));
            int s_minute = Convert.ToInt32((local_time - s_hour) * 60);

            if (set_rise == 1)
            {
                _Wschod[0] = s_hour;
                _Wschod[1] = s_minute;

            }
            else if (set_rise == 0)
            {
                _Zachod[0] = s_hour;
                _Zachod[1] = s_minute;
            }
        }
    }
}