using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;

namespace Exceptions
{
    public sealed class Item 
    { 
        /* ... */ 
    }

    public sealed class ShoppingCart
    {
        private List<Item> m_cart = new List<Item>();
        private Decimal m_totalCost = 0;

        public ShoppingCart()
        {
        }

        public void AddItem(Item item)
        {
            AddItemHelper(m_cart, item, ref m_totalCost);
        }

        private static void AddItemHelper(List<Item> m_cart, Item newItem, ref Decimal totalCost)
        {
            // Предусловия:
            Contract.Requires(newItem != null);
            Contract.Requires(Contract.ForAll(m_cart, s => s != newItem));

            // Постусловия:
            Contract.Ensures(Contract.Exists(m_cart, s => s == newItem));
            Contract.Ensures(totalCost >= Contract.OldValue(totalCost));
            Contract.EnsuresOnThrow<IOException>(totalCost == Contract.OldValue(totalCost));

            // Какие-то операции (способные сгенерировать IOException)
            m_cart.Add(newItem);
            totalCost += 1.00M;
            // почему-то срабатывает ложно AddItem, new ShoppingCart().AddItem(new Item());
        }

        // Инвариант
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(m_totalCost >= 0);
        }
    }
}