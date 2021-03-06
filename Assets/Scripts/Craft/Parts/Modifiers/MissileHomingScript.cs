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

    public class MissileHomingScript : PartModifierScript<MissileHomingData>,IFlightStart,IFlightFixedUpdate
    {
        ModApi.Craft.ICraftNode targetCraft;
        List<ModApi.Craft.ICraftNode> Crafts;
        Vector3 steering = Vector3.zero;
        float burnTime;
        bool fired = false;
        public void FlightStart(in FlightFrameData frame)//Called on the first frame in flight scene
        {
            burnTime = Data.burnTime;
            targetCraft = null;
        }
        public void FlightFixedUpdate(in FlightFrameData frame)//Called every physics update in flight scene
        {
            ApplyAreoEffect();//aerodynamics
            if (targetCraft==null && Data.guidanceMethod!="Unguided" && Data.targetAngle*Data.targetRange!=0)
            {//try to acquire a target
                Crafts = FilterCraft((List<ModApi.Flight.Sim.INode>)PartScript.CraftScript.CraftNode.Parent.DynamicNodes);
                targetCraft = ConeAcquireTarget(Data.targetRange, Data.targetAngle);
            }
            else if (PartScript.Data.Activated)//If has target and launched
                ComputeSteering();
            else if (Vector3d.Angle(targetCraft.Position - PartScript.CraftScript.FlightData.Position, PartScript.CraftScript.FlightData.CraftForward) >= Data.targetAngle)
                targetCraft = null;//if target exceeds locking angle limits before launching

            if (PartScript.Data.Activated^fired)//first frame after part activation
                GetComponentInChildren<ParticleSystem>().Play();
            if (burnTime > 0 && PartScript.Data.Activated)
            {//adds thrust
                fired = true;
                PartScript.BodyScript.RigidBody.AddForce(PartScript.Transform.up * Data.missileImpulse / Data.burnTime,ForceMode.Acceleration);
                burnTime -= (float)frame.DeltaTime;
            }
            else GetComponentInChildren<ParticleSystem>().Stop();
        }

        //Returns the closest target inside the acquisition range and angle. Uses data in the list Crafts.
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
            }
            return target;
        }
        //Adds force and torque from aerodynamics
        void ApplyAreoEffect()//Yes, naming it Aero Effect is deliberate.
        {
            ModApi.Craft.IBodyScript body = PartScript.BodyScript;//save the reference to reduce a little bit of code
            Vector3 force = Vector3.right * Vector3.Dot(body.SurfaceVelocity, PartScript.Transform.right) + Vector3.forward * Vector3.Dot(body.SurfaceVelocity, PartScript.Transform.forward);
            //lift is proportional to cosine of AoA and velocity^2
            body.RigidBody.AddForceAtPosition(PartScript.Transform.TransformVector(force) * Data.wingArea * -body.SurfaceVelocity.magnitude * PartScript.BodyScript.FluidDensity, PartScript.Transform.TransformVector(Vector3.up * Data.centerOfDrag) + PartScript.Transform.position, ForceMode.Force);
            //steering torque is proportional to velocity^2 and wing area
            body.RigidBody.AddTorque(steering * Data.torque * Data.wingArea * body.SurfaceVelocity.sqrMagnitude * PartScript.BodyScript.FluidDensity,ForceMode.Force);
            //conter-roll torque
            body.RigidBody.AddTorque(-0.1f * PartScript.Transform.up * Vector3.Dot(PartScript.Transform.up, body.RigidBody.angularVelocity),ForceMode.Force);
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
            else
            {
                return;
            }
            steering *= Data.guidanceConstant;
            //limits vector magnitude to below 1
            if (steering.magnitude > 1)
                steering = steering.normalized;
        }
        //Converts INode list to ICraftNode list. Could use a better alternative but I don't want to waste more time on trial and error.
        List<ModApi.Craft.ICraftNode> FilterCraft(List<ModApi.Flight.Sim.INode> nodes)
        {
            ModApi.Craft.ICraftNode craft;
            List<ModApi.Craft.ICraftNode> crafts = new List<ModApi.Craft.ICraftNode>();
            foreach (ModApi.Flight.Sim.INode node in nodes)
            {
                try { craft = (ModApi.Craft.ICraftNode)node;crafts.Add(craft);}
                catch (Exception) { craft = null;Debug.Log("Node conversion failed"); }
            }
            return crafts;
        }
    }
}