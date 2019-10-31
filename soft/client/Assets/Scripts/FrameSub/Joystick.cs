using UnityEngine;  
using System.Collections;  
using System.Collections.Generic;  

public class Joystick : MonoBehaviour  
{
    [SerializeField]
    private string m_name;
    [SerializeField]
    private int radius = 100;
    [SerializeField]
    private int tradius = 30;
    [SerializeField]
    private float minAlpha = 0.3f;
    [SerializeField]
    private bool inner = false;

    private Vector2 joystickAxis = Vector2.zero;	
	private Vector2 lastJoystickAxis = Vector2.zero;

    [SerializeField]
    private int move_dis = 10;
    private Vector3 m_local_pos;

    private bool isForBid = false;
    private bool isHolding = false;
    private bool isStart = false;
    private bool isCancel = false;
    private UIRoot m_uiroot;

    [SerializeField]
    private UIPanel m_root;
    [SerializeField]
    private UIPanel m_panel;
    [SerializeField]
    private UISprite m_bg;
    [SerializeField]
    private UISprite m_thumb;
    [SerializeField]
    private GameObject m_fx;
    [SerializeField]
    private int m_mode = 0;
    [SerializeField]
    private GameObject m_cancel;

    void Awake()
	{
		Init();
	}

    void Start()
    {
        m_uiroot = AppFacade._instance.ResourceManager.UIRoot.transform.parent.GetComponent<UIRoot>();
    }

    public void setMode(int mode)
    {
        m_mode = mode;
    }

    public void setForBid(bool forbid)
    {
        isForBid = forbid;
        if (forbid && isHolding)
        {
            m_thumb.transform.localPosition = Vector3.zero;
            if (m_fx != null)
            {
                m_fx.gameObject.SetActive(false);
            }
            Lighting(minAlpha);
            isHolding = false;
            {
                m_root.transform.localPosition = m_local_pos;
            }
            isStart = false;
        }
    }

    void Init()
	{
        isForBid = false;
        m_local_pos = m_root.transform.localPosition;
        m_panel.transform.localPosition = Vector3.zero;
        m_bg.transform.localPosition = Vector3.zero;
        m_thumb.transform.localPosition = Vector3.zero;
        if (m_fx != null)
        {
            m_fx.transform.localPosition = Vector3.zero;
        }
		Lighting(minAlpha);
        Joy.OnInit(name, this);
        //Util.CallMethod("Joy", "OnInit", name, this);
    }
	
	void OnPress (bool isPressed)
	{
		if (isForBid)
		{
			return;
		}
        if (isPressed)
        {
            isHolding = true;
            isStart = true;
            isCancel = false;
            if (m_mode != 0)
            {
                return;
            }
            {
                Vector3 offset = ScreenPos_to_NGUIPos(UICamera.currentTouch.pos);
                float w = AppFacade._instance.PanelManager.get_w();
                float h = AppFacade._instance.PanelManager.get_h();
                if (offset.x - radius < -w / 2 + 10)
                {
                    offset.x = radius - w / 2 + 10;
                }
                if (offset.x + radius > w / 2 - 10)
                {
                    offset.x = -radius + w / 2 - 10;
                }
                if (offset.y - radius < -h / 2 + 10)
                {
                    offset.y = radius - h / 2 + 10;
                }
                if (offset.y + radius > h / 2 - 10)
                {
                    offset.y = -radius + h / 2 - 10;
                }
                Transform t = transform.parent;
                do
                {
                    offset -= t.localPosition;
                    t = t.parent;
                } while (t.gameObject != AppFacade._instance.ResourceManager.UIRoot.gameObject);
                if ((offset - m_local_pos).magnitude > move_dis)
                {
                    offset = (offset - m_local_pos).normalized * move_dis + m_local_pos;
                }
                m_root.transform.localPosition = offset;

            }
            Lighting(1f);
            if (m_fx != null)
            {
                m_fx.SetActive(true);
            }
            if (m_cancel != null)
            {
                m_cancel.SetActive(true);
            }
            CalculateJoystickAxis();
        }
        else
        {
            isHolding = false;
            isStart = false;
            if (m_mode != 0)
            {
                return;
            }
            CalculateJoystickAxis();
            m_thumb.transform.localPosition = Vector3.zero;
            if (m_fx != null)
            {
                m_fx.SetActive(false);
            }
            if (m_cancel != null)
            {
                m_cancel.SetActive(false);
            }
            Lighting(minAlpha);
            {
                m_root.transform.localPosition = m_local_pos;
            }
        }
	}
	
	void OnDrag(Vector2 delta)
	{
		if (isForBid)
		{
			return;
		}

        isStart = false;
        if (m_mode != 0)
        {
            return;
        }
        CalculateJoystickAxis();
    }
	
	void CalculateJoystickAxis()
	{
		Vector3 offset = ScreenPos_to_NGUIPos(UICamera.currentTouch.pos);
		Transform t = transform;
		do
		{
			offset -= t.localPosition;
			t = t.parent;
		} while (t.gameObject != AppFacade._instance.ResourceManager.UIRoot.gameObject);
		{
            int r = radius;
            if (inner)
            {
                r = r - tradius;
            }
            if (offset.magnitude > r)
			{
				offset = offset.normalized * r;
			}
		}
		m_thumb.transform.localPosition = offset;
		lastJoystickAxis = joystickAxis;
		joystickAxis = new Vector2(offset.x / radius, offset.y / radius);
        if (m_fx != null)
        {
            float r = Axis2Angle(true);
            m_fx.transform.localEulerAngles = new Vector3(0, 0, r);
        }
        if (m_cancel != null)
        {
            offset = ScreenPos_to_NGUIPos(UICamera.currentTouch.pos);
            t = m_cancel.transform;
            do
            {
                offset -= t.localPosition;
                t = t.parent;
            } while (t.gameObject != AppFacade._instance.ResourceManager.UIRoot.gameObject);
            {
                if (offset.magnitude > m_cancel.GetComponent<UISprite>().width / 2)
                {
                    isCancel = false;
                }
                else
                {
                    isCancel = true;
                }
            }
        }
    }
	
	Vector3 ScreenPos_to_NGUIPos(Vector3 screenPos)
	{
		Vector3 uiPos = UICamera.currentCamera.ScreenToWorldPoint(screenPos);
		uiPos = UICamera.currentCamera.transform.InverseTransformPoint(uiPos);
		return uiPos;
	}

	Vector3 ScreenPos_to_NGUIPos(Vector2 screenPos)
	{
		return ScreenPos_to_NGUIPos(new Vector3(screenPos.x, screenPos.y, 0f));
	}

	void Lighting(float alpha)
	{
		m_panel.alpha = alpha;
	}

    public bool IsHolding()
    {
        return isHolding;
    }

    public bool IsStart()
    {
        return isStart;
    }

    public bool IsCancel()
    {
        return isCancel;
    }

    public int Mode()
    {
        return m_mode;
    }

    public float Axis2Angle(bool inDegree = true)
	{
		float angle = Mathf.Atan2(joystickAxis.y, joystickAxis.x);
        if (angle < 0)
        {
            angle += Mathf.PI * 2;
        }
        if (inDegree)
		{
			return angle * Mathf.Rad2Deg;
		}
		else
		{
			return angle;
		}
	}

    public Vector2 GetPos()
    {
        return joystickAxis;
    }
}
