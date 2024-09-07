using BepInEx;
using RoR2;
using RoR2.CharacterAI;
using System.Diagnostics;
using UnityEngine.Networking;

namespace Unlobotomize
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    public class UnlobotomizePlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Gorakh";
        public const string PluginName = "Unlobotomize";
        public const string PluginVersion = "1.0.0";

        internal static UnlobotomizePlugin Instance { get; private set; }

        void Awake()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Log.Init(Logger);

            Instance = SingletonHelper.Assign(Instance, this);

            CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;

            stopwatch.Stop();
            Log.Message_NoCallerPrefix($"Initialized in {stopwatch.Elapsed.TotalMilliseconds:F0}ms");
        }

        void OnDestroy()
        {
            CharacterMaster.onStartGlobal -= CharacterMaster_onStartGlobal;

            Instance = SingletonHelper.Unassign(Instance, this);
        }

        static void CharacterMaster_onStartGlobal(CharacterMaster master)
        {
            if (!NetworkServer.active || !master || master.aiComponents == null)
                return;

            foreach (BaseAI ai in master.aiComponents)
            {
                if (!ai)
                    continue;

#if DEBUG
                if (ai.shouldMissFirstOffScreenShot)
                {
                    Log.Debug($"Unlobotomized {master}");
                }
#endif

                ai.shouldMissFirstOffScreenShot = false;
            }
        }
    }
}
