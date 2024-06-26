/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

/*
* Farseer Physics Engine:
* Copyright (c) 2012 Ian Qvist
* 
* Original source Box2D:
* Copyright (c) 2006-2011 Erin Catto http://www.box2d.org 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using System;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Collision;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using nkast.Aether.Physics2D.Samples.Testbed.Framework;

namespace nkast.Aether.Physics2D.Samples.Testbed.Tests
{
    public class BreakableTest : Test
    {
        private float _angularVelocity;
        private Body _body1;
        private bool _break;
        private bool _broke;
        private Fixture _piece1;
        private Fixture _piece2;
        private PolygonShape _shape2;
        private Vector2 _velocity;

        public BreakableTest()
        {
            // Ground body
            World.CreateEdge(new Vector2(-40.0f, 0.0f), new Vector2(40.0f, 0.0f));

            // Breakable dynamic body
            _body1 = World.CreateBody();
            _body1.BodyType = BodyType.Dynamic;
            _body1.Position = new Vector2(0.0f, 40.0f);
            _body1.Rotation = 0.25f * MathHelper.Pi;

            Vertices box = PolygonTools.CreateRectangle(0.5f, 0.5f, new Vector2(-0.5f, 0.0f), 0.0f);

            PolygonShape shape1 = new PolygonShape(box, 1);
            _piece1 = _body1.CreateFixture(shape1);

            box = PolygonTools.CreateRectangle(0.5f, 0.5f, new Vector2(0.5f, 0.0f), 0.0f);
            _shape2 = new PolygonShape(box, 1);

            _piece2 = _body1.CreateFixture(_shape2);

            _break = false;
            _broke = false;
        }
        
        protected override void PostSolve(Contact contact, ContactVelocityConstraint impulse)
        {
            if (_broke)
            {
                // The body already broke.
                return;
            }

            // Should the body break?
            float maxImpulse = 0.0f;
            Manifold manifold = contact.Manifold;

            for (int i = 0; i < manifold.PointCount; ++i)
            {
                maxImpulse = Math.Max(maxImpulse, impulse.points[i].normalImpulse);
            }

            if (maxImpulse > 40.0f)
            {
                // Flag the body for breaking.
                _break = true;
            }
        }

        private void Break()
        {
            // Create two bodies from one.
            Body body1 = _piece1.Body;
            Vector2 center = body1.WorldCenter;

            body1.Remove(_piece2);
            _piece2 = null;

            Body body2 = World.CreateBody();
            body2.BodyType = BodyType.Dynamic;
            body2.Position = body1.Position;
            body2.Rotation = body1.Rotation;

            _piece2 = body2.CreateFixture(_shape2);

            // Compute consistent velocities for new bodies based on
            // cached velocity.
            Vector2 center1 = body1.WorldCenter;
            Vector2 center2 = body2.WorldCenter;

            Vector2 center1Diff = center1 - center;
            Vector2 center2Diff = center2 - center;

            Vector2 velocity1 = _velocity + MathUtils.Cross(_angularVelocity, ref center1Diff);
            Vector2 velocity2 = _velocity + MathUtils.Cross(_angularVelocity, ref center2Diff);

            body1.AngularVelocity = _angularVelocity;
            body1.LinearVelocity = velocity1;

            body2.AngularVelocity = _angularVelocity;
            body2.LinearVelocity = velocity2;
        }

        public override void Update(GameSettings settings, GameTime gameTime)
        {
            if (_break)
            {
                Break();
                _broke = true;
                _break = false;
            }

            // Cache velocities to improve movement on breakage.
            if (_broke == false)
            {
                _velocity = _body1.LinearVelocity;
                _angularVelocity = _body1.AngularVelocity;
            }

            base.Update(settings, gameTime);
        }

    }
}