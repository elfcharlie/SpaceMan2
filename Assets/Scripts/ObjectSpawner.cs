using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{


    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        Vector2 currentDestination = SetRandomDestination();
        Instantiate(obj, new Vector3(currentDestination.x, currentDestination.y, 0f), Quaternion.identity, transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount < 4)
        {
            InstantiateObject();
        }
    }

    Vector2 SetRandomDestination()
    {
        // Set a random destination within a specified range
        Vector2 currentDestination = new Vector2(Random.Range(-2.8f, 2.8f), Random.Range(-4.97f, 4.97f));
        currentDestination = CheckBoundaries(currentDestination);
        return currentDestination;
    }

    Vector2 CheckBoundaries(Vector2 currentDestination)
    {
        // Check if the circle is outside the screen boundaries
        if (!IsInScreen(currentDestination))
        {
            // Change direction
            currentDestination = SetRandomDestination();        
        }
        return currentDestination;
    }
    bool IsInScreen(Vector2 currentDestination)
    {
        // Check if the circle is within the screen boundaries
        Vector2 screenPos = Camera.main.WorldToScreenPoint(currentDestination);
        return screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height;
    }

    void InstantiateObject()
    {
        Vector2 currentDestination = SetRandomDestination();
        GameObject newGameObject = Instantiate(obj, new Vector3(currentDestination.x, currentDestination.y, 0f), Quaternion.identity, transform);

        Sprite newSprite = Resources.Load<Sprite>("Sprites/Hellmanns/Hellmanns1");
        newGameObject.GetComponent<SpriteRenderer>().sprite = newSprite;
    }
    
}
