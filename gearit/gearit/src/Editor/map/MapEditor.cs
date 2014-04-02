using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using gearit.src.utility.Menu;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using gearit.src.editor.robot;
using FarseerPhysics.Factories;
using gearit.src.map;
using gearit.src.editor.map.action;

namespace gearit.src.editor.map
{

	class MapEditor : GameScreen, IDemoScreen
	{
		private World _world;
		private Map _map;
		private EditorCamera _camera;
		private DrawGame _draw_game;
//		private MenuOverlay _menu_tools;
//		private MenuOverlay _menu_properties;
		private const int PropertiesMenuSize = 40;
		private IAction _currentAction;
		private MapChunk _dummyChunk;
		private MapChunk _select;


		public static MapEditor Instance { set; get; }

		public MapChunk Select
		{
			get
			{
				return _select;
			}
			set
			{
				if (value == null)
					_select = _dummyChunk;
				else
					_select = value;
			}
		}

		public Map Map { get { return _map; } }
		public World World { get { return _world; } }

		#region IDemoScreen Members

		public string GetTitle()
		{
			return "Map Editor";
		}

		public string GetDetails()
		{
			return "";
		}

		#endregion

		public MapEditor()
		{
			ActionFactory.init();
			TransitionOnTime = TimeSpan.FromSeconds(0.75);
			TransitionOffTime = TimeSpan.FromSeconds(0.75);
			Select = null;
			_world = null;
			HasCursor = true;
			Instance = this;
		}


		public override void LoadContent()
		{
			base.LoadContent();

			if (_world == null)
				_world = new World(Vector2.Zero);
			else
				_world.Clear();
			_dummyChunk = new PolygonChunk(_world, false, Vector2.Zero);
			_currentAction = ActionFactory.Dummy;
			SerializerHelper.World = _world;
			_map = new Map(_world);
			ScreenManager.Game.ResetElapsedTime();
			_camera = new EditorCamera(ScreenManager.GraphicsDevice);
			_camera.Position = new Vector2(0, 0);
			_world.Gravity = new Vector2(0f, 0f);
			HasVirtualStick = true;
			Select = null;

			_draw_game = new DrawGame(ScreenManager.GraphicsDevice);
			Rectangle rec = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);

			#region MENU
			/*
			MenuItem item;
			Vector2 pos = new Vector2(0, 50);
			Vector2 size = new Vector2(PropertiesMenuSize, ScreenManager.GraphicsDevice.Viewport.Height - 28);
			_menu_properties = new MenuOverlay(ScreenManager, pos, size, Color.LightSteelBlue, MenuLayout.Vertical);

			item = new SpriteMenuItem(_menu_properties, "EditorIcon/place", new Vector2(1), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
			item.addFocus((int)Mode.PLACE, new Color(110, 110, 110), new Color(120, 120, 120));

			item = new SpriteMenuItem(_menu_properties, "EditorIcon/ball", new Vector2(1), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
			item.addFocus((int)Mode.BALL, new Color(110, 110, 110), new Color(120, 120, 120));

			item = new SpriteMenuItem(_menu_properties, "EditorIcon/rotate", new Vector2(1), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
			item.addFocus((int)Mode.ROTATE, new Color(110, 110, 110), new Color(120, 120, 120));

			item = new SpriteMenuItem(_menu_properties, "EditorIcon/move", new Vector2(1), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
			item.addFocus((int)Mode.MOVE, new Color(110, 110, 110), new Color(120, 120, 120));

			item = new SpriteMenuItem(_menu_properties, "EditorIcon/resize", new Vector2(1), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
			item.addFocus((int)Mode.RESIZE, new Color(110, 110, 110), new Color(120, 120, 120));

			item = new SpriteMenuItem(_menu_properties, "EditorIcon/static", new Vector2(1), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
			item.addFocus((int)Mode.STATIC, new Color(110, 110, 110), new Color(120, 120, 120));

			item = new SpriteMenuItem(_menu_properties, "EditorIcon/dynamic", new Vector2(1), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
			item.addFocus((int)Mode.DYNAMIC, new Color(110, 110, 110), new Color(120, 120, 120));

			item = new SpriteMenuItem(_menu_properties, "EditorIcon/delete", new Vector2(1), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
			item.addFocus((int)Mode.DELETE, new Color(110, 110, 110), new Color(120, 120, 120));

			pos.X = 0;
			pos.Y = 0;
			size = new Vector2(400, 50);
			_menu_tools = new MenuOverlay(ScreenManager, pos, size, Color.LightGray, MenuLayout.Horizontal);
			item = new TextMenuItem(_menu_tools, "New", ScreenManager.Fonts.DetailsFont, Color.Black, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
			item.addFocus((int)Act.NEW, new Color(110, 110, 110), new Color(120, 120, 120));

			item = new TextMenuItem(_menu_tools, "Open", ScreenManager.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
			item.addFocus((int)Act.LOAD, new Color(110, 110, 110), new Color(120, 120, 120));

			item = new TextMenuItem(_menu_tools, "Save", ScreenManager.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
			item.addFocus((int)Act.SAVE, new Color(110, 110, 110), new Color(120, 120, 120));

			_menu_tools.Adjusting = true;
			*/
			#endregion
 
		}

		public override void Update(GameTime gameTime)
		{
			//Input.update();
			_camera.update();
			HandleInput();
			//_menu_tools.Update();
			//_menu_properties.Update();
			_world.Step(0f);
			base.Update(gameTime);
		}

		private void changeModeDebug()
		{
			if (Input.ctrlAltShift(true, false, false) && Input.justPressed(Keys.S))
			{
				Serializer.SerializeItem("moon.gim", _map);
			}
			if (Input.ctrlAltShift(true, false, false) && Input.justPressed(Keys.D))
			{
				_world.Clear();
				_map = (Map)Serializer.DeserializeItem("moon.gim");
			}
		}

		private void select()
		{
			if (Select != null)
				Select = null;
			else
			{
				Select = _map.getChunk(Input.SimMousePos);
			}
		}

		private void HandleInput()
		{
			if (_currentAction.type() == ActionTypes.NONE)
			{
				if (MenuRobotEditor.Instance.hasFocus())
					return;
				_currentAction = ActionFactory.createFromShortcut();
				_currentAction.init();
			}
			if (_currentAction.run() == false)
			{
				_currentAction = ActionFactory.Dummy;
			}
			_camera.input();
		}

#if false
		private void HandleInput()
		{
			changeModeDebug();
			MenuItem pressed;
			if ((pressed = _menu_properties.justPressed()) != null)
			{
				_mode = (Mode)pressed.Id;
			}
			if (Input.pressed(Keys.Escape))
			{
				ScreenManager.RemoveScreen(this);
			}
			if ((pressed = _menu_tools.justPressed()) != null)
			{
				switch (pressed.Id)
				{
					case (int)Act.LOAD:
						_world.Clear();
						_map = (Map)Serializer.DeserializeItem("moon.gim");
						break;
					case (int)Act.SAVE:
						Serializer.SerializeItem("moon.gim", _map);
						break;
					case (int)Act.NEW:
						//_world.Clear();
						break;
				}
			}

			if (Input.justPressed(MouseKeys.LEFT) && !_menu_properties.isMouseOn() && !_menu_tools.isMouseOn())
			{
				MapChunk t = _map.getChunk(Input.SimMousePos);
				switch (_mode)
				{
					case Mode.STATIC:
						if (t != null)
							t.BodyType = BodyType.Static;
						break;
					case Mode.DYNAMIC:
						if (t != null)
							t.BodyType = BodyType.Dynamic;
						break;
					case Mode.MOVE:
							select();
						break;
					case Mode.PLACE:
						_map.addChunk(new PolygonChunk(_world, false, Input.SimMousePos));
						break;
					case Mode.BALL:
							_map.addChunk(new CircleChunk(_world, true, Input.SimMousePos));
						break;
					case Mode.DELETE:
							_map.getChunks().Remove(_map.getChunk(Input.SimMousePos));
						break;
					case Mode.RESIZE:
						break;
				}
			}

			if (_mode == Mode.MOVE && Select != null)
			{
				Select.Position = Input.SimMousePos; /*(Input.SimMousePos - Select.Position) + Input.SimMousePos*/
			}

			if (_mode == Mode.ROTATE && !_menu_properties.isMouseOn() && !_menu_tools.isMouseOn())
			{
				if (Input.pressed(MouseKeys.LEFT))
				{
					MapChunk tmp = _map.getChunk(Input.SimMousePos);
					if (tmp != null)
					{
						tmp.Rotation += 0.01f;
					}
				}
				if (Input.pressed(MouseKeys.RIGHT))
				{
					MapChunk tmp = _map.getChunk(Input.SimMousePos);
					if (tmp != null)
					{
						tmp.Rotation -= 0.01f;
					}
				}
			}

			if (_mode == Mode.RESIZE)
			{
				if (Input.justPressed(MouseKeys.LEFT))
				{
					MapChunk backup = Select;
					Select = _map.getChunk(Input.SimMousePos);
					if (Select == null)
						Select = backup;
					if (Select != null && Select.GetType() == typeof(PolygonChunk))
						((PolygonChunk)Select).selectVertice(Input.SimMousePos);
				}

				if (Input.pressed(MouseKeys.LEFT) && Select != null)
					Select.resize(Input.SimMousePos); /*(Input.SimMousePos - ;Select.Position) + Input.SimMousePos*/
			}
			_camera.input();
		}
#endif

		public override void Draw(GameTime gameTime)
		{
			//ScreenManager.GraphicsDevice.Clear(Color.LightSeaGreen);
			_draw_game.Begin(_camera);

			_map.drawDebug(_draw_game);

			//_menu_tools.draw(_draw_game);
			//_menu_properties.draw(_draw_game);
			_draw_game.End();
			base.Draw(gameTime);
		}
	}
}
