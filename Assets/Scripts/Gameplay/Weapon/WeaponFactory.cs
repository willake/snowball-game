using UnityEngine;

namespace Game.Gameplay
{
    public class WeaponFactory : MonoBehaviour
    {
        public GameObject prefabSnowball;

        public Weapon GenerateWeapon(int id)
        {
            GameObject obj = Instantiate(prefabSnowball, Vector3.zero, Quaternion.identity);
            obj.name = "snowball";
            Weapon weapon = obj.GetComponent<Weapon>();
            return weapon;
        }
    }
}