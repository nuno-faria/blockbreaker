using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ProbabilityPicker<T> {

    private Dictionary<T, int> items;
    private Random rand;
    private List<T> bag;

    /// <summary>
    /// Constructor
    /// </summary>
    public ProbabilityPicker() {
        items = new Dictionary<T, int>();
        rand = new Random();
        bag = null;
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
        if (bag == null)
            CreateBag();
        return bag[rand.Next(bag.Count)];
    }


    /// <summary>
    /// Removes an item
    /// </summary>
    /// <param name="item">Item to remove</param>
    public void Remove(T item) {
        if (items.ContainsKey(item)) {
            items.Remove(item);
            CreateBag();
        }
    }


    private void CreateBag() {
        bag = new List<T>();
        foreach (var keyPair in items)
            for (int i = 0; i < keyPair.Value; i++)
                bag.Add(keyPair.Key);
    }
}
