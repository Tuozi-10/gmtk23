using UnityEngine;

namespace src.Extensions
{
    public static class VectorsExtensions
    {
        #region Vector3

        public static Vector3 AddRandXYZ(this Vector3 vector, float rand)
        {
            return vector + new Vector3(Random.Range(-rand, rand), Random.Range(-rand, rand), Random.Range(-rand, rand));
        }
        
        public static Vector3 AddRandXZ(this Vector3 vector, float rand)
        {
            return vector + new Vector3(Random.Range(-rand, rand), 0, Random.Range(-rand, rand));
        }

        public static Vector3 AddRandXY(this Vector3 vector, float rand)
        {
            return vector + new Vector3(Random.Range(-rand, rand), Random.Range(-rand, rand),0);
        }
        
        #endregion
        
        #region Vector2

        public static Vector2 AddRandXY(this Vector2 vector, float rand)
        {
            return vector + new Vector2(Random.Range(-rand, rand), Random.Range(-rand, rand));
        }
        
        public static Vector2 AddRandX(this Vector2 vector, float rand)
        {
            return vector + new Vector2(Random.Range(-rand, rand), 0);
        }

        public static Vector2 AddRandY(this Vector2 vector, float rand)
        {
            return vector + new Vector2(0, Random.Range(-rand, rand));
        }
        
        #endregion
        
    }
}