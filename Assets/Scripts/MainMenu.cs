//Imports
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.Android;

//This block was giving error, so commenting
//#if UNITY_ANDROID
//using UnityEngine.Android;
//#endif

using Photon.Pun;

/// <summary>
/// MainMenu Class to Handle UI Buttons Events and Multiplayer
/// </summary>
public class MainMenu : MonoBehaviourPunCallbacks
{
    public Button button;
    public uint maxPlayers = 16;
    private uint playerInRoom = 0;


    /// <summary>
    /// Asks for required Android Device Permissions from User
    /// </summary>
    void AskForPermissions()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            Permission.RequestUserPermission(Permission.Camera);
    }

    /// <summary>
    /// Connect To Photon Servers using Available PhotonNetwork Settings
    /// </summary>
    void ConnectToPhotonServers()
    {
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// Starts The Actual Simulator
    /// Binded to <em>Join Game</em> button's <c>OnClick</c> event
    /// </summary>
    public void StartSimulator()
    {
        if (PhotonNetwork.InRoom)
        {
            //Load Actual Simulator Scene
            PhotonNetwork.LoadLevel(1);
        }
    }

    /// <summary>
    /// Quits the Simulator
    /// Binded to <em>Exit</em> button's <see cref="Button.onClick"/> event
    /// </summary>
    public void QuitSimulator()
    {
        //Disconnect from Photon Cloud Servers
        PhotonNetwork.Disconnect();

        //Quit Application
        Application.Quit();
    }

    /// <summary>
    /// Callback from <see cref="IInRoomCallbacks.OnPlayerEnteredRoom(Player)"/>
    /// Called when a new player joins the room
    /// </summary>
    /// <param name="newPlayer">Information regarding the joining Player</param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ++playerInRoom;
        button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "START ROOM (" + playerInRoom.ToString() + "/" + maxPlayers.ToString() + ")";
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        --playerInRoom;
        button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "START ROOM (" + playerInRoom.ToString() + "/" + maxPlayers.ToString() + ")";
    }

    /// <summary>
    /// Callback from <see cref="IMatchmakingCallbacks.OnJoinedRoom"/>
    /// Called when Photon connects to a Room
    /// </summary>
    public override void OnJoinedRoom()
    {
        playerInRoom = PhotonNetwork.CurrentRoom.PlayerCount;
        button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "START ROOM (" + playerInRoom.ToString() + "/" + maxPlayers.ToString() + ")";
    }

    /// <summary>
    /// Callback from <see cref="IConnectionCallbacks.OnConnected"/>
    /// Called when connected to Photon Servers
    /// </summary>
    public override void OnConnectedToMaster()
    {
        //Enable `Join Room` Button
        button.interactable = true;

        //Join or Create a random Room. Will layter be replaced by static Room naes stored in a persistent storage
        PhotonNetwork.JoinOrCreateRoom(
            "TestingRoom",          //Room Name
            new RoomOptions
            {
                IsOpen = true,      //This Room is open for all players
                IsVisible = true,   //This Room is visible in RoomList
                MaxPlayers = (byte)maxPlayers,    //This Room can have a Maximum of `maxPlayers` Players
            },
            TypedLobby.Default
        );
    }


    /// <summary>
    /// Callback from <see cref="IConnectionCallbacks.OnDisconnected(DisconnectCause)"/>
    /// Called when disconnected from Photon Servers
    /// </summary>
    public override void OnDisconnected(DisconnectCause cause)
    {
        button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "INTERNET UNAVAILABLE";
    }


    void Awake()
    {
        //Automtically Sync Scene for Multiplayer
        PhotonNetwork.AutomaticallySyncScene = true;

        //Only for Android Devices
#if UNITY_ANDROID
        //Ask for Camera permission from User
        AskForPermissions();
#endif
    }

    void Start()
    {
        //Set `Join Button` to be disabled until connected to Photon Servers
        button.interactable = false;

        //Connect to Photon Servers
        ConnectToPhotonServers();
    }
}
