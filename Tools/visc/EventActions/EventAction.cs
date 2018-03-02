using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using DG.Tweening;

namespace Visc
{
	public abstract class EventAction : ScriptableObject
	{
		public const string ActionName = "Generic event action";

		[SerializeField] protected string _description;
		[SerializeField] protected GameObject _actor;
        [SerializeField] protected bool _triggered;
        [SerializeField] protected float _startTime;
		[SerializeField] protected float _duration = 1f;
		[SerializeField] protected int _editingTrack;

		protected GUIStyle GuiStyle;

		public int EditingTrack
		{
			get { return _editingTrack; }
			set { _editingTrack = value >= 0 ? value : 0; }
		}

		public GameObject Actor { get { return _actor; } set { _changed = true; _actor = value; } }
		public string Description { get { return _description; } }
        public bool IsTriggered { get { return _triggered; } }
        public float StartTime { get { return _startTime; } set { _startTime = value >= 0f ? value : 0f; } }
		public float Duration { get { return _duration; } set { _duration = value >= 0.1f ? value : 0.1f; } }
		public float EndTime { get { return _startTime + _duration; } }

		public bool NowPlaying { get; protected set; }
        
        protected bool _changed;

        [SerializeField]
        protected int _typeSelect;
        [SerializeField]
        protected int typeSelect
        {
            get
            {
                return _typeSelect;
            }
            set
            {
                _changed = true;
                _typeSelect = value;
            }
        }

        protected Ease easer;
        protected string[] easingOptions = System.Enum.GetNames(typeof(Ease));
        [SerializeField]
        protected int _easingMode = 1; //linear

        public void ActionStart(float starTime)
		{
			Debug.Log("[EventSystem] Started event " + _description);
			NowPlaying = true;
			OnStart(starTime);
		}

		public void ActionUpdate(ref float timeSinceActionStart) { OnUpdate(ref timeSinceActionStart); }

		public void Stop()
		{
			Debug.Log("[EventSystem] Finished event " + _description);
			NowPlaying = false;
			OnStop();
		}

        protected virtual string GetColor()
        {
            return "#FF0000";
        }

        protected virtual Color GetHighlightedColor()
        {
            Color c = hexToColor(GetColor());
            c.r *= 1.5f;
            c.g *= 1.5f;
            c.b *= 1.5f;
            return c;
        }

        public virtual void DrawTimelineGui(Rect rect, bool selected)
        {
            if (GuiStyle == null || GuiStyle.name != ToString() || _changed)
            {
                GuiStyle = new GUIStyle(GUI.skin.box)
                {
                    normal = { background = MakeTexSq(GetColor()) },
                    name = ToString()
                };

                _changed = false;
            }
            if (selected) { 
                GuiStyle.normal.background = MakeTex(2, 2, GetHighlightedColor());
            }
            else
            {
                _changed = true;
            }
        }

        public virtual void DrawEditorGui()
		{
#if UNITY_EDITOR
            _description = EditorGUILayout.TextField("Description", _description);
            _startTime = EditorGUILayout.FloatField("Start time", _startTime);
            _duration = EditorGUILayout.FloatField("Duration", _duration);
            _actor = EditorGUILayout.ObjectField("Actor", _actor, typeof(GameObject), true) as GameObject;

            OnEditorGui();
#endif
        }
        
        protected virtual void OnEditorGui() {}
        protected virtual void OnStart(float startTime) { }
		protected virtual void OnUpdate(ref float currentTime) { }
		protected virtual void OnStop() { }

        private void OnEnable()
        {
            GrayTex = MakeTex(2, 2, new Color(.3f, .3f, .3f, 1f));
        }

        public bool CheckIntersection(EventAction action)
		{
			return CheckIntersection(action, action.EditingTrack, action.StartTime, action.EndTime);
		}

		public bool CheckIntersection(EventAction action, float track, float startTime, float endTime)
		{
			if (action == this) return false;
			var sameEditingTrack = track == EditingTrack;
			var intersectsIn = StartTime < endTime && startTime < EndTime;

			return (sameEditingTrack && intersectsIn);
		}

        public static Color TexColor(float r, float g, float b)
        {
            // easier to transfer from Kuler to here
            return new Color(r / 255f, g / 255f, b / 255f, 1f);
        }
		
		public static Texture2D MakeTex(int width, int height, Color col)
		{
			var pix = new Color[width * height];
			for (var i = 0; i < pix.Length; ++i)
				pix[i] = col;
			var result = new Texture2D(width, height);
			result.SetPixels(pix);
			result.Apply();
			return result;
		}

        public static Texture2D MakeTexSq(string hex)
        {
            return MakeTex(2, 2, hexToColor(hex));
        }

        public static T ParseEnum<T>(string val)
        {
            return (T)Enum.Parse(typeof(T), val, true);
        }

        protected Texture2D GrayTex;
        protected string gray = "CCCCCC";

        public static string colorToHex(Color color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        public static Color hexToColor(string hex)
        {
            hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = 1;//assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color(r / 255f, g / 255f, b / 255f, a);
        }
    }
}
