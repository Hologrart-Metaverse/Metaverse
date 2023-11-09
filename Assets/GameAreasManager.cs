using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GameAreasManager : MonoBehaviour
{
    public static GameAreasManager Instance { get; private set; }

    [SerializeField] private AllGamesSO allGames;

    private void Awake()
    {
        Instance = this;
        if (PhotonNetwork.IsMasterClient)
        {
            GameAreas gameAreas = new GameAreas();
            foreach (var game in allGames.gameList)
            {
                Dictionary<int, bool> dict = new();
                gameAreas.gameAreas.Add(game.gameId, dict);
            }
            var gameAreasJson = JsonHelper<GameAreas>.Serialize(gameAreas);
            Debug.Log(gameAreasJson);
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "GameAreas", gameAreasJson } });
        }
    }
    public int FindEmptyAreaViewId(GameSO.GameId gameId)
    {
        string json = (string)PhotonNetwork.CurrentRoom.CustomProperties["GameAreas"];
        Debug.Log(json);
        GameAreas areas = JsonHelper<GameAreas>.Deserialize(json);
        foreach (var area in areas.gameAreas)
        {
            //Tum arealardan sadece game id si ayný olanlarý çek
            if (area.Key == gameId)
            {
                // Areadaki alan viewId - doluluk durumu dictionary sine bak boþ olan alaný bul
                for (int i = 0; i < area.Value.Count; i++)
                {
                    var p = area.Value.ElementAt(i);
                    Debug.Log(p.Key);
                    if (!p.Value)
                    {
                        area.Value[p.Key] = true;
                        var gameAreasJson = JsonHelper<GameAreas>.Serialize(areas);
                        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "GameAreas", gameAreasJson } });
                        return p.Key;
                    }
                }
            }
        }
        return -1;
    }
    public void AddOrFillGameArea(GameSO.GameId gameId, int viewId)
    {
        bool isAdded = false;
        string json = (string)PhotonNetwork.CurrentRoom.CustomProperties["GameAreas"];
        GameAreas areas = JsonHelper<GameAreas>.Deserialize(json);
        var places = areas.gameAreas[gameId];
        for (int i = 0; i < places.Count; i++)
        {
            var place = places.ElementAt(i);
            if (place.Key == viewId)
            {
                isAdded = true;
                places[viewId] = true;
            }
        }
        if (!isAdded)
        {
            places.Add(viewId, true);
        }
        var gameAreasJson = JsonHelper<GameAreas>.Serialize(areas);
        Debug.Log("son durum: " + gameAreasJson);
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "GameAreas", gameAreasJson } });
    }
    public void MakeAreaEmpty(GameSO.GameId gameId, int areaViewId)
    {
        string json = (string)PhotonNetwork.CurrentRoom.CustomProperties["GameAreas"];
        Debug.Log("ilk durum: " + json);
        GameAreas areas = JsonHelper<GameAreas>.Deserialize(json);
        var places = areas.gameAreas[gameId];
        places[areaViewId] = false;
        var gameAreasJson = JsonHelper<GameAreas>.Serialize(areas);
        Debug.Log("son durum: " + gameAreasJson);
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "GameAreas", gameAreasJson } });
    }
    public int GetAreaCountByGameId(GameSO.GameId gameId)
    {
        string json = (string)PhotonNetwork.CurrentRoom.CustomProperties["GameAreas"];
        GameAreas areas = JsonHelper<GameAreas>.Deserialize(json);
        return areas.gameAreas[gameId].Count;
    }
}
public class GameAreas
{
    public Dictionary<GameSO.GameId, Dictionary<int, bool>> gameAreas = new();
}