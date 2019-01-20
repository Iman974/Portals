using UnityEngine;

public class PortalTextureInitializer : MonoBehaviour {

    [SerializeField] private Camera camera1 = null;
    [SerializeField] private Material camera1Mat = null;
    [SerializeField] private Camera camera2 = null;
    [SerializeField] private Material camera2Mat = null;
    [SerializeField] private Camera camera3 = null;
    [SerializeField] private Material camera3Mat = null;

    private void Start() {
        if (camera1.targetTexture != null) {
            camera1.targetTexture.Release();
        }
        camera1.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        camera1Mat.mainTexture = camera1.targetTexture;

        if (camera2.targetTexture != null) {
            camera2.targetTexture.Release();
        }
        camera2.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        camera2Mat.mainTexture = camera2.targetTexture;

        if (camera3.targetTexture != null) {
            camera3.targetTexture.Release();
        }
        camera3.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        camera3Mat.mainTexture = camera3.targetTexture;
    }
}
