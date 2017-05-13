// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.NetManagerMod
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework;
using ImprovedPublicTransport.Detour;
using System;

namespace ImprovedPublicTransport
{
  public class NetManagerMod
  {
    private static readonly string _dataID = "IPT_NodeData";
    private static readonly string _dataVersion = "v003";
    private static bool _isDeployed = false;
    private static NetManagerMod.PreReleaseNodeImplementationCallback PreReleaseNodeImplementation;
    private static NetManagerMod.ReleaseNodeImplementationCallback ReleaseNodeImplementation;
    private static Redirection<NetManager, NetManagerMod> _redirection;
    public static NodeData[] m_cachedNodeData;

    public static void Init()
    {
      if (NetManagerMod._isDeployed)
        return;
      if (!NetManagerMod.TryLoadData(out NetManagerMod.m_cachedNodeData))
        Utils.Log((object) "Loading default net node data.");
      NetManagerMod.PreReleaseNodeImplementation = (NetManagerMod.PreReleaseNodeImplementationCallback) Utils.CreateDelegate<NetManager, NetManagerMod.PreReleaseNodeImplementationCallback>("PreReleaseNodeImplementation", (object) Singleton<NetManager>.instance);
      NetManagerMod.ReleaseNodeImplementation = (NetManagerMod.ReleaseNodeImplementationCallback) Utils.CreateDelegate<NetManager, NetManagerMod.ReleaseNodeImplementationCallback>("ReleaseNodeImplementation", (object) Singleton<NetManager>.instance);
      NetManagerMod._redirection = new Redirection<NetManager, NetManagerMod>("ReleaseNode");
      SerializableDataExtension.instance.EventSaveData += new SerializableDataExtension.SaveDataEventHandler(NetManagerMod.OnSaveData);
      NetManagerMod._isDeployed = true;
    }

    public static void Deinit()
    {
      if (!NetManagerMod._isDeployed)
        return;
      NetManagerMod.m_cachedNodeData = (NodeData[]) null;
      NetManagerMod._redirection.Revert();
      NetManagerMod._redirection = (Redirection<NetManager, NetManagerMod>) null;
      NetManagerMod.PreReleaseNodeImplementation = (NetManagerMod.PreReleaseNodeImplementationCallback) null;
      NetManagerMod.ReleaseNodeImplementation = (NetManagerMod.ReleaseNodeImplementationCallback) null;
      SerializableDataExtension.instance.EventSaveData -= new SerializableDataExtension.SaveDataEventHandler(NetManagerMod.OnSaveData);
      NetManagerMod._isDeployed = false;
    }

    public static bool TryLoadData(out NodeData[] data)
    {
      data = new NodeData[32768];
      byte[] data1 = SerializableDataExtension.instance.SerializableData.LoadData(NetManagerMod._dataID);
      if (data1 == null)
        return false;
      int index1 = 0;
      string empty = string.Empty;
      try
      {
        Utils.Log((object) "Try to load net node data.");
        string str = SerializableDataExtension.ReadString(data1, ref index1);
        if (string.IsNullOrEmpty(str) || str.Length != 4)
        {
          Utils.LogWarning((object) "Unknown data found.");
          return false;
        }
        Utils.Log((object) ("Found net node data version: " + str));
        while (index1 < data1.Length)
        {
          int index2 = SerializableDataExtension.ReadInt32(data1, ref index1);
          if (str == "v001")
          {
            double num = (double) SerializableDataExtension.ReadFloat(data1, ref index1);
          }
          data[index2].PassengersIn = SerializableDataExtension.ReadInt32(data1, ref index1);
          data[index2].PassengersOut = SerializableDataExtension.ReadInt32(data1, ref index1);
          data[index2].LastWeekPassengersIn = SerializableDataExtension.ReadInt32(data1, ref index1);
          data[index2].LastWeekPassengersOut = SerializableDataExtension.ReadInt32(data1, ref index1);
          data[index2].PassengerInData = SerializableDataExtension.ReadFloatArray(data1, ref index1);
          data[index2].PassengerOutData = SerializableDataExtension.ReadFloatArray(data1, ref index1);
          data[index2].Unbunching = str == "v001" || str == "v002" || SerializableDataExtension.ReadBool(data1, ref index1);
        }
        return true;
      }
      catch (Exception ex)
      {
        Utils.LogWarning((object) ("Could not load net node data. " + ex.Message));
        data = new NodeData[32768];
        return false;
      }
    }

    private static void OnSaveData()
    {
      FastList<byte> data = new FastList<byte>();
      try
      {
        SerializableDataExtension.WriteString(NetManagerMod._dataVersion, data);
        for (int index = 0; index < 32768; ++index)
        {
          if (!NetManagerMod.m_cachedNodeData[index].IsEmpty)
          {
            SerializableDataExtension.WriteInt32(index, data);
            SerializableDataExtension.WriteInt32(NetManagerMod.m_cachedNodeData[index].PassengersIn, data);
            SerializableDataExtension.WriteInt32(NetManagerMod.m_cachedNodeData[index].PassengersOut, data);
            SerializableDataExtension.WriteInt32(NetManagerMod.m_cachedNodeData[index].LastWeekPassengersIn, data);
            SerializableDataExtension.WriteInt32(NetManagerMod.m_cachedNodeData[index].LastWeekPassengersOut, data);
            SerializableDataExtension.WriteFloatArray(NetManagerMod.m_cachedNodeData[index].PassengerInData, data);
            SerializableDataExtension.WriteFloatArray(NetManagerMod.m_cachedNodeData[index].PassengerOutData, data);
            SerializableDataExtension.WriteBool(NetManagerMod.m_cachedNodeData[index].Unbunching, data);
          }
        }
        SerializableDataExtension.instance.SerializableData.SaveData(NetManagerMod._dataID, data.ToArray());
      }
      catch (Exception ex)
      {
        Utils.LogError((object) ("Error while saving net node data! " + ex.Message + " " + (object) ex.InnerException));
      }
    }

    public void ReleaseNode(ushort nodeID)
    {
      Singleton<InstanceManager>.instance.SetName(new InstanceID()
      {
        NetNode = nodeID
      }, (string) null);
      if (!NetManagerMod.m_cachedNodeData[(int) nodeID].IsEmpty)
        NetManagerMod.m_cachedNodeData[(int) nodeID] = new NodeData();
      NetManager instance = Singleton<NetManager>.instance;
      NetManagerMod.PreReleaseNodeImplementation(nodeID, ref instance.m_nodes.m_buffer[(int) nodeID], false, false);
      NetManagerMod.ReleaseNodeImplementation(nodeID, ref instance.m_nodes.m_buffer[(int) nodeID]);
    }

    private delegate void PreReleaseNodeImplementationCallback(ushort node, ref NetNode data, bool checkDeleted, bool checkTouchable);

    private delegate void ReleaseNodeImplementationCallback(ushort node, ref NetNode data);
  }
}
