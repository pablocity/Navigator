using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MapNavi
{
    // Written by Paweł Woliński in July 2016
    class AStar
    {

        PriorityQueue costQueue;
        List<Node> closedList;
        int tempCost;


        public List<Node> CostSearch(Node startPoint, Node goalPoint, bool useHeuristic)
        {

            if (startPoint.Successors.Count == 0)
                return null;

            costQueue = new PriorityQueue();
            closedList = new List<Node>();
            costQueue.Enqueue(startPoint);
            costQueue[0].Ancestor = new Node("Beginner");

            while (costQueue.Count != 0)
            {

                Node current = costQueue.Dequeue();
                closedList.Add(current);

                if (current.Equals(goalPoint))
                {

                    Node beginning = new Node(" ");
                    Queue<Node> myRoute = new Queue<Node>();
                    List<Node> route = new List<Node>();

                    myRoute.Enqueue(current);
                    route.Add(current);

                    while (!beginning.Equals(startPoint))
                    {
                        beginning = myRoute.Dequeue().Ancestor;
                        if (beginning == null)
                            break;
                        myRoute.Enqueue(beginning);
                        route.Add(beginning);
                    }
                    costQueue = null;
                    return route;
                }

                // Algorytm uruchamiany jest synchronicznie gdyż udało mi się go na tyle zoptymalizować że nawet przy większych mapach
                // czas wyszukiwania jest praktycznie niezauważalny, zostawiłem jednak na przyszłość prostą możliwość uruchamiania go asynchronicznie,
                // w nnowym wątku z wykorzystaniem słów kluczowych async i await

                // await Task.Run(() => 
                //{
                for (int i = 0; i < current.Successors.Count; i++)
                {

                    tempCost = 0;

                    if (!closedList.Contains(current.Successors[i]))
                    {

                        if (!costQueue.Contains(current.Successors[i]))
                        {
                            costQueue.Enqueue(current.Successors[i]);
                            current.Successors[i].Ancestor = current;

                            foreach (Edge e in current.Successors[i].Ancestor.Edges)
                            {
                                if (e.LocB == current.Successors[i])
                                {
                                    tempCost = e.Cost;
                                    break;
                                }
                            }

                            current.Successors[i].TotalCost = current.TotalCost + tempCost;
                            current.Successors[i].FullValue = current.Successors[i].TotalCost
                                + Heuristic.HeuristicValue(current.Successors[i], goalPoint);
                            if (!useHeuristic)
                                current.Successors[i].FullValue = current.Successors[i].TotalCost;
                        }
                        else
                        {
                            Node potentiallyBetterNeighbor = CloneNode(current.Successors[i]);
                            potentiallyBetterNeighbor.Ancestor = current;

                            foreach (Edge e in potentiallyBetterNeighbor.Ancestor.Edges)
                            {
                                if (e.LocB == current.Successors[i])
                                {
                                    tempCost = e.Cost;
                                    break;
                                }
                            }

                            potentiallyBetterNeighbor.TotalCost = current.TotalCost + tempCost;

                            if (potentiallyBetterNeighbor.TotalCost < current.Successors[i].TotalCost)
                            {
                                current.Successors[i].Ancestor = potentiallyBetterNeighbor.Ancestor;
                                current.Successors[i].TotalCost = potentiallyBetterNeighbor.TotalCost;
                                current.Successors[i].FullValue = current.Successors[i].TotalCost
                                    + Heuristic.HeuristicValue(current.Successors[i], goalPoint);
                                if (!useHeuristic)
                                    current.Successors[i].FullValue = current.Successors[i].TotalCost;
                            }
                        }

                        
                    }

                    costQueue.CheckPriority();
                }
                //});

            }

            return null;
        }

        // Metoda do głębokiego klonowania obiektu - tworzy nowy obiekt i ustawia go na wartość obiektu deserializowanego
        // dzięki czemu mamy osobny obiekt o tych samych właściwościach a nie tylko nową referencję, co nie sprawdziłoby się w algorytmie,
        // pomysł na taki sposób klonowania, chyba najkrótszy jaki widziałem zawdzięczam użytkownikowi ze StackOverflow niestety nie pamiętam jego nazwy,
        public Node CloneNode(Node toClone)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, toClone);
                ms.Seek(0, SeekOrigin.Begin);
                Object clone = bf.Deserialize(ms);
                return (Node)clone;
            }
        }
    }
}
