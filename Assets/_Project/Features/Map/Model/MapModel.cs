using UnityEngine;

namespace Huye.Features.Map.Model
{
    [System.Serializable]
    public class MapModel
    {
        [Header("Estado")]
        public bool IsOpen = false;

        [Header("UI")]
        public string PromptText = "Presiona E para ver el mapa";
    }
}
