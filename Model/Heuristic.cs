using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapNavi
{
    public enum HeuristicType { Euclidian } // Typ heurystyki, narazie 1 najbardziej odpowiedni do tego rodzaju obliczeń


    // Heurystyka to informacja dla algorytmu jak daleko od celu się znajduje, znacznie obniża czas wyliczenia drogi dzięki temu, że
    // algorytm mając do wyboru kilka ścieżek zawsze wybierze tą bliżej celu, rzadko będzie się cofał gdyż będzie wiedział że oddala się od puinktu docelowego
    // Wykorzystany jest tutaj wzór na długość odcinka (matematyka :)) i w sumie tyle, jest więcej rodzajów heurystyki a każda powinna być dobrana do
    // konkretnego zastosowania
    class Heuristic
    {

        public static HeuristicType heuristic = HeuristicType.Euclidian;
        public static int HeuristicValue(Node current, Node end)
        {
            switch (heuristic)
            {
                case HeuristicType.Euclidian:
                    return (int)Math.Sqrt(Math.Pow(end.X - current.X, 2) + Math.Pow(end.Y - current.Y, 2));
                default:
                    return (int)Math.Sqrt(Math.Pow(end.X - current.X, 2) + Math.Pow(end.Y - current.Y, 2));
            }
        }
    }
}
