using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using System.Collections;
using System.Xml;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Linq;
using UI.Xml.CustomAttributes;

namespace UI.Xml
{
    [Serializable]
    public partial class XmlElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDropHandler
    {
        [SerializeField]
        protected string _tagType = null;

        public string tagType
        {
            get
            {
                return _tagType;
            }
        }

        [NonSerialized]
        protected ElementTagHandler _tagHandler;
        public ElementTagHandler tagHandler
        {
            get
            {
                if (_tagHandler == null)
                {
                    if (tagType != null) _tagHandler = XmlLayoutUtilities.GetXmlTagHandler(tagType);
                }

                return _tagHandler;
            }
        }

        [SerializeField]
        protected RectTransform _rectTransform;

        public RectTransform rectTransform
        {
            get
            {
                return _rectTransform;
            }
        }

        [NonSerialized]
        private Selectable _selectable;
        private Selectable selectable
        {
            get
            {
                if (_selectable == null)
                {
                    _selectable = this.GetComponent<Selectable>();
                }

                return _selectable;
            }
        }

        [SerializeField]
        public AttributeDictionary attributes = new AttributeDictionary();

        /// <summary>
        /// This is a list of attributes set directly on the element (not derived from a class).        
        /// </summary>
        [SerializeField]
        public List<string> elementAttributes = new List<string>();

        [SerializeField]
        protected XmlLayout xmlLayout;
        public XmlLayout xmlLayoutInstance { get { return xmlLayout; } }

        [SerializeField]
        public List<XmlElement> childElements = new List<XmlElement>();

        [SerializeField]
        public List<string> classes = new List<string>();

        [SerializeField]
        public List<string> hoverClasses = new List<string>();

        [SerializeField]
        public XmlElement parentElement = null;

        [SerializeField]
        protected string m_id = "";
        public string id
        {
            get
            {
                return m_id;
            }
        }

        [SerializeField]
        protected string m_internalId = "";
        public string internalId
        {
            get
            {
                return m_internalId;
            }
        }

        #region Animation
        public ShowAnimation ShowAnimation = ShowAnimation.None;
        public HideAnimation HideAnimation = HideAnimation.None;
        public float AnimationDuration = 0.25f;
        public float ShowAnimationDelay = 0f;
        public float HideAnimationDelay = 0f;

        public bool _IsAnimating { get; protected set; }
        public bool IsAnimating
        {
            get
            {
                return _IsAnimating || GetCleansedChildElements().Any(c => c.IsAnimating);
            }
        }
        #endregion

        [NonSerialized]
        public bool Visible = false;

        public float DefaultOpacity = 1f;

        #region Audio
        public AudioClip OnClickSound = null;
        public AudioClip OnMouseEnterSound = null;
        public AudioClip OnMouseExitSound = null;

        public AudioClip OnShowSound = null;
        public AudioClip OnHideSound = null;

        public float AudioVolume = 1f;        
        
        public string AudioMixerGroup = null;
        private AudioMixerGroup _AudioMixerGroup = null;

        private AudioSource m_AudioSource = null;
        protected AudioSource AudioSource
        {
            get
            {
                if (m_AudioSource == null)
                {
                    m_AudioSource = this.GetComponent<AudioSource>();
                    if (m_AudioSource == null) m_AudioSource = this.gameObject.AddComponent<AudioSource>();
                }

                return m_AudioSource;
            }
        }
        #endregion

        #region Dragging and Dropping
        // Can this XmlElement be dragged?
        public bool AllowDragging = false;
        // If AllowDragging is true, should this XmlElement be able to be dragged beyond the confines of its parent?
        public bool RestrictDraggingToParentBounds = true;

        public bool ReturnToOriginalPositionWhenReleased = true;

        // This is static - as only one element may be dragged at a time, this will record which element it is for Drag & Drop functionality
        public static XmlElement ElementCurrentlyBeingDragged = null;
        // Can this XmlElement receive drop events?
        public bool IsDropReceiver = false;

        #endregion

        #region Tooltip
        public string Tooltip;
        public XmlLayoutTooltip.TooltipPosition TooltipPosition = XmlLayoutTooltip.TooltipPosition.Right;
        public bool TooltipFollowMouse = false;
        public float TooltipOffset = 8f;
        public Color TooltipBackgroundColor;
        public Color TooltipBorderColor;
        public Color TooltipTextColor;
        public Color TooltipTextOutlineColor;
        public Sprite TooltipBackgroundImage;
        public Sprite TooltipBorderImage;
        public int TooltipFontSize;
        public Font TooltipFont;
        public RectOffset TooltipPadding;
        #endregion

        #region Event-Handling
        private EventTrigger m_EventTrigger = null;
        public EventTrigger EventTrigger
        {
            get
            {
                if (m_EventTrigger == null)
                {
                    m_EventTrigger = this.GetComponent<EventTrigger>();

                    if (m_EventTrigger == null) m_EventTrigger = this.gameObject.AddComponent<EventTrigger>();
                }

                return m_EventTrigger;
            }
        }

        [SerializeField]
        public List<Action> m_onClickEvents = new List<Action>();
        [SerializeField]
        protected List<Action> m_onMouseEnterEvents = new List<Action>();
        [SerializeField]
        protected List<Action> m_onMouseExitEvents = new List<Action>();
        [SerializeField]
        public List<Action<XmlElement, XmlElement>> m_onElementDroppedEvents = new List<Action<XmlElement, XmlElement>>();
        [SerializeField]
        public List<Action> m_onBeginDragEvents = new List<Action>();
        [SerializeField]
        public List<Action> m_onEndDragEvents = new List<Action>();
        [SerializeField]
        public List<Action> m_onDragEvents = new List<Action>();
        [SerializeField]
        public List<Action> m_onSubmitEvents = new List<Action>();
        [SerializeField]
        public List<Action> m_onShowEvents = new List<Action>();
        [SerializeField]
        public List<Action> m_onHideEvents = new List<Action>();
        #endregion

        #region Cursors
        public XmlLayoutCursorController.CursorInfo cursor;
        public XmlLayoutCursorController.CursorInfo cursorClick;
        #endregion

        [SerializeField]
        internal Vector2 currentOffset = Vector2.zero;

        void Awake()
        {
            //if (gameObject.activeInHierarchy) Visible = true;

            XmlLayoutTimer.DelayedCall(0, () =>
            {
                if (gameObject.activeInHierarchy) Visible = true;
            }, this);
        }

        public void Initialise(XmlLayout xmlLayout, RectTransform rectTransform, ElementTagHandler tagHandler)
        {
            this.xmlLayout = xmlLayout;
            this._rectTransform = rectTransform;
            this._tagHandler = tagHandler;

            if (this.tagHandler != null)
            {
                this._tagType = tagHandler.tagType;
            }
        }

        public void SetAttribute(string attribute, string value)
        {
            if (attribute == "class")
            {
                Debug.LogWarning("[XmlLayout][XmlElement][SetAttribute]:: Please use 'SetClass', 'AddClass', and/or 'RemoveClass' to manipulate the class attribute.");
                return;
            }

            if (HasAttribute(attribute))
            {
                attributes[attribute] = value;
            }
            else
            {
                attributes.Add(attribute, value);
                elementAttributes.Add(attribute);
            }
        }

        public void RemoveAttribute(string name)
        {
            if (HasAttribute(name))
            {
                attributes.Remove(name);
                elementAttributes.Remove(name);
            }
        }

        public string GetAttribute(string name)
        {
            if (HasAttribute(name))
            {
                return attributes[name];
            }

            return null;
        }

        public bool HasAttribute(string name)
        {
            return attributes.ContainsKey(name);
        }

        /// <summary>
        /// Apply the provided attributes to this XmlElement. If no attributes are provided, then this function will use this XmlElement's attribute collection instead.
        /// </summary>
        /// <param name="_attributes"></param>
        public void ApplyAttributes(AttributeDictionary _attributes = null)
        {
            if (_attributes != null)
            {
                if (this.attributes != null)
                {
                    // merge attribute dictionaries
                    var temp = this.attributes.Clone();

                    foreach (var attribute in _attributes)
                    {
                        if (temp.ContainsKey(attribute.Key))
                        {
                            temp[attribute.Key] = attribute.Value;
                        }
                        else
                        {
                            temp.Add(attribute.Key, attribute.Value);
                        }
                    }

                    this.attributes = temp;
                }
                else
                {
                    this.attributes = _attributes;
                }
            }
            else
            {
                _attributes = this.attributes;
            }

            if (_attributes.ContainsKey("internalid"))
            {
                this.m_internalId = _attributes["internalid"];
            }

            if (_attributes.ContainsKey("id"))
            {
                this.m_id = _attributes["id"];
            }

            tagHandler.SetInstance(rectTransform, xmlLayout);
            tagHandler.ApplyAttributes(_attributes);
        }

        /// <summary>
        /// Convenience method to set and apply a single attribute immediately.
        /// Use this in place of calling ApplyAttributes(AttributeDictionary) when you wish to set a single attribute
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetAndApplyAttribute(string name, string value)
        {
            var attributeDictionary = new AttributeDictionary();
            attributeDictionary.Add(name, value);

            tagHandler.SetInstance(rectTransform, xmlLayout);
            tagHandler.ApplyAttributes(attributeDictionary);
        }

        /// <summary>
        /// Add the specified class to this element.
        /// </summary>
        /// <param name="name"></param>
        public void AddClass(string name)
        {
            if (this.classes.Contains(name)) return;

            this.classes.Add(name);
            ClassChanged();
        }

        /// <summary>
        /// Remove the specified class from this element.
        /// </summary>
        /// <param name="name"></param>
        public void RemoveClass(string name)
        {
            if (classes.Remove(name))
            {
                ClassRemoved(name);
                ClassChanged();
            }
        }

        /// <summary>
        /// Set the class(es) of this element, replacing any existing classes.
        /// </summary>
        /// <param name="newClasses"></param>
        public void SetClass(params string[] newClasses)
        {
            var classesRemoved = this.classes.Where(c => !newClasses.Contains(c)).ToArray();

            this.classes = newClasses.ToList();
            ClassRemoved(classesRemoved);
            ClassChanged();
        }

        protected void ClassChanged()
        {
            this.tagHandler.SetInstance(this.rectTransform, this.xmlLayout);
            this.tagHandler.ClassChanged();
        }

        protected void ClassRemoved(params string[] classesRemoved)
        {
            if (!xmlLayout.defaultAttributeValues.ContainsKey(this.tagType)) return;
            
            List<string> attributesReset = new List<string>();

            foreach (var classRemoved in classesRemoved)
            {
                if (!xmlLayout.defaultAttributeValues[this.tagType].ContainsKey(classRemoved)) continue;                

                var attributesDefinedByClass = xmlLayout.defaultAttributeValues[this.tagType][classRemoved].Select(a => a.Key).ToList();                

                // add all attributes defined by this class that are not defined by the element
                attributesReset.AddRange(attributesDefinedByClass.Where(a => !elementAttributes.Contains(a)));
            }

            if (!attributesReset.Any()) return;

            var _classes = classes;
            _classes.Insert(0, "all");

            // remove any attributes covered by other classes (no need to reset them)
            foreach (var _class in _classes)
            {
                if (!xmlLayout.defaultAttributeValues[this.tagType].ContainsKey(_class)) continue;

                var attributesDefinedByClass = xmlLayout.defaultAttributeValues[this.tagType][_class].Select(a => a.Key).ToList();

                attributesReset.RemoveAll(a => attributesDefinedByClass.Contains(a));
            }

            if (attributesReset.Any())
            {                
                // Now for the hard part: we need to determine the default values for these attributes
                AttributeDictionary defaultAttributesToApply = new AttributeDictionary();
                foreach (var attributeToReset in attributesReset)
                {
                    var value = tagHandler.GetDefaultValueForAttribute(attributeToReset);

                    defaultAttributesToApply.Add(attributeToReset, value);
                }                

                if (defaultAttributesToApply.Any())
                {
                    ApplyAttributes(defaultAttributesToApply);
                }
            }
        }

        /// <summary>
        /// Get the value of this XmlElement.
        /// Returns null if this element doesn't have a GetValue implementation.
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            if (tagHandler is Tags.IHasXmlFormValue)
            {
                tagHandler.SetInstance(this.rectTransform, this.xmlLayout);

                return ((Tags.IHasXmlFormValue)tagHandler).GetValue(this);
            }

            return null;
        }

        /// <summary>
        /// Does this XmlElement have a specific class? (as set in Xml)
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool HasClass(string c)
        {
            return classes.Contains(c, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Retrieve a child XmlElement by its internalId.
        /// </summary>
        /// <param name="internalId"></param>
        /// <returns></returns>
        public XmlElement GetElementByInternalId(string internalId)
        {
            if (!childElements.Any()) return null;

            var topLevelChild = childElements.FirstOrDefault(c => String.Equals(c.internalId, internalId, StringComparison.OrdinalIgnoreCase));

            if (topLevelChild != null) return topLevelChild;

            foreach (var child in childElements)
            {
                var depthSearch = child.GetElementByInternalId(internalId);

                if (depthSearch != null) return depthSearch;
            }

            return null;
        }

        /// <summary>
        /// Get a child XmlElement by its internalId (and call GetComponent to retrieve the desired component)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="internalId"></param>
        /// <returns></returns>
        public T GetElementByInternalId<T>(string internalId)
            where T : MonoBehaviour
        {
            var element = GetElementByInternalId(internalId);

            if (element != null)
            {
                return element.GetComponent<T>();
            }

            return null;
        }

        /// <summary>
        /// Add a child XmlElement to this XmlElement
        /// </summary>
        /// <param name="child"></param>
        public void AddChildElement(XmlElement child, bool adjustRectTransform = true)
        {
            if (adjustRectTransform)
            {
                child.transform.SetParent(this.transform);
                child.transform.localScale = Vector3.one;
                child.transform.position = new Vector3(child.transform.position.x, child.transform.position.y, 0);
                //child.rectTransform.anchoredPosition3D = new Vector3(child.rectTransform.anchoredPosition3D.x, child.rectTransform.anchoredPosition3D.y, 0);
                child.rectTransform.anchoredPosition3D = Vector3.zero;
                child.transform.localRotation = new Quaternion();
                child.transform.SetAsLastSibling();
            }

            child.parentElement = this;

            if (!childElements.Contains(child)) childElements.Add(child);
        }

        /// <summary>
        /// Break the link between a child element and this element
        /// </summary>
        /// <param name="child"></param>
        /// <param name="destroyChild">If this is set to true, then the specified child element will be destroyed (removed from the scene entirely).</param>
        public void RemoveChildElement(XmlElement child, bool destroyChild = false)
        {
            if (childElements.Contains(child))
            {
                childElements.Remove(child);
            }

            if (destroyChild)
            {
                if (Application.isPlaying) Destroy(child.gameObject);
                else DestroyImmediate(child.gameObject);
            }
        }

        void OnDestroy()
        {
            if (parentElement != null)
            {
                parentElement.childElements.Remove(this);
            }
        }

        /// <summary>
        /// Show this XmlElement and trigger its show animation (if it has one set)        
        /// </summary>
        /// <param name="recursiveCall">Internal - should always be left as the default value of (false)</param>
        public void Show(bool recursiveCall = false, Action onCompleteCallback = null)
        {
            if (Visible)
            {
                if (onCompleteCallback != null) onCompleteCallback();                
                return;
            }

            if (!recursiveCall) // if this is a recursive call, then there's no need to mark this object as active (and besides, we may not want to)
            {
                gameObject.SetActive(true);
            }

            Visible = gameObject.activeInHierarchy;

            if (!Visible) return;
            
            foreach (var childElement in GetCleansedChildElements())
            {
                childElement.Show(true);
            }

            if (!Application.isPlaying) return;

            PlaySound(OnShowSound);

            if (this.gameObject.activeInHierarchy && ShowAnimation != ShowAnimation.None)
            {
                StartCoroutine(PlayShowAnimation(ShowAnimation, onCompleteCallback));
            }
            else
            {
                CanvasGroup.alpha = DefaultOpacity;
                StartCoroutine(WaitForShowAnimationToComplete(onCompleteCallback));
            }
        }

        /// <summary>
        /// Hide this XmlElement and trigger the hide animation if necessary.
        /// </summary>
        /// <param name="recursiveCall">Internal - should always be left as the default value of (false)</param>
        /// <param name="onCompleteCallback">Specifies an Action to be called after this XmlElement is hidden (after any animation is completed).</param>
        public void Hide(bool recursiveCall = false, Action onCompleteCallback = null)
        {
            if (!gameObject.activeInHierarchy || !Visible)
            {
                Visible = false;

                if (onCompleteCallback != null) onCompleteCallback();

                return;
            }            

            foreach (var childElement in GetCleansedChildElements())
            {
                childElement.Hide(true);
            }

            PlaySound(OnHideSound);

            if (Application.isPlaying && HideAnimation != HideAnimation.None)
            {
                StartCoroutine(PlayHideAnimation(HideAnimation));
            }

            if (hoverClasses != null && hoverClasses.Any())
            {
                hoverClasses.ForEach((c) => RemoveClass(c));
            }

            // if Hide() was called by the parent XmlElement, then don't actually disable the gameobject (there's no need, as the parent will itself be disabled)
            // additionally, hiding this element before the parent has finished whatever animation it is playing (if any) will cause it to disappear too early
            if (!recursiveCall)
            {
                if (!Application.isPlaying)
                {
                    gameObject.SetActive(false);
                    return;
                }

                // hide this XmlElement, but only once it and all of its children have finished their hide animations
                StartCoroutine(HideWhenAllAnimationIsComplete(onCompleteCallback));
            }
            else
            {
                Visible = false;

                if (m_onHideEvents.Any())
                {
                    m_onHideEvents.ForEach(he => he.Invoke());
                }
            }
        }

        protected IEnumerator PlayShowAnimation(ShowAnimation animation, Action onCompleteCallback = null)
        {
            // Don't start animating if we're already animating (wait for the animation to finish first)            
            while (_IsAnimating) yield return new WaitForEndOfFrame();

            _IsAnimating = true;

            if (!xmlLayout.IsReady) yield return new WaitForEndOfFrame();

            if (ShowAnimationDelay > 0)
            {
                CanvasGroup.alpha = 0;
                yield return new WaitForSeconds(ShowAnimationDelay);
            }
            
            CanvasGroup.alpha = DefaultOpacity;
            CanvasGroup.blocksRaycasts = true;

            if (animation.IsSlideAnimation())
            {
                m_Animator.enabled = false;
                yield return PlaySlideInAnimation(animation);
            }
            else
            {
                m_Animator.enabled = true;
                m_Animator.speed = 0.25f / AnimationDuration;
                m_Animator.Play(animation.ToString());

                var delay = m_Animator.GetCurrentAnimatorStateInfo(0).length / m_Animator.speed;

                // If we disable the animator immediately after the animation is complete, things get a bit wonky
                XmlLayoutTimer.DelayedCall(delay, () => { m_Animator.enabled = false; }, this);

                yield return new WaitForSeconds(delay * 0.25f);                                
            }

            _IsAnimating = false;

            StartCoroutine(WaitForShowAnimationToComplete(onCompleteCallback));
        }

        protected IEnumerator PlayHideAnimation(HideAnimation animation)
        {
            // Don't start animating if we're already animating (wait for the animation to finish first)            
            while (_IsAnimating) yield return new WaitForEndOfFrame();

            _IsAnimating = true;

            if (HideAnimationDelay > 0)
            {
                yield return new WaitForSeconds(HideAnimationDelay);
            }

            CanvasGroup.blocksRaycasts = false;

            if (animation.IsSlideAnimation())
            {
                m_Animator.enabled = false;
                yield return PlaySlideOutAnimation(animation);
            }
            else
            {
                m_Animator.enabled = true;
                m_Animator.speed = 0.25f / AnimationDuration;
                m_Animator.Play(animation.ToString());
                yield return new WaitForSeconds(m_Animator.GetCurrentAnimatorStateInfo(0).length / m_Animator.speed);
                m_Animator.enabled = false;
            }

            _IsAnimating = false;
        }

        protected Vector2 GetDistanceForSlideAnimation(SlideDirection direction)
        {
            float desiredXChange = 0, desiredYChange = 0;

            Vector3[] parentCorners = new Vector3[4], elementCorners = new Vector3[4];
            ((RectTransform)rectTransform.parent).GetWorldCorners(parentCorners);
            rectTransform.GetWorldCorners(elementCorners);

            switch (direction)
            {
                case SlideDirection.Top:
                    {
                        // distance from the element's bottom edge to the parent's top edge
                        var parentTopEdge = parentCorners[2].y;
                        var elementBottomEdge = elementCorners[0].y;

                        desiredYChange = parentTopEdge - elementBottomEdge;
                    }
                    break;

                case SlideDirection.Bottom:
                    {
                        // distance from the element's top edge to the parent's bottom edge
                        var parentBottomEdge = parentCorners[3].y;
                        var elementTopEdge = elementCorners[1].y;

                        desiredYChange = parentBottomEdge - elementTopEdge;
                    }
                    break;

                case SlideDirection.Left:
                    {
                        // distance from the element's right edge to the parent's left edge
                        var parentLeftEdge = parentCorners[0].x;
                        var elementRightEdge = elementCorners[3].x;

                        desiredXChange = parentLeftEdge - elementRightEdge;
                    }
                    break;

                case SlideDirection.Right:
                    {
                        // distance from the element's left edge to the parent's right edge
                        var parentRightEdge = parentCorners[3].x;
                        var elementLeftEdge = elementCorners[0].x;

                        desiredXChange = parentRightEdge - elementLeftEdge;
                    }
                    break;
            }

            return new Vector2(desiredXChange, desiredYChange);
        }

        protected IEnumerator PlaySlideInAnimation(ShowAnimation animation)
        {
            var distance = GetDistanceForSlideAnimation(animation.ToSlideDirection());

            if (distance.x != 0)
            {
                yield return MoveDistanceX(distance.x, 0);
                yield return MoveDistanceX(-distance.x, AnimationDuration);
            }
            else if (distance.y != 0)
            {
                yield return MoveDistanceY(distance.y, 0);
                yield return MoveDistanceY(-distance.y, AnimationDuration);
            }
        }

        protected IEnumerator PlaySlideOutAnimation(HideAnimation animation)
        {
            var distance = GetDistanceForSlideAnimation(animation.ToSlideDirection());

            if (distance.x != 0)
            {
                yield return MoveDistanceX(distance.x, AnimationDuration);
                CanvasGroup.alpha = 0;

                yield return null;

                yield return MoveDistanceX(-distance.x, 0);
            }
            else if (distance.y != 0)
            {
                yield return MoveDistanceY(distance.y, AnimationDuration);
                CanvasGroup.alpha = 0;

                yield return null;

                yield return MoveDistanceY(-distance.y, 0);
            }
        }

        protected IEnumerator MoveDistanceX(float distance, float animationDuration = 0.25f)
        {
            float initialX = transform.localPosition.x;
            float destinationX = initialX + distance;

            if (animationDuration == 0)
            {
                transform.localPosition = new Vector2(destinationX, transform.localPosition.y);

                yield break;
            }

            float rate = 1.0f / animationDuration;
            float index = 0f;

            while (index < 1)
            {
                transform.localPosition = new Vector2(Mathf.Lerp(initialX, destinationX, index), transform.localPosition.y);
                index += rate * Time.deltaTime;

                yield return null;
            }

            transform.localPosition = new Vector2(destinationX, transform.localPosition.y);
        }

        protected IEnumerator MoveDistanceY(float distance, float animationDuration = 0.25f)
        {
            float initialY = transform.localPosition.y;
            float destinationY = initialY + distance;

            if (animationDuration == 0)
            {
                transform.localPosition = new Vector2(transform.localPosition.x, destinationY);

                yield break;
            }

            float rate = 1.0f / animationDuration;
            float index = 0f;

            while (index < 1)
            {
                transform.localPosition = new Vector2(transform.localPosition.x, Mathf.Lerp(initialY, destinationY, index));
                index += rate * Time.deltaTime;

                yield return null;
            }

            transform.localPosition = new Vector2(transform.localPosition.x, destinationY);
        }

        protected IEnumerator HideWhenAllAnimationIsComplete(Action onCompleteCallback)
        {
            while (IsAnimating) yield return new WaitForEndOfFrame();

            if (m_onHideEvents.Any())
            {
                m_onHideEvents.ForEach(he => he.Invoke());
            }

            gameObject.SetActive(false);

            Visible = false;

            if (onCompleteCallback != null) onCompleteCallback();

            yield return new WaitForEndOfFrame();
        }

        protected IEnumerator WaitForShowAnimationToComplete(Action onCompleteCallback)
        {
            while (IsAnimating) yield return new WaitForEndOfFrame();            

            if (m_onShowEvents.Any())
            {
                m_onShowEvents.ForEach(se => se.Invoke());
            }

            if (onCompleteCallback != null) onCompleteCallback();
        }

        protected Animator m_Animator
        {
            get
            {
                var animator = this.GetComponent<Animator>();
                if (animator == null) animator = this.gameObject.AddComponent<Animator>();

                var animatorController = "Animation/XmlLayoutAnimationController".ToRuntimeAnimatorController();
                animator.runtimeAnimatorController = animatorController;
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;

                // Some animations require a canvas group in order to function correctly
                GetCanvasGroup();

                return animator;
            }
        }

        public CanvasGroup CanvasGroup
        {
            get
            {
                return GetCanvasGroup();
            }
        }

        private CanvasGroup GetCanvasGroup()
        {
            var canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = this.gameObject.AddComponent<CanvasGroup>();

            return canvasGroup;
        }

        private void DelayCallUntilEndOfFrame(Action call)
        {
            StartCoroutine(_DelayCallUntilEndOfFrame(call));
        }

        private IEnumerator _DelayCallUntilEndOfFrame(Action call)
        {
            yield return new WaitForEndOfFrame();

            call();
        }

        private List<XmlElement> GetCleansedChildElements()
        {
            childElements = childElements.Where(c => c != null).ToList();

            return childElements;
        }

        /// <summary>
        /// Get the values of this element and any child elements by either 'id' or 'internalId'
        /// Please note: duplicate id/internalId values will be ignored (only the first located value will be used)
        /// </summary>
        /// <param name="onlyFormElements"></param>
        /// <param name="locateElementsBy"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetFormData(eLocateElementsBy locateElementsBy = eLocateElementsBy.InternalId)
        {
            var formData = new Dictionary<string, string>();

            // if this element's tag handler does not use the IHasXmlFormValue interface, then it does not implement GetValue(), therefore we do not have a value
            if (this.tagHandler is Tags.IHasXmlFormValue)
            {
                switch (locateElementsBy)
                {
                    case eLocateElementsBy.Id:
                        if (!String.IsNullOrEmpty(this.id))
                        {
                            formData.AddIfKeyNotExists(this.id, this.GetValue());
                        }
                        break;
                    case eLocateElementsBy.InternalId:
                        if (!String.IsNullOrEmpty(this.internalId))
                        {
                            formData.AddIfKeyNotExists(this.internalId, this.GetValue());
                        }
                        break;
                }
            }

            var cleansedChildElements = GetCleansedChildElements();

            if (!cleansedChildElements.Any()) return formData;

            foreach (var childElement in cleansedChildElements)
            {
                var childResults = childElement.GetFormData(locateElementsBy);

                if (childResults != null && childResults.Any())
                {
                    foreach (var result in childResults)
                    {
                        formData.AddIfKeyNotExists(result.Key, result.Value);
                    }
                }
            }

            return formData;
        }

        public enum eLocateElementsBy
        {
            Id,
            InternalId
        }

        #region Internal Events
        public void OnPointerClick(PointerEventData eventData)
        {
            PlaySound(OnClickSound);

            if (m_onClickEvents != null && m_onClickEvents.Any())
            {
                m_onClickEvents.ForEach(a => a.Invoke());
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!String.IsNullOrEmpty(Tooltip))
            {
                xmlLayout.ShowTooltip(this, Tooltip);
            }

            if (selectable != null && !selectable.interactable) return;

            PlaySound(OnMouseEnterSound);

            if (hoverClasses != null && hoverClasses.Any())
            {
                hoverClasses.ForEach((c) => AddClass(c));
            }

            if (m_onMouseEnterEvents != null && m_onMouseEnterEvents.Any())
            {
                m_onMouseEnterEvents.ForEach(a => a.Invoke());
            }

            if (cursor != null && cursor.cursor != null)
            {
                XmlLayoutCursorController.Instance.SetCursorForState(XmlLayoutCursorController.eCursorState.Default, cursor);
            }

            if (cursorClick != null && cursorClick.cursor != null)
            {
                XmlLayoutCursorController.Instance.SetCursorForState(XmlLayoutCursorController.eCursorState.Click, cursorClick);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PlaySound(OnMouseExitSound);

            var hadCursor = cursor != null && cursor.cursor != null;
            var hadCursorClick = cursorClick != null && cursorClick.cursor != null;

            if (hoverClasses != null && hoverClasses.Any())
            {
                hoverClasses.ForEach((c) => RemoveClass(c));
            }

            if (!String.IsNullOrEmpty(Tooltip))
            {
                xmlLayout.HideTooltip(this);
            }

            if (hadCursor)
            {
                if (XmlLayoutCursorController.Instance != null) XmlLayoutCursorController.Instance.ResetCursorToDefaultForState(XmlLayoutCursorController.eCursorState.Default);
            }

            if (hadCursorClick)
            {
                if (XmlLayoutCursorController.Instance != null) XmlLayoutCursorController.Instance.ResetCursorToDefaultForState(XmlLayoutCursorController.eCursorState.Click);
            }

            if (selectable != null && !selectable.interactable) return;

            if (m_onMouseExitEvents != null && m_onMouseExitEvents.Any())
            {
                m_onMouseExitEvents.ForEach(a => a.Invoke());
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (m_onSubmitEvents != null && m_onSubmitEvents.Any())
            {
                m_onSubmitEvents.ForEach(m => m.Invoke());
            }
        }

        public void AddOnClickEvent(Action action, bool clearExisting = false)
        {
            if (clearExisting) m_onClickEvents.Clear();

            m_onClickEvents.Add(action);
        }

        public void AddOnMouseEnterEvent(Action action, bool clearExisting = false)
        {
            if (clearExisting) m_onMouseEnterEvents.Clear();

            m_onMouseEnterEvents.Add(action);
        }

        public void AddOnMouseExitEvent(Action action, bool clearExisting = false)
        {
            if (clearExisting) m_onMouseExitEvents.Clear();

            m_onMouseExitEvents.Add(action);
        }

        public void AddOnElementDroppedEvent(Action<XmlElement, XmlElement> action, bool clearExisting = false)
        {
            if (clearExisting) m_onElementDroppedEvents.Clear();

            m_onElementDroppedEvents.Add(action);
        }

        public void AddOnBeginDragEvent(Action action, bool clearExisting = false)
        {
            if (clearExisting) m_onBeginDragEvents.Clear();

            m_onBeginDragEvents.Add(action);
        }

        public void AddOnEndDragEvent(Action action, bool clearExisting = false)
        {
            if (clearExisting) m_onEndDragEvents.Clear();

            m_onEndDragEvents.Add(action);
        }

        public void AddOnDragEvent(Action action, bool clearExisting = false)
        {
            if (clearExisting) m_onDragEvents.Clear();

            m_onDragEvents.Add(action);
        }

        public void AddOnSubmitEvent(Action action, bool clearExisting = false)
        {
            if (clearExisting) m_onSubmitEvents.Clear();

            m_onSubmitEvents.Add(action);
        }

        public void AddOnShowEvent(Action action, bool clearExisting = false)
        {
            if (clearExisting) m_onShowEvents.Clear();

            m_onShowEvents.Add(action);
        }

        public void AddOnHideEvent(Action action, bool clearExisting = false)
        {
            if (clearExisting) m_onHideEvents.Clear();

            m_onHideEvents.Add(action);
        }

        #endregion

        #region Audio Methods
        public void SetAudioMixerGroup(string path)
        {
            var _details = path.Split('|');
            var mixerName = _details[0];

            if (_details.Length >= 2)
            {
                var mixer = XmlLayoutUtilities.LoadResource<AudioMixer>(mixerName);

                if (mixer != null)
                {
                    var groupPath = _details[1];
                    var group = mixer.FindMatchingGroups(groupPath).FirstOrDefault();

                    if (group != null)
                    {
                        AudioSource.outputAudioMixerGroup = group;                        
                    }
                    else
                    {
                        Debug.LogWarning("[XmlLayout][XmlElement] Warning: Audio Mixer Group with path '" + groupPath + "' was not found in Audio Mixer '" + mixerName + "'.");
                    }
                }
                else
                {
                    Debug.LogWarning("[XmlLayout][XmlElement] Warning: Audio Mixer '" + mixerName + "' was not found. Please note that the Mixer must be accessible to XmlLayout in a Resources folder or Resource Database.");
                }
            }
            else
            {
                Debug.LogWarning("[XmlLayout][XmlElement] Warning: '" + path + "' is an invalid AudioMixerGroup path. Please specify a path to the Audio Mixer followed by the Group name / path, separated by a pipe operator, e.g. Audio/MyAudioMixer|MyAudioMixerGroup. Please note that the Mixer must be accessible to XmlLayout in a Resources folder or Resource Database.");
            }
        }

        protected void PlaySound(AudioClip sound)
        {
            if (sound == null) return;

            AudioSource.volume = AudioVolume;            
            //AudioSource.outputAudioMixerGroup = AudioMixerGroup;
            AudioSource.clip = sound;

            if (!String.IsNullOrEmpty(AudioMixerGroup) && _AudioMixerGroup == null)
            {
                SetAudioMixerGroup(AudioMixerGroup);   
            }
            
            AudioSource.Play();
        }
        #endregion

        #region Dragging
        public void OnDrag(PointerEventData eventData)
        {            
            if (m_onDragEvents.Any())
            {
                m_onDragEvents.ForEach(e => e.Invoke());
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {            
            if (m_onEndDragEvents.Any())
            {
                m_onEndDragEvents.ForEach(e => e.Invoke());
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (m_onBeginDragEvents.Any())
            {
                m_onBeginDragEvents.ForEach(e => e.Invoke());
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (!IsDropReceiver || eventData == null) return;
            if (XmlElement.ElementCurrentlyBeingDragged == null) return;

            if (m_onElementDroppedEvents != null && m_onElementDroppedEvents.Any())
            {
                m_onElementDroppedEvents.ForEach(a => a.Invoke(XmlElement.ElementCurrentlyBeingDragged, this));
            }
        }

        // Thank you jmorhart: http://answers.unity3d.com/questions/976201/set-a-recttranforms-pivot-without-changing-its-pos.html
        public void SetPivot(Vector2 pivot, RectTransform rectTransform = null)
        {
            if (rectTransform == null) rectTransform = this.rectTransform;
            if (rectTransform == null) return;

            Vector2 size = rectTransform.rect.size;
            Vector2 deltaPivot = rectTransform.pivot - pivot;
            Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }
        #endregion

    }
}
