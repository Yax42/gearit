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
	public class DrawGame
	{
		// a basic effect, which contains the shaders that we will use to draw our
		// primitives. (it's about gpu)
		private const int _circleSegments = 32;
		private BasicEffect _basicEffect;
		private VertexPositionColor[] _lineVertices = new VertexPositionColor[500];
		private VertexPositionColor[] _triangleVertices = new VertexPositionColor[5];
		private int _lineVertsCount = 0;
		private int _triangleVertsCount = 0;
		private AssetCreator _asset;
		private GraphicsDevice _device;
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
			BeginPrimitive(camera);
			return;
			//tell our basic effect to begin.
			_batch.Begin();
			_basicEffect.Projection = camera.projection();
			_basicEffect.View = camera.view();
			_basicEffect.CurrentTechnique.Passes[0].Apply();
		}

		public void Begin()
		{
			_primitiveBatch.Begin(ref _staticProj, ref _staticView);

			return;
			_batch.Begin();
			_basicEffect.Projection = _staticProj;
			_basicEffect.View = _staticView;
			_basicEffect.CurrentTechnique.Passes[0].Apply();
		}

		public void BeginPrimitive(ICamera camera)
		{
            // C'est à cause de ça que c'est pas aligné. Faut trouver un moyen d'envoyer camera.projection() et camera.view() a _primitiveBatch(ref x, ref x)
			/*
			Matrix projection = Matrix.CreateOrthographicOffCenter(0f, ConvertUnits.ToSimUnits(_device.Viewport.Width),
											 ConvertUnits.ToSimUnits(_device.Viewport.Height), 0f, 0f,
											 1f);
			Vector2 screen_center = new Vector2(_device.Viewport.Width / 2f,
												_device.Viewport.Height / 2f);
			Matrix view = Matrix.CreateTranslation(new Vector3((ConvertUnits.ToSimUnits(Vector2.Zero) -
			ConvertUnits.ToSimUnits(screen_center)), 0f)) * Matrix.CreateTranslation(new Vector3(ConvertUnits.ToSimUnits(screen_center), 0f));
			*/
			Matrix view = camera.view();
			Matrix projection = camera.projection();
			_primitiveBatch.Begin(ref projection, ref view);
		}

		public void End()
		{
			_primitiveBatch.End();

			return;
			flushTriangles();
			flushLines();
			_batch.End();
		}

		public void EndPrimitive()
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
						CircleShape circle = (CircleShape)fixture.Shape;

						Vector2 center = MathUtils.Multiply(ref xf, circle.Position);
						float radius = circle.Radius;

						drawCircle(center, radius, color, true);
					}
					break;

				case ShapeType.Polygon:
					{
						PolygonShape poly = (PolygonShape)fixture.Shape;
						int vertexCount = poly.Vertices.Count;
						var ver = new Vector2[vertexCount];

						for (int i = 0; i < vertexCount; ++i)
							ver[i] = MathUtils.Multiply(ref xf, poly.Vertices[i]);

						for (int i = 1; i < vertexCount - 1; i++)
						{
							_primitiveBatch.AddVertex(ver[0], color, PrimitiveType.TriangleList);
							_primitiveBatch.AddVertex(ver[i], color, PrimitiveType.TriangleList);
							_primitiveBatch.AddVertex(ver[i + 1], color, PrimitiveType.TriangleList);
						}
							//drawTriangle(ver[0], ver[i], ver[i + 1], Color.Orange);
					}
					break;
			}
		}

		public void drawTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color col)
		{
			if (_triangleVertsCount >= _triangleVertices.Length - 3)
			{
				flushTriangles();
			}
			_lineVertices[_triangleVertsCount++] = new VertexPositionColor(new Vector3(p1, 0), col);
			_lineVertices[_triangleVertsCount++] = new VertexPositionColor(new Vector3(p2, 0f), col);
			_lineVertices[_triangleVertsCount++] = new VertexPositionColor(new Vector3(p3, 0f), col);
		}

		public void drawLine(Vector2 p1, Vector2 p2, Color col)
		{
			_primitiveBatch.AddVertex(p1, col, PrimitiveType.LineList);
			_primitiveBatch.AddVertex(p2, col, PrimitiveType.LineList);
			return;
			if (_lineVertsCount >= _lineVertices.Length - 2)
			{
				flushLines();
			}
			_lineVertices[_lineVertsCount++] = new VertexPositionColor(new Vector3(p1, 0), col);
			_lineVertices[_lineVertsCount++] = new VertexPositionColor(new Vector3(p2, 0f), col);
		}

		private void flushTriangles()
		{
			if (_triangleVertsCount >= 3)
			{
				_device.SamplerStates[0] = SamplerState.LinearClamp;
				_device.DrawUserPrimitives(PrimitiveType.TriangleList, _triangleVertices, 0, _triangleVertsCount / 3);
				_triangleVertsCount = 0;
			}
		}

		private void flushLines()
		{
			if (_lineVertsCount >= 2)
			{
				_device.SamplerStates[0] = SamplerState.AnisotropicClamp;
				_device.DrawUserPrimitives(PrimitiveType.LineList, _lineVertices, 0, _lineVertsCount / 2);
				_lineVertsCount = 0;
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

						drawCircle(center, radius, color, false);
						float angle = fixture.Body.Rotation;
						drawLine(center, center + new Vector2((float)(Math.Cos(angle)) * radius, (float) (Math.Sin(angle) * radius)), color);
					}
					break;

				case ShapeType.Polygon:
					{
						PolygonShape poly = (PolygonShape)fixture.Shape;
						int vertexCount = poly.Vertices.Count;
						var ver = new Vector2[vertexCount];

						for (int i = 0; i < vertexCount; ++i)
						{
							ver[i] = MathUtils.Multiply(ref xf, poly.Vertices[i]);
						}

						drawPolygon(ver, 0, vertexCount, color);
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

		public void paintPolygon(Vector2[] vertices, int from, int count, Color color)
		{
			for (int i = from + 1; i < count + from - 1; i++)
			{
				_primitiveBatch.AddVertex(vertices[from], color, PrimitiveType.TriangleList);
				_primitiveBatch.AddVertex(vertices[i], color, PrimitiveType.TriangleList);
				_primitiveBatch.AddVertex(vertices[i + 1], color, PrimitiveType.TriangleList);
			}
		}

		public void drawPolygon(Vector2[] vertices, int from, int count, Color color)
		{
			color.A = 128;
			paintPolygon(vertices, from, count, color);
			for (int i = from; i < count + from - 1; i++)
				drawLine(vertices[i], vertices[i + 1], color);
			drawLine(vertices[from + count - 1], vertices[from], color);
		}

		public void drawCircle(Vector2 center, float radius, Color color, bool paint = false)
		{
			color.A = 128;
			const double increment = Math.PI * 2.0 / _circleSegments;
			double theta = 0.0;
			Vector2[] vers = new Vector2[_circleSegments];


			for (int i = 0; i < _circleSegments; i++)
			{
				//Vector2 v1 = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
				vers[i] = center +
							 radius *
							 new Vector2((float)Math.Cos(theta + increment), (float)Math.Sin(theta + increment));
				//drawLine(v1, v2, color);
				theta += increment;
			}
			drawPolygon(vers, 0, _circleSegments, color);
			paintPolygon(vers, 0, _circleSegments, color);
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
