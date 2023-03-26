// <copyright file="CopyPaste.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using ImprovedPublicTransport2.UI.AlgernonCommons;

namespace VehicleSelector
{
    using System.Collections.Generic;
    using ColossalFramework;
    using UnityEngine;

    /// <summary>
    /// Handles copying and pasting of building settings.
    /// </summary>
    public class CopyPaste
    {
        // Instance reference.
        private static CopyPaste s_instance;

        // Copy buffer.
        private readonly TransferManager.TransferReason[] _copyReasons = new TransferManager.TransferReason[Transfers.MaxTransfers];
        private readonly List<VehicleInfo>[] _copyBuffer = new List<VehicleInfo>[Transfers.MaxTransfers]
        {
            new List<VehicleInfo>(),
            new List<VehicleInfo>(),
            new List<VehicleInfo>(),
        };

        // Prevent heap allocations every time we copy.
        private readonly Transfers.TransferStruct[] _transferBuffer = new Transfers.TransferStruct[Transfers.MaxTransfers];

        // Copy metadata.
        private bool _isCopied = false;
        private int _bufferSize;

        /// <summary>
        /// Gets the active instance.
        /// </summary>
        internal static CopyPaste Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new CopyPaste();
                }

                return s_instance;
            }
        }

        /// <summary>
        /// Copies vehicle data from the given building to the copy buffer.
        /// </summary>
        /// <param name="buildingID">Source building ID.</param>
        internal void Copy(ushort buildingID)
        {
            // Safety check.
            if (buildingID == 0)
            {
                Logging.Error("zero buildingID passed to CopyPaste.Copy");
                return;
            }

            BuildingInfo buildingInfo = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info;
            if (buildingInfo == null)
            {
                Logging.Error("invalid buildingID passed to CopyPaste.Copy");
                return;
            }

            // Number of records to copy - make sure there's at least one before proceeding.
            int length = Transfers.BuildingEligibility(buildingID, buildingInfo, _transferBuffer);
            _bufferSize = length;

            // Make sure there's at least one transfer before proceeding.
            if (length > 0)
            {
                // Clear copied flag (it will be set later if valid data was copied).
                _isCopied = false;

                // Copy records from source building to buffer.
                for (int i = 0; i < length; ++i)
                {
                    // Clear the buffer entry.
                    _copyBuffer[i].Clear();

                    // Try to get vehicle list entry.
                    List<VehicleInfo> thisList = VehicleControl.GetVehicles(buildingID, _transferBuffer[i].Reason);
                    if (thisList != null && thisList.Count > 0)
                    {
                        // Valid list retrieved - copy it to buffer (don't just copy the list reference, but the content).
                        _copyBuffer[i].AddRange(thisList);
                        _copyReasons[i] = _transferBuffer[i].Reason;
                        _isCopied = true;
                    }
                }
            }
        }

        /// <summary>
        /// Copies the current settings of the given building to all others of the same type, optionally limiting to a district and/or park area.
        /// </summary>
        /// <param name="buildingID">Source building ID.</param>
        /// <param name="district">District to limit to (0 for no limit).</param>
        /// <param name="park">Park area to limit to (0 for no limit).</param>
        internal void CopyToBuildings(ushort buildingID, byte district, byte park)
        {
            // Safety check.
            if (buildingID == 0)
            {
                Logging.Error("zero buildingID passed to CopyPaste.CopyToBuildings");
                return;
            }

            // Local references.
            Building[] buildingBuffer = Singleton<BuildingManager>.instance.m_buildings.m_buffer;
            DistrictManager districtManager = Singleton<DistrictManager>.instance;

            // Get building prefab.
            BuildingInfo buildingInfo = buildingBuffer[buildingID].Info;
            if (buildingInfo == null)
            {
                Logging.Error("invalid buildingID passed to CopyPaste.CopyToBuildings");
                return;
            }

            // Determine if any district or park restrictions apply.
            bool restricted = district != 0 | park != 0;

            // Number of records to copy - make sure there's at least one transfer before proceeding.
            int numTransfers = Transfers.BuildingEligibility(buildingID, buildingInfo, _transferBuffer);
            if (numTransfers > 0)
            {
                // Copy to all other matching buildings.
                for (ushort i = 0; i < buildingBuffer.Length; ++i)
                {
                    // Look for any created buildings with matching service and subservice that aren't this one.
                    if ((buildingBuffer[i].m_flags & Building.Flags.Created) != 0 && i != buildingID && buildingBuffer[i].Info == buildingInfo)
                    {
                        // Apply any district and park restrictions.
                        if (restricted)
                        {
                            if ((district != 0 && districtManager.GetDistrict(buildingBuffer[i].m_position) != district) || (park != 0 && districtManager.GetPark(buildingBuffer[i].m_position) != park))
                            {
                                continue;
                            }
                        }

                        // Paste vehicles.
                        for (int j = 0; j < numTransfers; ++j)
                        {
                            TransferManager.TransferReason reason = _transferBuffer[j].Reason;
                            VehicleControl.PasteVehicles(i, reason, VehicleControl.GetVehicles(buildingID, reason));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the given building is a valid target for the current copy buffer..
        /// </summary>
        /// <param name="buildingID">Source building ID.</param>
        /// <returns>True the building is a valid copy buffer target, false otherwise.</returns>
        internal bool IsValidTarget(ushort buildingID)
        {
            // Don't do anything if there's no active copy data.
            if (!_isCopied || buildingID == 0)
            {
                return false;
            }

            BuildingInfo buildingInfo = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info;
            if (buildingInfo == null)
            {
                return false;
            }

            // Determine length of target building transfer buffer (smallest of the two buffers).
            int length = Mathf.Min(_bufferSize, Transfers.BuildingEligibility(buildingID, buildingInfo, _transferBuffer));

            // Check buffer content variability.
            for (int i = 0; i < length; ++i)
            {
                // Check for a matching reason.
                if (_transferBuffer[i].Reason == _copyReasons[i])
                {
                    return true;
                }
            }

            // If we got here, no match was found.
            return false;
        }

        /// <summary>
        /// Attempts to paste vehicle data from the copy buffer to the given building.
        /// </summary>
        /// <param name="buildingID">Source building ID.</param>
        /// <returns>True if copy was successful, false otherwise.</returns>
        internal bool Paste(ushort buildingID)
        {
            // Don't do anything if there's no active copy data.
            if (!_isCopied)
            {
                return false;
            }

            // Safety check.
            if (buildingID == 0)
            {
                Logging.Error("zero buildingID passed to CopyPaste.Paste");
                return false;
            }

            BuildingInfo buildingInfo = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info;
            if (buildingInfo == null)
            {
                Logging.Error("invalid buildingID passed to CopyPaste.Paste");
                return false;
            }

            // Determine length of target building transfer buffer (smallest of the two buffers).
            int length = Mathf.Min(_bufferSize, Transfers.BuildingEligibility(buildingID, buildingInfo, _transferBuffer));

            // All checks passed - copy records from buffer to building.
            for (int i = 0; i < length; ++i)
            {
                // Skip non-matching reasons.
                if (_transferBuffer[i].Reason != _copyReasons[i])
                {
                    continue;
                }

                // Paste vehicles.
                VehicleControl.PasteVehicles(buildingID, _copyReasons[i], _copyBuffer[i]);
            }

            // If we got here, then pasting was successful.
            return true;
        }
    }
}
