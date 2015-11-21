using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NetworkObjectPool : Photon.MonoBehaviour {
	[HideInInspector]
	public List<int> SpawnedObjects = new List<int> ();
	[HideInInspector]
	public List<GameObject> PooledObjects = new List<GameObject> ();
	
	void OnJoinedRoom()
	{
		if(isMaster)
			BuildPool ();
	}
	
	public virtual void BuildPool()
	{
		for (int i=0; i < 10; i++) {
			PoolUnit("minion_alpha");
			PoolUnit("minion_beta");
		}
	}
	void Awake()
	{
		GetInstance ();
	}
	public static NetworkObjectPool instance;
	
	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>The instance.</value>
	public static NetworkObjectPool Instance
	{
		get
		{
			return instance;
		}
	}
	void GetInstance()
	{
		instance = this;
	}
	void SendPlayerCurrentSpawnedAndPooled(PhotonPlayer playerTarget)
	{
		photonView.RPC ("ReceiveSpawnedObjectList", playerTarget, SpawnedObjects.ToArray ());
		List<int> pooledIDs = new List<int> ();
		foreach (GameObject obj in PooledObjects)
		{
			pooledIDs.Add (FindView (obj));
		}
		photonView.RPC ("ReceivePooledObjectList", playerTarget, pooledIDs.ToArray ());
	}
	
	[PunRPC]
	public void ReceiveSpawnedObjectList(int[] toSpawn)
	{
		Debug.Log ("From Master Server: <color=green>"+toSpawn.Length+"  Units spawned</color>");
		foreach (int activeThis in toSpawn)
		{
			FindGO (activeThis).SetActive (true);  
		}
	}
	
	[PunRPC]
	public void ReceivePooledObjectList(int[] pooled)
	{
		foreach(int obj in pooled)
		{
			FindGO (obj).transform.parent = transform;
			FindGO (obj).SetActive(false);
			PooledObjects.Add (FindGO (obj));
		}
	}
	
	/// <summary>
	/// Despawns the object. Must be called only from master server
	/// </summary>
	/// <param name="obj">Object.</param>
	public virtual void DespawnObject(GameObject obj)
	{
		if (!isMaster)
			return;
		
		int objectID = obj.GetPhotonView ().viewID;
		photonView.RPC ("RPC_DespawnObject", PhotonTargets.AllBufferedViaServer, objectID);
		//[[Updating the list]]
		if(SpawnedObjects.Contains (objectID))
			SpawnedObjects.Remove (objectID);  
	}
	
	public virtual GameObject SpawnObject(String name, Vector3 spawnPosition)
	{
		if (!isMaster)
			return null;

		GameObject NextObjectFound = NextToSpawn (name); // check null?
		if (!NextObjectFound)
		{
			Debug.Log ("<color=red>Not Enough Object in the Pool</color>");
			return null;
		}
		int objectID = NextObjectFound.GetPhotonView ().viewID;
		
		photonView.RPC ("RPC_SpawnObject", PhotonTargets.AllBufferedViaServer, spawnPosition, objectID);
		//[[Updating the list]]
		SpawnedObjects.Add (objectID);  

		return NextObjectFound;
	}
	
	
	[PunRPC]
	public void RPC_DespawnObject(int ourObjectID)
	{
		FindGO (ourObjectID).SetActive (false);
		FindGO (ourObjectID).transform.position = Vector3.zero;
		
	}
	
	[PunRPC]
	public void RPC_SpawnObject(Vector3 spawnPos, int ourObjectID)
	{
		GameObject obj = FindGO (ourObjectID);
		obj.transform.position = spawnPos;
		obj.SetActive (true);
		if (obj.tag == "minion") {
			Debug.Log (obj.name);
			if (obj.name == "minion_alpha(Clone)")
				obj.GetComponent<MinionController> ().initState (GameManager.team.ALPHA);
			else 
				obj.GetComponent<MinionController> ().initState (GameManager.team.BETA);
		}
	}
	
	/// <summary>
	/// create unit as scene object, add it to pooled list, and immediatly disable it.
	/// </summary>
	/// <returns>The unit.</returns>
	public GameObject PoolUnit(string UnitName)
	{
		GameObject newUnit = PhotonNetwork.InstantiateSceneObject
			(
				UnitName,
				Vector3.zero,
				Quaternion.identity,
				0,
				null
				);
		newUnit.transform.parent = transform;
		newUnit.SetActive(false);
		PooledObjects.Add (newUnit);
		return newUnit;
	}
	#region PhotonEvents
	void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		if (isMaster)
			SendPlayerCurrentSpawnedAndPooled (newPlayer);
		
	}
	void OnMasterClientSwitched()
	{
		
		if (isMaster)
		{Debug.Log ("you are now MasterClient");
			SpawnedObjects.Clear ();
			for(int i =0; i < PooledObjects.Count; i++)
			{
				if(PooledObjects[i].activeSelf)
					SpawnedObjects.Add (PooledObjects[i].GetPhotonView().viewID);
			}
		}
	}
	#endregion
	#region helper
	
	private GameObject FindGO(int view_ID)
	{
		PhotonView v = PhotonView.Find (view_ID);
		if (v.viewID != 0)
			return v.gameObject;
		else
			return null;
	}
	
	private int FindView(GameObject go)
	{
		return go.GetPhotonView ().viewID;
	}
	
	public bool isMine
	{
		get
		{
			return photonView.isMine;
		}
	}
	
	public bool isMaster
	{
		get
		{
			return PhotonNetwork.isMasterClient;
		}
	}
	
	public bool IsSpawned(GameObject _o)
	{
		return SpawnedObjects.Contains (FindView (_o));
	}
	
	public bool SpawnedLeft()
	{
		return SpawnedObjects.Count > 0;
	}
	
	private GameObject NextToSpawn(String name)
	{
		//Debug.Log (PooledObjects.Count);
		int i = 0;
		for (i = 0; i < PooledObjects.Count; i++)
		{
			if(PooledObjects[i].name == name+"(Clone)" && !PooledObjects[i].activeSelf)
				return PooledObjects[i];
		}
		//TODO Case: we dont have enough units in our pool
		
		return null;
	}
	
	private GameObject NextToDespawn(GameObject _type)
	{
		GameObject go;
		for (int i = 0; i < SpawnedObjects.Count; i++)
		{
			go = FindGO(SpawnedObjects[i]);
			if(go.name == _type.name+"(Clone)" && !go.activeSelf)
				return go;
		}
		return null;
	}
	#endregion
}