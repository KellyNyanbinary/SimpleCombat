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

    public class MissileAerodynamicsScript : PartModifierScript<MissileAerodynamicsData>, IFlightFixedUpdate,IFlightStart
    {
        Vector2 steering = Vector2.zero;
        public Vector2 Steering
        {
            set { steering = value; }
        }
        bool fired = false;
        float burnTime;
        public void FlightStart(in FlightFrameData frame)
        {
            burnTime = Data.burnTime;
        }
        public void FlightFixedUpdate(in FlightFrameData frame)
        {
            ModApi.Craft.IBodyScript body = PartScript.BodyScript;//save the reference to reduce a little bit of code
            Vector3 force = Vector3.right * CalculateLift(PartScript.BodyScript.FluidDensity, body.SurfaceVelocity.magnitude, 90 - Vector3.Angle(body.SurfaceVelocity, -PartScript.Transform.right))
                    + Vector3.forward * CalculateLift(PartScript.BodyScript.FluidDensity, body.SurfaceVelocity.magnitude, 90 - Vector3.Angle(body.SurfaceVelocity, -PartScript.Transform.forward))
                    + Vector3.down * CalculateDrag(PartScript.BodyScript.FluidDensity, body.SurfaceVelocity.magnitude, Vector3.Angle(body.SurfaceVelocity, -PartScript.Transform.up));

            //lift is determined by curve
            body.RigidBody.AddForceAtPosition(PartScript.Transform.TransformVector(force), PartScript.Transform.TransformVector(Vector3.up * Data.centerOfDrag) + PartScript.Transform.position, ForceMode.Force);
            //steering torque is proportional to velocity^2 and wing area
            body.RigidBody.AddTorque(steering * Data.torque * Data.wingArea * Mathf.Min(body.SurfaceVelocity.sqrMagnitude, 122500) * PartScript.BodyScript.FluidDensity, ForceMode.Force);

            if (PartScript.Data.Activated ^ fired)//first frame after part activation
                GetComponentInChildren<ParticleSystem>().Play();
            if (burnTime > 0 && PartScript.Data.Activated)
            {//adds thrust
                fired = true;
                PartScript.BodyScript.RigidBody.AddForce(PartScript.Transform.up * Data.missileImpulse / Data.burnTime, ForceMode.Acceleration);
                burnTime -= (float)frame.DeltaTime;
            }
            else GetComponentInChildren<ParticleSystem>().Stop();
        }
        float CalculateLift(float density, float speed, float aoa)
        {
            return Data.wingArea * density * Data.LiftCurve.Evaluate(aoa) * Mathf.Sign(aoa) * Mathf.Pow(Mathf.Min(349, speed), 2);
        }
        float CalculateDrag(float density, float speed, float aoa)
        {
            return Data.wingArea * density * Data.DragCurve.Evaluate(aoa) * Mathf.Sign(aoa) * Mathf.Pow(Mathf.Min(349, speed), 2);
        }
    }
}