﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;
using SteamSDK;

namespace SEModAPIInternal.API.Server
{
	public class ServerNetworkManager : NetworkManager
	{
		#region "Attributes"

		new protected static ServerNetworkManager m_instance;

		public static string ServerNetworkManagerNamespace = "C42525D7DE28CE4CFB44651F3D03A50D";
		public static string ServerNetworkManagerClass = "3B0B7A338600A7B9313DE1C3723DAD14";

		public static string ServerNetworkManagerDisconnectPlayerMethod = "09FDD2D9700A7E602BE8722F81A55AC6";        
        //public static string ServerNetworkManagerSetPlayerBannedMethod = "853EC4BBB230C17D5629B95AA1FFB4AD";
        public static string ServerNetworkManagerSetPlayerBannedMethod = "D8507C079F9959AD7F06F37BFDC7712C";
        //public static string ServerNetworkManagerKickPlayerMethod = "3FFFD9DDA4057EFA95A8EB04D27D2C7D";
        public static string ServerNetworkManagerKickPlayerMethod = "B0CA1C2C2E07E744FC44C43547A8BDDB";
		public static string ServerNetworkManagerConnectedPlayersField = "89E92B070228A8BC746EFB57A3F6D2E5";

		#endregion

		#region "Properties"

		new public static ServerNetworkManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new ServerNetworkManager();

				return m_instance;
			}
		}

		#endregion

		#region "Methods"

		new public static bool ReflectionUnitTest()
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(ServerNetworkManagerNamespace, ServerNetworkManagerClass);
				if (type1 == null)
					throw new Exception("Could not find internal type for ServerNetworkManager");
				bool result = true;
				result &= BaseObject.HasMethod(type1, ServerNetworkManagerSetPlayerBannedMethod);
				result &= BaseObject.HasMethod(type1, ServerNetworkManagerDisconnectPlayerMethod);
				result &= BaseObject.HasMethod(type1, ServerNetworkManagerKickPlayerMethod);
				result &= BaseObject.HasField(type1, ServerNetworkManagerConnectedPlayersField);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return false;
			}
		}

		public override List<ulong> GetConnectedPlayers()
		{
			try
			{
				Object steamServerManager = GetNetworkManager();
				if (steamServerManager == null)
					return new List<ulong>();

				FieldInfo connectedPlayersField = steamServerManager.GetType().GetField(ServerNetworkManagerConnectedPlayersField, BindingFlags.NonPublic | BindingFlags.Instance);
				List<ulong> connectedPlayers = (List<ulong>)connectedPlayersField.GetValue(steamServerManager);

				return connectedPlayers;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return new List<ulong>();
			}
		}

		public void SetPlayerBan(ulong remoteUserId, bool isBanned)
		{
			try
			{
				Object steamServerManager = GetNetworkManager();
				BaseObject.InvokeEntityMethod(steamServerManager, ServerNetworkManagerSetPlayerBannedMethod, new object[] { remoteUserId, isBanned });

				KickPlayer(remoteUserId);
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		public void KickPlayer(ulong remoteUserId)
		{
			try
			{
				Object steamServerManager = GetNetworkManager();
				BaseObject.InvokeEntityMethod(steamServerManager, ServerNetworkManagerKickPlayerMethod, new object[] { remoteUserId });
				BaseObject.InvokeEntityMethod(steamServerManager, ServerNetworkManagerDisconnectPlayerMethod, new object[] { remoteUserId, ChatMemberStateChangeEnum.Kicked });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
