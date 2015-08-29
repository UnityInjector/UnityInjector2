using UnityEngine;

namespace WarmodePayload
{
    public class MainPayloadFun : MonoBehaviour
    {
        public void OnGUI() => GUI.Label(new Rect(20.0F, 20.0F, 1000.0F, 1000.0F), "HELLO WORLD");
    }
}