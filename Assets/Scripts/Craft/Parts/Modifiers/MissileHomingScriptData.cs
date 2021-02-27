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
    [DesignerPartModifier("MissileHomingScript")]
    [PartModifierTypeId("SimpleCombat.MissileHomingScript")]
    public class MissileHomingScriptData : PartModifierData<MissileHomingScriptScript>
    {
        [DesignerPropertySlider(Label = "Missile Impulse", MinValue = 0, MaxValue = 2000)]
        public float missileImpulse;
        [DesignerPropertySlider(Label = "Missile Burn Time", MinValue = 0, MaxValue = 20)]
        public float burnTime;
        [DesignerPropertySlider(Label = "Target Acquire Range", MinValue = 0, MaxValue = 10000)]
        public float targetRange;
        [DesignerPropertySlider(Label = "Target Acquire Angle", MinValue = 0, MaxValue = 30)]
        public float targetAngle;
        [DesignerPropertySlider(Label = "Wing Area", MinValue = 0, MaxValue = 0.5f)]
        public float wingArea;
        [DesignerPropertySlider(Label = "Turning Torque", MinValue = 0, MaxValue = 1)]
        public float torque;
        [DesignerPropertySlider(Label = "CoD Offset", MinValue = -1, MaxValue = 0)]
        public float centerOfDrag;
        [DesignerPropertySpinner("Pure Pursuit", "Proportional", Label = "Guidance Method")]
        public string guidanceMethod = "Pure Pursuit";
    }
}