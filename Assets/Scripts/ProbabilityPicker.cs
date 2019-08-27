using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ProbabilityPicker<T> {

    private Dictionary<T, int> items;
    private Random rand;

    /// <summary>
    /// Constructor
    /// </summary>
    public ProbabilityPicker() {
        items = new Dictionary<T, int>();
        rand = new Random();
    }


    /// <summary>
    /// Adds a new 'item' with some weight
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <param name="weight">Wheight</param>
    public void Add(T item, int weight) {
        items[item] = weight;
    }


    /// <summary>
    /// Picks a random item
    /// </summary>
    /// <returns>Item picked</returns>
    public T Pick() {
        List<T> l = new List<T>();
        foreach (var keyPair in items)
            for (int i = 0; i < keyPair.Value; i++)
                l.Add(keyPair.Key);
        return l[rand.Next(l.Count)];
    }
}
