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
using System.IO;
using System.Windows.Markup;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;
using System.Threading;
using System.Timers;

namespace MapNavi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    
    public partial class MainWindow : Window
    {
        
        Map currentMap;
        Polyline polyLine = null; // Klasa Polyline pozwala na stworzenie lini na podstawie kilku punktów - idealna do wyświetlania znalezionej trasy
        
        List<Node> path;
        int i = 0; // Numer kolejnego miejsca dodawanego do mapy, na jego podstawie określana jest nazwa miejsca

        UniformCostSearch ucs;
        AStar aStar;
        


        public MainWindow()
        {
            InitializeComponent();

            currentMap = new Map();
            path = new List<Node>();
            ucs = new UniformCostSearch();
            aStar = new AStar();
            
            

        }
        
        private void mouseClick(object sender, MouseButtonEventArgs e)
        {
            
            CheckPath();

            // Pobiera miejsce ze współrzędnych kliknięcia, jeśli nie ma jest równy null
            Node location = currentMap.getNodeAtPoint((int)e.GetPosition(dataCanvas).X, (int)e.GetPosition(dataCanvas).Y);
            if ((bool)mode.IsChecked) // Jeśli tryb tworzenia jest włączony
            {
                
                if (location != null) // sprawdza czy kliknęliśmy miejsce czy puste pole
                {
                    if (currentMap.selectedLocation != null) // Jeśli już raz zaznaczyliśmy jakąś lokację czyli nie jest ona równa null
                    {
                        
                        if (!currentMap.selectedLocation.Equals(location)) // Oraz jeśli ta wcześniejsza lokacja nie jest tą samą którą zaznaczyliśmy teraz
                        {
                            foreach(Edge edge in currentMap.selectedLocation.Edges)
                            {
                                if (edge.LocB.Equals(location))
                                {
                                    return;
                                }
                            }

                            // Tworzy połączenie pomiędzy wcześniej wybraną i teraz zaznaczoną lokacją
                            DeselectNodes(location);

                            if ((bool)scale.IsChecked) // Koszt połączenia to albo wartość od użytkownika albo wyznaczona na podstawie prawdziwych odległości
                            {
                                currentMap.CreateEdges(int.Parse(costLabel.Content.ToString()), location, dataCanvas, wayMode.IsChecked.Value);
                            }
                            else
                                currentMap.CreateEdges(Heuristic.HeuristicValue(currentMap.selectedLocation, location), location, dataCanvas, wayMode.IsChecked.Value);

                        }
                        else // Jeśli lokację są takie same, odznacza je
                        {
                            DeselectNodes();
                            currentMap.selectedLocation = null;
                        }
                        
                    }
                    else // Jeśli wcześniej nie było wybranej lokacji to wybiera tą klikniętą
                    {
                        currentMap.SelectNode(location);
                    }
                }
                
            }
            else
            {
                if (location != null)
                {
                    if (currentMap.selectedLocation != null) // Jeśli tryb tw. jest wyłączony, kliknięcie nie jest równe null oraz już poprzednio zostało wybrane jedno miejsce uruchamiany jest algorytm
                    {
                        if (!location.Equals(currentMap.selectedLocation))
                        {

                            if (ASTAR.IsChecked.Value)
                                path = aStar.CostSearch(currentMap.selectedLocation, location, true);
                            else
                                path = aStar.CostSearch(currentMap.selectedLocation, location, false);


                            CheckPath();

                            DeselectNodes(location);


                            foreach (Node next in currentMap.places) // Po znalezieniu trasy resetuje wartości które były używane w algorytmie, umożliwiając ponowne jego uruchomienie
                            {
                                next.Ancestor = null;
                                next.TotalCost = 0;
                            }

                            location = null;
                            currentMap.selectedLocation = null;
                        }
                        else
                        {
                            DeselectNodes();
                            currentMap.selectedLocation = null;
                        }
                        

                    }
                    else
                        currentMap.SelectNode(location);
                }
            }
            
        }

        // Tworzy punkt na mapie pobierając współrzędne kliknięcia
        private void mouseRightClick(object sender, MouseButtonEventArgs e)
        {
            
            if ((bool)mode.IsChecked)
            {
                
                int x = (int)e.GetPosition(dataCanvas).X;
                int y = (int)e.GetPosition(dataCanvas).Y;
                currentMap.CreateNodes(x, y, "Punkt " + i.ToString(), dataCanvas, ref i);
                
            }
            
        }

        //*
        static void Delay(int ms, EventHandler action)
        {
            var tmp = new System.Windows.Forms.Timer { Interval = ms };
            tmp.Tick += new EventHandler((o, e) => tmp.Enabled = false);
            tmp.Tick += action;
            tmp.Enabled = true;
        }

        // Zapisywanie mapy - serializacja binarna, otwiera okno zapisu, strumień i serializuje
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {


            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Data Files|*.dat";

            bool? retval = dlg.ShowDialog();
            if (retval.HasValue && retval.Value)
            {
                using (FileStream localStream = new FileStream(dlg.FileName, FileMode.Create))
                {
                    BinaryFormatter binaryF = new BinaryFormatter();
                    binaryF.Serialize(localStream, currentMap.places);
                }
            }
        }
        

        // Wczytywanie zapisanej mapy - lekko skomplikowane z braku możliwości serializacji obiektów Windows.Shapes (elips i linii)
        // metoda opiera się (oprócz samego procesu serializacji, który zapisuje w tym wypadku jedynie listę miejsc na mapie wraz z właściwościami)
        // ponownym narysowaniu graficznych elementów na podstawie właściwości zserializowanych obiektów
        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            string file = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            

            if (openFileDialog.ShowDialog() == true) //Otwiera okno dialogowe, czyści mapę
            {

                ClearMap();

                file = openFileDialog.FileName;

                currentMap = null;
                currentMap = new Map();

                try
                {
                    // Otwiera strumień, w wygodnej instrukcji using która po wyjściu z jej zakresu zamyka strumień, działa również
                    // z klasami implementującymi IDisposible i po wyjściu z jej zakresu jak można się domyśleć wywołuje metodę Dispose()
                    using (FileStream fls = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        List<Node> newPlaces = new List<Node>();
                        BinaryFormatter binaryFormatter = new BinaryFormatter(); // Wygodna serializacja binarna
                        newPlaces = (List<Node>)binaryFormatter.Deserialize(fls);

                        if (currentMap.places.Count != 0)
                            currentMap.places.Clear();

                        currentMap.places.AddRange(newPlaces);

                        Line tempLine;

                        foreach (Node n in currentMap.places)
                        {
                            GUI.CreateNode(n, dataCanvas);

                            // Pobiera aktualny numer miejsca z nazwy node'a
                            // aby po wczytaniu mapy kolejne miejsca miały kolejne numery a nie zaczynały się od 0
                            int maxI = int.Parse(n.Name.Split()[1]);
                            if (maxI > i)
                                i = maxI;



                            foreach (Edge edge in n.Edges)
                            {
                                if (!edge.isDrawn)
                                {
                                    GUI.DrawEdge(edge.LocA, edge.LocB, dataCanvas, out tempLine, edge.twoWay);
                                    edge.graphicEdge = tempLine;
                                }
                            }
                        }
                        i++;

                    }
                }
                // Możliwość wystąpienia dużej ilości różnych wyjątków dlatego taka ogólna deklaracja Exception
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                

                
            }

            

        }


        // Przycisk wyznaczający trasę
        private void calcRoute_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (!String.IsNullOrEmpty(fromText.Text) && !String.IsNullOrEmpty(targetText.Text))
                {

                    path = Search(fromText.Text, targetText.Text);

                    CheckPath();
                }
                else
                {
                    MessageBox.Show("Aby wyznaczyć trasę wpisz potrzebne informacje");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CheckPath()
        {
            if (dataCanvas.Children.Contains(polyLine))
                dataCanvas.Children.Remove(polyLine);

            polyLine = null;


            if (path != null)
            {

                GUI.SelectRoute(path, dataCanvas, out polyLine);
                path = null;

            }
        } // Prosta metoda zapobiegająca powielaniu kodu

        private void DeselectNodes(Node loc = null)
        {
            if (loc != null)
                GUI.Deselect(loc);

            GUI.Deselect(currentMap.selectedLocation);
        }  // Prosta metoda zapobiegająca powielaniu kodu

        // Metoda lekko zmienia łańcuchy znaków, dopasowując wpisane nazwy do punktów na mapie i uruchamia algorytm wyszukiwania trasy
        private List<Node> Search(string from, string to)
        {
            Node start = null;
            Node end = null;
            foreach (Node n in currentMap.places)
            {
                if (n.Name.ToLower().Trim().Replace(" ", "") == from.ToLower().Trim().Replace(" ", ""))
                {
                    start = n;
                }

                if (n.Name.ToLower().Trim().Replace(" ", "") == to.ToLower().Trim().Replace(" ", ""))
                {
                    end = n;
                }
            }

            if (start != null && end != null && !start.Equals(end))
            {
                List<Node> route = new List<Node>();

                if (scale.IsChecked.Value)
                    route = aStar.CostSearch(start, end, true);
                else
                    route = aStar.CostSearch(start, end, false);



                foreach (Node next in currentMap.places)
                {
                    next.Ancestor = null;
                    next.TotalCost = 0;
                }

                return route;
            }
            else
            {
                MessageBox.Show("Nieprawidłowe miejsce/a");
                return null;
            }
        } 

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            ClearMap();
        }

        private void ClearMap()
        {
            currentMap = null;
            currentMap = new Map();
            i = 0;

            dataCanvas.Children.Clear();
        }

        private void scale_Checked(object sender, RoutedEventArgs e)
        {
            slider.Visibility = Visibility.Visible;
            costLabel.Visibility = Visibility.Visible;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            costLabel.Content = (int)e.NewValue;
        }

        private void scale_Unchecked(object sender, RoutedEventArgs e)
        {
            slider.Visibility = Visibility.Hidden;
            costLabel.Visibility = Visibility.Hidden;
        }

    }
}
