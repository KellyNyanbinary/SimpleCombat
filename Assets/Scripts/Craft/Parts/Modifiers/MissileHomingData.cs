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
        [DesignerPropertySlider(Label = "Guidance Constant", MinValue = 0, MaxValue = 4, Tooltip = "Maps guidance input (angle is in radians) to turning effect output (maximum is 1)")]
        public float guidanceConstant = 1;
    }
}