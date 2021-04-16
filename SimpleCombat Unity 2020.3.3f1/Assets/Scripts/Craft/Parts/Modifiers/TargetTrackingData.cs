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
    [DesignerPartModifier("TargetTracking")]
    [PartModifierTypeId("SimpleCombat.TargetTracking")]
    public class TargetTrackingData : PartModifierData<TargetTrackingScript>
    {
        [DesignerPropertySlider(Label = "Target Acquire Range", MinValue = 0, MaxValue = 10000)]
        public float targetRange;
        [DesignerPropertySlider(Label = "Target Acquire Angle", MinValue = 0, MaxValue = 100)]
        public float targetAngle;
        [DesignerPropertyToggleButton(Label = "Output Steering")]
        bool calculateSteering = false;
        [DesignerPropertySpinner("Pure Pursuit", "Proportional", "Unguided", Label = "Guidance Method")]
        public string guidanceMethod = "Pure Pursuit";
        [DesignerPropertySlider(Label = "Guidance Constant", MinValue = 0, MaxValue = 4, Tooltip = "Maps guidance input (angle is in radians) to turning effect output")]
        public float guidanceConstant = 1;

    }
}