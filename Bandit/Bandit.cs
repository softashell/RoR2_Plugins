using BepInEx;
using RoR2;
using UnityEngine;

namespace Bandit
{
    //This attribute specifies that we have a dependency on R2API, as we're using it to add Bandit to the game.
    //You don't need this if you're not using R2API in your plugin, it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency("com.bepis.r2api")]

    //This attribute is required, and lists metadata for your plugin.
    //The GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config). I like to use the java package notation, which is "com.[your name here].[your plugin name here]"
    //The name is the name of the plugin that's displayed on load, and the version number just specifies what version the plugin is.
    [BepInPlugin("com.softashell.Bandit", "Bandit", "1.0")]

    //This is the main declaration of our plugin class. BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    //BaseUnityPlugin itself inherits from MonoBehaviour, so you can use this as a reference for what you can declare and use in your plugin class: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class Bandit : BaseUnityPlugin
    {
        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            //Here we are subscribing to the SurvivorCatalogReady event, which is run when the subscriber catalog can be modified.
            //We insert Bandit as a new character here, which is then automatically added to the internal game catalog and reconstructed.
            R2API.SurvivorAPI.SurvivorCatalogReady += (s, e) =>
            {
                var survivor = new SurvivorDef
                {
                    bodyPrefab = BodyCatalog.FindBodyPrefab("BanditBody"),
                    descriptionToken = "BANDIT_DESCRIPTION",
                    displayPrefab = Resources.Load<GameObject>("Prefabs/Characters/BanditDisplay"),
                    primaryColor = new Color(0.8039216f, 0.482352942f, 0.843137264f),
                    unlockableName = ""
                };

                var skill = survivor.bodyPrefab.GetComponent<SkillLocator>();

                skill.primary.skillNameToken = "Blast";
                skill.primary.skillDescriptionToken = "Fire a powerful slug for <style=cIsDamage>150% damage</style>.";

                skill.secondary.skillNameToken = "Lights Out";
                skill.secondary.skillDescriptionToken = "Take aim for a headshot, dealing <style=cIsDamage> 600 % damage </style>.If the ability <style=cIsDamage> kills an enemy </style>, the Bandit's <style=cIsUtility>Cooldowns are all reset to 0</style>.";

                skill.utility.skillNameToken = "Smokebomb";
                skill.utility.skillDescriptionToken = "Turn invisible for <style=cIsDamage>3 seconds</style>, gaining <style=cIsUtility>increased movement speed</style>.";

                skill.special.skillNameToken = "Thermite Toss";
                skill.special.skillDescriptionToken = "Fire off a burning Thermite grenade, dealing <style=cIsDamage>damage over time</style>.";

                R2API.SurvivorAPI.SurvivorDefinitions.Insert(3, survivor);
            };
        }
    }
}