using UnityEngine;

namespace Script.Unity.Hand
{
    public class BlockPiece : MonoBehaviour
    {
        [SerializeField] private GameObject sprite;

        private void Awake()
        {
            Show();
        }

        public void Show()
        {
            sprite.SetActive(true);
        }

        public void Hide()
        {
            sprite.SetActive(false);
        }
    }
}