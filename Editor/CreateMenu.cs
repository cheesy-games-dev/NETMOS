using KadenZombie8.BIMOS.Rig;
using KadenZombie8.BIMOS.Rig.Spawning;
using KadenZombie8.BIMOS.Sockets;
using UnityEditor;
using UnityEngine;

namespace KadenZombie8.BIMOS.Samples.Editor
{
    public class CreateMenu : MonoBehaviour
    {
        [MenuItem("GameObject/BIMOS/Spawn Point")]
        static void CreateSpawnPoint()
        {
            GameObject spawnPointPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.kadenzombie8.bimos/Assets/[BIMOS] Spawn Point.prefab");
            GameObject spawnPointInstance = PrefabUtility.InstantiatePrefab(spawnPointPrefab) as GameObject;

            SpawnPointManager spawnPointManager = FindFirstObjectByType<SpawnPointManager>();

            if (!spawnPointManager)
            {
                GameObject spawnPointManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.kadenzombie8.bimos/Assets/[BIMOS] Managers.prefab");
                GameObject spawnPointManagerInstance = PrefabUtility.InstantiatePrefab(spawnPointManagerPrefab) as GameObject;
                GameObjectUtility.SetParentAndAlign(spawnPointManagerInstance, null);
                spawnPointManager = spawnPointManagerInstance.GetComponentInChildren<SpawnPointManager>();
            }

            if (!spawnPointManager.SpawnPoint)
                spawnPointManager.SpawnPoint = spawnPointInstance.GetComponent<SpawnPoint>();

            GameObjectUtility.SetParentAndAlign(spawnPointInstance, spawnPointManager.gameObject);
        }

        [MenuItem("GameObject/BIMOS/Socket")]
        static void CreateSocket()
        {
            GameObject socket = new("Socket", typeof(Socket));

            GameObject attach = new("AttachPoint");
            GameObject detach = new("DetachPoint");
            attach.transform.parent = socket.transform;
            detach.transform.parent = socket.transform;

            socket.GetComponent<Socket>().AttachPoint = attach.transform;
            socket.GetComponent<Socket>().DetachPoint = detach.transform;

            GameObjectUtility.SetParentAndAlign(socket, Selection.activeGameObject);
        }

        [MenuItem("GameObject/BIMOS/Grabbables/Snap")]
        static void CreateSnapGrab()
        {
            GameObject grab = new("Grab", typeof(SnapGrabbable));
            GameObjectUtility.SetParentAndAlign(grab, Selection.activeGameObject);
        }

        [MenuItem("GameObject/BIMOS/Grabbables/Offhand")]
        static void CreateOffhandGrab()
        {
            GameObject grab = new("Grab", typeof(OffhandGrabbable));
            GameObjectUtility.SetParentAndAlign(grab, Selection.activeGameObject);
        }

        [MenuItem("GameObject/BIMOS/Grabbables/Line")]
        static void CreateLineGrab()
        {
            GameObject grab = new("Grab", typeof(LineGrabbable));

            GameObject start = new("Start");
            GameObject end = new("End");
            start.transform.parent = grab.transform;
            start.transform.localPosition = Vector3.right * 0.5f;
            end.transform.parent = grab.transform;
            start.transform.localPosition = Vector3.left * 0.5f;

            grab.GetComponent<LineGrabbable>().Start = start.transform;
            grab.GetComponent<LineGrabbable>().End = end.transform;

            GameObjectUtility.SetParentAndAlign(grab, Selection.activeGameObject);
        }
    }
}
