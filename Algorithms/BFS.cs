using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapNavi
{
    // Written by Paweł Woliński in June 2016
    //*
    // Prosty sposób obliczania drogi tylko na podstawie liczby połączeń niezależnie od kosztu (odległości) każdego z nich
    class BFS
    {
        Queue<Node> queue = new Queue<Node>();
        int currentLevel = 0;

        Queue<Node> routeQueue = new Queue<Node>();
        List<string> route = new List<string>();

        public Dictionary<string, int> Search(Node startPoint, Node endPoint)
        {
            Console.WriteLine("Punkt początkowy to " + startPoint.Name + "\n\nCel to " + endPoint.Name + "\n");

            queue.Enqueue(startPoint);

            while (queue.Count != 0)
            {
                Node current = queue.Dequeue();
                if (current == endPoint)
                {
                    Dictionary<string, int> goal = new Dictionary<string, int>();
                    goal.Add(current.Name, current.Distance);
                    return goal;
                }
                currentLevel = current.Distance + 1;

                for (int i = 0; i < current.Successors.Count; i++)
                {
                    if (current.Successors[i].Distance == 0)
                    {
                        queue.Enqueue(current.Successors[i]);
                        current.Successors[i].Distance = currentLevel;
                        current.Successors[i].Ancestor = current;
                    }
                }
            }

            return new Dictionary<string, int>(); 
            
        }

        public List<string> GetRoute(Node endPoint)
        {
            routeQueue.Enqueue(endPoint);
            route.Add(endPoint.Name);

            for (int i = 0; i < endPoint.Distance; i++)
            {
                Node ancestor = routeQueue.Dequeue().Ancestor;
                route.Add(ancestor.Name);
                routeQueue.Enqueue(ancestor);
            }

            return route;
        }


        public void ShowOutput(Dictionary<string, int> output)
        {
            if (output != null)
            {
                foreach (string n in output.Keys)
                    Console.WriteLine("Dotarłeś do " + n + "\n");
                foreach (int i in output.Values)
                    Console.WriteLine("Liczba połączeń do celu: " + i + "\n");
            }
        }
    }
}
