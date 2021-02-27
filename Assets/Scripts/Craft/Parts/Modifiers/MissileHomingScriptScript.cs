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
        public void FlightStart(in FlightFrameData frame)//Called on the first frame in flight scene
        {
            burnTime = Data.burnTime;
            targetCraft = null;
        }
        public void FlightFixedUpdate(in FlightFrameData frame)//Called every physics update in flight scene
        {
            ApplyAreoEffect();//aerodynamics
            if (targetCraft == null)
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
<<<<<<< HEAD
            if (burnTime > 0 && PartScript.Data.Activated)
<<<<<<< HEAD
            {//adds thrust
=======
=======
            if (burnTime >= 0 && PartScript.Data.Activated)
>>>>>>> parent of b8d2748 (Trying to commit to development branch)
            {
>>>>>>> a247e0d6c0f6a3fcc23dc94d307f08e72a065f06
                fired = true;
                PartScript.BodyScript.RigidBody.AddRelativeForce(Vector3.forward * Data.missileImpulse / Data.burnTime,ForceMode.Acceleration);
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
                Debug.Log(Vector3d.Distance(craft.Position, PartScript.CraftScript.FlightData.Position));
            }
            return target;
        }
        //Adds force and torque from aerodynamics
        void ApplyAreoEffect()//Yes, naming it Aero Effect is deliberate.
        {
<<<<<<< HEAD
            ModApi.Craft.IBodyScript body = PartScript.BodyScript;//save the reference to reduce a little bit of code
            Vector3 force = Vector3.right * Vector3.Dot(body.SurfaceVelocity, PartScript.Transform.right) + Vector3.forward * Vector3.Dot(body.SurfaceVelocity, PartScript.Transform.forward);
            //lift is proportional to cosine of AoA and velocity^2
            body.RigidBody.AddForceAtPosition(PartScript.Transform.TransformVector(force) * Data.wingArea * -body.SurfaceVelocity.magnitude * PartScript.BodyScript.FluidDensity, PartScript.Transform.TransformVector(Vector3.up * Data.centerOfDrag) + PartScript.Transform.position, ForceMode.Force);
            //steering torque is proportional to velocity^2 and wing area
=======
            ModApi.Craft.IBodyScript body = PartScript.BodyScript;
<<<<<<< HEAD
            Vector3 force = Vector3.right * Vector3.Dot(body.SurfaceVelocity, PartScript.Transform.right) + Vector3.forward * Vector3.Dot(body.SurfaceVelocity, PartScript.Transform.forward);
            body.RigidBody.AddForceAtPosition(PartScript.Transform.TransformVector(force) * Data.wingArea * -body.SurfaceVelocity.magnitude * PartScript.BodyScript.FluidDensity, PartScript.Transform.TransformVector(Vector3.up * Data.centerOfDrag) + PartScript.Transform.position, ForceMode.Force);
>>>>>>> a247e0d6c0f6a3fcc23dc94d307f08e72a065f06
            body.RigidBody.AddTorque(steering * Data.torque * Data.wingArea * body.SurfaceVelocity.sqrMagnitude * PartScript.BodyScript.FluidDensity,ForceMode.Force);
=======
            Vector3 force = Vector3.right * Vector3.Dot(body.SurfaceVelocity, body.Transform.right) + Vector3.up * Vector3.Dot(body.SurfaceVelocity, body.Transform.up);
            body.RigidBody.AddForceAtPosition(body.Transform.TransformVector(force) * Data.wingArea * -body.SurfaceVelocity.magnitude, body.Transform.TransformPoint(Vector3.forward * Data.centerOfDrag));
            body.RigidBody.AddRelativeTorque((Vector3.right * steering.y + Vector3.up * steering.x) * Data.torque * body.SurfaceVelocity.sqrMagnitude);
>>>>>>> parent of b8d2748 (Trying to commit to development branch)
        }
        void ComputeSteering()
        {
            //Vector3 losAngularVel = (Vector3)Vector3d.Cross(targetCraft.Velocity - PartScript.CraftScript.CraftNode.Velocity, targetCraft.Position - PartScript.BodyScript.CraftScript.CraftNode.Position);
            steering.x = PartScript.BodyScript.Transform.InverseTransformVector((Vector3)(targetCraft.Position - PartScript.BodyScript.CraftScript.CraftNode.Position)).x/ PartScript.BodyScript.Transform.InverseTransformVector((Vector3)(targetCraft.Position - PartScript.BodyScript.CraftScript.CraftNode.Position)).z;// Vector3.Dot(losAngularVel, PartScript.CraftScript.Transform.up);
            steering.y = PartScript.BodyScript.Transform.InverseTransformVector((Vector3)(targetCraft.Position - PartScript.BodyScript.CraftScript.CraftNode.Position)).y / PartScript.BodyScript.Transform.InverseTransformVector((Vector3)(targetCraft.Position - PartScript.BodyScript.CraftScript.CraftNode.Position)).z;// Vector3.Dot(losAngularVel, PartScript.CraftScript.Transform.right);
        }
        //Converts INode list to ICraftNode list. Could use a better alternative but I don't want to waste more time on trial and error.
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