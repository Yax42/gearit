using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;
using gearit.xna;

namespace SquidXNA
{
    /// <summary>
    /// Bridge between Squid and XNA for rendering
    /// </summary>
	public class RendererXNA : Squid.ISquidRenderer
	{
		[DllImport("user32.dll")]
		private static extern int GetKeyboardLayout(int dwLayout);
		[DllImport("user32.dll")]
		private static extern int GetKeyboardState(ref byte pbKeyState);
		[DllImport("user32.dll", EntryPoint = "MapVirtualKeyEx")]
		private static extern int MapVirtualKeyExA(int uCode, int uMapType, int dwhkl);
		[DllImport("user32.dll")]
		private static extern int ToAsciiEx(int uVirtKey, int uScanCode, ref byte lpKeyState, ref short lpChar, int uFlags, int dwhkl);

		private static int KeyboardLayout;
		private byte[] KeyStates;

		private Dictionary<int, SpriteFont> Fonts = new Dictionary<int, SpriteFont>();
		private Dictionary<string, int> FontLookup = new Dictionary<string, int>();

		private Dictionary<int, Texture2D> Textures = new Dictionary<int, Texture2D>();
		private Dictionary<string, int> TextureLookup = new Dictionary<string, int>();

		private Dictionary<string, FontDetail> FontTypes = new Dictionary<string, FontDetail>();

		private Game Game;
		private SpriteBatch Batch;

		private int FontIndex;
		private int TextureIndex;
		private Texture2D BlankTexture;


		private RasterizerState Rasterizer;
		private SamplerState Sampler;

		public class FontDetail
		{
			public FontDetail(string name, int size, bool bold)
			{
				Name = name;
				Size = size;
				Bold = bold;
			}

			public bool Bold { get; set; }
			public string Family { get; set; }
			public bool International { get; set; }
			public bool Italic { get; set; }
			public string Name { get; set; }
			public int Size { get; set; }
			public bool Underlined { get; set; }
		}

		public RendererXNA(Game game)
		{
			Game = game;
			Batch = new SpriteBatch(game.GraphicsDevice);

			BlankTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
			BlankTexture.SetData<Color>(new Color[] { new Color(255, 255, 255, 255) });

			FontTypes.Add(Squid.Font.Default, new FontDetail ("Normal", 8, true));
			FontTypes.Add("SubTitle", new FontDetail ("Subtitle", 14, true));
			FontTypes.Add("Title", new FontDetail ("Title", 20, false));

			KeyboardLayout = GetKeyboardLayout(0);
			KeyStates = new byte[0x100];

			Rasterizer = new RasterizerState();
			Rasterizer.ScissorTestEnable = true;

			Sampler = new SamplerState();
			Sampler.Filter = TextureFilter.Anisotropic;
		}

		public static int VirtualKeyToScancode(int key)
		{
			return MapVirtualKeyExA(key, 0, KeyboardLayout);
		}

		public bool TranslateKey(int code, ref char character)
		{
			short lpChar = 0;
			if (GetKeyboardState(ref KeyStates[0]) == 0)
				return false;

			int result = ToAsciiEx(MapVirtualKeyExA(code, 1, KeyboardLayout), code, ref KeyStates[0], ref lpChar, 0, KeyboardLayout);
			if (result == 1)
			{
				character = (char)((ushort)lpChar);
				return true;
			}

			return false;
		}

		private Microsoft.Xna.Framework.Color ColorFromtInt32(int color)
		{
			Byte[] bytes = BitConverter.GetBytes(color);

			return new Microsoft.Xna.Framework.Color(bytes[2], bytes[1], bytes[0], bytes[3]);
		}

		public int GetTexture(string name)
		{
			if (TextureLookup.ContainsKey(name))
				return TextureLookup[name];

			Texture2D texture = Game.Content.Load<Texture2D>(Path.ChangeExtension("GUI/" + name, null));
			TextureIndex++;

			TextureLookup.Add(name, TextureIndex);
			Textures.Add(TextureIndex, texture);

			return TextureIndex;
		}

		public int GetFont(string name)
		{
			if (FontLookup.ContainsKey(name))
				return FontLookup[name];

			if (!FontTypes.ContainsKey(name))
				return -1;

			FontDetail type = FontTypes[name];

			SpriteFont font = Game.Content.Load<SpriteFont>("GUI/Fonts/" + type.Name);
			FontIndex++;

			FontLookup.Add(name, FontIndex);
			Fonts.Add(FontIndex, font);

			return FontIndex;
		}

		public Squid.Point GetTextSize(string text, int font)
		{
			if (string.IsNullOrEmpty(text))
				return new Squid.Point();

			SpriteFont f = Fonts[font];
			Vector2 size = f.MeasureString(text);
			return new Squid.Point((int)size.X, (int)size.Y);
		}

		public Squid.Point GetTextureSize(int texture)
		{
			Texture2D tex = Textures[texture];
			return new Squid.Point(tex.Width, tex.Height);
		}

		public void DrawBox(int x, int y, int w, int h, int color)
		{
			Rectangle destination = new Rectangle(x, y, w, h);
			Batch.Draw(BlankTexture, destination, destination, ColorFromtInt32(color));
		}

		public void DrawText(string text, int x, int y, int font, int color)
		{
			if (!Fonts.ContainsKey(font)) 
				return;

			SpriteFont f = Fonts[font];
            Batch.End();
			Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Sampler, null, Rasterizer);
            Batch.DrawString(f, text, new Vector2(x, y), ColorFromtInt32(color));
            Batch.End();
			Batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Sampler, null, Rasterizer);
		}

		public void DrawTexture(int texture, int x, int y, int w, int h, Squid.Rectangle rect, int color)
		{
			if (!Textures.ContainsKey(texture))
				return;

			Texture2D tex = Textures[texture];

			Rectangle destination = new Rectangle(x, y, w, h);
			Rectangle source = new Rectangle();

			source.X = rect.Left;
			source.Y = rect.Top;
			source.Width = rect.Width;
			source.Height = rect.Height;
			
			Batch.Draw(tex, destination, source, ColorFromtInt32(color));
		}

		public void Scissor(int x, int y, int w, int h)
		{
			if (x < 0) x = 0;
			if (y < 0) y = 0;
			if (x + w > ScreenManager.Instance.Width)
				w = ScreenManager.Instance.Width - x;
			if (y + h > ScreenManager.Instance.Height)
				h = ScreenManager.Instance.Height - y;
			// Scissor (dont draw if not in rect)
			Game.GraphicsDevice.ScissorRectangle = new Rectangle(x, y, w, h);
		}

		public void StartBatch()
		{
			Batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Sampler, null, Rasterizer);
		}

		public void EndBatch(bool final)
		{
			Batch.End();

			// Fix scissor - Cant draw anything else except desktop without it
			Game.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Game.ScreenManager.Width, Game.ScreenManager.Height);
		}

		public void Dispose() { }
	}
}
