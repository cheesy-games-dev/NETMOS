using System.Net.Sockets;
using UnityEditor;
using UnityEngine;

namespace BIMOS
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
                GameObject spawnPointManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.kadenzombie8.bimos/Assets/[BIMOS] Spawn Point Manager.prefab");
                GameObject spawnPointManagerInstance = PrefabUtility.InstantiatePrefab(spawnPointManagerPrefab) as GameObject;
                GameObjectUtility.SetParentAndAlign(spawnPointManagerInstance, null);
                spawnPointManager = spawnPointManagerInstance.GetComponent<SpawnPointManager>();
            }

            if (!spawnPointManager.SpawnPoint)
                spawnPointManager.SpawnPoint = spawnPointInstance.GetComponent<SpawnPoint>();

            GameObjectUtility.SetParentAndAlign(spawnPointInstance, spawnPointManager.gameObject);
        }

        [MenuItem("GameObject/BIMOS/Socket")]
        static void CreateSocket()
        {
            GameObject socket = new GameObject("Socket", typeof(Socket));

            GameObject attach = new GameObject("AttachPoint");
            GameObject detach = new GameObject("DetachPoint");
            attach.transform.parent = socket.transform;
            detach.transform.parent = socket.transform;

            socket.GetComponent<Socket>().AttachPoint = attach.transform;
            socket.GetComponent<Socket>().DetachPoint = detach.transform;

            GameObjectUtility.SetParentAndAlign(socket, Selection.activeGameObject);
        }

        [MenuItem("GameObject/BIMOS/Grabs/Snap")]
        static void CreateSnapGrab()
        {
            GameObject grab = new GameObject("Grab", typeof(SnapGrab));
            GameObjectUtility.SetParentAndAlign(grab, Selection.activeGameObject);
        }

        [MenuItem("GameObject/BIMOS/Grabs/Offhand")]
        static void CreateOffhandGrab()
        {
            GameObject grab = new GameObject("Grab", typeof(OffhandGrab));
            GameObjectUtility.SetParentAndAlign(grab, Selection.activeGameObject);
        }

        [MenuItem("GameObject/BIMOS/Grabs/Line")]
        static void CreateLineGrab()
        {
            GameObject grab = new GameObject("Grab", typeof(LineGrab));

            GameObject start = new GameObject("Start");
            GameObject end = new GameObject("End");
            start.transform.parent = grab.transform;
            start.transform.localPosition = Vector3.right * 0.5f;
            end.transform.parent = grab.transform;
            start.transform.localPosition = Vector3.left * 0.5f;

            grab.GetComponent<LineGrab>().Start = start.transform;
            grab.GetComponent<LineGrab>().End = end.transform;

            GameObjectUtility.SetParentAndAlign(grab, Selection.activeGameObject);
        }
    }
}
