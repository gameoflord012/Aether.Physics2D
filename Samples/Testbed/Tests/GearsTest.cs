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

using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Joints;
using nkast.Aether.Physics2D.Samples.Testbed.Framework;
using Microsoft.Xna.Framework;

namespace nkast.Aether.Physics2D.Samples.Testbed.Tests
{
    public class GearsTest : Test
    {
        private GearJoint _joint5;
        private GearJoint _joint4;
        private PrismaticJoint _joint3;
        private RevoluteJoint _joint2;
        private RevoluteJoint _joint1;

        public GearsTest()
        {
            Body ground = World.CreateEdge(new Vector2(50.0f, 0.0f), new Vector2(-50.0f, 0.0f));

            {
                CircleShape circle1 = new CircleShape(1.0f, 5f);

                PolygonShape box = new PolygonShape(5f);
                box.Vertices = PolygonTools.CreateRectangle(0.5f, 5.0f);

                CircleShape circle2 = new CircleShape(2.0f, 5f);

                Body body1 = World.CreateBody(new Vector2(10.0f, 9.0f));
                body1.CreateFixture(circle1);

                Body body2 = World.CreateBody(new Vector2(10.0f, 8.0f));
                body2.BodyType = BodyType.Dynamic;
                body2.CreateFixture(box);

                Body body3 = World.CreateBody(new Vector2(10.0f, 6.0f));
                body3.BodyType = BodyType.Dynamic;
                body3.CreateFixture(circle2);

                RevoluteJoint joint1 = new RevoluteJoint(body2, body1, body1.Position, true);
                World.Add(joint1);

                RevoluteJoint joint2 = new RevoluteJoint(body2, body3, body3.Position, true);
                World.Add(joint2);

                GearJoint joint4 = new GearJoint(body1, body3, joint1, joint2, circle2.Radius / circle1.Radius);
                World.Add(joint4);
            }

            {
                CircleShape circle1 = new CircleShape(1.0f, 5.0f);

                CircleShape circle2 = new CircleShape(2.0f, 5.0f);

                PolygonShape box = new PolygonShape(5f);
                box.Vertices = PolygonTools.CreateRectangle(0.5f, 5.0f);

                Body body1 = World.CreateBody(new Vector2(-3.0f, 12.0f));
                body1.BodyType = BodyType.Dynamic;
                body1.CreateFixture(circle1);

                _joint1 = new RevoluteJoint(ground, body1, body1.Position, true);
                _joint1.ReferenceAngle = body1.Rotation - ground.Rotation;
                World.Add(_joint1);

                Body body2 = World.CreateBody(new Vector2(0.0f, 12.0f));
                body2.BodyType = BodyType.Dynamic;
                body2.CreateFixture(circle2);

                _joint2 = new RevoluteJoint(ground, body2, body2.Position, true);
                World.Add(_joint2);

                Body body3 = World.CreateBody(new Vector2(2.5f, 12.0f));
                body3.BodyType = BodyType.Dynamic;
                body3.CreateFixture(box);

                _joint3 = new PrismaticJoint(ground, body3, body3.Position, new Vector2(0.0f, 1.0f), true);
                _joint3.LowerLimit = -5.0f;
                _joint3.UpperLimit = 5.0f;
                _joint3.LimitEnabled = true;

                World.Add(_joint3);

                _joint4 = new GearJoint(body1, body2, _joint1, _joint2, circle2.Radius / circle1.Radius);
                World.Add(_joint4);

                _joint5 = new GearJoint(body2, body3, _joint2, _joint3, -1.0f / circle2.Radius);
                World.Add(_joint5);
            }
        }

        public override void Update(GameSettings settings, GameTime gameTime)
        {
            base.Update(settings, gameTime);

            float ratio = _joint4.Ratio;
            float value = _joint1.JointAngle + ratio * _joint2.JointAngle;
            DrawString(string.Format("theta1 + {0} * theta2 = {1}", ratio, value));

            ratio = _joint5.Ratio;
            value = _joint2.JointAngle + ratio * _joint3.JointTranslation;
            DrawString(string.Format("theta2 + {0} * delta = {1}", ratio, value));
        }

    }
}