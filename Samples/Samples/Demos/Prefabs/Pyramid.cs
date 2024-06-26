﻿/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using System.Collections.Generic;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Samples.DrawingSystem;
using nkast.Aether.Physics2D.Samples.ScreenSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace nkast.Aether.Physics2D.Samples.Demos.Prefabs
{
    public class Pyramid
    {
        private Sprite _box;
        private List<Body> _boxes;
        private SpriteBatch _batch;

        public Pyramid(World world, ScreenManager screenManager, Vector2 position, int count, float density)
        {
            _batch = screenManager.SpriteBatch;

            Vertices rect = PolygonTools.CreateRectangle(1f/2f, 1f/2f);
            PolygonShape shape = new PolygonShape(rect, density);

            Vector2 rowStart = position;
            rowStart.Y -= 0.5f + count * 1.1f;

            Vector2 deltaRow = new Vector2(-0.625f, -1.1f);
            const float spacing = 1.25f;

            _boxes = new List<Body>();

            for (int i = 0; i < count; ++i)
            {
                Vector2 pos = rowStart;

                for (int j = 0; j < i + 1; ++j)
                {
                    Body body = world.CreateBody();
                    body.BodyType = BodyType.Dynamic;
                    body.Position = pos;
                    body.CreateFixture(shape);
                    _boxes.Add(body);

                    pos.X += spacing;
                }

                rowStart += deltaRow;
            }


            //GFX
            AssetCreator creator = screenManager.Assets;
            _box = new Sprite(creator.TextureFromVertices(rect, MaterialType.Dots, Color.SaddleBrown, 2f, 24f));
        }

        public void Draw()
        {
            for (int i = 0; i < _boxes.Count; ++i)
            {
                _batch.Draw(_box.Texture, _boxes[i].Position, null, Color.White, _boxes[i].Rotation, _box.Origin, new Vector2(1f, 1f) * _box.TexelSize, SpriteEffects.FlipVertically, 0f);
            }
        }
    }
}