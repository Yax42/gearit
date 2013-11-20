using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using gearit.xna;
using gearit.src.editor;
using gearit.src.map;

namespace gearit.src.game
{
  class GearitNetworkGame : GearitGame
  {
    const int maxGamers = 4;
    const int maxLocalGamers = 1;

    private NetworkSession networkSession;

    private PacketWriter packetWriter;
    private PacketReader packetReader;

    private bool isHost;

    public GearitNetworkGame() : base()
    {
      packetWriter = new PacketWriter();
      packetReader = new PacketReader();
    }

    // TODO: Call `base.LoadContent()`.
    public override void LoadContent()
    {
      // We should call our base class method, but we won't for now since it
      // loads a map and robot from files.

      // For now, just copy the things from the base class.
      _time = 0;
      _drawGame = new DrawGame(ScreenManager.GraphicsDevice);
      _camera = new Camera2D(ScreenManager.GraphicsDevice);
      _world.Clear();
      _world.Gravity = new Vector2(0f, 9.8f);
      SerializerHelper.World = _world;

      // Load robots and map from network. (From files for now).
      // addRobot((Robot)Serializer.DeserializeItem("r2d2.gir"));
      _map = (Map) Serializer.DeserializeItem("moon.gim");

      // Reset time elapsed time that happened during loading.
      ScreenManager.Game.ResetElapsedTime();
    }

    /// <summary>
    /// Allows the game to run logic.
    /// </summary>
    public override void Update(GameTime gameTime, bool otherScreenHasFocus,
        bool coveredByOtherScreen)
    {
      UpdateNetworkSession();
      base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
    }

    /// <summary>
    /// Updates the state of the network session, moving the robots
    /// around and synchronizing their state over the network.
    /// </summary>
    private void UpdateNetworkSession()
    {
      // TODO: For each local player (only one?), read imput and send them to
      // the server.

      // TODO: If host, update all robots and send positions to all players.
    }
  }
}
