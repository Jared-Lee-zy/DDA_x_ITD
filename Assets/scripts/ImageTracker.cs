using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ImageTracker : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager trackedImageManager;

    [SerializeField]
    private GameObject[] placeablePrefabs;

    public Vector3 rotationOffset = new Vector3(0, 180, 0);

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    private Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

    private string[] someArray = new string[]{"Image1", "Image2", "Image3"};

    private void Start()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.AddListener(OnImageChanged);
            SetupPrefabs();
        }
    }

    void SetupPrefabs()
    {
        foreach (GameObject prefab in placeablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab);
            newPrefab.name = prefab.name;
            newPrefab.SetActive(false);
            spawnedPrefabs.Add(prefab.name, newPrefab);
            spawnedObjects.Add(newPrefab, prefab);
        }
    }

    void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }

        foreach (KeyValuePair<TrackableId, ARTrackedImage> lostObj in eventArgs.removed)
        {
            UpdateImage(lostObj.Value);
        }
    }

    void UpdateImage(ARTrackedImage trackedImage)
    {
        if(trackedImage != null)
        {
            if (trackedImage.trackingState == TrackingState.Limited || trackedImage.trackingState == TrackingState.None)
            {
                //Disable the associated content
                spawnedPrefabs[trackedImage.referenceImage.name].transform.SetParent(null);
                spawnedPrefabs[trackedImage.referenceImage.name].SetActive(false);
            }
            else if (trackedImage.trackingState == TrackingState.Tracking)
            {
                Debug.Log(trackedImage.gameObject.name + " is being tracked.");
                //Enable the associated content
                if(spawnedPrefabs[trackedImage.referenceImage.name].transform.parent != trackedImage.transform)
                {
                    Debug.Log("Enabling associated content: " + spawnedPrefabs[trackedImage.referenceImage.name].name);
                    spawnedPrefabs[trackedImage.referenceImage.name].transform.SetParent(trackedImage.transform);
                    spawnedPrefabs[trackedImage.referenceImage.name].transform.localPosition = spawnedObjects[spawnedPrefabs[trackedImage.referenceImage.name]].transform.localPosition;
                    Vector3 imageForward = trackedImage.transform.forward; // image's facing direction
                    Vector3 lookDirection = -imageForward;                 // opposite direction
                    Quaternion lookRotation = Quaternion.LookRotation(lookDirection, trackedImage.transform.up);

                    // Apply extra offset so you can control how the prefab faces
                    spawnedPrefabs[trackedImage.referenceImage.name].transform.rotation = 
                        lookRotation * Quaternion.Euler(rotationOffset);
                    spawnedPrefabs[trackedImage.referenceImage.name].SetActive(true);
                }
            }
        }
    }
}
