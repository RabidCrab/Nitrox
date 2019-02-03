using System;
using System.Collections.Generic;
using NitroxClient.Communication.Abstract;
using NitroxClient.GameLogic;
using NitroxModel.Core;
using NitroxModel.DataStructures.GameLogic;
using NitroxModel.DataStructures.Util;
using NitroxModel.Logger;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NitroxClient.MonoBehaviours.Gui.MainMenu
{
    public class SpawnCommand : MonoBehaviour
    {
        private const string DEFAULT_IP_ADDRESS = "127.0.0.1";
        private GameObject multiplayerClient;
        private IMultiplayerSession multiplayerSession;
        private string userName;
        private SpawnConsoleCommand spawnConsoleCommand = new SpawnConsoleCommand();

        public void Awake()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            DontDestroyOnLoad(gameObject);
        }

        public void OnDestroy()
        {
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        public void OnConsoleCommand_sub(NotificationCenter.Notification n)
        {
            if (n?.data?.Count > 0)
            {
                TechType techType;

                if (!UWE.Utils.TryParseEnum((string)n.data[0], out techType))
                {
                    Log.InGame("Could not parse " + n.data[0] + " to TechType");
                    return;
                }

                switch (techType)
                {
                    // Vehicles get their own special treatment
                    case TechType.Cyclops:
                        string guid = Guid.NewGuid().ToString();
                        // Because why not
                        Vector4 yellow = new Vector4(0.846f, 1f, 0.231f, 1);
                        Vector3 yellowHSB = new Vector3(0.2f, 0.8f, 1);
                        Vector4 green = new Vector4(0.003f, 0.795f, 0f, 1);
                        Vector3 greenHSB = new Vector3(0.3f, 1, 0.8f);
                        Vector3 black = new Vector3(0, 0, 0);
                        Vector3 blackHSB = new Vector3(1, 1, 0);
                        Vector3[] HSB = new Vector3[4] { yellowHSB, greenHSB, yellowHSB, blackHSB };
                        Vector3[] Colours = new Vector3[4] { yellow, green, yellow, black };

                        VehicleModel newVehicle = new VehicleModel(TechType.Cyclops,
                            guid,
                            MainCamera.camera.transform.position + 20f * MainCamera.camera.transform.forward,
                            Quaternion.LookRotation(MainCamera.camera.transform.right),
                            Optional<List<InteractiveChildObjectIdentifier>>.Empty(),
                            Optional<string>.Empty(),
                            "Nitrox",
                            HSB,
                            Colours);

                        NitroxServiceLocator.LocateService<Vehicles>().CreateVehicle(newVehicle);
                        NitroxServiceLocator.LocateService<Vehicles>().BroadcastCreatedVehicle(newVehicle);
                        break;
                    default:

                        break;
                }
            }
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            DevConsole.RegisterConsoleCommand(this, "sub", false);
        }
    }
}
