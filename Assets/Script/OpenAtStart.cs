using UnityEngine;

public class OpenAtStart : MonoBehaviour
{
    public DoorSlideController[] doors;

    void Start()
    {
        foreach (var door in doors)
        {
            if (door != null)
                door.OpenDoor();
        }
    }
}
