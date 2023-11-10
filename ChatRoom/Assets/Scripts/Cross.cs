using UnityEngine;

public class Cross : MonoBehaviour
{
    private void OnMouseUp()
    {
         WuZiQiManager.Instance.OnClickChess(this);
    }
}
