using UnityEngine;

namespace Game.Gameplay
{
    public class WeaponFactory : MonoBehaviour
    {
        public GameObject prefabPistol;

        public Weapon GenerateWeapon(int id)
        {
            GameObject obj = Instantiate(prefabPistol, Vector3.zero, Quaternion.identity);
            obj.name = "Pistol";
            Weapon weapon = obj.GetComponent<Weapon>();
            return weapon;
        }
    }
}