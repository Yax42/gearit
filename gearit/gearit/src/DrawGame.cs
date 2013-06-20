using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using Microsoft.Xna.Framework;

namespace gearit.src
{
    class DrawGame
    {
        // a basic effect, which contains the shaders that we will use to draw our
        // primitives. (it's about gpu)
        private BasicEffect _basicEffect;
        private VertexPositionColor[] _lineVertices;
        private AssetCreator _asset;
        private GraphicsDevice _device;
        private int _count;
        private SpriteBatch _batch;

        public DrawGame(GraphicsDevice device)
        {
            _batch = new SpriteBatch(device);
            _device = device;
            _asset = new AssetCreator(device);
            _lineVertices = new VertexPositionColor[50000];

            // set up a new basic effect, and enable vertex colors.
            _basicEffect = new BasicEffect(device);
            _basicEffect.VertexColorEnabled = true;
        }

        public SpriteBatch Batch()
        {
            return (_batch);
        }

        public void Begin(Viewport viewport, ICamera camera)
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
    }
}
