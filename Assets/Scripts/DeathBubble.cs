using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class DeathBubble : MonoBehaviour {

    public void SetPlayer(int _playerNum)
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Bubble_" + _playerNum.ToString());
    }
}
