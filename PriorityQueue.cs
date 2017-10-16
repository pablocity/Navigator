using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;


namespace MapNavi
{

    // Klasa reprezentująca własną kolekcję, potrzebna przy algorytmie wyszukiwania drogi
    class PriorityQueue : IEnumerable // Pozwala wyliczać elementy kolekcji w pętli foreach
    {


        private List<Node> priorityQueue;
        

        // Indekser - dzięki niemu można uzyskać dostęp do listy poprzez indeks instancji PriorityQueue
        public Node this[int index]
        {
            get { return priorityQueue[index]; } 
        }

        public int Count { get { return priorityQueue.Count; } }

        public PriorityQueue()
        {
            priorityQueue = new List<Node>();
        }


        public void Enqueue(Node element)
        {
            priorityQueue.Add(element);

            CheckPriority();
        }

        
        public Node Dequeue()
        {
            Node edge = priorityQueue[0];
            priorityQueue.RemoveAt(0);
            return edge;
        }

        public void Remove(Node element)
        {
            if (priorityQueue.Contains(element))
            {
                priorityQueue.Remove(element);
                CheckPriority();
            }
        }

        public bool Contains(Node element)
        {
            if (priorityQueue.Contains(element))
                return true;
            else
                return false;
        }
        
        // Implementacja interfejsu IEnumerable, pozwala na wyliczanie w petli foreach
        IEnumerator IEnumerable.GetEnumerator()
        {
            return priorityQueue.GetEnumerator();
        }


        // Sortuje elemety w liście według podanego kryterium w tym wypadku według właściwości FullValue
        // LINQ bardzo upraszcza większość operacji na kolekcjach, tutaj sama operacja zajmuje tylko jedną linijkę
        public void CheckPriority()
        {
            var queue = priorityQueue.OrderBy(x => x.FullValue).ToList();
            
            priorityQueue.Clear();
            foreach (Node e in queue)
            {
                priorityQueue.Add(e);
            }
        }
    }
}
