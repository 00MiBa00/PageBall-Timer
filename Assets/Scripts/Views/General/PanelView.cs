using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Views.General
{
    public class PanelView : MonoBehaviour
    {
        public event Action<int> OnPressBtnAction;

        [SerializeField]
        private List<Button> _btns;
        
        private Tween _tween;

        private void OnEnable()
        {
            for (int i = 0; i < _btns.Count; i++)
            {
                int index = i;

                _btns[i].onClick.AddListener(() => Notification(index));
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _btns.Count; i++)
            {
                int index = i;

                _btns[i].onClick.RemoveAllListeners();
            }
        }

        public void Open()
        {
            _tween?.Kill();
            
            transform.localScale = Vector3.zero;
            
            gameObject.SetActive(true);

            _tween = transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }

        public void Close()
        {
            _tween?.Kill();

            _tween = transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
            
            gameObject.SetActive(false);
        }

        public void SetBtnInteractable(int index, bool value)
        {
            _btns[index].interactable = value;
        }
        
        private void Notification(int index)
        {
            OnPressBtnAction?.Invoke(index);
        }
    }
}