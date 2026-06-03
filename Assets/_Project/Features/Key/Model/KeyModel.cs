using UnityEngine;

namespace Huye.Features.Key.Model
{
    [System.Serializable]
    public class KeyModel
    {
        [Header("Estado")]
        public bool IsCollected = false;

        [Header("UI")]
        public string PromptText = "Presiona E para recoger la llave";
    }
}
