// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.MovingAverage
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System.Collections.Generic;
using System.Linq;

namespace ImprovedPublicTransport
{
  public class MovingAverage
  {
    private int _sampleLenght;
    private Queue<float> _items;

    public int SampleLenght
    {
      get
      {
        return this._sampleLenght;
      }
      set
      {
        if (this._sampleLenght == value)
          return;
        this._sampleLenght = value;
      }
    }

    public float Average
    {
      get
      {
        if (this._items.Count == 0)
          return 0.0f;
        lock (this._items)
          return this._items.Average();
      }
    }

    public MovingAverage()
      : this(10)
    {
    }

    public MovingAverage(int sampleLenght)
    {
      this._sampleLenght = sampleLenght;
      this._items = new Queue<float>(sampleLenght);
    }

    public MovingAverage(float[] array, int sampleLenght)
    {
      this._sampleLenght = sampleLenght;
      this._items = new Queue<float>((IEnumerable<float>) array);
    }

    public void Clear()
    {
      lock (this._items)
        this._items.Clear();
    }

    public void Push(float value)
    {
      lock (this._items)
      {
        if (this._items.Count == this._sampleLenght)
        {
          double num = (double) this._items.Dequeue();
        }
        this._items.Enqueue(value);
      }
    }

    public float[] ToArray()
    {
      lock (this._items)
        return this._items.ToArray();
    }
  }
}
