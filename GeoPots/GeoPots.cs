using Modding;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeoPots
{
    public class GeoPots : Mod
    {
        internal static GeoPots Instance;

        public override string GetVersion() => "1.1.0.0";

        /// <summary>
        /// Stores the geo rock from the global pool
        /// </summary>
        private GameObject geoPrefab;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;

            // Store geo rock from global pool
            foreach (var pool in ObjectPool.instance.startupPools)
            {
                if (pool.prefab.name.Equals("Geo Small"))
                {
                    geoPrefab = pool.prefab;
                }
            }

            On.Breakable.Break += BreakPots;

            Log("Initialized");
        }

        /// <summary>
        /// Like in Legend of Zelda, whenever a cart or a barrel is smashed, we want to drop geo
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="flingAngleMin"></param>
        /// <param name="flingAngleMax"></param>
        /// <param name="impactMultiplier"></param>
        private void BreakPots(On.Breakable.orig_Break orig, Breakable self, float flingAngleMin, float flingAngleMax, float impactMultiplier)
        {
            //Log($"Breaking {self.gameObject.name}");
            orig(self, flingAngleMin, flingAngleMax, impactMultiplier);

            // Barrels are smaller and more common, so they will have a 50% of dropping 1 geo
            if (self.gameObject.name.ToLower().Contains("barrel") ||
                self.gameObject.name.ToLower().Contains("deep_egg_set"))
            {
                int random = UnityEngine.Random.Range(1, 101);
                //Log($"Barrel chance: {random} vs 50");
                if (random <= 50)
                {
                    FlingGeo(self.gameObject.transform.position, 1);
                }
            }

            // Carts are larger, so they will have a 75% of dropping 5 geo
            if (self.gameObject.name.ToLower().Contains("cart"))
            {
                int random = UnityEngine.Random.Range(1, 101);
                //Log($"Cart chance: {random} vs 75");
                if (random <= 75)
                {
                    FlingGeo(self.gameObject.transform.position, 5);
                }
            }
        }

        /// <summary>
        /// Flings a random range of geo in various directions
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="geoCount"></param>
        private void FlingGeo(Vector3 origin, int geoCount)
        {
            //Log($"Spawning {geoCount} geo");
            for (int i = 0; i < geoCount; i++)
            {
                GameObject gameObject = geoPrefab.Spawn(origin, Quaternion.Euler(Vector3.zero));
                Vector3 newPosition = gameObject.transform.position;
                newPosition.Set(newPosition.x + UnityEngine.Random.Range(-0.25f, 0.25f),
                                newPosition.y + UnityEngine.Random.Range(-0.25f, 0.25f),
                                newPosition.z);
                gameObject.transform.position = newPosition;

                float speed = UnityEngine.Random.Range(23, 30);
                float angle = UnityEngine.Random.Range(80, 100);
                Vector2 velocity = default;
                velocity.x = speed * Mathf.Cos(angle * ((float)Math.PI / 180f));
                velocity.y = speed * Mathf.Sin(angle * ((float)Math.PI / 180f));

                Rigidbody2D rb2d = gameObject.GetComponent<Rigidbody2D>();
                rb2d.velocity = velocity;
            }
        }
    }
}