namespace Assets.Scripts.Craft.Parts.Modifiers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using ModApi.Craft.Parts;
    using ModApi.GameLoop;
    using ModApi.GameLoop.Interfaces;
    using UnityEngine;

    public class MissileHomingScriptScript : PartModifierScript<MissileHomingScriptData>,IFlightStart,IFlightFixedUpdate
    {
        ModApi.Craft.ICraftNode targetCraft;
        List<ModApi.Craft.ICraftNode> Crafts;
        Vector2 steering = Vector2.zero;
        float burnTime;
        bool fired = false;
        public void FlightStart(in FlightFrameData frame)
        {
            burnTime = Data.burnTime;
            targetCraft = null;
        }
        public void FlightFixedUpdate(in FlightFrameData frame)
        {
            ApplyAreoEffect();
            Crafts = FilterCraft((List<ModApi.Flight.Sim.INode>)PartScript.CraftScript.CraftNode.Parent.DynamicNodes);
            if (targetCraft == null)
            {
                Debug.Log("null target");
                targetCraft = ConeAcquireTarget(Data.targetRange, Data.targetAngle);
            }
            else if (PartScript.Data.Activated)
            {
                ComputeSteering();
                Debug.Log(steering);//-7,13
            }
            else if (Vector3d.Angle(targetCraft.Position - PartScript.CraftScript.FlightData.Position, PartScript.CraftScript.FlightData.CraftForward) >= Data.targetAngle)
            { Debug.Log("Unlocked "+Vector3d.Angle(targetCraft.Position - PartScript.CraftScript.FlightData.Position, PartScript.CraftScript.FlightData.CraftForward)); targetCraft = null; } 
            if (PartScript.Data.Activated^fired)
                GetComponentInChildren<ParticleSystem>().Play();
            if (burnTime >= 0 && PartScript.Data.Activated)
            {
                fired = true;
                PartScript.BodyScript.RigidBody.AddRelativeForce(Vector3.forward * Data.missileImpulse / Data.burnTime,ForceMode.Acceleration);
                burnTime -= (float)frame.DeltaTime;
            }
            else GetComponentInChildren<ParticleSystem>().Stop();
        }


        ModApi.Craft.ICraftNode ConeAcquireTarget(float range, float angle)
        {
            float shortest = range;
            ModApi.Craft.ICraftNode target = null;
            foreach (ModApi.Craft.ICraftNode craft in Crafts)
            {
                if (Vector3d.Distance(craft.Position, PartScript.CraftScript.FlightData.Position) <= shortest && Vector3d.Angle(craft.Position - PartScript.CraftScript.FlightData.Position, PartScript.CraftScript.FlightData.CraftForward) <= angle && craft != PartScript.CraftScript.CraftNode)
                {
                    shortest = (float)Vector3d.Distance(craft.Position, PartScript.CraftScript.FlightData.Position);
                    target = craft;
                }
                Debug.Log(Vector3d.Distance(craft.Position, PartScript.CraftScript.FlightData.Position));
            }
            return target;
        }
        void ApplyAreoEffect()
        {
            ModApi.Craft.IBodyScript body = PartScript.BodyScript;
            Vector3 force = Vector3.right * Vector3.Dot(body.SurfaceVelocity, body.Transform.right) + Vector3.up * Vector3.Dot(body.SurfaceVelocity, body.Transform.up);
            body.RigidBody.AddForceAtPosition(body.Transform.TransformVector(force) * Data.wingArea * -body.SurfaceVelocity.magnitude, body.Transform.TransformPoint(Vector3.forward * Data.centerOfDrag));
            body.RigidBody.AddRelativeTorque((Vector3.right * steering.y + Vector3.up * steering.x) * Data.torque * body.SurfaceVelocity.sqrMagnitude);
        }
        void ComputeSteering()
        {
            //Vector3 losAngularVel = (Vector3)Vector3d.Cross(targetCraft.Velocity - PartScript.CraftScript.CraftNode.Velocity, targetCraft.Position - PartScript.BodyScript.CraftScript.CraftNode.Position);
            steering.x = PartScript.BodyScript.Transform.InverseTransformVector((Vector3)(targetCraft.Position - PartScript.BodyScript.CraftScript.CraftNode.Position)).x/ PartScript.BodyScript.Transform.InverseTransformVector((Vector3)(targetCraft.Position - PartScript.BodyScript.CraftScript.CraftNode.Position)).z;// Vector3.Dot(losAngularVel, PartScript.CraftScript.Transform.up);
            steering.y = PartScript.BodyScript.Transform.InverseTransformVector((Vector3)(targetCraft.Position - PartScript.BodyScript.CraftScript.CraftNode.Position)).y / PartScript.BodyScript.Transform.InverseTransformVector((Vector3)(targetCraft.Position - PartScript.BodyScript.CraftScript.CraftNode.Position)).z;// Vector3.Dot(losAngularVel, PartScript.CraftScript.Transform.right);
        }
        List<ModApi.Craft.ICraftNode> FilterCraft(List<ModApi.Flight.Sim.INode> nodes)
        {
            ModApi.Craft.ICraftNode craft;
            List<ModApi.Craft.ICraftNode> crafts = new List<ModApi.Craft.ICraftNode>();
            foreach (ModApi.Flight.Sim.INode node in nodes)
            {
                try { craft = (ModApi.Craft.ICraftNode)node;crafts.Add(craft);}
                catch (Exception) { craft = null; }
            }
            return crafts;
        }
    }
}