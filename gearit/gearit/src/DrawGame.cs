using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using Microsoft.Xna.Framework;
using gearit.xna;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using gearit.src.editor;
using FarseerPhysics.DebugViews;

namespace gearit.src
{
	class DrawGame
	{
		// a basic effect, which contains the shaders that we will use to draw our
		// primitives. (it's about gpu)
		private const int _circleSegments = 32;
		private BasicEffect _basicEffect;
		private VertexPositionColor[] _lineVertices = new VertexPositionColor[50000];
		private Vector2[] _tempVertices = new Vector2[500];
		private AssetCreator _asset;
		private GraphicsDevice _device;
		private int _count;
		private SpriteBatch _batch;
		private Matrix _staticProj;
		private Matrix _staticView;
		private DebugViewXNA _debug;
		private PrimitiveBatch _primitiveBatch;

		public DrawGame(GraphicsDevice device)
		{
			_staticProj = Matrix.CreateOrthographicOffCenter(0f, device.Viewport.Width, device.Viewport.Height, 0f, 0f, 1f);
			Vector3 translateCenter = new Vector3(device.Viewport.Width / 2f, device.Viewport.Height / 2f, 0);
			_staticView = Matrix.CreateTranslation( -translateCenter) * Matrix.CreateTranslation(translateCenter);

			_batch = new SpriteBatch(device);
			_asset = new AssetCreator(device);
			_device = device;

			_primitiveBatch = new PrimitiveBatch(_device, 1000);


			// set up a new basic effect, and enable vertex colors.
			_basicEffect = new BasicEffect(device);
			_basicEffect.VertexColorEnabled = true;
		}



		public DrawGame(GraphicsDevice device, DebugViewXNA debug)
		{
			_debug = debug;
			_staticProj = Matrix.CreateOrthographicOffCenter(0f, device.Viewport.Width, device.Viewport.Height, 0f, 0f, 1f);
			Vector3 translateCenter = new Vector3(device.Viewport.Width / 2f, device.Viewport.Height / 2f, 0);
			_staticView = Matrix.CreateTranslation( -translateCenter) * Matrix.CreateTranslation(translateCenter);

			_batch = new SpriteBatch(device);
			_asset = new AssetCreator(device);
			_primitiveBatch = new PrimitiveBatch(device, 1000);
			_device = device;

			// set up a new basic effect, and enable vertex colors.
			_basicEffect = new BasicEffect(device);
			_basicEffect.VertexColorEnabled = true;
		}

		public SpriteBatch Batch()
		{
			return (_batch);
		}

		public void Begin(ICamera camera)
		{
			//tell our basic effect to begin.
			_batch.Begin();
			_basicEffect.Projection = camera.projection();
			_basicEffect.View = camera.view();
			_basicEffect.CurrentTechnique.Passes[0].Apply();
		}

		public void Begin()
		{
			//tell our basic effect to begin.
			_batch.Begin();
			_basicEffect.Projection = _staticProj;
			_basicEffect.View = _staticView;
			_basicEffect.CurrentTechnique.Passes[0].Apply();
		}

		public void BeginTexture(ICamera camera)
		{
            // C'est à cause de ça que c'est pas aligné. Faut trouver un moyen d'envoyer camera.projection() et camera.view() a _primitiveBatch(ref x, ref x)
			Matrix projection = Matrix.CreateOrthographicOffCenter(0f, ConvertUnits.ToSimUnits(_device.Viewport.Width),
											 ConvertUnits.ToSimUnits(_device.Viewport.Height), 0f, 0f,
											 1f);
			Vector2 screen_center = new Vector2(_device.Viewport.Width / 2f,
												_device.Viewport.Height / 2f);
			Matrix view = Matrix.CreateTranslation(new Vector3((ConvertUnits.ToSimUnits(Vector2.Zero) -
			ConvertUnits.ToSimUnits(screen_center)), 0f)) * Matrix.CreateTranslation(new Vector3(ConvertUnits.ToSimUnits(screen_center), 0f));

			_primitiveBatch.Begin(ref projection, ref view);
		}

		public void End()
		{
			flushLines();
			_batch.End();
		}

		public void EndTexture()
		{
			_primitiveBatch.End();
		}

		public void drawTexture(Body b, Color c)
		{
			Transform xf;
			b.GetTransform(out xf);
			foreach (Fixture f in b.FixtureList)
				drawTexture(f, xf, c);
		}

		private void drawTexture(Fixture fixture, Transform xf, Color color)
		{
			switch (fixture.ShapeType)
			{
				case ShapeType.Circle:
					{
                        // Faut aller chercher dans DebugViewXNA le code source de la fonction DrawSolidCircle, et le foutre ici pour avoir les cercles aussi.
						return;
					}
					break;

				case ShapeType.Polygon:
					{
						PolygonShape poly = (PolygonShape)fixture.Shape;
						int vertexCount = poly.Vertices.Count;

						for (int i = 0; i < vertexCount; ++i)
							_tempVertices[i] = MathUtils.Multiply(ref xf, poly.Vertices[i]);

						for (int i = 1; i < vertexCount - 1; i++)
						{
							_primitiveBatch.AddVertex(_tempVertices[0], Color.Orange, PrimitiveType.TriangleList);
							_primitiveBatch.AddVertex(_tempVertices[i], Color.Orange, PrimitiveType.TriangleList);
							_primitiveBatch.AddVertex(_tempVertices[i + 1], Color.Orange, PrimitiveType.TriangleList);
						}
					}
					break;
			}
		}

		public void drawLine(Vector2 p1, Vector2 p2, Color col)
		{
			if (_count >= _lineVertices.Length)
			{
				flushLines();
			}
			_lineVertices[_count++] = new VertexPositionColor(new Vector3(p1, 0), col);
			_lineVertices[_count++] = new VertexPositionColor(new Vector3(p2, 0f), col);
		}

		private void flushLines()
		{
			if (_count >= 2)
			{
				_device.SamplerStates[0] = SamplerState.AnisotropicClamp;
				_device.DrawUserPrimitives(PrimitiveType.LineList, _lineVertices, 0, _count / 2);
				_count = 0;
			}
		}

		public void draw(Body b, Color col)
		{
			Transform xf;
			b.GetTransform(out xf);
			foreach (Fixture f in b.FixtureList)
				draw(f, xf, col);
			/*** Bon gros bullshit
			Texture2D texture = new Texture2D(_device, 1, 1);
			texture.SetData<Color>(new Color[] { Color.Orange});
			Vector2 posBody = ConvertUnits.ToDisplayUnits(b.Position);
			Vector2 posCenter = new Vector2(10.0f, 10.0f);
			//_batch.Draw(texture, posBody, null, Color.Blue, b.Rotation, posCenter, 1f, SpriteEffects.None, 0f);
			_batch.Draw(texture, new Rectangle(200, 200, 200, 200), Color.White);
			 ***/
		}

		private void draw(Fixture fixture, Transform xf, Color color)
		{
			switch (fixture.ShapeType)
			{
				case ShapeType.Circle:
					{
						CircleShape circle = (CircleShape)fixture.Shape;

						Vector2 center = MathUtils.Multiply(ref xf, circle.Position);
						float radius = circle.Radius;

						drawCircle(center, radius, color);
						float angle = fixture.Body.Rotation;
						drawLine(center, center + new Vector2((float)(Math.Cos(angle)) * radius, (float) (Math.Sin(angle) * radius)), color);
					}
					break;

				case ShapeType.Polygon:
					{
						PolygonShape poly = (PolygonShape)fixture.Shape;
						int vertexCount = poly.Vertices.Count;

						for (int i = 0; i < vertexCount; ++i)
						{
							_tempVertices[i] = MathUtils.Multiply(ref xf, poly.Vertices[i]);
						}

						drawPolygon(_tempVertices, vertexCount, color);
					}
					break;

				case ShapeType.Edge:
					{
						EdgeShape edge = (EdgeShape)fixture.Shape;
						Vector2 v1 = MathUtils.Multiply(ref xf, edge.Vertex1);
						Vector2 v2 = MathUtils.Multiply(ref xf, edge.Vertex2);
						drawLine(v1, v2, color);
					}
					break;
			}
		}

		private void drawPolygon(Vector2[] vertices, int count, Color color)
		{
			for (int i = 0; i < count - 1; i++)
				drawLine(vertices[i], vertices[i + 1], color);
			drawLine(vertices[count - 1], vertices[0], color);
		}

		private void drawCircle(Vector2 center, float radius, Color color)
		{
			const double increment = Math.PI * 2.0 / _circleSegments;
			double theta = 0.0;

			for (int i = 0; i < _circleSegments; i++)
			{
				Vector2 v1 = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
				Vector2 v2 = center +
							 radius *
							 new Vector2((float)Math.Cos(theta + increment), (float)Math.Sin(theta + increment));

				drawLine(v1, v2, color);
				theta += increment;
			}
		}

		public void drawString(SpriteFont font, string text, Vector2 pos, Color col, float rotation = 0f, float scale = 0f, SpriteEffects effects = SpriteEffects.None, float depth = 0f)
		{
			_batch.DrawString(font, text, pos, col, rotation, Vector2.Zero, scale, effects, depth);
		}

		public void drawTexture(Texture2D texture, Rectangle rect, Color col)
		{
			_batch.Draw(texture, rect, col);
		}

		public void drawTexture(Texture2D texture, Vector2 pos, Color col)
		{
			_batch.Draw(texture, pos, col);
		}

		public Texture2D textureFromShape(Shape shape, MaterialType mater)
		{
		return (_asset.TextureFromShape(shape, mater, Color.White, 1f));
		}
	}
}
