using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleUsage : MonoBehaviour
{
    public BackgroundController backgroundController;

    void Start()
    {
      
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // ��ʼ��Բ�Ǿ��ε�λ�á���С��Բ�ǰ뾶
            Vector4[] positions = new Vector4[]
            {
            new Vector4(0.3f, 0.54f, 0, 0),
           new Vector4(0.7f, 0.54f, 0, 0),
            };

            Vector4[] sizes = new Vector4[]
            {
            new Vector4(0.1f, 0.1f, 0, 0),
           new Vector4(0.1f, 0.1f, 0, 0),
            };

            Vector4[] radii = new Vector4[]
            {
            new Vector4(0.01f, 2, 0, 0),
            new Vector4(0.01f, 2, 0, 0),
            };

            backgroundController.UpdateRectangles(positions, sizes, radii);
        }
        //// ��ʼ��Բ�Ǿ��ε�λ�á���С��Բ�ǰ뾶
        //Vector4[] positions = new Vector4[]
        //{
        //    new Vector4(0.25f, 0.25f, 0, 0),
        //    new Vector4(0.75f, 0.75f, 0, 0)
        //};

        //Vector4[] sizes = new Vector4[]
        //{
        //    new Vector4(0.2f, 0.2f, 0, 0),
        //    new Vector4(0.2f, 0.2f, 0, 0)
        //};

        //Vector4[] radii = new Vector4[]
        //{
        //    new Vector4(0.05f, 0, 0, 0),
        //    new Vector4(0.05f, 0, 0, 0)
        //};

        //backgroundController.UpdateRectangles(positions, sizes, radii);
    }
}
