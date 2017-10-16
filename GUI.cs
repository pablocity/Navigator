using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;

namespace MapNavi
{

    // Klasa reprezentująca graficzne przedstawienie map
    class GUI
    {

        // Rysuje połączenia na mapie
        public static void DrawEdge(Node beginning, Node end, Canvas canvas, out Line properEdge, bool twoWay)
        {
            Point start = new Point(beginning.X, beginning.Y);
            Point ended = new Point(end.X, end.Y);

            Line edge = new Line()
            {
                X1 = start.X,
                X2 = ended.X,
                Y1 = start.Y,
                Y2 = ended.Y,
                Stroke = Brushes.Black,
                IsHitTestVisible = false // Ta właściwość umożliwia przekazanie eventu do następnej kontrolki np. kliknięcie na linię a wykonanie eventu obiektu pod nią
                
            };

            if (!twoWay)
                edge.Stroke = Brushes.DarkCyan;

            properEdge = edge;
            canvas.Children.Insert(0, edge); // Dodanie linii na 0 miejsce w liście dzięki czemu nie jest widoczna nad punktami na mapie tylko pod nimi
            

        }

        public static void SelectNode(Node select)
        {
            if (select != null)
            {
                select.shape.Stroke = Brushes.Red;
            }
            
        }

        public static void Deselect(Node select)
        {
            if (select != null)
            {
                select.shape.Stroke = Brushes.Black;
            }
        }

        //Rysuje odpowiednią drogę przekazaną w parametrze
        public static void SelectRoute(List<Node> route, Canvas canvas, out Polyline line)
        {
            line = new Polyline();

            line.Stroke = Brushes.Red;
            line.StrokeThickness = 3;
            line.IsHitTestVisible = false;
            
            foreach (Node n in route)
            {
                line.Points.Add(new Point(n.X, n.Y));
            }

            canvas.Children.Insert(0, line);
        }


        // "Rysuje" textboxy do wyświetlania nazw miejsc
        public static void DrawName(int x, int y, Canvas canvas, Node sign)
        {
            TextBox name = new TextBox()
            {
                Width = 50,
                Height = 15,
                FontSize = 9,
                IsHitTestVisible = false
            };

            name.Text = sign.Name;

            canvas.Children.Add(name);

            Canvas.SetLeft(name, x - 20);
            Canvas.SetTop(name, y + 15);
        }


        // Tworzy graficzny punkt na mapie na podstawie przekazanych danych
        public static void CreateNode(Node n, Canvas canvas)
        {
            Point point = new Point(n.X, n.Y);

            Ellipse node = new Ellipse()
            {
                Width = 25,
                Height = 25,
                Fill = Brushes.CadetBlue,
                StrokeThickness = 2.5,
                Stroke = Brushes.Black
            };

            node.IsHitTestVisible = false;
            canvas.Children.Add(node);

            Canvas.SetLeft(node, point.X - 10);
            Canvas.SetTop(node, point.Y - 10);
            n.shape = node; // przypisuje stworzony obiekt (elipsę) do pola shape konkretnego miejsca, co pozwala na późniejsze zmienianie właściwości elipsy poprzez właśnie tą zmienną

            DrawName((int)point.X, (int)point.Y, canvas, n);
        }

        
    }
}
