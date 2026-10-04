using UnityEngine;

namespace Stockwell.World
{
    /// <summary>
    /// A named place the player can arrive at. The door names the spawn point it wants
    /// and the arriving scene supplies it, so neither scene needs a reference to the other.
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        [Tooltip("Must match the spawnPointId the incoming door asked for, e.g. 'StoreEntrance'.")]
        [SerializeField] private string id = "Default";

        public string Id => id;

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.9f);
            Gizmos.DrawWireSphere(transform.position + Vector3.up, 0.35f);
            Gizmos.DrawRay(transform.position + Vector3.up, transform.forward);
        }
    }
}
