using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MapNavi
{
    // Written by Paweł Woliński in June 2016


    
        //Uniform cost search

    class UniformCostSearch
    {
        PriorityQueue costQueue;
        int tempCost;

        public List<Node> CostSearch(Node startPoint, Node goalPoint)
        {

            if (goalPoint.Successors.Count == 0 && startPoint.Successors.Count == 0)
                return null;

            costQueue = new PriorityQueue();
            costQueue.Enqueue(startPoint);
            costQueue[0].Ancestor = new Node("Beginner");

            while (costQueue.Count != 0)
            {

                Node current = costQueue.Dequeue();

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


               // await Task.Run(() => //Wyłączyć przyciski wczytywania/zapisywania kiedy liczona jest droga
                //{
                    for (int i = 0; i < current.Successors.Count; i++)
                    {


                        tempCost = 0;
                        if (current.Successors[i].Ancestor == null)
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
                        }
                        else
                        {
                            Node clones = CloneNode(current.Successors[i]);

                            costQueue.Enqueue(clones);

                            if (clones.Equals(current.Ancestor) || current.Successors[i].Equals(startPoint))
                            {
                                costQueue.Remove(clones);
                                clones = null;
                            }
                            else
                            {
                                clones.Ancestor = current;

                                if (costQueue.Contains(clones))
                                {
                                    foreach (Edge e in clones.Ancestor.Edges)
                                    {
                                        if (e.LocB == current.Successors[i])
                                        {
                                            tempCost = e.Cost;
                                            break;
                                        }
                                    }

                                    clones.TotalCost = current.TotalCost + tempCost;

                                }
                            }
                        }

                        costQueue.CheckPriority();
                    }
                //});

            }

            return null;
        }


        // Metoda do głębokiego klonowania obiektu
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
