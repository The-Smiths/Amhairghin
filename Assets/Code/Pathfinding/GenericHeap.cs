using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericHeap<T> where T : IGenericHeapItem<T>
{
    public int Count { get { return _itemCount; } }

    private T[] _items;
    private int _itemCount;

    public GenericHeap(int maxSize)
    {
        _items = new T[maxSize];
    }

    public void Add(T item, bool intelligent)
    {
        item.index = _itemCount;
        _items[_itemCount] = item;

        SortUp(item, intelligent);
        _itemCount++;
    }

    public T RemoveFirst()
    {
        T item = _items[0];
        _itemCount--;

        _items[0] = _items[_itemCount];
        _items[0].index = 0;

        SortDown(_items[0]);
        return item;
    }

    public void UpdateItem(T item, bool intelligent)
    {
        SortUp(item, intelligent);
    }

    public bool Contains(T item)
    {
        return Equals(_items[item.index], item);
    }

    private void SortUp(T item, bool intelligent)
    {
        int parentIndex = (item.index - 1) / 2;

        while (true)
        {
            T parent = _items[parentIndex];

            if (item.CompareTo(parent) > 0)
            {
                Swap(item, parent);
            }
            else
            {
                break;
            }

            if (intelligent)
            {
                parentIndex = (item.index - 1) / 2;
            }
        }
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int childA = item.index * 2 + 1;
            int childB = item.index * 2 + 2;
            int swapIndex = 0;

            if (childA < _itemCount)
            {
                swapIndex = childA;
                if (childB < _itemCount && _items[childA].CompareTo(_items[childB]) < 0 )
                {
                    swapIndex = childB;
                }

                if (item.CompareTo(_items[swapIndex]) < 0)
                {
                    Swap(item, _items[swapIndex]);
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }

    private void Swap(T a, T b)
    {
        _items[a.index] = b;
        _items[b.index] = a;

        int temp = a.index;
        a.index = b.index;
        b.index = temp;
    }
}

public interface IGenericHeapItem<T> : IComparable<T>
{
   int index { get; set; }
}