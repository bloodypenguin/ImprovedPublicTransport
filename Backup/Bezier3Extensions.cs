// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.Bezier3Extensions
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework.Math;
using UnityEngine;

namespace ImprovedPublicTransport
{
  public static class Bezier3Extensions
  {
    public static float CalculateLength(this Bezier3 curve, int steps = 10)
    {
      float t1 = 0.0f;
      Vector3 b = curve.Position(t1);
      float num = 0.0f;
      for (int index = 1; index <= steps; ++index)
      {
        float t2 = (float) index / (float) steps;
        Vector3 a = curve.Position(t2);
        num += Vector3.Distance(a, b);
        b = a;
      }
      return num;
    }
  }
}
