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

    public class TargetTrackingScript : PartModifierScript<TargetTrackingData>,IFlightStart,IFlightFixedUpdate
    {
        ModApi.Craft.ICraftNode targetCraft;
        List<ModApi.Craft.ICraftNode> Crafts;
        MissileAerodynamicsScript aerodynamics;
        Vector3 steering = Vector3.zero;
        public void FlightStart(in FlightFrameData frame)//Called on the first frame in flight scene
        {
            targetCraft = null;
            aerodynamics = GetComponent<MissileAerodynamicsScript>();
        }
        public void FlightFixedUpdate(in FlightFrameData frame)//Called every physics update in flight scene
        {
            if (targetCraft == null && Data.guidanceMethod != "Unguided" && Data.targetAngle * Data.targetRange != 0)
            {//try to acquire a target
                Crafts = FilterCraft((List<ModApi.Flight.Sim.INode>)PartScript.CraftScript.CraftNode.Parent.DynamicNodes);
                targetCraft = ConeAcquireTarget(Data.targetRange, Data.targetAngle);
            }
            else if (PartScript.Data.Activated)//If has target and launched
                ComputeSteering();
            if (aerodynamics)//if not null
                aerodynamics.Steering = steering;
            else if (Vector3d.Angle(targetCraft.Position - PartScript.CraftScript.FlightData.Position, PartScript.CraftScript.FlightData.CraftForward) >= Data.targetAngle)
                targetCraft = null;//if target exceeds locking angle limits before launching
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
            }
            return target;
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
            List<ModApi.Craft.ICraftNode> crafts = new List<ModApi.Craft.ICraftNode>();
            foreach (ModApi.Craft.ICraftNode craft in nodes.OfType<ModApi.Craft.ICraftNode>())
            {
                try { crafts.Add(craft); }
                catch (Exception) { Debug.Log("Node conversion failed"); }
            }
            return crafts;
        }
    }
}