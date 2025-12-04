using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NoMoreLasso;

public class LassoRemove : MapComponent {
	private byte skipTicks;
	private bool pawnsOrMapSwitch;

	public LassoRemove(Map map) : base(map) { }

	public override void FinalizeInit() {
		base.FinalizeInit();
		RemoveFromMap();
		RemoveFromMap();
	}

	public override void MapComponentTick() {
		base.MapComponentTick();
		if (skipTicks++ < 60) return;
		skipTicks = 0;
		RemoveFromMap();
	}

	private void RemoveFromMap() {
		if (pawnsOrMapSwitch) {
			foreach (Pawn pawn in map.mapPawns.AllPawns) {
				if (!pawn.Spawned) continue;
				if (pawn.apparel == null && pawn.inventory == null) continue;
				RemoveLasso(pawn);
			}
		}
		else RemoveItems(map);
		pawnsOrMapSwitch = !pawnsOrMapSwitch;
	}

	private static void RemoveLasso(Pawn pawn) {
		if (pawn.inventory != null) {
			List<Thing> toRemoveItems = new();

			foreach (Thing item in pawn.inventory.innerContainer) {
				if (!IsLasso(item)) continue;
				toRemoveItems.Add(item);
			}

			foreach (Thing item in toRemoveItems) {
				pawn.inventory.innerContainer.Remove(item);
				item.Destroy();
			}
		}

		if (pawn.apparel != null) {
			List<Apparel> toRemoveApparel = new();

			foreach (Apparel apparel in pawn.apparel.WornApparel) {
				if (!IsLasso(apparel)) continue;
				toRemoveApparel.Add(apparel);
			}

			foreach (Apparel apparel in toRemoveApparel) {
				pawn.apparel.WornApparel.Remove(apparel);
				apparel.Destroy();
			}
		}
	}

	private static void RemoveItems(Map map) {
		List<Thing> things = map.listerThings.AllThings;
		List<Thing> toRemoveItems = new();

		foreach (Thing thing in things) {
			if (!IsLasso(thing)) continue;
			toRemoveItems.Add(thing);
		}

		foreach (Thing item in toRemoveItems) {
			item.Destroy();
		}
	}

	private static bool IsLasso(Thing item) {
		return item.def.defName.StartsWith("AM_Lasso");
	}
}
