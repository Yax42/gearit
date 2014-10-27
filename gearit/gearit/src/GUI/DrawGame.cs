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
using gearit.src.robot;

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

        public void drawBackground(Texture2D _b, ICamera camera, Effect _effect)
        {
            _effect.Parameters["ViewportSize"].SetValue(new Vector2(_device.Viewport.Width, _device.Viewport.Height));
            _effect.Parameters["ScrollMatrix"].SetValue(camera.GetScrollMatrix(new Vector2(_b.Width, _b.Height)));
            _batch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null, _effect);
            _batch.Draw(_b, _device.Viewport.Bounds, _device.Viewport.Bounds, Color.White);
            _batch.End();
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

		public void BeginPrimitive(ICamera camera, Matrix proj)
		{
			Matrix view = camera.view();
			_primitiveBatch.Begin(ref proj, ref view);
		}

		public void BeginPrimitive(ICamera camera)
		{
			Matrix view = camera.view();
			Matrix proj = camera.projection();
			_primitiveBatch.Begin(ref proj, ref view);
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
			switch (fixture.Shape.ShapeType)
			{
				case ShapeType.Circle:
					{
						CircleShape circle = (CircleShape)fixture.Shape;

						Vector2 center = MathUtils.Mul(ref xf, circle.Position);
						float radius = circle.Radius;

						DrawCircle(center, radius, color, true);
					}
					break;

				case ShapeType.Polygon:
					{
						PolygonShape poly = (PolygonShape)fixture.Shape;
						int vertexCount = poly.Vertices.Count;
						var ver = new Vector2[vertexCount];

						for (int i = 0; i < vertexCount; ++i)
							ver[i] = MathUtils.Mul(ref xf, poly.Vertices[i]);

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

		public void drawSquare(Vector2 pos, float ray, Color col, bool full)
		{
			if (full)
			{
				_primitiveBatch.AddVertex(pos + new Vector2(-ray, -ray), col, PrimitiveType.TriangleList);
				_primitiveBatch.AddVertex(pos + new Vector2(ray, -ray), col, PrimitiveType.TriangleList);
				_primitiveBatch.AddVertex(pos + new Vector2(-ray, ray), col, PrimitiveType.TriangleList);

				_primitiveBatch.AddVertex(pos + new Vector2(-ray, ray), col, PrimitiveType.TriangleList);
				_primitiveBatch.AddVertex(pos + new Vector2(ray, -ray), col, PrimitiveType.TriangleList);
				_primitiveBatch.AddVertex(pos + new Vector2(ray, ray), col, PrimitiveType.TriangleList);
			}
			else
			{
				DrawLine(pos + new Vector2(-ray, -ray), pos + new Vector2(ray, -ray), col);
				DrawLine(pos + new Vector2(ray, -ray), pos + new Vector2(ray, ray), col);
				DrawLine(pos + new Vector2(ray, ray), pos + new Vector2(-ray, ray), col);
				DrawLine(pos + new Vector2(-ray, ray), pos + new Vector2(-ray, -ray), col);
			}
		}

		public void DrawLNumber(int v, Vector2 p, float alpha)
		{
			/*
			float delta = (float) (2f * Math.PI) / 10f;

			for (int i = 0; i < 10; i++)
			{
				Color col = new Color(1f - ((i + 2) / 3) / 3f, 1f - ((i + 2) / 3) / 3f,1, alpha );

				var pos = ori + 0.5f * new Vector2((float)Math.Cos(i * delta),
					(float)Math.Sin(i * delta));
				DrawGame.DrawCircle(pos, (i == unit ? power * 0.05f : 0.05f),
					(i == unit) ? new Color(0.5f, 0.2f + power / 10f, power / 20f, 1) : Color.Brown , true);
			}
			*/
		}

		public void DrawLine(Vector2 p1, Vector2 p2, Color col)
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

		public void draw(Body b, Color col, int a = 128)
		{
			col.A = (byte) a;
			Transform xf;
			b.GetTransform(out xf);
			foreach (Fixture f in b.FixtureList)
				draw(f, xf, col);
#if DRAW_DEBUG
			if (b.GetType() == typeof(Rod))
			{
				Rod p = (Rod)b;
				drawCircle(p.TMP_pos, 1 + p.TMP_dist, new Color(1, 0,0,0.03f), false);
			}
#endif

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
			switch (fixture.Shape.ShapeType)
			{
				case ShapeType.Circle:
					{
						CircleShape circle = (CircleShape)fixture.Shape;

						Vector2 center = MathUtils.Mul(ref xf, circle.Position);
						float radius = circle.Radius;

						DrawCircle(center, radius, color, false);
						float angle = fixture.Body.Rotation;
						DrawLine(center, center + new Vector2((float)(Math.Cos(angle)) * radius, (float) (Math.Sin(angle) * radius)), color);
					}
					break;

				case ShapeType.Polygon:
					{
						PolygonShape poly = (PolygonShape)fixture.Shape;
						int vertexCount = poly.Vertices.Count;
						var ver = new Vector2[vertexCount];

						for (int i = 0; i < vertexCount; ++i)
						{
							ver[i] = MathUtils.Mul(ref xf, poly.Vertices[i]);
						}

						drawPolygon(ver, 0, vertexCount, color);
					}
					break;

				case ShapeType.Edge:
					{
						EdgeShape edge = (EdgeShape)fixture.Shape;
						Vector2 v1 = MathUtils.Mul(ref xf, edge.Vertex1);
						Vector2 v2 = MathUtils.Mul(ref xf, edge.Vertex2);
						DrawLine(v1, v2, color);
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

		public void DrawGrille()
		{
			DrawGrille(Vector2.Zero, 100, 3);
		}

		public void DrawGrille(Vector2 center, int count, int size)
		{
			Vector2 origin = center - new Vector2(count * size / 2);
			int globalSize = count * size;
			Color col = Color.Cyan;
			col.A = 30;

			for (int i = 1; i < count; i++)
			{
				DrawLine(origin + new Vector2(i * size, 0),
					origin + new Vector2(i * size, globalSize),
					col);
				DrawLine(origin + new Vector2(0, i * size),
					origin + new Vector2(globalSize, i * size),
					col);
			}

		}

		public void drawPolygon(Vector2[] vertices, int from, int count, Color color)
		{
			paintPolygon(vertices, from, count, color);
			for (int i = from; i < count + from - 1; i++)
				DrawLine(vertices[i], vertices[i + 1], color);
			DrawLine(vertices[from + count - 1], vertices[from], color);
		}

		public void DrawCircle(Vector2 center, float radius, Color color, bool paint = false)
		{
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

		public void DrawSpirale(Vector2 pos, int total, int id, int size)
		{
			for (int i = id; i >= 0 && i >= id - size; i--)
			{
				int intDelta = i % total;
				intDelta = 1 + (intDelta > total / 2 ? total - intDelta : intDelta);
				float delta = intDelta / (total / 2f);
				float ray = 1 + delta * delta * 3;
				Color col = new Color(new Vector4(0f, delta, 0f, 1f));
				Vector2 deltaPos = new Vector2((float)Math.Cos(i * 10 * Math.PI / total), (float)Math.Sin(i * 10 * Math.PI / total)) * 10f * delta;
				DrawCircle(pos + deltaPos, ray, col, true);
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
