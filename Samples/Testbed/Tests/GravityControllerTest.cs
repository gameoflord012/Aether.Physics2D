﻿/* Original source Farseer Physics Engine:
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
using nkast.Aether.Physics2D.Controllers;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Samples.Testbed.Framework;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Common.PhysicsLogic;

namespace nkast.Aether.Physics2D.Samples.Testbed.Tests
{
    public class GravityControllerTest : Test
    {
        public GravityControllerTest()
        {
            //Ground
            World.CreateEdge(new Vector2(-40.0f, 0.0f), new Vector2(40.0f, 0.0f));

            //Create the gravity controller
            GravityController gravity = new GravityController(20);
            gravity.DisabledOnGroup = 3;
            gravity.EnabledOnGroup = 2;
            gravity.DisabledOnCategories = Category.Cat2;
            gravity.EnabledOnCategories = Category.Cat3;
            gravity.ControllerCategory = ControllerCategory.Cat14; // set ControllerCategory to a unique category
            gravity.GravityType = GravityType.Linear;

            World.Add(gravity);

            Vector2 startPosition = new Vector2(-10, 2);
            Vector2 offset = new Vector2(2);

            //Create the planet
            Body planet = World.CreateBody();
            planet.Position = new Vector2(0, 20);

            CircleShape planetShape = new CircleShape(2, 1);
            planet.CreateFixture(planetShape);

            //Add the planet as the one that has gravity
            gravity.AddBody(planet);

            //Create 10 smaller circles
            for (int i = 0; i < 10; i++)
            {
                Body circle = World.CreateBody();
                circle.BodyType = BodyType.Dynamic;
                circle.Position = startPosition + offset * i;
                circle.SleepingAllowed = false;

                CircleShape circleShape = new CircleShape(1, 0.1f);
                Fixture fix = circle.CreateFixture(circleShape);
                fix.CollisionCategories = Category.Cat3;
                fix.CollisionGroup = 2;

                if (i == 4)
                    circle.ControllerFilter.IgnoreController(gravity.ControllerCategory);

                if (i == 5)
                {
                    fix.CollisionCategories = Category.Cat2;
                    fix.Body.IgnoreGravity = true;
                }

                if (i == 6)
                    fix.CollisionGroup = 3;
            }
        }

    }
}