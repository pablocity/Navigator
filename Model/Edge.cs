using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace MapNavi
{

    // Klasa reprezentująca połączenie na mapie
    [Serializable]
    public class Edge
    {

        public Node LocA;
        public Node LocB;

        public int Cost = 0;

        public bool isDrawn = false;

        public bool twoWay = true;

        [NonSerialized]
        public Line graphicEdge = null; // Jak w przypadku elipsy, brak możliwości serializacji utrudnia odtworzenie mapy

        public Edge(Node location_a, Node location_b, int cost)
        {
            LocA = location_a;
            LocB = location_b;

            Cost = cost;
        }
    }
}
