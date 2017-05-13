// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.Detour.RedirectionHelper
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using System.Reflection;

namespace ImprovedPublicTransport.Detour
{
  internal static class RedirectionHelper
  {
    public static RedirectCallsState RedirectCalls(MethodInfo from, MethodInfo to)
    {
      RuntimeMethodHandle methodHandle = from.MethodHandle;
      IntPtr functionPointer1 = methodHandle.GetFunctionPointer();
      methodHandle = to.MethodHandle;
      IntPtr functionPointer2 = methodHandle.GetFunctionPointer();
      return RedirectionHelper.PatchJumpTo(functionPointer1, functionPointer2);
    }

    public static void RevertRedirect(MethodInfo from, RedirectCallsState state)
    {
      RedirectionHelper.RevertJumpTo(from.MethodHandle.GetFunctionPointer(), state);
    }

    private static unsafe RedirectCallsState PatchJumpTo(IntPtr site, IntPtr target)
    {
      RedirectCallsState redirectCallsState = new RedirectCallsState();
      byte* pointer = (byte*) site.ToPointer();
      int num1 = (int) *pointer;
      redirectCallsState.a = (byte) num1;
      int num2 = (int) pointer[1];
      redirectCallsState.b = (byte) num2;
      int num3 = (int) pointer[10];
      redirectCallsState.c = (byte) num3;
      int num4 = (int) pointer[11];
      redirectCallsState.d = (byte) num4;
      int num5 = (int) pointer[12];
      redirectCallsState.e = (byte) num5;
      long num6 = *(long*) (pointer + 2);
      redirectCallsState.f = (ulong) num6;
      *pointer = (byte) 73;
      pointer[1] = (byte) 187;
      *(long*) (pointer + 2) = target.ToInt64();
      pointer[10] = (byte) 65;
      pointer[11] = byte.MaxValue;
      pointer[12] = (byte) 227;
      return redirectCallsState;
    }

    private static unsafe void RevertJumpTo(IntPtr site, RedirectCallsState state)
    {
      byte* pointer = (byte*) site.ToPointer();
      *pointer = state.a;
      pointer[1] = state.b;
      *(long*) (pointer + 2) = (long) state.f;
      pointer[10] = state.c;
      pointer[11] = state.d;
      pointer[12] = state.e;
    }
  }
}
