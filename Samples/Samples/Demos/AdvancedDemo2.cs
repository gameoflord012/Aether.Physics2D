﻿/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Samples.Demos.Prefabs;
using nkast.Aether.Physics2D.Samples.DrawingSystem;
using nkast.Aether.Physics2D.Samples.ScreenSystem;
using nkast.Aether.Physics2D.Dynamics.Joints;
using Path = nkast.Aether.Physics2D.Common.Path;

namespace nkast.Aether.Physics2D.Samples.Demos
{
    internal class AdvancedDemo2 : PhysicsGameScreen, IDemoScreen
    {
        private Border _border;

        private List<Body> _bridgeBodies;

        private Sprite _bridgeBox;
        private List<Body> _softBodies;
        private Sprite _softBodyBox;
        private Sprite _softBodyCircle;

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Path generator";
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("TODO: Add sample description!");
            sb.AppendLine(string.Empty);
            sb.AppendLine("GamePad:");
            sb.AppendLine("  - Move cursor: left thumbstick");
            sb.AppendLine("  - Grab object (beneath cursor): A button");
            sb.AppendLine("  - Drag grabbed object: left thumbstick");
            sb.AppendLine("  - Exit to menu: Back button");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Keyboard:");
            sb.AppendLine("  - Exit to menu: Escape");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Mouse / Touchscreen");
            sb.AppendLine("  - Grab object (beneath cursor): Left click");
            sb.AppendLine("  - Drag grabbed object: move mouse / finger");
            return sb.ToString();
        }

        #endregion

        public override void LoadContent()
        {
            base.LoadContent();

            World.Gravity = new Vector2(0, -9.82f);

            _border = new Border(World, ScreenManager, Camera);

            /* Bridge */
            //We make a path using 2 points.
            Path bridgePath = new Path();
            bridgePath.Add(new Vector2(-15, -5));
            bridgePath.Add(new Vector2(15, -5));
            bridgePath.Closed = false;

            Vertices box = PolygonTools.CreateRectangle(0.25f/2f, 1f/2f);
            PolygonShape shape = new PolygonShape(box, 20);

            _bridgeBodies = PathManager.EvenlyDistributeShapesAlongPath(World, bridgePath, shape, BodyType.Dynamic, 29);
            _bridgeBox = new Sprite(ScreenManager.Assets.TextureFromShape(shape, MaterialType.Dots, Color.SandyBrown, 1f));

            //Attach the first and last fixtures to the world
            JointFactory.CreateRevoluteJoint(World, HiddenBody, _bridgeBodies[0], new Vector2(0f, 0.5f));
            JointFactory.CreateRevoluteJoint(World, HiddenBody, _bridgeBodies[_bridgeBodies.Count - 1], new Vector2(0, -0.5f));

            PathManager.AttachBodiesWithRevoluteJoint(World, _bridgeBodies, new Vector2(0f, 0.5f), new Vector2(0f, -0.5f), false, true);

            /* Soft body */
            //We make a rectangular path.
            Path rectanglePath = new Path();
            rectanglePath.Add(new Vector2(-6, 11));
            rectanglePath.Add(new Vector2(-6, -1));
            rectanglePath.Add(new Vector2(6, -1));
            rectanglePath.Add(new Vector2(6, 11));
            rectanglePath.Closed = true;

            //Creating two shapes. A circle to form the circle and a rectangle to stabilize the soft body.
            List<Shape> shapes = new List<Shape>(2);
            shapes.Add(new PolygonShape(PolygonTools.CreateRectangle(1f/2f, 1f/2f, new Vector2(-0.1f, 0f), 0f), 1f));
            shapes.Add(new CircleShape(0.5f, 1f));

            //We distribute the shapes in the rectangular path.
            _softBodies = PathManager.EvenlyDistributeShapesAlongPath(World, rectanglePath, shapes,
                                                                      BodyType.Dynamic, 30);
            _softBodyBox = new Sprite(ScreenManager.Assets.TextureFromShape(shapes[0], MaterialType.Blank, Color.Silver * 0.8f, 1f));
            _softBodyBox.Origin += new Vector2(2.4f, 0f);
            _softBodyCircle = new Sprite(ScreenManager.Assets.TextureFromShape(shapes[1], MaterialType.Waves, Color.Silver, 1f));

            //Attach the bodies together with revolute joints. The rectangular form will converge to a circular form.
            PathManager.AttachBodiesWithRevoluteJoint(World, _softBodies, new Vector2(0f, 0.5f), new Vector2(0f, -0.5f),
                                                      true, true);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.BatchEffect.View = Camera.View;
            ScreenManager.BatchEffect.Projection = Camera.Projection;
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullNone, ScreenManager.BatchEffect);
            for (int i = 0; i < _softBodies.Count; ++i)
            {
                ScreenManager.SpriteBatch.Draw(_softBodyBox.Texture, _softBodies[i].Position, null, Color.White, _softBodies[i].Rotation, _softBodyBox.Origin, new Vector2(1f, 1f) * _softBodyBox.TexelSize, SpriteEffects.FlipVertically, 0f);
            }

            for (int i = 0; i < _softBodies.Count; ++i)
            {
                ScreenManager.SpriteBatch.Draw(_softBodyCircle.Texture, _softBodies[i].Position, null, Color.White, _softBodies[i].Rotation, _softBodyCircle.Origin, new Vector2(1f, 1f) * _softBodyCircle.TexelSize, SpriteEffects.FlipVertically, 0f);
            }

            for (int i = 0; i < _bridgeBodies.Count; ++i)
            {
                ScreenManager.SpriteBatch.Draw(_bridgeBox.Texture, _bridgeBodies[i].Position, null, Color.White, _bridgeBodies[i].Rotation, _bridgeBox.Origin, new Vector2(0.25f, 1f) * _bridgeBox.TexelSize, SpriteEffects.FlipVertically, 0f);
            }

            ScreenManager.SpriteBatch.End();
            _border.Draw();
            base.Draw(gameTime);
        }
    }
}