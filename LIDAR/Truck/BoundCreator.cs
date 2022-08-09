using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundCreator 
{
    private Component[] m_meshRendererList;
    private GameObject m_obj;
    private Bounds m_bounds;

    public BoundCreator(GameObject a_obj=null)
    {
        if(a_obj!=null)
        {
            m_meshRendererList = a_obj.GetComponentsInChildren<MeshRenderer>();
            m_obj = a_obj;
            m_bounds = CreateBound(m_meshRendererList,false);
        }
    }
       

  public void SetBound(GameObject a_obj,string a_name)
    {
        GameObject _obj = new GameObject();
        switch(a_name)
        {
            case "Truck":
                _obj = FindTruck(a_obj);
                m_meshRendererList = _obj.GetComponentsInChildren<MeshRenderer>();
                break;
            case "Cabin":
                _obj = a_obj.transform.parent.parent.gameObject;
                m_meshRendererList = _obj.GetComponentsInChildren<MeshRenderer>();
                break;
            case "Human":
                _obj = a_obj.transform.gameObject;
                m_meshRendererList = _obj.GetComponentsInChildren<CapsuleCollider>();
                break;
            default:
                _obj = a_obj.transform.gameObject;
                m_meshRendererList = _obj.GetComponentsInChildren<MeshRenderer>();
                break;
        }
        
        m_obj = a_obj;

        m_bounds = CreateBound(m_meshRendererList, a_name.Contains("Human")) ;
        
    }
    

    private GameObject FindTruck(GameObject a_obj)
    {
        GameObject _obj = a_obj;
        int[] _idxTab = new int[10];
        int i = 0;
        _idxTab[i] = _obj.transform.GetSiblingIndex();
        while (_obj.transform.parent!=null)
        {
            i += 1;
            _obj = _obj.transform.parent.gameObject;
            _idxTab[i] = _obj.transform.GetSiblingIndex();
        }
        _obj = _obj.transform.GetChild(_idxTab[i-1]).GetChild(_idxTab[i - 2]).gameObject;
        return _obj;
        
    }
    

    private Bounds CreateBound(Component[] a_compoList, bool a_human)
    {
        Bounds _bounds=new Bounds();
        if (a_compoList!=null)
        {
            if(a_human)
            {
                int i = 0;
                foreach (CapsuleCollider _caps in a_compoList )
                {
                    if (i == 0)
                    {
                        _bounds = _caps.bounds;
                        i += 1; 
                    }
                }
            }
            else
            {
                int i = 0;
                foreach (MeshRenderer _meshRenderer in a_compoList)
                {
                    if (i == 0)
                    {
                        _bounds = _meshRenderer.bounds;
                        i += 1;
                    }
                    else
                    {
                        _bounds.Encapsulate(_meshRenderer.bounds.center);
                    }
                }
            }
            
        }
        
        return _bounds;
    }

    public Bounds GetBounds() => m_bounds;

    private Vector3[,] CreateVector(Vector3 a_mid, Vector3 a_size)
    {
        int _counter = 0;
        Vector3[,] _array= new Vector3[2,12];
        for (int i = 1; i < 3; i++)
        {
            for (int k = 1; k < 3; k++)
            {
                
                Vector3 _1S = new Vector3(a_mid.x + Mathf.Pow(-1,i) * a_size.x/2, a_mid.y + a_size.y/2, a_mid.z + Mathf.Pow(-1, k) * a_size.z/2); 
                Vector3 _1E = new Vector3(a_mid.x + Mathf.Pow(-1, i) * a_size.x/2, a_mid.y - a_size.y/2, a_mid.z + Mathf.Pow(-1, k) * a_size.z/2);

                Vector3 _2S = new Vector3(a_mid.x + a_size.x/2, a_mid.y + Mathf.Pow(-1, i) * a_size.y/2, a_mid.z + Mathf.Pow(-1, k) * a_size.z/2);
                Vector3 _2E = new Vector3(a_mid.x - a_size.x/2, a_mid.y + Mathf.Pow(-1, i) * a_size.y/2, a_mid.z + Mathf.Pow(-1, k) * a_size.z/2);

                Vector3 _3S = new Vector3(a_mid.x + Mathf.Pow(-1, i) * a_size.x/2, a_mid.y + Mathf.Pow(-1, k) * a_size.y/2, a_mid.z + a_size.z/2);
                Vector3 _3E = new Vector3(a_mid.x + Mathf.Pow(-1, i) * a_size.x/2, a_mid.y + Mathf.Pow(-1, k) * a_size.y/2, a_mid.z - a_size.z/2);

                _array[0, _counter * 3] = _1S;
                _array[0, _counter * 3 + 1] = _2S;
                _array[0, _counter * 3 + 2] = _3S;


                _array[1, _counter * 3] = _1E;
                _array[1, _counter * 3 + 1] = _2E;
                _array[1, _counter * 3 + 2] = _3E;
                _counter += 1;
            }
        }

        return _array;
    }

    public void PrintBound(Bounds a_bound)
    {
        Vector3 _center = a_bound.center;
        Vector3 _size = a_bound.size;

        Vector3[,] _array = CreateVector(_center, _size);

        for (int i = 0; i < 12; i++)
        {
            Debug.DrawLine(_array[0, i],_array[1,i], Color.white, 0, false);
        }
        
    }

}
