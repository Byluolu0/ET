﻿using System;
using System.IO;
using System.Reflection;
using Base;
using Model;
using Object = Base.Object;

namespace App
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				Log.Info("server start........................");

				Object.ObjectManager.Register("Base", typeof(Game).Assembly);
				Object.ObjectManager.Register("Model", typeof(ErrorCode).Assembly);
				Object.ObjectManager.Register("Controller", DllHelper.GetController());

				Options options = Game.Scene.AddComponent<OptionsComponent, string[]>(args).Options;
				
				Game.Scene.AddComponent<EventComponent>();
				Game.Scene.AddComponent<TimerComponent>();
				Game.Scene.AddComponent<NetworkComponent, NetworkProtocol, string, int>(options.Protocol, options.Host, options.Port);
				Game.Scene.AddComponent<MessageDispatherComponent, string>(options.AppType);
				
				// 根据不同的AppType添加不同的组件
				switch (options.AppType)
				{
					case AppType.Manager:
						Game.Scene.AddComponent<AppManagerComponent>();
						break;
					case AppType.Realm:
						Game.Scene.AddComponent<RealmGateAddressComponent>();
						break;
					case AppType.Gate:
						Game.Scene.AddComponent<GateSessionKeyComponent>();
						break;
					default:
						throw new Exception($"命令行参数没有设置正确的AppType: {options.AppType}");
				}
				

				while (true)
				{
					Object.ObjectManager.Update();
				}
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}
	}
}
