using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gltLoader : MonoBehaviour
{
    public string currentUrl = "http://ozone.kuubix.be/3D.glb";
    public float speed = 1.0f;
    private string oldUrl = "";


    // Start is called before the first frame update
    void Start()
    {
        SetGltfAsset(currentUrl);
    }

    // Update is called once per frame
    void Update()
    {
        if (oldUrl != currentUrl) {
            SetGltfAsset(currentUrl);
        }
        
            Vector3 currentRot = transform.rotation.eulerAngles;
            currentRot.y += 0.1f * Time.deltaTime * speed;
            
            // Pingpong lerp z between 0 and 1
            float z = Mathf.PingPong(Time.time * (speed /500), 1);
            currentRot.z = Mathf.Lerp(-20, 20, z);

            transform.rotation = Quaternion.Euler(currentRot);
    }
    
    public void SetGltfAsset(string url) {
        var existing = gameObject.GetComponent<GLTFast.GltfAsset>();
        if (existing != null) {
            Destroy(existing);
        }
        
        GLTFast.GltfAsset gltf = gameObject.AddComponent<GLTFast.GltfAsset>();
        gltf.Url = url;
        oldUrl = url;
    }
}
