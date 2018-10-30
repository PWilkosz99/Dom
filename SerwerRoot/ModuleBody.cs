using SerwerRoot.Podzespoły;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;


namespace SerwerRoot
{
    public class ModuleBody
    {

        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public bool On { get { return on; } set { localSettings.Values[Id.ToString()] = value; on = value; Change(on); } }

        private bool on;

        /// <summary>
        /// Nazwa modułu
        /// </summary>
        App.ModulesId Id;
        /// <summary>
        /// Tytuł w UI
        /// </summary>
        public string Title;

        public ModuleBody()
        {
            Object value = localSettings.Values[Id.ToString()];
            if (value == null)
            {
                localSettings.Values[Id.ToString()] = true;
            }
            else
            {
                try
                {
                    on = (bool)value;
                    Change(on);
                }
                catch
                {

                }
            }
        }

        private void Change(bool on)
        {
            if(on)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        public virtual void Start()
        {
            Log.Write("Moduł " + Title + " rozpoczął pracę", true);          
        }

        public virtual void Stop()
        {
            Log.Write("Moduł " + Title + " zakończył pracę",true);
        }

        public override string ToString()
        {
            return Title;
        }

    }
}
