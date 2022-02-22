using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Wpf_UgraloGomb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    

    public partial class MainWindow : Window
    {
        /// <summary> 
        /// Az elért pontszám. 
        /// </summary> 
        private int eredmeny;
        /// <summary> 
        /// Véletlenszámok előállítására szolgáló objektum. 
        /// </summary> 
        private Random veletlen;
        private DateTime kezdoIdo;
        private int maxJatekIdo;
        private DispatcherTimer dtIdozito;
        private bool ervenyes;

        public MainWindow()
        {
            InitializeComponent();
            dtIdozito = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 500),
                IsEnabled = false
            };

            dtIdozito.Tick += DtIdozito_Tick;
            // Alsó és felső határérték ezredmásodpercben arra, hogy mennyi 
            // ideig maradhat egy helyben a mozgó nyomógomb. 
            slCsuszka.Minimum = 100;
            slCsuszka.Maximum = 1500; 
            //A csúszka jelzővonalainak távolsága. 
            slCsuszka.TickFrequency = 200; 
            // Mekkora elmozdulást jelent a csúszkán a le/fel nyíl //billentyű lenyomása? 
            slCsuszka.SmallChange = 100;
            // Mekkora elmozdulást jelent a csúszkán a Page Up/Page Down // billentyű lenyomása? 
            slCsuszka.LargeChange = 500;
            // A csúszka kezdeti pozíciója. 
            slCsuszka.Value = 500;
            // A csúszka bal és jobb oldali címkének (feliratok) szövege. 
            llMin.Content = slCsuszka.Minimum + " ms";
            llMax.Content = slCsuszka.Maximum + " ms";
            // A mozgó gomb kezdetben letiltva. 
            btKapjEl.IsEnabled = false;
            // A megengedett játékidő másodpercben. 
            maxJatekIdo = 10;
            // Végrehajtásjelző szélsőértékeihez társított számértékek. 
            pbVegrehajtasJelzo.Minimum = 0;
            // A játékidő másodpercben. 
            pbVegrehajtasJelzo.Maximum = maxJatekIdo;
            // Végrehajtásjelző kezdőértéke. 
            pbVegrehajtasJelzo.Value = 0;
            // Véletlenszámokat előállító objektum létrehozása. 
            veletlen = new Random();
        }

        private void FeliratKiir()
        {
            Title = string.Format("Találatok: {0}, Időzítés: {1,7:F2} ms, Még hátravan: {2,5:F2} s", eredmeny, slCsuszka.Value, Math.Max(0, maxJatekIdo - ElteltIdo()));
        }

        double ElteltIdo()
        {
            // Lekérdezzük az aktuális időt. 
            DateTime most = DateTime.Now;
            // A játék kezdete óta eltelt idő. 
            return most.Subtract(kezdoIdo).TotalSeconds;
        }

        private void DtIdozito_Tick(object sender, EventArgs e)
        {
            FeliratKiir();
            pbVegrehajtasJelzo.Value = ElteltIdo();
            if (ElteltIdo() < maxJatekIdo)
            {
                // A gomb bal felső sarkának új x koordinátája. 
                btKapjEl.SetValue(LeftProperty, veletlen.NextDouble() * (cvLap.ActualWidth - btKapjEl.ActualWidth));
                // A gomb bal felső sarkának új y koordinátája. 
                btKapjEl.SetValue(TopProperty, veletlen.NextDouble() * (cvLap.ActualHeight - btKapjEl.ActualHeight));
                //Ha ezt bekapcsoljuk akkor újra engedélyezi a gombot amennyiben azt letiltjuk a btKapjEl_Click eseményben.
                //Így kivédhető az, hogy egy ütemben több kattintás érkezzen a gombra így növelve az ereményt 
                // btKapjEl.IsEnabled = true;
            }
            else
            {
                // Ha lejárt a játékidő. 
                // Időzítő leállítása. 
                dtIdozito.IsEnabled = false;
                // Start gomb engedélyezése. 
                btStart.IsEnabled = true;
                // Mozgó nyomógomb letiltása. 
                btKapjEl.IsEnabled = false;
            }
        }



        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            // Lenullázzuk az eredményt. 
            eredmeny = 0;
            // Játékidő előlről kezdődik. 
            kezdoIdo = DateTime.Now;
            // Végrehajtásjelző a kezdő pozícióba. 
            pbVegrehajtasJelzo.Value = 0;
            // Az időzítő beállítása a csúszka állása alapján. 
            dtIdozito.Interval = new TimeSpan(0, 0, 0, 0, (int)slCsuszka.Value);
            // Start gomb letiltása. 
            btStart.IsEnabled = false;
            // Időzítő indítása. 
            dtIdozito.IsEnabled = true;
            // Eredmény megjelenítése az ablak fejlécében. 
            FeliratKiir();
            // Kapj el nyomógomb engedély 
            btKapjEl.IsEnabled = true;
        }

        private void btKapjEl_Click(object sender, RoutedEventArgs e)
        {
            if (!ervenyes) return;
            eredmeny++;
            FeliratKiir();
           //Ha ezt bekapcsoljuk akkor addig letiltja a gombot míg az új pozicióba nem kerül
           // btKapjEl.IsEnabled = false;
        }

        private void btKapjEl_MouseEnter(object sender, MouseEventArgs e)
        {
            //a kattintás érvényes területen történt-e
            ervenyes = true;
        }

        private void btKapjEl_MouseLeave(object sender, MouseEventArgs e)
        {
            //Nem a gombon kattintottunk
            ervenyes = false;
        }

        private void slCsuszka_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Tároljuk, hogy most folyik-e játék. 
            bool vanJatek = dtIdozito.IsEnabled;
            // Időzítő letiltása az intervallum módosítás miatt. 
            dtIdozito.IsEnabled = false;
            // Időzítő intervallumának beállítása. 
            dtIdozito.Interval = new TimeSpan(0, 0, 0, 0, (int)slCsuszka.Value);
            // A játék közben mozgatja a felhasználó a csúszkát, akkor
            if (vanJatek) dtIdozito.IsEnabled = true;
            // Eredmény megjelenítése az ablak fejlécében. 
            FeliratKiir();
        }
    }
}
