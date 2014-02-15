using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using FarseerPhysics;
using gearit.src.editor;

//namespace gearit
//{
//#if WINDOWS || XBOX
//	static class Program
//	{
//		/// <summary>
//		/// The main entry point for the application.
//		/// </summary>
//		static void Main(string[] args)
//		{
//			Serializer.init();
//			Settings.UseFPECollisionCategories = true;
//			Input.init();
//			using (GearIt game = new GearIt())
//			{
//				game.Run();
//			}

//			// Test Serialization.

//			// Serializer s = new Serializer();
//			// World world = new World(new Vector2(0, 9.82f));
//			// SerializerHelper._world = world;
//			// Robot r = new Robot(world);
//			// string filename = "gally.robot";
//			// s.SerializeItem(filename, r);
//			// Robot r2 = (Robot)s.DeserializeItem(filename);

//			// Test robot
//			//BruteRobot game = new BruteRobot();
//			//game.Run();

//			// Test lua
//			//BruteRobot game = new BruteRobot();

//			//SpiderBot game = new SpiderBot();
//			//game.Run();
//			//LuaTest l = new LuaTest();
//		}
//	}
//#endif
//}
