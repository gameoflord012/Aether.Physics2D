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
using nkast.Aether.Physics2D.Dynamics.Contacts;

namespace nkast.Aether.Physics2D.Samples.Testbed.Tests
{
    /// <summary>
    /// This is a test of collision filtering.
    /// There is a triangle, a box, and a circle.
    /// There are 7 shapes. 4 large and 3 small.
    /// The 3 small ones always collide.
    /// The 4 large ones never collide.
    /// The boxes don't collide with triangles (except if both are small).
    /// </summary>
    public class CollisionFilteringTest : Test
    {
        private const short SmallGroup = 1;
        private const short LargeGroup = -1;

        private const Category TriangleCategory = Category.Cat2;
        private const Category BoxCategory = Category.Cat3;
        private const Category CircleCategory = Category.Cat4;

        private const Category TriangleMask = Category.All;
        private const Category BoxMask = Category.All ^ TriangleCategory;
        private const Category CircleMask = Category.All;

        Fixture circleFixture3;

        public CollisionFilteringTest()
        {
            //Ground
            World.CreateEdge(new Vector2(-40.0f, 0.0f), new Vector2(40.0f, 0.0f));

            {
                // Small triangle
                Vertices vertices = new Vertices(3);
                vertices.Add(new Vector2(-1.0f, 0.0f));
                vertices.Add(new Vector2(1.0f, 0.0f));
                vertices.Add(new Vector2(0.0f, 2.0f));
                PolygonShape polygon = new PolygonShape(vertices, 1);

                Body triangleBody = World.CreateBody();
                triangleBody.BodyType = BodyType.Dynamic;
                triangleBody.Position = new Vector2(-5.0f, 2.0f);

                Fixture triangleFixture = triangleBody.CreateFixture(polygon);
                triangleFixture.CollisionGroup = SmallGroup;
                triangleFixture.CollisionCategories = TriangleCategory;
                triangleFixture.CollidesWith = TriangleMask;

                // Large triangle (recycle definitions)
                vertices[0] *= 2.0f;
                vertices[1] *= 2.0f;
                vertices[2] *= 2.0f;
                polygon.Vertices = vertices;

                Body triangleBody2 = World.CreateBody();
                triangleBody2.BodyType = BodyType.Dynamic;
                triangleBody2.Position = new Vector2(-5.0f, 6.0f);
                triangleBody2.FixedRotation = true; // look at me!

                Fixture triangleFixture2 = triangleBody2.CreateFixture(polygon);
                triangleFixture2.CollisionGroup = LargeGroup;
                triangleFixture2.CollisionCategories = TriangleCategory;
                triangleFixture2.CollidesWith = TriangleMask;

                {
                    Body body = World.CreateBody();
                    body.BodyType = BodyType.Dynamic;
                    body.Position = new Vector2(-5.0f, 10.0f);

                    Vertices box = PolygonTools.CreateRectangle(0.5f, 1.0f);
                    PolygonShape p = new PolygonShape(box, 1);
                    body.CreateFixture(p);

                    PrismaticJoint jd = new PrismaticJoint(triangleBody2, body, new Vector2(0, 4), Vector2.Zero, new Vector2(0.0f, 1.0f));
                    jd.LimitEnabled = true;
                    jd.LowerLimit = -1.0f;
                    jd.UpperLimit = 1.0f;

                    World.Add(jd);
                }

                // Small box
                polygon.Vertices = PolygonTools.CreateRectangle(1.0f, 0.5f);

                Body boxBody = World.CreateBody();
                boxBody.BodyType = BodyType.Dynamic;
                boxBody.Position = new Vector2(0.0f, 2.0f);

                Fixture boxFixture = boxBody.CreateFixture(polygon);
                boxFixture.Restitution = 0.1f;

                boxFixture.CollisionGroup = SmallGroup;
                boxFixture.CollisionCategories = BoxCategory;
                boxFixture.CollidesWith = BoxMask;

                // Large box (recycle definitions)
                polygon.Vertices = PolygonTools.CreateRectangle(2, 1);

                Body boxBody2 = World.CreateBody();
                boxBody2.BodyType = BodyType.Dynamic;
                boxBody2.Position = new Vector2(0.0f, 6.0f);

                Fixture boxFixture2 = boxBody2.CreateFixture(polygon);
                boxFixture2.CollisionGroup = LargeGroup;
                boxFixture2.CollisionCategories = BoxCategory;
                boxFixture2.CollidesWith = BoxMask;

                // Small circle
                CircleShape circle = new CircleShape(1.0f, 1);

                Body circleBody = World.CreateBody();
                circleBody.BodyType = BodyType.Dynamic;
                circleBody.Position = new Vector2(5.0f, 2.0f);

                Fixture circleFixture = circleBody.CreateFixture(circle);

                circleFixture.CollisionGroup = SmallGroup;
                circleFixture.CollisionCategories = CircleCategory;
                circleFixture.CollidesWith = CircleMask;

                // Large circle
                circle.Radius *= 2.0f;

                Body circleBody2 = World.CreateBody();
                circleBody2.BodyType = BodyType.Dynamic;
                circleBody2.Position = new Vector2(5.0f, 6.0f);

                Fixture circleFixture2 = circleBody2.CreateFixture(circle);
                circleFixture2.CollisionGroup = LargeGroup;
                circleFixture2.CollisionCategories = CircleCategory;
                circleFixture2.CollidesWith = CircleMask;

                // Large circle - Ignore with other large circle
                Body circleBody3 = World.CreateBody();
                circleBody3.BodyType = BodyType.Dynamic;
                circleBody3.Position = new Vector2(6.0f, 9.0f);

                //Another large circle. This one uses IgnoreCollisionWith() logic instead of categories.
                circleFixture3 = circleBody3.CreateFixture(circle);
                circleFixture3.CollisionGroup = LargeGroup;
                circleFixture3.CollisionCategories = CircleCategory;
                circleFixture3.CollidesWith = CircleMask;

                circleFixture3.BeforeCollision = circleFixture3_BeforeCollision;
            }
        }

        bool circleFixture3_BeforeCollision(Fixture sender, Fixture other)
        {
            if (other == circleFixture3)
                return false;

            return true;
        }

    }
}