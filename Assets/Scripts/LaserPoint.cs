using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaserPoint : MonoBehaviour
{
    private LineRenderer laser;        // ������
    private RaycastHit Collided_object; // �浹�� ��ü
    private GameObject currentObject;   // ���� �ֱٿ� �浹�� ��ü�� �����ϱ� ���� ��ü

    public Shader lit;
    public MeshRenderer targetSphere;
    public float raycastDistance = 100f; // ������ ������ ���� �Ÿ�

    // Start is called before the first frame update
    void Start()
    {
        // ��ũ��Ʈ�� ���Ե� ��ü�� ���� ��������� ������Ʈ�� �ְ��ִ�.
        laser = this.gameObject.AddComponent<LineRenderer>();

        // ������ �������� ���� ǥ��
        Material material = new Material(lit);
        material.color = new Color(1, 1, 0.5f, 0.5f);
        laser.material = material;
        // �������� �������� 2���� �ʿ� �� ���� ������ ��� ǥ�� �� �� �ִ�.
        laser.positionCount = 2;
        // ������ ���� ǥ��
        laser.startWidth = 0.01f;
        laser.endWidth = 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        laser.SetPosition(0, transform.position); // ù��° ������ ��ġ
                                                   // ������Ʈ�� �־� �����ν�, �÷��̾ �̵��ϸ� �̵��� ���󰡰� �ȴ�.
                                                   //  �� �����(�浹 ������ ����)
        Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.green, 0.5f);

        // �浹 ���� ��
        if (Physics.Raycast(transform.position, transform.forward, out Collided_object, raycastDistance))
        {
            laser.SetPosition(1, Collided_object.point);

            targetSphere.gameObject.SetActive(true);
            targetSphere.transform.position = Collided_object.point;
            targetSphere.material.color = laser.material.color;

            // �浹 ��ü�� �±װ� Button�� ���
            if (Collided_object.collider.gameObject.CompareTag("Button"))
            {
                //if (OVRInput.Get())
                // ��ŧ���� �� �����ܿ� ū ���׶�� �κ��� ���� ���
                if (OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Mouse0))
                {
                    EffectManager.instance.PlayEffect(2);
                    // ��ư�� ��ϵ� onClick �޼ҵ带 �����Ѵ�.
                    Collided_object.collider.gameObject.GetComponent<Button>().onClick.Invoke();
                }

                else
                {
                    Collided_object.collider.gameObject.GetComponent<Button>().OnPointerEnter(null);
                    currentObject = Collided_object.collider.gameObject;
                }
            }
            else if(currentObject != null)
            {
                currentObject.GetComponent<Button>().OnPointerExit(null);
                currentObject = null;
            }
        }

        else
        {
            // �������� ������ ���� ���� ������ ������ �ʱ� ���� ���̸�ŭ ��� �����.
            laser.SetPosition(1, transform.position + (transform.forward * raycastDistance));
            targetSphere.gameObject.SetActive(false);

            // �ֱ� ������ ������Ʈ�� Button�� ���
            // ��ư�� ���� �����ִ� �����̹Ƿ� �̰��� Ǯ���ش�.
            if (currentObject != null)
            {
                currentObject.GetComponent<Button>().OnPointerExit(null);
                currentObject = null;
            }

        }

    }

    private void LateUpdate()
    {
        // ��ư�� ���� ���        
        if (OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            laser.material.color = new Color(1, 0, 0, 0.5f);
            //EffectManager.instance.PlayEffect(2);
        }

        // ��ư�� �� ���          
        else if (OVRInput.GetUp(OVRInput.Button.One) || Input.GetKeyUp(KeyCode.Mouse0))
        {
            laser.material.color = new Color(1, 1, 0.5f, 0.5f);
            //EffectManager.instance.PlayEffect(2);
        }
    }

    public void Clicked()
    {
        //Debug.Log(1);
    }
}

