/*
 * Module:      RandomList.cs
 * Author:      T. Haworth
 * Date:        January 22, 2009
 * Description: The RandomList<> class extends 
 *              System.Collections.Generic.List<> by adding the
 *              Shuffle() method for randomizing the sequence
 *              of the collection's elements.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public class RandomList<T> : List<T>
    {
        public RandomList()
        {
        }

        public RandomList(int capacity) : base(capacity)
        {
        }

        public RandomList(IEnumerable<T> c) : base(c)
        {
        }

        public void Shuffle()
        {
            Random rand = new Random();
            T hold;
            int index;
            for (int i = 0; i < this.Count; i++)
            {
                // Choose a random index
                index = rand.Next(this.Count);

                // Swap the elements at indexes i and index
                hold = this[i];
                this[i] = this[index];
                this[index] = hold;
            }
        }
    } // end class RandomList<>
}
