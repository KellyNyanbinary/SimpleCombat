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
    [DesignerPartModifier("Missile_Homing")]
    [PartModifierTypeId("SimpleCombat.Missile_Homing")]
    public class Missile_HomingData : PartModifierData<Missile_HomingScript>
    {
    }
}