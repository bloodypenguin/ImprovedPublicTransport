// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.Detour.Redirection`2
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using System.Reflection;

namespace ImprovedPublicTransport.Detour
{
  public class Redirection<TSource, TDestination>
  {
    private RedirectCallsState _callState;
    private MethodInfo _source;
    private MethodInfo _destination;

    public Redirection(string methodName, bool redirectAfterCreation)
      : this(methodName, methodName, redirectAfterCreation)
    {
    }

    public Redirection(string methodName)
      : this(methodName, methodName, true)
    {
    }

    public Redirection(string methodName, string destinationMethodName)
      : this(methodName, destinationMethodName, true)
    {
    }

    public Redirection(string sourceMethodName, string destinationMethodName, bool redirectAfterCreation)
    {
      if (string.IsNullOrEmpty(sourceMethodName))
        throw new ArgumentNullException("sourceMethodName");
      if (string.IsNullOrEmpty(destinationMethodName))
        throw new ArgumentNullException("destinationMethodName");
      BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
      MethodInfo[] methods1 = typeof (TSource).GetMethods(sourceMethodName, bindingAttr);
      if (methods1.Length == 0)
        throw new ArgumentNullException("No source methods found!");
      MethodInfo[] methods2 = typeof (TDestination).GetMethods(destinationMethodName, bindingAttr | BindingFlags.DeclaredOnly);
      if (methods2.Length == 0)
        throw new ArgumentNullException("No destination methods found!");
      bool flag = false;
      foreach (MethodInfo methodInfo1 in methods2)
      {
        foreach (MethodInfo methodInfo2 in methods1)
        {
          if (methodInfo2.ReturnType == methodInfo1.ReturnType && Utils.AreParametersEqual(methodInfo2.GetParameters(), methodInfo1.GetParameters()))
          {
            this._source = methodInfo2;
            this._destination = methodInfo1;
            flag = true;
            break;
          }
        }
      }
      if (!flag)
        throw new ArgumentNullException("No method match found.");
      if (!redirectAfterCreation)
        return;
      this.Redirect();
    }

    public void Redirect()
    {
      if (this._callState != null)
        return;
      Utils.Log((object) string.Format("Detouring from {0}.{1} to {2}.{3}.", (object) this._source.DeclaringType.Name, (object) this._source.Name, (object) this._destination.DeclaringType.Name, (object) this._destination.Name));
      this._callState = RedirectionHelper.RedirectCalls(this._source, this._destination);
    }

    public void Revert()
    {
      if (this._callState == null)
        return;
      Utils.Log((object) string.Format("Reverting detour for {0}.{1}", (object) this._source.DeclaringType.Name, (object) this._source.Name));
      RedirectionHelper.RevertRedirect(this._source, this._callState);
      this._callState = (RedirectCallsState) null;
    }
  }
}
