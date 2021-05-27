using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestGuard
{
    public class Tile       //Rather than making this a flesehd out class with small applications, I'll just use it as a data storage class
    {
        public static Texture2D grassTile;
        public static Texture2D grassyGrassTile;

        private Texture2D tileTexture;
        private Vector2 tilePosition;

        public Tile(Texture2D texture, Vector2 position)
        {
            tileTexture = texture;
            tilePosition = position;
        }


        public void Update()
        { }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tileTexture, tilePosition - Main.cameraPosition, null, Color.White);
        }
    }
}
