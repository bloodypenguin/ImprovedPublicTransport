// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.Detour.TypeExtensions
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace ImprovedPublicTransport.Detour
{
  public static class TypeExtensions
  {
    public static MethodInfo[] GetMethods(this Type t, string name, BindingFlags bindingAttr)
    {
      MethodInfo[] methods = t.GetMethods(bindingAttr);
      List<MethodInfo> methodInfoList = new List<MethodInfo>();
      for (int index = 0; index < methods.Length; ++index)
      {
        if (methods[index].Name.Equals(name))
          methodInfoList.Add(methods[index]);
      }
      return methodInfoList.ToArray();
    }
  }
}
