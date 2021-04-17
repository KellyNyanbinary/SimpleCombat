namespace Assets.Scripts.Craft.Parts.Modifiers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using ModApi.Craft.Parts;
    using ModApi.Craft.Parts.Attributes;
    using UnityEngine;

    [Serializable]
    [DesignerPartModifier("MissileHoming")]
    [PartModifierTypeId("SimpleCombat.MissileHoming")]
    public class MissileHomingData : PartModifierData<MissileHomingScript>
    {
        [DesignerPropertySlider(Label = "Missile Impulse", MinValue = 0, MaxValue = 2000)]
        public float missileImpulse;
        [DesignerPropertySlider(Label = "Missile Burn Time", MinValue = 0, MaxValue = 20)]
        public float burnTime;
        [DesignerPropertySlider(Label = "Target Acquire Range", MinValue = 0, MaxValue = 10000)]
        public float targetRange;
        [DesignerPropertySlider(Label = "Target Acquire Angle", MinValue = 0, MaxValue = 100)]
        public float targetAngle;
        [DesignerPropertySlider(Label = "Wing Area", MinValue = 0, MaxValue = 0.1f)]
        public float wingArea;
        [DesignerPropertyToggleButton(Label = "Use Lift Curve")]
        public bool useLiftCurve = false;
        [DesignerPropertySlider(Label = "Turning Torque", MinValue = 0, MaxValue = 0.5f)]
        public float torque;
        [DesignerPropertySlider(Label = "Center of Drag Offset", MinValue = -1, MaxValue = 0)]
        public float centerOfDrag;
        [DesignerPropertySpinner("Pure Pursuit", "Proportional", "Unguided", Label = "Guidance Method")]
        public string guidanceMethod = "Pure Pursuit";
        [DesignerPropertySlider(Label = "Guidance Constant", MinValue = 0, MaxValue = 4, Tooltip = "Maps guidance input (angle is in radians) to turning effect output")]
        public float guidanceConstant = 1;
        [DesignerPropertyToggleButton(Label = "Use Contact Fuse")]
        public bool contactFuse = true;
        [DesignerPropertySlider(Label = "Proximity fuse activation distance", MinValue = 0, MaxValue = 20, Tooltip = "0 to disable proximity fuse")]
        public float proximityFuse = 10;

        private AnimationCurve liftCurve = new AnimationCurve();
        public AnimationCurve LiftCurve
        {
            get
            {
                if (liftCurve.length == 0)
                {
                    liftCurve = new AnimationCurve(KeysFromPoints(5,new float[] { 0,0.33f,0.662f,0.822f,0.786f,0.759f,0.732f,0.715f,0.707f,0.705f,0.714f,0.727f,0.745f,0.764f,0.783f,0.801f,0.815f,0.822f,0.825f,0.827f,0.830f,0.835f,0.84f}));
                    liftCurve.preWrapMode = WrapMode.PingPong;
                }
                return liftCurve;
            }
        }
        private AnimationCurve dragCurve = new AnimationCurve();
        public AnimationCurve DragCurve
        {
            get
            {
                if (dragCurve.length == 0)
                {
                    dragCurve = new AnimationCurve(KeysFromPoints(5,new float[] { 0,0.025f,0.103f,0.204f,0.264f,0.299f,0.315f,0.317f,0.31f,0.291f,0.269f,0.244f,0.217f,0.19f,0.162f,0.13f,0.095f,0.053f,0,-0.062f,-0.112f,-0.161f,-0.198f,-0.233f,-0.253f,-0.262f,-0.256f,-0.238f,-0.213f,-0.183f,-0.147f,-0.11f,-0.075f,-0.043f,-0.015f,-0.005f,0}));
                    dragCurve.preWrapMode = WrapMode.PingPong;
                }
                return dragCurve;
            }
        }
        Keyframe[] KeysFromPoints(float interval, float[] values)
        {
            List<Keyframe> keyframes = new List<Keyframe>();
            for (int i = 0; i < values.Length; i++)
                keyframes.Add(new Keyframe(i * interval, values[i]));
            return keyframes.ToArray();
        }
    }
}