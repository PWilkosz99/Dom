using SerwerRoot.Podzespoły;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace SerwerRoot
{
    public class ModuleBody
    {

        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public bool On { get { return on; } set { localSettings.Values[Id.ToString()] = value; on = value;/* Change(on)*/; } }

        Task task;

        private bool on;

        /// <summary>
        /// Nazwa modułu
        /// </summary>
        public App.ModulesId Id;
        /// <summary>
        /// Tytuł w UI
        /// </summary>
        public string Title;

        public CancellationTokenSource cancellationTokenSource;
        public CancellationToken cancellationToken;

        public void ModuleBodyWork()
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
                    Change((bool)value);
                }
                catch
                {

                }
            }
        }

        public void Change(bool on)
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            // Zmieniając wartość on można zmienić stan modułu przy uruchominiu debugowania
            On = on;
            if(on)
            {
                try
                {
                    if (task != null)
                    {
                        throw new Exception("Zadanie nie jest puste");
                    }
                    else
                    {
                        task = new Task(Start);
                        task.Start();
                    }                   
                }
                catch (Exception e)
                {
                    Log.Write(e.Message, true);
                }
            }
            else
            {
                if(task == null)
                {
                    return;
                }
                cancellationTokenSource.Cancel();
                task.Wait();
                task = null;
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
