using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NoMoreLasso;

public class LassoRemove : MapComponent {
	private readonly HashSet<int> checkedPawns = new(128);

	public LassoRemove(Map map) : base(map) { }

	public override void FinalizeInit() {
		base.FinalizeInit();
		foreach (Pawn pawn in map.mapPawns.AllPawns) {
			RemoveLasso(pawn);
		}
		RemoveItems(map);
	}

	public override void MapComponentTick() {
		base.MapComponentTick();

		List<Pawn> allPawns = map.mapPawns.AllPawns;

		foreach (Pawn pawn in allPawns) {
			if (pawn.Spawned && (pawn.inventory != null || pawn.apparel != null) && !checkedPawns.Contains(pawn.thingIDNumber)) {
				RemoveLasso(pawn);
				checkedPawns.Add(pawn.thingIDNumber);
			}
		}

		if (checkedPawns.Count > (allPawns.Count << 1)) {
			checkedPawns.Clear();
		}
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
