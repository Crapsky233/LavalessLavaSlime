using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ModLoader;

namespace LavalessLavaSlime
{
    public class LavalessLavaSlime : Mod
    {
        // IL_11726: call bool Terraria.Main::get_expertMode()
        // IL_1172B: brfalse.s IL_1173F
        // IL_1172D: ldarg.0
        // IL_1172E: ldfld int32 Terraria.NPC::'type'
        // IL_11733: ldc.i4.s  59 (IL指针落点在这里)
        // IL_11735: bne.un.s IL_1173F
        // IL_11737: ldsfld int32 Terraria.Main::netMode
        // IL_1173C: ldc.i4.1
        // IL_1173D: bne.un.s IL_11740

        public override void Load() {
            base.Load();
            IL.Terraria.NPC.VanillaHitEffect += context => {
                try {
                    var c = new ILCursor(context);

                    c.GotoNext(
                        MoveType.After,
                        i => i.MatchCall(typeof(Main), "get_expertMode"),
                        i => i.Match(OpCodes.Brfalse_S),
                        i => i.Match(OpCodes.Ldarg_0),
                        i => i.MatchLdfld(typeof(NPC), nameof(NPC.type)),
                        i => i.MatchLdcI4(59)
                    );

                    c.EmitDelegate<Func<int, int>>((returnValue) => {
                        return NPCLoader.NPCCount; // NPC.type不可能为NPCLoader.NPCCount
                    });

                }
                catch {
                    throw new Exception("Hook location not found, if (!Main.expertMode || type != 59 || Main.netMode == 1) { return; }");
                }
            };
        }
    }
}