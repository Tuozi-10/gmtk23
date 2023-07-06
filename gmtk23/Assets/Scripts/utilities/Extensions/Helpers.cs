using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace src.Extensions
{
    /// <summary>
    /// used to add methods on components to reduce code writing
    /// </summary>
    public static class Helpers
    {
        #region Transforms

        #region Pos

        public static void SetPosX(this Transform t, float pos)
        {
            Vector3 position = t.position;
            position = new Vector3(pos, position.y, position.z);
            t.position = position;
        }

        public static void SetPosY(this Transform t, float pos)
        {
            Vector3 position = t.position;
            position = new Vector3(position.x, pos, position.z);
            t.position = position;
        }


        public static void SetLocalPosX(this Transform t, float pos)
        {
            Vector3 localPosition = t.localPosition;
            localPosition = new Vector3(pos, localPosition.y, localPosition.z);
            t.localPosition = localPosition;
        }

        public static void SetLocalPosY(this Transform t, float pos)
        {
            Vector3 localPosition = t.localPosition;
            localPosition = new Vector3(localPosition.x, pos, localPosition.z);
            t.position = localPosition;
        }

        #endregion

        #region Scale

        public static void SetScaleX(this Transform t, float scaleX)
        {
            Vector3 scale = t.localScale;
            scale = new Vector3(scaleX, scale.y, scale.z);
            t.position = scale;
        }

        public static void SetScaleY(this Transform t, float scaleY)
        {
            Vector3 scale = t.localScale;
            scale = new Vector3(scale.x, scaleY, scale.z);
            t.position = scale;
        }

        public static void SetScale(this Transform t, float s)
        {
            t.localScale = new Vector3(s, s, s);
        }

        #endregion

        #endregion

        #region Recttransform

        #region Size Delta

        public static void SetWidth(this RectTransform rt, float w)
        {
            rt.sizeDelta = new Vector2(w, rt.sizeDelta.y);
        }

        public static void SetHeight(this RectTransform rt, float h)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, h);
        }

        public static void SetSize(this RectTransform rt, float size)
        {
            rt.sizeDelta = new Vector2(size, size);
        }

        public static float GetWidth(this RectTransform rt)
        {
            return rt.sizeDelta.x;
        }

        public static float GetHeight(this RectTransform rt)
        {
            return rt.sizeDelta.y;
        }

        #endregion

        #region LocalEulers

        public static void SetLocalEulerX(this RectTransform t, float rota)
        {
            Vector3 rotation = t.localEulerAngles;
            rotation = new Vector3(rota, rotation.y, rotation.z);
            t.localEulerAngles = rotation;
        }

        public static void SetLocalEulerY(this RectTransform t, float rota)
        {
            Vector3 rotation = t.localEulerAngles;
            rotation = new Vector3(rotation.x, rota, rotation.z);
            t.localEulerAngles = rotation;
        }

        public static void SetLocalEulerZ(this RectTransform t, float rota)
        {
            Vector3 rotation = t.localEulerAngles;
            rotation = new Vector3(rotation.x, rotation.y, rota);
            t.localEulerAngles = rotation;
        }

        #endregion

        #region anchored position

        public static void SetAnchored(this RectTransform rectTransform, Vector2 newAnchored)
        {
            rectTransform.anchoredPosition = newAnchored;
        }

        public static void SetAnchoredX(this RectTransform rectTransform, float newAnchoredX)
        {
            Vector2 anchor = rectTransform.anchoredPosition;
            anchor.x = newAnchoredX;
            rectTransform.anchoredPosition = anchor;
        }

        public static void SetAnchoredY(this RectTransform rectTransform, float newAnchoredY)
        {
            Vector2 anchor = rectTransform.anchoredPosition;
            anchor.y = newAnchoredY;
            rectTransform.anchoredPosition = anchor;
        }

        #endregion

        #endregion

        #region Image

        public static void SetAlpha(this Image image, float alpha)
        {
            Color c = image.color;
            c.a = alpha;
            image.color = c;
        }

        #endregion

        #region Buttons

        #region Navigation

        /// <summary>
        /// used to copy component without garbage
        /// </summary>
        private static UnityEngine.UI.Navigation s_copyNav;

        public static void UpdateSelectableDown(this Selectable navToModify, Selectable selectableDown)
        {
            s_copyNav = navToModify.navigation;
            s_copyNav.selectOnDown = selectableDown;
            navToModify.navigation = s_copyNav;
        }

        public static void UpdateSelectableUp(this Selectable navToModify, Selectable selectableUp)
        {
            s_copyNav = navToModify.navigation;
            s_copyNav.selectOnUp = selectableUp;
            navToModify.navigation = s_copyNav;
        }

        public static void UpdateSelectableLeft(this Selectable navToModify, Selectable selectableLeft)
        {
            s_copyNav = navToModify.navigation;
            s_copyNav.selectOnLeft = selectableLeft;
            navToModify.navigation = s_copyNav;
        }

        public static void UpdateSelectableRight(this Selectable navToModify, Selectable selectableRight)
        {
            s_copyNav = navToModify.navigation;
            s_copyNav.selectOnRight = selectableRight;
            navToModify.navigation = s_copyNav;
        }


        public static void EraseNav(this Selectable navToModify)
        {
            s_copyNav = navToModify.navigation;
            s_copyNav.selectOnRight = s_copyNav.selectOnLeft = s_copyNav.selectOnUp = s_copyNav.selectOnDown = null;
            navToModify.navigation = s_copyNav;
        }

        public static void EraseNavExceptLeft(this Selectable navToModify)
        {
            s_copyNav = navToModify.navigation;
            s_copyNav.selectOnRight = s_copyNav.selectOnUp = s_copyNav.selectOnDown = null;
            navToModify.navigation = s_copyNav;
        }

        public static void EraseNavExceptRight(this Selectable navToModify)
        {
            s_copyNav = navToModify.navigation;
            s_copyNav.selectOnLeft = s_copyNav.selectOnUp = s_copyNav.selectOnDown = null;
            navToModify.navigation = s_copyNav;
        }

        #endregion

        #endregion

        #region Graphic

        public static void SetAlpha(this Graphic graphic, float alpha)
        {
            Color c = graphic.color;
            c.a = alpha;
            graphic.color = c;
        }

        #endregion

        #region SpriteRenderer

        public static void SetAlpha(this SpriteRenderer spriteRenderer, float value)
        {
            Color color = spriteRenderer.color;
            color = new Color(color.r, color.g, color.b, value);
            spriteRenderer.color = color;
        }

        #endregion

        #region string

        /// <summary>
        /// get a minute stylized timer
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string ToTimeMinSec(this int seconds)
        {
            if (seconds <= 60) return $"{seconds} s"; // < 60 sec

            int time = seconds;
            int minutes = 0;

            while (time >= 60)
            {
                minutes++;
                time -= 60;
            }

            return time > 0 ? $"{minutes} min {time} s" : $"{(minutes < 10 ? "0" : "")}{minutes} min"; // > 60 sec
        }

        #region Time Formater

        /// <summary>
        /// format 12:00
        /// </summary>
        /// <returns></returns>
        public static string GetTimeChat() =>
            $"{System.DateTime.Now.Hour}:{(System.DateTime.Now.Minute < 10 ? "0" : "")}{System.DateTime.Now.Minute}";

        #endregion

        #endregion

        #region Lists

        /// <summary>
        /// set enable state to true if id >= list index 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="id"></param>
        public static void IdChecker(this List<GameObject> list, int id)
        {
            for (var i = 0; i < list.Count; i++)
                list[i].SetActive(i >= id);
        }

        /// <summary>
        /// set enable state to true if id == list index 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="id"></param>
        public static void IdEqual(this List<GameObject> list, int id)
        {
            for (var i = 0; i < list.Count; i++)
                list[i].SetActive(i == id);
        }

        #endregion

        #region CanvasGroup

        public static void NoInteractions(this CanvasGroup canvasGroup)
        {
            canvasGroup.interactable = canvasGroup.blocksRaycasts = false;
        }

        public static void EnableInteractions(this CanvasGroup canvasGroup)
        {
            canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
        }

        #endregion
    }
}