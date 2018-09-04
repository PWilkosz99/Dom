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
using System.Threading;

namespace Dom_android
{
    public class SimsonMieszanka : Android.Support.V4.App.Fragment, NumberPicker.IOnValueChangeListener
    {
        NumberPicker Prop1;
        NumberPicker Prop2;

        NumberPicker ilosc;
        NumberPicker material;

        TextView result;

        string[] iloscValues = new[] { "10ml", "20ml", "25ml", "40ml", "50ml", "75ml", "100ml", "200ml", "250ml", "500ml", "750ml", "1L", "2L", "2,5L","3L", "4L", "5L", "10L" };
        string[] Prop2Values = new[] { "10", "20", "25", "30", "35", "40", "45", "50" };

        public static string Nazwa { get { return "Mieszanka"; } }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.SimsonMieszanka, container, false);
            Prop1 = view.FindViewById<NumberPicker>(Resource.Id.numberPicker1);
            Prop2 = view.FindViewById<NumberPicker>(Resource.Id.numberPicker2);
            ilosc = view.FindViewById<NumberPicker>(Resource.Id.numberPicker3);
            material = view.FindViewById<NumberPicker>(Resource.Id.numberPicker4);
            result = view.FindViewById<TextView>(Resource.Id.result);

            Prop1.SetDisplayedValues(new[] { "1" });
            Prop2.SetDisplayedValues(Prop2Values);

            Prop1.MinValue = Prop2.MinValue = material.MinValue = ilosc.MinValue = 0;

            Prop1.MaxValue = 0;
            Prop2.MaxValue = 7;

            material.SetDisplayedValues(new[] {"Benzyny","Oleju" });
            material.MaxValue = 1;

            ilosc.SetDisplayedValues(iloscValues);
            Prop2.Value = 7;
            ilosc.MaxValue = iloscValues.Length - 1;

            Prop2.SetOnValueChangedListener(this);
            ilosc.SetOnValueChangedListener(this);
            material.SetOnValueChangedListener(this);

            OnValueChange(null, 0, 0);

            return view;
        }

        public void OnValueChange(NumberPicker picker, int oldVal, int newVal)
        {
            new Thread(() =>
           {

               double IProp2 = int.Parse(Prop2Values[Prop2.Value]);
               double IIlosc = 0;

               String tmp = iloscValues[ilosc.Value];
               char[] tmp2 = new char[10];


               if (tmp[tmp.Length - 1] == 'L')
               {
                   tmp.CopyTo(0, tmp2, 0, (tmp.Length - 1));
                   IIlosc = 1000*double.Parse(new string(tmp2));
               }
               else if (tmp[tmp.Length - 1] == 'l')
               {
                   tmp.CopyTo(0, tmp2, 0, (tmp.Length - 2));
                   IIlosc = double.Parse(new string(tmp2));
               }

               String result;
               double res =0;
               bool benzyna = false;

              if(material.Value == 0) // jak benzyny to ile oleju
               {
                   res = IIlosc / IProp2;
                   benzyna = true;
               }
              if(material.Value == 1) // jak oleju to ile benzyny
               {
                   res = IIlosc * IProp2;
                   benzyna = false;
               }

              if(res >= 1000)  // wieksza jednostka
               {
                   res = res / 1000;
                   result = res + "L ";
               }
               else  // jak wystarczy ml
               {
                   res = Math.Round(res, 2);
                   result = res + "ml ";
               }

              if(benzyna)
               {
                   result += "oleju";
               }
              else
               {
                   result += "benzyny";
               }

               Activity.RunOnUiThread(() => this.result.Text = result);

           }).Start();
        }

    }
}