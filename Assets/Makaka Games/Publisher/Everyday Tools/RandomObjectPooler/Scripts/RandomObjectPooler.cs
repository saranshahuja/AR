/*
===================================================================
Unity Assets by MAKAKA GAMES: https://makaka.org/o/all-unity-assets
===================================================================

Online Docs (Latest): https://makaka.org/unity-assets
Offline Docs: You have a PDF file in the package folder.

=======
SUPPORT
=======

First of all, read the docs. If it didn’t help, get the support.

Web: https://makaka.org/support
Email: info@makaka.org

If you find a bug or you can’t use the asset as you need, 
please first send email to info@makaka.org
before leaving a review to the asset store.

I am here to help you and to improve my products for the best.
*/

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[HelpURL("https://makaka.org/unity-assets")]
public class RandomObjectPooler : MonoBehaviour
{
    [Range(1, 30)]
    public int initPooledAmount = 7;
	public Transform poolParent = null;

    [Space]
    [SerializeField]
    private bool isDebugLogging = false;

    [Space]
    public Transform positionAtInit = null;
    public Transform rotationAtInit = null;

    [Header("Single (actual for Testing target prefab; None => Multiple)")]
    public GameObject prefab;

    [Header("Multiple")]
    public bool areRandomizedObjectsWhenCreating = false;
    public bool areRandomizedObjectsWhenGetting = false;

    public GameObject[] prefabs;

	[Header("Events")]
    [Space]
    public UnityEvent OnInitialized;

	[HideInInspector]
	public List<GameObject> pooledObjects = null;

    private List<GameObject> availableRandomizedObjects = null;

    private GameObject currentInstantiated = null;

    [HideInInspector]
    public List<MonoBehaviour> controlScripts;
	
    private MonoBehaviour controlScriptTempForRegistration;
    
    private System.Type controlScriptType;
	
	private void Start()
    {
        InitAndPopulatePool();
    }

    private void InitAndPopulatePool()
    {
        pooledObjects = new List<GameObject>();

        availableRandomizedObjects = new List<GameObject>();
        
        for (int i = 0; i < initPooledAmount; i++)
        {
            pooledObjects.Add(InstantiateObject(i));
        }

        OnInitialized?.Invoke();
    }

    public void InitControlScripts(System.Type type)
    {
        controlScripts = new List<MonoBehaviour>();

        controlScriptType = type;
    }

    private GameObject InstantiateObject(int index)
    {
        GameObject tempPrefab;

        if (prefab)
        {
            tempPrefab = prefab;
        }
        else if (areRandomizedObjectsWhenCreating)
        {
            tempPrefab = prefabs[Random.Range(0, prefabs.Length - 1)];
        }
        else
        {
            tempPrefab = prefabs[index % prefabs.Length];
        }

        if (tempPrefab)
        {
            currentInstantiated = Instantiate(
                tempPrefab,
                positionAtInit
                    ? positionAtInit.position
                    : tempPrefab.transform.position,
                rotationAtInit
                    ? rotationAtInit.rotation
                    : tempPrefab.transform.rotation,
                poolParent);

            currentInstantiated.SetActive(false);
            currentInstantiated.name = tempPrefab.name + index;
        }
        else
        {
            currentInstantiated = null;

            if (isDebugLogging)
            {
                DebugPrinter.Print("Throwing Object is not Assigned!");
            }
        }

        return currentInstantiated;
    }

    public GameObject GetPooledObject()
	{
        availableRandomizedObjects.Clear();

        for (int i = 0; i < pooledObjects.Count; i++) 
		{
			if (!pooledObjects[i])
            {
                //DebugPrinter.Print("GetPooledObject(): Create New Instance");

                pooledObjects[i] = InstantiateObject(i);

                return pooledObjects[i];
            }

            if (!pooledObjects[i].activeInHierarchy)
			{
                if (areRandomizedObjectsWhenGetting)
                {
                    availableRandomizedObjects.Add(pooledObjects[i]);
                }
                else
                {
                    return pooledObjects[i];
                }
            }    
		}

        if (areRandomizedObjectsWhenGetting
            && availableRandomizedObjects.Count > 0)
        {
            return availableRandomizedObjects[
                Random.Range(0, availableRandomizedObjects.Count - 1)];
        }
        else
        {
            if (isDebugLogging)
            {
                DebugPrinter.Print("GetPooledObject():" +
                    " All Game Objects in Pool are not available");
            }

            return null;
        }
	}

    /// <summary>
    /// For initial registration (cashing)
    /// and subsequent getting Control Script of GameObject
    /// </summary>
    public MonoBehaviour RegisterControlScript(GameObject gameObject)
    {
		controlScriptTempForRegistration = null;

		// Search of cached Control Script
		for (int i = 0; i < controlScripts.Count; i++)
		{
			controlScriptTempForRegistration = controlScripts[i];

			if (controlScriptTempForRegistration)
			{
				if (controlScriptTempForRegistration.gameObject == gameObject)
				{
                    //DebugPrinter.Print(i);

                    break;
				}
				else
				{
					controlScriptTempForRegistration = null;
				}
			}
			else // Game Object is null
			{
				controlScripts.RemoveAt(i);

                //DebugPrinter.Print("Remove null Control Script from List");
            }
        }

		if (!controlScriptTempForRegistration)
		{
			controlScriptTempForRegistration =
                gameObject.GetComponent(controlScriptType) as MonoBehaviour;

            //DebugPrinter.Print("Try to get Control Script");

            if (controlScriptTempForRegistration)
            {
			    controlScripts.Add(controlScriptTempForRegistration);

                //DebugPrinter.Print("Register New Control Script");
            }

        }

		return controlScriptTempForRegistration;
    }
}