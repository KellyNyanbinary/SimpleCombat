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
        Vector3 steering = Vector3.zero;
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
            if (burnTime > 0 && PartScript.Data.Activated)
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
            Vector3 force = Vector3.right * Vector3.Dot(body.SurfaceVelocity, PartScript.Transform.right) + Vector3.forward * Vector3.Dot(body.SurfaceVelocity, PartScript.Transform.forward);
            body.RigidBody.AddForceAtPosition(PartScript.Transform.TransformVector(force) * Data.wingArea * -body.SurfaceVelocity.magnitude * PartScript.BodyScript.FluidDensity, PartScript.Transform.TransformVector(Vector3.up * Data.centerOfDrag) + PartScript.Transform.position, ForceMode.Force);
            body.RigidBody.AddTorque(steering * Data.torque * Data.wingArea * body.SurfaceVelocity.sqrMagnitude * PartScript.BodyScript.FluidDensity,ForceMode.Force);
        }
        void ComputeSteering()
        {
            if (Data.guidanceMethod == "Pure Pursuit")
                steering = (Vector3)Vector3d.Cross(PartScript.Transform.up, (targetCraft.Position - PartScript.BodyScript.CraftScript.CraftNode.Position).normalized);
            else if (Data.guidanceMethod == "Proportional")
            {
                steering = (Vector3)Vector3d.Cross(PartScript.CraftScript.CraftNode.Velocity - targetCraft.Velocity, (targetCraft.Position - PartScript.BodyScript.CraftScript.CraftNode.Position) / (targetCraft.Position - PartScript.BodyScript.CraftScript.CraftNode.Position).sqrMagnitude);
                steering -= PartScript.Transform.up * Vector3.Dot(PartScript.Transform.up, steering);
            }
            else steering = Vector3.zero;
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