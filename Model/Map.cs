using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace MapNavi
{

    // Klasa reprezentuje obiekt mapy

    class Map
    {
        
        public List<Node> places; // Lista wszystkich miejsc (node'ów) na mapie

        public Node selectedLocation = null;

        public Map()
        {
            places = new List<Node>();
        }

        // Tworzy połączenie między miejscami i oddelegowuje zadanie narysowani ich do klasy GUI
        public void CreateEdges(int cost, Node secondLocation, Canvas c, bool twoWay)
        {
            if (secondLocation != null)
            {

                Line propEdge;


                Edge tempEdge = new Edge(selectedLocation, secondLocation, cost);
                tempEdge.twoWay = false; //
                selectedLocation.Successors.Add(secondLocation);
                selectedLocation.Edges.Add(tempEdge);

                if (twoWay)
                {
                    tempEdge.twoWay = true; //
                    tempEdge.isDrawn = true; // Zmienne służące do odtworzenia obrazu mapy podczas deserializacji
                    secondLocation.Successors.Add(selectedLocation);
                    secondLocation.Edges.Add(new Edge(secondLocation, selectedLocation, cost));

                }

                GUI.DrawEdge(secondLocation, selectedLocation, c, out propEdge, twoWay);

                Edge temporaryEdge = new Edge(secondLocation, selectedLocation, cost);
                temporaryEdge.graphicEdge = propEdge;


                secondLocation = null;
                selectedLocation = null;

            }
            
        }


        // Tworzy wierzchołek jeśli nie wykracza po za konkretny obszar (eliminuje możliwość dodania wierzchołka, który jest w połowie w ścianie)
        //Współrzędne wpisane na sztywno ale spokojnie można by zamiast nich pobrać x, y konkretnych ścian i na nich operować,
        //w tym wypadku niepotrzebne bo nie ma możliwości rozszerzania okna, oraz nie pozwala na nachodzenie na siebie miejsc
        public void CreateNodes(int x, int y, string name, Canvas canvas, ref int nodeNumber)
        {

            if (x >= 15 && y >= 15 && y <= 387 && x <= 715)
            {
                foreach(Node n in places)
                {
                    if (n.X + 40 < x || n.X - 40 > x)
                    {
                        continue;
                    }
                    else
                    {
                        if (n.Y - 40 > y || n.Y + 40 < y)
                            continue;
                        else
                            return;
                    }
                }

                nodeNumber++;
                Node node = new Node(name);

                node.X = x;
                node.Y = y;

                this.places.Add(node);

                GUI.CreateNode(node, canvas);
            }
            else
                return;

            
        }


        // Zaznacza konkretne miejsce
        public void SelectNode(Node toSelect)
        {
            if (toSelect != null)
            {
                selectedLocation = toSelect;

                GUI.SelectNode(toSelect);
            }
            

        }


        // Wyszukuje miejsce na mapie o podanych współrzędnych
        public Node getNodeAtPoint(int x, int y)
        {
            foreach (Node placeOnMap in places)
            {   

                if (x >= (placeOnMap.X - (placeOnMap.shape.ActualWidth / 2)) && x <= (placeOnMap.X + (placeOnMap.shape.ActualWidth/2))
                    && y >= (placeOnMap.Y - (placeOnMap.shape.ActualHeight / 2)) && y <= (placeOnMap.Y + (placeOnMap.shape.ActualHeight)-5))
                {
                    return placeOnMap;
                }
            }

            return null;
        }


    }
}
