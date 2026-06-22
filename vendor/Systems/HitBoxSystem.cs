namespace Cecs.Systems;
public record struct Hitbox() : IComponent;
public static class HitBoxSystem {
    public static void  ScanCollisions <P, G>
    (this Store<Hitbox> hitBoxStore, Store<P> positionStore, Store<G> geometryStore, Dictionary<World.Entity, List<World.Entity>> hitPair, List<World.Entity> buffer, List<World.Entity> searchArea)
    where P : struct, IComponent, IHasPosition2D
    where G : struct, IComponent, IHasSize2D
    {
        var itemsWithHitBox = buffer.GetEntitiesWith(hitBoxStore).And(positionStore).And(geometryStore);
        for (int i = 0; i < itemsWithHitBox.Count; i++)
        {
            searchArea.Clear();
            var item = itemsWithHitBox[i];
            hitPair[item].EnsureCapacity(itemsWithHitBox.Count);

            var itemPosition = positionStore.Components[item.Id].Point;
            var itemSize = geometryStore.Components[item.Id].Point;

            for (int j = 0; j < itemsWithHitBox.Count; j++)
            {
                var itemSearched = itemsWithHitBox[j];
                if (itemSearched == item) continue;

                var itemSearchedPosition = positionStore.Components[itemSearched.Id].Point;
                var itemSearchedSize = geometryStore.Components[itemSearched.Id].Point;

                var searchUpperY = itemSearchedPosition.Y;
                var searchLowerY = itemSearchedPosition.Y + itemSearchedSize.Y;
                var itemUpperY = itemPosition.Y;
                var itemLowerY = itemPosition.Y + itemSize.Y;

                var searchUpperX = itemSearchedPosition.X;
                var searchLowerX = itemSearchedPosition.X + itemSearchedSize.X;
                var itemUpperX = itemPosition.X;
                var itemLowerX = itemPosition.X + itemSize.X;

                if (searchLowerY >= itemUpperY && searchUpperY <= itemLowerY || searchLowerX >= itemUpperX && searchUpperX <= itemLowerX ) searchArea.Add(itemSearched);
            }

            for (int j = 0; j < searchArea.Count; j++)
            {
                var itemSearchedPosition = positionStore.Components[searchArea[j].Id].Point;
                var itemSearchedSize = geometryStore.Components[searchArea[j].Id].Point;

                var leftA = itemPosition.X;
                var rightA = itemPosition.X + itemSize.X;
                var topA = itemPosition.Y;
                var bottomA = itemPosition.Y + itemSize.Y;

                var leftB = itemSearchedPosition.X;
                var rightB = itemSearchedPosition.X + itemSearchedSize.X;
                var topB = itemSearchedPosition.Y;
                var bottomB = itemSearchedPosition.Y + itemSearchedSize.Y;

                if (
                    leftA < rightB &&
                    rightA > leftB &&
                    topA < bottomB &&
                    bottomA > topB
                )
                {
                    hitPair[item].Add(searchArea[j]);
                }
            }
        }
    }
}