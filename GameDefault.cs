// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.GameDefault
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System.Collections.Generic;

namespace ImprovedPublicTransport
{
  public static class GameDefault
  {
    private static readonly Dictionary<ItemClass.SubService, ushort> _capacity = new Dictionary<ItemClass.SubService, ushort>()
    {
      {
        ItemClass.SubService.PublicTransportBus,
        (ushort) 30
      },
      {
        ItemClass.SubService.PublicTransportMetro,
        (ushort) 30
      },
      {
        ItemClass.SubService.PublicTransportTrain,
        (ushort) 30
      },
      {
        ItemClass.SubService.PublicTransportShip,
        (ushort) 100
      },
      {
        ItemClass.SubService.PublicTransportPlane,
        (ushort) 200
      },
      {
        ItemClass.SubService.PublicTransportTram,
        (ushort) 30
      }
    };

    public static ushort GetCapacity(ItemClass.SubService subService)
    {
      ushort num;
      if (GameDefault._capacity.TryGetValue(subService, out num))
        return num;
      return 0;
    }
  }
}
