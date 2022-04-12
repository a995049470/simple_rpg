using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{

    public class TreeManager : BaseTreeManager
    {
        [SerializeField]
        [Range(1, 60)]
        private int fps = 30;
        private void Start() {
            InitTree();
            OnValidate();
        }

        protected void Update() {
            Root.Instance.Update(Time.deltaTime);
        }

        protected void OnValidate() {
            Root.Instance.FPS = fps;
            Application.targetFrameRate = fps;
            QualitySettings.vSyncCount = 2;
        }

        
        
    }
}

