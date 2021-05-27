using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGuard
{
    public class Decoration
    {
        public static Texture2D treeTexture;
        public static Texture rockTexture;

        public Texture2D texture;
        public Vector2 position;

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position - Main.cameraPosition, null, Color.White);
        }
    }
}
