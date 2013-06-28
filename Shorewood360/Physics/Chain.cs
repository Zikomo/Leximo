using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics;
using Microsoft.Xna.Framework.Graphics;

namespace IETGames.Shorewood.Physics
{
    public enum LinkAttachLocation { UpperLeft, UpperRight, BottomLeft, BottomRight, UpperMiddle };
    public struct BodyNode
    {
        public Body body;
        public Texture2D texture;
        public Vector2 bounds;
        public bool isVisible;
        public bool isLink;

        public BodyNode(Body body, Texture2D texture)
        {

            this.body = body;
            this.texture = texture;
            if (texture != null)
            {
                this.bounds = new Vector2(texture.Width, texture.Height);
            }
            else
            {
                this.bounds = new Vector2(10, 10);
            }
            isVisible = true;
            isLink = true;
        }

        public BodyNode(Body body, Texture2D texture, bool isLink)
        {

            this.body = body;
            this.texture = texture;
            if (texture != null)
            {
                this.bounds = new Vector2(texture.Width, texture.Height);
            }
            else
            {
                this.bounds = new Vector2(10, 10);
            }
            isVisible = true;
            this.isLink = isLink;
        }
    }
    public class Chain
    {
        int count;
        int blockSize = 10;
        public List<BodyNode> links;
        public List<BodyNode> bodies;
        private Vector2 startPosition;
        private List<PinJoint> joints;

        PhysicsSimulator simulator;
        public int chainMass = 1;
        public Chain(PhysicsSimulator simulator, int links, Vector2 startPosition, int blockSize, int chainMass)
        {
            this.startPosition = startPosition;
            this.chainMass = chainMass;
            this.simulator = simulator;
            this.blockSize = blockSize;
            bodies = new List<BodyNode>(links);
            joints = new List<PinJoint>(links);
            this.links = new List<BodyNode>(links);
            Body previousBody = BodyFactory.Instance.CreateRectangleBody(simulator, this.blockSize, this.blockSize, this.chainMass);
            previousBody.Position = this.startPosition;
            previousBody.IsStatic = true;

            previousBody.Rotation = 0;
            this.links.Add(new BodyNode(previousBody, null));
            count++;
            for (int i = 1; i < links; i++)
            {
                AttachEnd(new BodyNode(BodyFactory.Instance.CreateRectangleBody(simulator, this.blockSize, this.blockSize, this.chainMass), null), 0);
            }
        }

        public void AttachEnd(BodyNode body, float initialRotation)
        {
            int end = links.Count - 1;
            //AttachEnd(body, initialRotation, (bodies[end].bounds.X - 2) * Vector2.UnitX, Vector2.Zero);
            AttachEnd(body, initialRotation, (10 - 2) * Vector2.UnitX, Vector2.Zero);
        }

        public void AttachEnd(BodyNode body, float initialRotation, Vector2 endAnchor, Vector2 bodyAnchor)
        {
            int end = links.Count - 1;
            PinJoint joint = null;

            //body.body.Position = bodies[end].body.Position + (bodies[end].bounds.X - 4) * Vector2.UnitX;
            body.body.Position = links[end].body.Position + (7) * Vector2.UnitX;
            body.body.Rotation = initialRotation;
            //joint = JointFactory.Instance.CreatePinJoint(simulator, bodies[end].body, endAnchor, body.body, bodyAnchor);         
            joint = JointFactory.Instance.CreatePinJoint(simulator, links[end].body, endAnchor, body.body, bodyAnchor);

            joint.Softness = 0.0000f;
            joint.TargetDistance = 0;
            joint.BiasFactor = 0.0f;
            body.body.LinearDragCoefficient = 100000;
            if (body.isLink)
            {
                links.Add(body);
            }
            else
            {
                body.isVisible = false;
                links.Add(body);
                body.isVisible = true;
                bodies.Add(body);
            }

            joints.Add(joint);
        }

        public void Reset()
        {
            Vector2 previousPosition = startPosition;
            foreach (BodyNode body in links)
            {
                body.body.Position = previousPosition + (7) * Vector2.UnitX;
                previousPosition = body.body.Position;
            }
        }

        public void CapEnd()
        {
            BodyNode end = new BodyNode(BodyFactory.Instance.CreateRectangleBody(simulator, 10, 10, 1000), null);
            end.isVisible = false;
            AttachEnd(end, 0);
        }
    }
}