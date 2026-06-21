namespace Cecs.Systems;
public record struct Hitbox() : IComponent;
public static class HitBoxSystem {
    public static void  ScanCollisions <T>(this Store<Hitbox> hitBoxStore, Store<Geometry> geometryStore, Store<Rendereable<T>> textures, Dictionary<World.Entity, List<World.Entity>> hitPair, List<World.Entity> buffer, List<World.Entity> searchArea)
    {
        var itemsWithHitBox = buffer.GetEntitiesWith(hitBoxStore).And(geometryStore).And(textures);
        for (int i = 0; i < itemsWithHitBox.Count; i++)
        {
            searchArea.Clear();
            var item = itemsWithHitBox[i];
            hitPair[item].EnsureCapacity(itemsWithHitBox.Count);

            var itemPosition = geometryStore.Components[item.Id].Position;
            var itemSize = textures.Components[item.Id].Size;

            for (int j = 0; j < itemsWithHitBox.Count; j++)
            {
                var itemSearched = itemsWithHitBox[j];
                if (itemSearched == item) continue;

                var itemSearchedPosition = geometryStore.Components[itemSearched.Id].Position;
                var itemSearchedSize = textures.Components[itemSearched.Id].Size;

                var searchUpperY = itemSearchedPosition.Point.Y;
                var searchLowerY = itemSearchedPosition.Point.Y + itemSearchedSize.Y;
                var itemUpperY = itemPosition.Point.Y;
                var itemLowerY = itemPosition.Point.Y + itemSize.Y;

                var searchUpperX = itemSearchedPosition.Point.X;
                var searchLowerX = itemSearchedPosition.Point.X + itemSearchedSize.X;
                var itemUpperX = itemPosition.Point.X;
                var itemLowerX = itemPosition.Point.X + itemSize.X;

                if (searchLowerY >= itemUpperY && searchUpperY <= itemLowerY || searchLowerX >= itemUpperX && searchUpperX <= itemLowerX ) searchArea.Add(itemSearched);
            }

            for (int j = 0; j < searchArea.Count; j++)
            {
                var itemSearchedPosition = geometryStore.Components[searchArea[j].Id].Position;
                var itemSearchedSize = textures.Components[searchArea[j].Id].Size;

                var searchUpperY = itemSearchedPosition.Point.Y;
                var searchLowerY = itemSearchedPosition.Point.Y + itemSearchedSize.Y;
                var searchUpperX = itemSearchedPosition.Point.X;
                var searchLowerX = itemSearchedPosition.Point.X + itemSearchedSize.X;

                var searchUpperBouonds = new Vec2 (searchUpperX, searchUpperY);
                var searchLowerBouonds = new Vec2 (searchLowerX, searchLowerY);

                var itemUpperX = itemPosition.Point.X;
                var itemLowerX = itemPosition.Point.X + itemSize.X;
                var itemUpperY = itemPosition.Point.Y;
                var itemLowerY = itemPosition.Point.Y + itemSize.Y;

                var itemUpperBouonds = new Vec2 (itemUpperX, itemUpperY);
                var itemLowerBouonds = new Vec2 (itemLowerX, itemLowerY);

                if (
                    (searchLowerBouonds > itemUpperBouonds && searchLowerBouonds < itemLowerBouonds) ||
                    (searchUpperBouonds > itemUpperBouonds && searchUpperBouonds < itemLowerBouonds)
                )
                {
                    hitPair[item].Add(searchArea[j]);
                }
            }
        }
    }
}