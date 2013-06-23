using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;

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

        public DrawGame(GraphicsDevice device)
        {
            _batch = new SpriteBatch(device);
            _device = device;
            _asset = new AssetCreator(device);

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

        public void addLine(Vector2 p1, Vector2 p2, Color col)
        {
            if (_count >= _lineVertices.Length)
            {
                flushLines();
            }
            _lineVertices[_count++] = new VertexPositionColor(new Vector3(p1, 0), col);
            _lineVertices[_count++] = new VertexPositionColor(new Vector3(p2, 0f), col);
        }

        public void End()
        {
            flushLines();
            _batch.End();
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
                        addLine(v1, v2, color);
                    }
                    break;
            }
        }

        private void drawPolygon(Vector2[] vertices, int count, Color color)
        {
            for (int i = 0; i < count - 1; i++)
                addLine(vertices[i], vertices[i + 1], color);
            addLine(vertices[count - 1], vertices[0], color);
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

                addLine(v1, v2, color);
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
