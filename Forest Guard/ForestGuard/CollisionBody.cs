using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestGuard
{
    public abstract class CollisionBody
    {
        public Vector2 position;
        public Rectangle hitbox;

        public void DetectManualCollisions(Rectangle possibleCollidingRect, bool friendly)
        {
            if (hitbox.Intersects(possibleCollidingRect))
            {
                HandleManualCollisions(possibleCollidingRect, friendly);
            }
        }

        public void DetectCollisions()
        {
            CollisionBody[] entitiesListCopy = Main.entitiesList.ToArray();
            for (int i = 0; i < entitiesListCopy.Length; i++)
            {
                if (hitbox.Intersects(entitiesListCopy[i].hitbox))
                {
                    HandleCollisions(entitiesListCopy[i]);
                }
            }
        }

        public virtual void HandleManualCollisions(Rectangle collidingRect, bool friendly)
        { }

        public virtual void HandleCollisions(CollisionBody collider)
        { }

        public virtual void Update()
        { }

        public virtual void Draw(SpriteBatch spriteBatch)
        { }
    }
}
