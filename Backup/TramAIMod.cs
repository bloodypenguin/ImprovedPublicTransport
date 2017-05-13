// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.TramAIMod
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ImprovedPublicTransport.Detour;

namespace ImprovedPublicTransport
{
  public class TramAIMod
  {
    private static bool _isDeployed;
    private static Redirection<TramAI, PassengerTrainAIMod> _redirectionLoadPassengers;
    private static Redirection<TramAI, PassengerTrainAIMod> _redirectionUnloadPassengers;
    private static Redirection<TramAI, BusAIMod> _redirectionCanLeave;

    public static void Init()
    {
      if (TramAIMod._isDeployed)
        return;
      TramAIMod._redirectionLoadPassengers = new Redirection<TramAI, PassengerTrainAIMod>("LoadPassengers");
      TramAIMod._redirectionUnloadPassengers = new Redirection<TramAI, PassengerTrainAIMod>("UnloadPassengers");
      TramAIMod._redirectionCanLeave = new Redirection<TramAI, BusAIMod>("CanLeave");
      TramAIMod._isDeployed = true;
    }

    public static void Deinit()
    {
      if (!TramAIMod._isDeployed)
        return;
      TramAIMod._redirectionLoadPassengers.Revert();
      TramAIMod._redirectionLoadPassengers = (Redirection<TramAI, PassengerTrainAIMod>) null;
      TramAIMod._redirectionUnloadPassengers.Revert();
      TramAIMod._redirectionUnloadPassengers = (Redirection<TramAI, PassengerTrainAIMod>) null;
      TramAIMod._redirectionCanLeave.Revert();
      TramAIMod._redirectionCanLeave = (Redirection<TramAI, BusAIMod>) null;
      TramAIMod._isDeployed = false;
    }
  }
}
