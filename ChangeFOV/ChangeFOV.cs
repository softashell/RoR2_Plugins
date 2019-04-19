using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Utils;

namespace ChangeFOV
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.softashell.changeFOV", "ChangeFOV", "1.0")]
    public class ChangeFOV : BaseUnityPlugin
    {
        private static ConfigWrapper<int> ConfigBaseFOV { get; set; }
        private static ConfigWrapper<bool> ConfigStatic { get; set; }
        private static ConfigWrapper<bool> ConfigStaticSensivity { get; set; }
        private static ConfigWrapper<bool> ConfigParticles { get; set; }

        public void Awake()
        {
            InitConfig();
            SetHooks();
        }

        private void SetHooks()
        {
            On.RoR2.CameraRigController.Start += (orig, self) =>
            {
                self.baseFov = ConfigBaseFOV.Value;

                orig(self);
            };

            if (ConfigStatic.Value)
            {
                On.RoR2.CameraRigController.Update += (orig, self) =>
                {
                    orig(self);

                    // Disable most of the FOV offset while sprinting
                    self.SetFieldValue("fovVelocity", 0f);
                };
            }

            IL.RoR2.CameraRigController.Update += (il) =>
            {
                var c = new ILCursor(il);

                if (ConfigStaticSensivity.Value)
                {
                    // Don't reduce mouse sensivity while sprinting
                    c.Goto(696);
                    c.Remove();
                    c.Emit(OpCodes.Ldc_R4, 1f);
                    c.Goto(700);
                    c.Remove();
                    c.Emit(OpCodes.Ldc_R4, 1f);
                }

                if (!ConfigParticles.Value)
                {
                    // Disable sprinting particle effect
                    c.Goto(720);
                    c.RemoveRange(4);
                }
            };
        }

        private void InitConfig()
        {
            ConfigBaseFOV = Config.Wrap(
                section: "FOV",
                key: "Base",
                description: "Base FOV",
                defaultValue: 60);

            ConfigStatic = Config.Wrap(
                section: "FOV",
                key: "Static",
                description: "Don't increase FOV while sprinting",
                defaultValue: true);

            ConfigStaticSensivity = Config.Wrap(
                section: "Sensivity",
                key: "Static",
                description: "Don't reduce sensivity while sprinting",
                defaultValue: true);

            ConfigParticles = Config.Wrap(
                section: "Misc",
                key: "Particles",
                description: "Toggle sprinting particle effect",
                defaultValue: true);
        }
    }
}